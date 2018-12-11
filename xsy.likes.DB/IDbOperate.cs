using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace xsy.likes.Db
{
    public interface IDbOperate
    {
        DbParameter CreateParameter(string field, object value);
        DbParameter CreateParameter(string field, DbType dbType, object value);
        void Dispose();
        bool TestConnection(out string result);
        int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        bool ExecuteNonQueryTran(Action action);
        DbDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        DataSet GetDataDataSet(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        DataTable GetDataSet(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        List<T> ReaderToList<T>(DataSet ds);
        List<T> ReaderToList<T>(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
        T ReaderToModel<T>(string cmdText, CommandType cmdType = CommandType.Text, params DbParameter[] paras);
    }
}