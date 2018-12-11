using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Configuration;

namespace xsy.likes.Db
{
    public class DbOperate : IDisposable, IDbOperate
    {
        private readonly DbProvider dbProvider;
        private bool isAutoClose;
        private DbConnection cnn;
        //private DbConnection TranCnn;
        private bool isTran = false;
        private DbTransaction transaction;
        private DbConnection Cnn
        {
            get
            {
                if (cnn == null)
                {
                    cnn = dbProvider.Cnn;
                }
                if (cnn.State != ConnectionState.Open)
                {
                    cnn.Open();
                }
                return cnn;
            }
        }

        public bool TestConnection(out string result)
        {
            try
            {
                if (cnn == null)
                {
                    cnn = dbProvider.Cnn;
                }
                cnn.Open();

                result = cnn.Database;
                return true;

            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return false;
        }

        public DbOperate(bool isAutoClose = true)
        {
            dbProvider = new DbProvider();
            this.isAutoClose = isAutoClose;
        }
        public DbOperate(string cnnName, bool isAutoClose = true)
        {
            dbProvider = new DbProvider(cnnName);
            this.isAutoClose = isAutoClose;
        }
        public DbOperate(string cnnStr, string providerName, bool isAutoClose = true)
        {
            dbProvider = new DbProvider(cnnStr, providerName);
            this.isAutoClose = isAutoClose;
        }
        public bool ExecuteNonQueryTran(Action action)
        {
            bool result;
            var isAutoCloseTemp = isAutoClose;

            isAutoClose = false;

            using (var dbTransaction = Cnn.BeginTransaction())
            {
                try
                {
                    transaction = dbTransaction;
                    isTran = true;
                    action.Invoke();
                    dbTransaction.Commit();
                    result = true;
                }
                catch (Exception ex)
                {
                    dbTransaction.Rollback();
                    result = false;
                }
                finally
                {
                    isTran = false;
                    transaction = null;
                }
            }

            isAutoClose = isAutoCloseTemp;

            if (!isAutoClose) return result;
            Cnn.Close(); ;

            return result;
        }
        public object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            object result;
            using (var dbCommand = CreateCommand(cmdText, cmdType, paras))
            {
                dbCommand.CommandTimeout = 60;
                var obj = dbCommand.ExecuteScalar();
                result = obj;
            }

            if (isAutoClose)
            {
                Cnn.Close();
            }

            return result;
        }
        public int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            int result;
            using (var dbCommand = CreateCommand(cmdText, cmdType, paras))
            {
                dbCommand.CommandTimeout = 60;
                var obj = dbCommand.ExecuteNonQuery();
                result = obj;
            }

            if (isAutoClose)
            {
                Cnn.Close();
            }

            return result;
        }
        public DbDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            DbConnection dbConnection = Cnn;
            DbCommand dbCommand = CreateCommand(cmdText, cmdType, paras);
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
            return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public T ReaderToModel<T>(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            DbDataReader dbDataReader = ExecuteReader(cmdText, cmdType, paras);
            T result;
            using (dbDataReader)
            {
                if (dbDataReader.Read())
                {
                    List<string> list = new List<string>(dbDataReader.FieldCount);
                    for (int i = 0; i < dbDataReader.FieldCount; i++)
                    {
                        list.Add(dbDataReader.GetName(i).ToLower());
                    }
                    var t = Activator.CreateInstance<T>();
                    var properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    for (int j = 0; j < properties.Length; j++)
                    {
                        var propertyInfo = properties[j];
                        if (!list.Contains(propertyInfo.Name.ToLower())) continue;
                        if (!IsNullOrDbNull(dbDataReader[propertyInfo.Name]))
                        {
                            propertyInfo.SetValue(t, HackType(dbDataReader[propertyInfo.Name], propertyInfo.PropertyType), null);
                        }
                    }
                    result = t;

                    if (isAutoClose)
                    {
                        Cnn.Close();
                    }

                    return result;
                }
            }

            result = default(T);

            if (isAutoClose)
            {
                Cnn.Close();
            }

            return result;
        }
        public List<T> ReaderToList<T>(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            DbDataReader dbDataReader = ExecuteReader(cmdText, cmdType, paras);
            List<T> result;
            using (dbDataReader)
            {
                List<string> list = new List<string>(dbDataReader.FieldCount);
                for (int i = 0; i < dbDataReader.FieldCount; i++)
                {
                    list.Add(dbDataReader.GetName(i).ToLower());
                }
                List<T> list2 = new List<T>();
                while (dbDataReader.Read())
                {
                    var t = Activator.CreateInstance<T>();
                    PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    for (int j = 0; j < properties.Length; j++)
                    {
                        PropertyInfo propertyInfo = properties[j];
                        if (list.Contains(propertyInfo.Name.ToLower()))
                        {
                            if (!IsNullOrDbNull(dbDataReader[propertyInfo.Name]))
                            {
                                propertyInfo.SetValue(t, HackType(dbDataReader[propertyInfo.Name], propertyInfo.PropertyType), null);
                            }
                        }
                    }
                    list2.Add(t);
                }
                result = list2;
            }


            if (isAutoClose)
            {
                Cnn.Close();
            }

            return result;
        }
        public List<T> ReaderToList<T>(DataSet ds)
        {
            DbDataReader dbDataReader = ds.CreateDataReader();
            List<T> result;
            using (dbDataReader)
            {
                List<string> list = new List<string>(dbDataReader.FieldCount);
                for (int i = 0; i < dbDataReader.FieldCount; i++)
                {
                    list.Add(dbDataReader.GetName(i).ToLower());
                }
                List<T> list2 = new List<T>();
                while (dbDataReader.Read())
                {
                    T t = Activator.CreateInstance<T>();
                    PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                    for (int j = 0; j < properties.Length; j++)
                    {
                        PropertyInfo propertyInfo = properties[j];
                        if (list.Contains(propertyInfo.Name.ToLower()))
                        {
                            if (!IsNullOrDbNull(dbDataReader[propertyInfo.Name]))
                            {
                                propertyInfo.SetValue(t, HackType(dbDataReader[propertyInfo.Name], propertyInfo.PropertyType), null);
                            }
                        }
                    }
                    list2.Add(t);
                }
                result = list2;
            }
            return result;
        }
        private object HackType(object value, Type type)
        {
            object result;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    result = null;
                    return result;
                }
                NullableConverter nullableConverter = new NullableConverter(type);
                type = nullableConverter.UnderlyingType;
            }
            if (type.FullName.TrimEnd().ToLower() == "system.bool" || type.FullName.TrimEnd().ToLower() == "system.boolean")
            {
                try
                {
                    result = Convert.ChangeType(value, type);
                }
                catch
                {
                    if (int.Parse(value.ToString()) == 0)
                        result = true;
                    else if (int.Parse(value.ToString()) == 1)
                        result = false;
                    else result = false;
                }
            }
            else
            {
                result = Convert.ChangeType(value, type);
            }
            return result;
        }
        private bool IsNullOrDbNull(object obj)
        {
            return obj is DBNull || string.IsNullOrEmpty(obj.ToString());
        }
        public DataTable GetDataSet(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            var dataSet = GetDataDataSet(cmdText, cmdType, paras);
            var result = dataSet.Tables.Count == 1 ? dataSet.Tables[0] : new DataTable();

            if (string.IsNullOrEmpty(result.TableName))
            {
                result.TableName = Guid.NewGuid().ToString();
            }

            return result;
        }
        public DataSet GetDataDataSet(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            DataSet result;
            using (var dbCommand = CreateCommand(cmdText, cmdType, paras))
            {
                using (var dbDataAdapter = dbProvider.Adapter)
                {
                    dbDataAdapter.SelectCommand = dbCommand;
                    var dataSet = new DataSet();
                    dbDataAdapter.Fill(dataSet);

                    if (isAutoClose)
                    {
                        Cnn.Close();
                    }

                    result = dataSet;
                }
            }

            return result;
        }
        private DbCommand CreateCommand(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras)
        {
            var dbCommand = dbProvider.Cmd;
            dbCommand.Connection = Cnn;
            dbCommand.CommandText = cmdText;
            dbCommand.CommandType = cmdType;
            if (isTran)
            {
                dbCommand.Transaction = transaction;
            }
            dbCommand.Parameters.Clear();
            if (paras != null && paras.Length > 0)
            {
                //dbCommand.Parameters.AddRange(paras);
                foreach (var dbParameter in paras)
                {
                    dbCommand.Parameters.Add((DbParameter)((ICloneable)dbParameter).Clone());
                }
            }
            return dbCommand;
        }
        public DbParameter CreateParameter(string field, DbType dbType, object value)
        {
            var dbParameter = dbProvider.Parameter;
            dbParameter.ParameterName = field.Trim().StartsWith("@") ? field : "@" + field;
            dbParameter.DbType = dbType;
            dbParameter.Value = value;
            return dbParameter;
        }
        public DbParameter CreateParameter(string field, object value)
        {

            var dbParameter = dbProvider.Parameter;
            dbParameter.ParameterName = field.Trim().StartsWith("@") ? field : "@" + field;
            dbParameter.DbType = DbType.String;
            dbParameter.Value = value;
            return dbParameter;
        }
        public void Dispose()
        {
            try
            {
                cnn?.Close();
            }
            catch
            {
            }
        }
        ~DbOperate()
        {
            Dispose();
        }
    }

    internal class DbProvider
    {
        private readonly DbProviderFactory provider;
        private readonly ConnectionStringSettings cnnStringSettings;
        private readonly string cnnStr;
        public DbProvider()
        {
            cnnStringSettings = ConfigurationManager.ConnectionStrings["cnnStr"];
            if (provider == null)
            {
                provider = DbProviderFactories.GetFactory(cnnStringSettings.ProviderName);
            }
            cnnStr = cnnStringSettings.ConnectionString;
        }

        public DbProvider(string cnnName)
        {
            cnnStringSettings = ConfigurationManager.ConnectionStrings[cnnName];
            if (provider == null)
            {
                provider = DbProviderFactories.GetFactory(cnnStringSettings.ProviderName);
            }
            cnnStr = cnnStringSettings.ConnectionString;
        }
        public DbProvider(string cnnStr, string providerName)
        {
            this.cnnStr = cnnStr;
            this.provider = DbProviderFactories.GetFactory(providerName);
        }

        public DbConnection Cnn
        {
            get
            {
                var cnn = provider.CreateConnection();
                cnn.ConnectionString = GetCnnStr();
                return cnn;
            }
        }

        public DbCommand Cmd => provider.CreateCommand();

        public DbDataAdapter Adapter => provider.CreateDataAdapter();

        public DbParameter Parameter => provider.CreateParameter();

        private string GetCnnStr()
        {
            return cnnStr;
        }
    }
}
