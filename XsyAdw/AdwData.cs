using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using xsy.likes.Base;
using xsy.likes.Db;
using xsy.likes.Log;
using xsy.likes.pwmd;

namespace XsyAdw
{
    public class AdwData : pwapi
    {
        #region 配置文件和参数
        private const string UserId = "13262123897";     //用户名
        private const string Pwd = "nan5211314.";
        private const string Code = "q4yDA8u1";          //安全令牌
        private const string TokenClientId = "870f66122445c021ea3ef84f8f1a0d8d";
        private const string TokenClientSecret = "2e0fa98806f1784a4ce6d3b8b7da243c";
        private const string TokenRedirectUrl = "http://www.adwerpcrm.com";

        DbOperate db = new DbOperate();
        private const int countOneTime = 30;//每次取的个数

        public string logMsg = string.Empty;//全局Log对象，用于记录错误日志


        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public AdwData() : base(UserId, Pwd, Code, TokenClientId, TokenClientSecret, TokenRedirectUrl)
        {

        }

        #region   指标项替换
        //客户级别
        public string repalceCustomerLev(string id)
        {
            switch (id)
            {
                case "1": return "A(重点客户)";
                case "2": return "B(普通客户)";
                case "3": return "C(非优先客户)";
                default: return "";
            }
        }
        //合作状态
        public string replaceCoop(string id)
        {
            switch (id)
            { //1是 2否
                case "1": return "是";
                case "2": return "否";
                default: return "";
            }
        }


        //string customerreputation = jocus["dbcSelect2"].ToString();   
        //客户信誉  A1 B2 C3
        public string repalceCusrr(string id)
        {
            switch (id)
            {
                case "1": return "A";
                case "2": return "B";
                case "3": return "C";
                default: return "";
            }
        }

        //string busregistration = jocus["srcFlg"].ToString();  //工商注册  0否  1是
        public string repalceBus(string id)
        {
            switch (id)
            {
                case "1": return "否";
                case "0": return "是";
                default: return "";
            }
        }


        //付款方式替换为id
        public string repalcePayType(string id)
        {
            switch (id)
            {
                case "现金": return "1";
                case "银行": return "2";
                case "电汇": return "3";
                case "转账支票": return "4";
                case "银行汇票": return "5";
                case "承兑汇票": return "6";
                case "银行承兑汇票": return "7";
                case "商业承兑汇票": return "8";
                case "冲销费用": return "10";
                default: return "9";
            }
        }


        #endregion


        /// <summary>
        /// 日期转换为时间戳
        /// </summary>
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        #region 客户信息
        /// <summary>
        /// CRM到ERP同步客户信息
        /// </summary>
        public void CustomerBackToERP()
        {
            string updateSqls = string.Empty;
            string suid = string.Empty;
            try
            {
                string sqls = $"select id from account where dbcSelect4 = '2'";
                string resultjson = sqlSelect(sqls);

                JObject jo = (JObject)JsonConvert.DeserializeObject(resultjson);
                int ct = int.Parse(jo["count"].ToString());
                if (ct > 0)
                {
                    for (int a = 0; a < ct; a++)
                    {
                        string records = jo["records"].ToString();
                        JArray jArray = (JArray)JsonConvert.DeserializeObject(records);
                        string accountId = jArray[a]["id"].ToString();
                        //根据客户id获取客户明细
                        string customerInfo = CustomerSelect(accountId);
                        JObject jocus = (JObject)JsonConvert.DeserializeObject(customerInfo);

                        //客户所有人
                        string ownerId = jocus["ownerId"].ToString();
                        string owneridhson = UserSelectById(ownerId);
                        JObject ojow = (JObject)JsonConvert.DeserializeObject(owneridhson);
                        string ownname = ojow["name"].ToString();
                        string id = jocus["id"].ToString();     //crm客户id
                        string customername = jocus["accountName"].ToString(); //客户名称
                        string detailedaddress = jocus["address"].ToString();      //地址
                        string customerlevel = repalceCustomerLev(jocus["level"].ToString());    //等级
                        string zipcode = jocus["zipCode"].ToString();       //邮编
                        string phone = jocus["phone"].ToString();   //电话
                        string fax = jocus["fax"].ToString();      //传真
                        string compurl = jocus["url"].ToString();   //公司网址
                        string sina = jocus["weibo"].ToString();      //微博
                        string industry = jocus["industryId"].ToString();   //行业
                        string totalnumber = jocus["employeeNumber"].ToString();   //总人数
                        string sales = jocus["annualRevenue"].ToString();       //销售额
                        string remaks = jocus["comment"].ToString();        //备注
                        string aflfunction = DepartNameGet(jocus["dimDepart"].ToString());        //所属部门
                        string legarepretive = jocus["dbcVarchar1"].ToString(); //法定代表人
                        string registeredcapital = jocus["dbcVarchar2"].ToString();  //注册资金
                        string setuptime = jocus["dbcDate1"].ToString(); //成立时间
                        string cooperativestate = replaceCoop(jocus["dbcSelect1"].ToString()); //合作状态  1是 2否
                        string customerreputation = repalceCusrr(jocus["dbcSelect2"].ToString());   //客户信誉  A1 B2 C3
                        string customerquality = jocus["dbcSelect3"].ToString();  //客户资质
                        string erpstatus = jocus["dbcSelect4"].ToString(); //是否回写到ERP  1是  2否  3错误
                        string customerno = jocus["dbcVarchar5"].ToString();  //ERP内码
                        string busregistration = repalceBus(jocus["srcFlg"].ToString());  //工商注册  0否  1是


                        updateSqls = $"if not exists(select * from dbo.customer where customerno = '{customerno}') 	"
                            + $"begin "
                            + $"insert into dbo.customer(customerno,customername,customerlevel,aflfunction,busregistration,industry,"
                            + $"detailedaddress,zipcode,phone,fax,compurl,sina,totalnumber,sales,remaks,legarepretive,registeredcapital,"
                            + $"setuptime,cooperativestate,customerreputation,customerquality,ownname,id)"
                            + $"values('{customerno}','{customername}','{customerlevel}','{aflfunction}','{busregistration}','{industry}','{detailedaddress}','{zipcode}','{phone}',"
                            + $" '{fax}','{compurl}','{sina}','{totalnumber}','{sales}','{remaks}','{legarepretive}','{registeredcapital}','{setuptime}','{cooperativestate}',"
                            + $"'{customerreputation}','{customerquality}','{ownname}','{id}');"
                            + $" end";

                        //更新到本地数据库
                        try
                        {
                            int st = db.ExecuteNonQuery(updateSqls, CommandType.Text);
                            //成功之后打标记以及写入日志
                            if (st > -2)
                            {

                                string jsoncus = "{\"id\":"
                                    + $"{accountId},"
                                    + "\"dbcSelect4\": \"1\""
                                    + "}";

                                CustomerUpdate(jsoncus);

                                suid += accountId + ",";

                            }
                        }
                        catch (Exception exception)
                        {
                            logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                            LogHelper.Error(logMsg);
                        }


                    }

                    LogHelper.Info($"客户信息{suid} 回写到ERP成功！");

                }
                else
                {

                    LogHelper.Info($"客户信息已经全部同步！");

                }


                }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }



        }

        //客户信息置标记
        public void CustomerBackAllTotwo()
        {
            bool flg = true;
            while (flg)
            {
                string updateSqls = string.Empty;
                string suid = string.Empty;
                try
                {
                    string sqls = $"select id from account where dbcSelect4 = '1'  limit 0,300";
                    // string id = sqlSelectOutId(sqls);
                    string ids = sqlSelect(sqls);

                    JObject jo = (JObject)JsonConvert.DeserializeObject(ids);
                    int ct = int.Parse(jo["count"].ToString());
                    if (ct == 0)
                    {
                        flg = false;
                        return;
                    }
                    else if (ct > 0)
                    {
                        string records = jo["records"].ToString();
                        JArray jArray = (JArray)JsonConvert.DeserializeObject(records);

                        for (int a = 0; a < ct; a++)
                        {
                            //获取相对应的客户信息id
                            string accountId = jArray[a]["id"].ToString();
                            string jsoncus = "{\"id\":"
                                           + $"{accountId},"
                                           + "\"dbcSelect4\": \"2\""
                                           + "}";
                            CustomerUpdate(jsoncus);

                        }
                        LogHelper.Info($"客户信息{suid}信息标记重置成功！");
                    }


                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex.ToString());
                }

            }

        }


        /// <summary>
        /// 修改
        /// </summary>
        public void CustomerBackToERPUpdate()
        {
            var sjc = db.ExecuteScalar("SELECT sjc FROM dbo.sjc where tablename = 'customer'");
            long timeStamp = ConvertDateTimeToInt(DateTime.Now);
            string suid = string.Empty;
            try
            {
                string sqls = $"select id from account where updatedAt > '{sjc}' and dbcSelect4 = '1' limit 0,300";
                string ids = sqlSelect(sqls);

                JObject jo = (JObject)JsonConvert.DeserializeObject(ids);
                int ct = int.Parse(jo["count"].ToString());
                if (ct > 0)
                {


                    string records = jo["records"].ToString();
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(records);

                    for (int a = 0; a < ct; a++)
                    {
                        //根据客户id获取客户明细
                        string customerInfo = CustomerSelect(jArray[a]["id"].ToString());
                        JObject jocus = (JObject)JsonConvert.DeserializeObject(customerInfo);


                        //客户所有人
                        string ownerId = jocus["ownerId"].ToString();
                        string owneridhson = UserSelectById(ownerId);
                        JObject ojow = (JObject)JsonConvert.DeserializeObject(owneridhson);
                        string ownname = ojow["name"].ToString();


                        //解析json获取参数
                        string id = jocus["id"].ToString();     //crm客户id
                        string customername = jocus["accountName"].ToString(); //客户名称
                        string detailedaddress = jocus["address"].ToString();      //地址
                        string customerlevel = repalceCustomerLev(jocus["level"].ToString());    //等级
                        string zipcode = jocus["zipCode"].ToString();       //邮编
                        string phone = jocus["phone"].ToString();   //电话
                        string fax = jocus["fax"].ToString();      //传真
                        string compurl = jocus["url"].ToString();   //公司网址
                        string sina = jocus["weibo"].ToString();      //微博
                        string industry = jocus["industryId"].ToString();   //行业
                        string totalnumber = jocus["employeeNumber"].ToString();   //总人数
                        string sales = jocus["annualRevenue"].ToString();       //销售额
                        string remaks = jocus["comment"].ToString();        //备注
                        string aflfunction = DepartNameGet(jocus["dimDepart"].ToString());        //所属部门
                        string legarepretive = jocus["dbcVarchar1"].ToString(); //法定代表人
                        string registeredcapital = jocus["dbcVarchar2"].ToString();  //注册资金
                        string setuptime = jocus["dbcDate1"].ToString(); //成立时间
                        string cooperativestate = replaceCoop(jocus["dbcSelect1"].ToString()); //合作状态  1是 2否
                        string customerreputation = repalceCusrr(jocus["dbcSelect2"].ToString());   //客户信誉  A1 B2 C3
                        string customerquality = jocus["dbcSelect3"].ToString();  //客户资质
                        string erpstatus = jocus["dbcSelect4"].ToString(); //是否回写到ERP  1是  2否  3错误
                        string customerno = jocus["dbcVarchar5"].ToString();  //ERP内码
                        string busregistration = repalceBus(jocus["srcFlg"].ToString());  //工商注册  0否  1是

                        string updateSqls = $"if exists(select * from dbo.customer  where customerno ='{customerno}') 	"
                                    + $"begin 	"
                                    + $"update dbo.customer customername = '{customername}',customerlevel = '{customerlevel}',"
                                    + $"aflfunction  = '{aflfunction}',busregistration = '{busregistration}',industry = '{industry}',"
                                    + $"detailedaddress = '{detailedaddress}',zipcode = '{zipcode}',phone = '{phone}',fax = '{fax}',"
                                    + $"compurl ='{compurl}',sina ='{sina}',totalnumber ='{totalnumber}',sales ='{sales}',remaks ='{remaks}',"
                                    + $"legarepretive ='{legarepretive}',registeredcapital ='{registeredcapital}',setuptime ='{setuptime}',"
                                    + $"cooperativestate ='{cooperativestate}',ownname = '{ownname}',hxtoerp = 1,customerreputation = '{customerreputation}',customerquality ='{customerquality}'"
                                    + $"where customerno = '{customerno}' end;";

                        try
                        {
                            int st = db.ExecuteNonQuery(updateSqls, CommandType.Text);


                        }
                        catch (Exception exception)
                        {
                            logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                            LogHelper.Error(logMsg);
                        }


                    }
                    db.ExecuteNonQuery($"update dbo.sjc set sjc = '{timeStamp}' where tablename = 'customer'", CommandType.Text);
                    LogHelper.Info($"客户信息{suid} 回写到ERP成功！");

                }
                else
                {

                    LogHelper.Info("最近没有业务员修改客户信息！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());
            }


        }

        #endregion

        #region 产品   
        //新增
        public void GoodsCreate()
        {
            try
            {
                var dataStr = $"select top {countOneTime} * from Inventory  where cInvStd in (select cinvstd from Dispatchlist where ddate >= '2018-01-01' GROUP BY cinvstd) and CreateStatus > 0  and CreateStatus < 10  order by CreateStatus";
                var tableGd = db.GetDataSet(dataStr, CommandType.Text);

                if (tableGd.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要上传的产品信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableGd.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        #region json

                        var json = "{\"belongId\":\"100588408\",\"record\":{"
                        + $"\"name\":\"{dataRow["tuno"]}\""      //产品图号
                        + $",\"customItem1__c\":\"{dataRow["cInvCCode"]}\""       //产品系列 cInvCCode
                        + $",\"customItem2__c\":\"{dataRow["cInvName"]}\""       //产品名称
                        + $",\"customItem3__c\":\"{dataRow["cInvStd"]}\""       //产品型号
                        //+ $",\"customItem4__c\":\"{dataRow["LoadingNote"]}\""       //单价不含税
                        //+ $",\"customItem5__c\":\"{dataRow["TaxCode"]}\""       //配套车型            
                        + $",\"customItem6__c\":\"{dataRow["dw"]}\""       //销售单位
                        //+ $",\"customItem7__c\":\"{dataRow["StoreStatus"]}\""       //产品状态
                        + $",\"customItem8__c\":\"{dataRow["cInvCName"]}\""       //系列名称
                        + "}}";
                        #endregion


                        string id;
                        var result = CustomCreate(json, out id);//调用销售易同步接口同步数据
                        if (id.Contains("数据重复"))
                        {
                            string q = $"select id from customize  where belongId = 100588408 AND name ='{dataRow["tuno"].ToString()}';";
                            string sid = sqlSelectOutId(q);

                            id = sid;
                            if (id.Equals("") || id.Length > 12)
                            {
                                result = false;
                            }
                            else
                            {
                                result = true;
                            }

                        }

                        #region 销售易上传结果处理
                        if (result)
                        {
                            successIds += $"{dataRow["tuno"]},";
                        }
                        else
                        {
                            failedIds += $"{dataRow["tuno"]},";
                        }

                        if (string.IsNullOrWhiteSpace(id))
                        {
                            //updataCount += 1;
                            updateSqls += $"update Inventory set CreateStatus = CreateStatus + 1 where tuno = '{dataRow["tuno"].ToString()}';";
                        }
                        else
                        {
                            updateSqls += $"update Inventory set CreateStatus = 0,id = '{id}' where tuno = '{dataRow["tuno"].ToString()}';";
                        }
                        #endregion
                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["cInvStd"]},";
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"产品信息同步图号为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableGd.Rows.Count
                       ? $"产品信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableGd.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"产品信息同步失败图号为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        //修改
        public void GoodsUpdate()
        {
            try
            {
                var dataStr = $"select top {countOneTime} * from Inventory  where cInvStd in (select cinvstd from Dispatchlist where ddate >= '2018-01-01' GROUP BY cinvstd) and updatestatus > 0  and updatestatus < 10  and id <> '' order by updatestatus";
                var tableGd = db.GetDataSet(dataStr, CommandType.Text);

                if (tableGd.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要更改的产品信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableGd.Rows)
                {
                    try
                    {

                        #region json

                        var json = "{"
                       + $"\"id\":\"{dataRow?["Id"] ?? ""}\""      //id
                        + $",\"name\":\"{dataRow["tuno"]}\""      //产品编号
                        + $",\"customItem1__c\":\"{dataRow["cInvCCode"]}\""       //产品系列 cInvCCode
                        + $",\"customItem2__c\":\"{dataRow["cInvName"]}\""       //产品名称
                        + $",\"customItem3__c\":\"{dataRow["cInvStd"]}\""       //产品型号
                        //+ $",\"customItem4__c\":\"{dataRow["LoadingNote"]}\""       //单价不含税
                        //+ $",\"customItem5__c\":\"{dataRow["TaxCode"]}\""       //配套车型            
                        + $",\"customItem6__c\":\"{dataRow["dw"]}\""       //销售单位
                        //+ $",\"customItem7__c\":\"{dataRow["StoreStatus"]}\""       //产品状态
                        + $",\"customItem8__c\":\"{dataRow["cInvCName"]}\""       //系列名称
                       + "}";


                        #endregion


                        var result = CustomUpdate(json);

                        if (result)
                        {
                            successIds += $"{dataRow["Id"]},";
                            updateSqls += result
                          ? $"update Inventory set UpdateStatus = '0' where id ='{dataRow["Id"]}';"
                          : "";
                        }
                        else
                        {
                            failedIds += $"{dataRow["Id"]},";
                            updateSqls += $"update Inventory set UpdateStatus = UpdateStatus + 1 where id ='{dataRow["Id"]}';";
                        }


                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["Id"]},";
                        LogHelper.Error($"但在更新数据库时失败，失败原因：{exception.Message}");
                    }

                }

                var logMsg = $"产品信息更新的Id为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableGd.Rows.Count
                       ? $"产品信息更新失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableGd.Rows.Count}个。失败的Id为{failedIds}；成功的Id为{successIds}。"
                       : $"产品信息更新失败,失败的Id为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);

            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }
        #endregion

        #region 库存   
        //新增
        public void StockCreate()
        {
            try
            {
                var dataStr = $"select top {countOneTime} * from stock"
                            + " where CreateStatus > 0  and CreateStatus < 10 and cwhname <> '异地总仓'  and tuno in(select tuno from Inventory where id <> '' and id is not null) order by CreateStatus";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无新的库存信息！");
                    return;
                }



                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        string gidq = $"select id from customize  where belongId = 100588408 AND name ='{dataRow["tuno"].ToString()}';";
                        string goodid = sqlSelectOutId(gidq);

                        if (!string.IsNullOrEmpty(goodid))
                        {
                            #region json
                            var json = "{\"belongId\":\"100564845\",\"record\":{"
                            + $"\"name\":\"{dataRow["cInvName"]}\""      //产品名称
                            //+ $",\"ownerId\":\"{ownerId}\""       //所有人
                            + $",\"customItem2__c\":\"{dataRow["cinvstd"]}\""       //产品型号
                            + $",\"customItem16__c\":\"{(int)Convert.ToSingle(dataRow["iQuantity"].ToString())}\""       //可用
                            + $",\"customItem17__c\":\"{(int)Convert.ToSingle(dataRow["fInQuantity"].ToString())}\""       //待入库数量
                            + $",\"customItem18__c\":\"{(int)Convert.ToSingle(dataRow["fOutQuantity"].ToString())}\""       //待发货数量
                            + $",\"customItem4__c\":\"{dataRow["dw"]}\""       //单位           
                            + $",\"customItem5__c\":\"{dataRow["cinvCname"]}\""       //产品系列
                            + $",\"customItem8__c\":\"{goodid}\""       //图号关联
                            + $",\"customItem14__c\":\"{dataRow["tuno"].ToString()}\""       //图号id
                            + $",\"customItem13__c\":\"{dataRow["cwhname"]}\""       //仓库名称
                            + $",\"customItem15__c\":\"{dataRow["fromerpno"]}\""       //账套
                            + "}}";
                            #endregion


                            string id;
                            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据
                            if (id.Contains("数据重复"))
                            {
                                string q = $"select id from customize where belongId = 100564845 AND customItem14__c ='{dataRow["tuno"].ToString()}' and customItem13__c = '{dataRow["cwhname"].ToString()}' and customItem15__c = '{dataRow["fromerpno"].ToString()}';";
                                string sid = sqlSelectOutId(q);

                                id = sid;
                                if (id.Equals("") || id.Length > 12)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }

                            }

                            #region 销售易上传结果处理
                            if (result)
                            {
                                successIds += $"{dataRow["tuno"]} + {dataRow["cwhname"]},";
                            }
                            else
                            {
                                failedIds += $"{dataRow["tuno"]} + {dataRow["cwhname"]},";
                            }

                            if (string.IsNullOrWhiteSpace(id))
                            {
                                //updataCount += 1;
                                updateSqls += $"update stock set CreateStatus = CreateStatus + 1 where tuno = '{dataRow["tuno"]}' and cwhname = '{dataRow["cwhname"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            }
                            else
                            {
                                updateSqls += $"update stock set CreateStatus = 0,id = '{id}' where tuno = '{dataRow["tuno"]}' and cwhname = '{dataRow["cwhname"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            }
                            #endregion
                        }
                        else
                        {
                            LogHelper.Info($"产品还未上传，无法上传库存!");
                            updateSqls += $"update stock set CreateStatus = CreateStatus + 1 where tuno = '{dataRow["tuno"]}' and cwhname = '{dataRow["cwhname"]}' and fromerpno = '{dataRow["fromerpno"]}';";

                        }

                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["tuno"]} + {dataRow["cwhname"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"库存信息同步为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"库存信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"库存信息同步,失败库存信息名称为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }


        //修改
        public void StockUpdate()
        {
            try
            {
                var dataStr = $"select top {4 * countOneTime} * from stock  where (id is not null and id <> '') and tuno in(select tuno from Inventory where id <> '' and id is not null) and updatestatus > 0  and UpdateStatus < 10;";

                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要修改的的库存信息！");
                    return;
                }

                ////产品id
                //var gdSql = string.Join(" union ", tableSt.Rows.Cast<DataRow>().Select(dataRow =>
                //{
                //    string result = $"select * from Inventory where tuno ='{dataRow["tuno"]}' and id is not null and id <> ''";

                //    return result;
                //}));

                //var tableGd = db.GetDataTable(gdSql, CommandType.Text);



                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        //var goodsid = tableGd.Rows.Cast<DataRow>().FirstOrDefault(row => row["tuno"].ToString() == dataRow["tuno"].ToString());
                        //string goodid = goodsid?["Id"].ToString();
                        string gidq = $"select id from customize  where belongId = 100588408 AND name ='{dataRow["tuno"].ToString()}';";
                        string goodid = sqlSelectOutId(gidq);
                        if (!string.IsNullOrEmpty(goodid))
                        {
                            #region json

                            var json = "{"
                            + $"\"id\":\"{dataRow["id"]}\""
                            + $",\"name\":\"{dataRow["cInvName"]}\""      //产品名称
                            //+ $",\"ownerId\":\"{ownerId}\""       //所有人
                            + $",\"customItem2__c\":\"{dataRow["cinvstd"]}\""       //产品型号
                            + $",\"customItem16__c\":\"{(int)Convert.ToSingle(dataRow["iQuantity"].ToString())}\""       //可用
                            + $",\"customItem17__c\":\"{(int)Convert.ToSingle(dataRow["fInQuantity"].ToString())}\""       //待入库数量
                            + $",\"customItem18__c\":\"{(int)Convert.ToSingle(dataRow["fOutQuantity"].ToString())}\""       //待发货数量
                            + $",\"customItem4__c\":\"{dataRow["dw"]}\""       //单位           
                            + $",\"customItem5__c\":\"{dataRow["cinvCname"]}\""       //产品系列
                            + $",\"customItem8__c\":\"{goodid}\""       //图号关联
                            + $",\"customItem14__c\":\"{dataRow["tuno"]}\""       //图号id
                            + $",\"customItem13__c\":\"{dataRow["cwhname"]}\""       //仓库名称
                            + $",\"customItem15__c\":\"{dataRow["fromerpno"]}\""       //账套
                            + "}";
                            #endregion

                            var result = CustomUpdate(json);//调用销售易同步接口同步数据

                            #region 销售易上传结果处理
                            if (result)
                            {
                                successIds += $"{dataRow["id"]},";
                                updateSqls += $"update stock set Updatestatus = 0 where id = '{dataRow["id"]}';";
                            }
                            else
                            {
                                failedIds += $"{dataRow["id"]},";
                                updateSqls += $"update stock set Updatestatus = Updatestatus + 1 where id = '{dataRow["id"]}' ;";
                            }
                            #endregion
                        }


                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["id"]} ,";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"库存信息更新为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"库存信息更新失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"库存信息更新,失败库存信息名称为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        #endregion         

        #region 发货信息
        //新增
        public void DispatchlistCreate()
        {
            try
            {

                logMsg = string.Empty;

                var nore = db.ExecuteNonQuery("update Dispatchlist set tuno = cInvCode where tuno is  null or tuno = ''; ");
                var dataStr = $"select top {countOneTime * 0.5} dlid,dDate,ccusname,fromerpno from Dispatchlist  where ccusname is not null and ccusname <> '' and ddate >= '2018-01-01'"
                    + "and  CreateStatus > 0  and CreateStatus < 10 "
                    + " group by dlid,dDate,ccusname,fromerpno";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无新的发货信息！");
                    return;
                }
                DateTime dt = DateTime.Now;
                //当前年份
                string nyear = dt.Year.ToString();
                //当前月份
                string nmo = dt.Month.ToString();

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {
                        //查询客户所有人
                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["ccusname"]}'");



                        if (!string.IsNullOrEmpty(ownerIdJson) && ownerIdJson.Contains("id"))
                        {

                            string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2  - ownerIdJson.IndexOf("id") - 4);

                            //如果为空归给孔经理
                            string ownerId = !ownerIdJson.Contains("ownerId") ? "847069" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);
                            
                            
                            #region json

                            var json = "{\"belongId\":\"100593372\",\"record\":{"
                            + $"\"name\":\"{dataRow["DLID"]}\""      //发货单号
                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                            + $",\"customItem2__c\":\"{dataRow["dDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //发货日期
                            + $",\"customItem5__c\":\"{custid}\""       //客户 
                            + $",\"customItem10__c\":\"{dataRow["fromerpno"]}\""       //账套
                            + "}}";
                            #endregion
                            string id;
                            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                            if (id.Contains("数据重复"))
                            {
                                string q = $"select id from customize where belongId = 100593372 AND name ='{dataRow["DLID"].ToString()}' and customItem10__c = '{dataRow["fromerpno"]}' limit 0,1;";
                                string sid = sqlSelectOutId(q);

                                id = sid;
                                if (id.Equals("") || id.Length > 12)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }

                            }

                            #region 销售易上传结果处理
                            if (result)
                            {
                                successIds += $"{dataRow["DLID"]},";
                                //select a.*,b.cInvName from Dispatchlist a,Inventory b where DLID = '1000028922' and a.cInvCode = b.cInvCode
                                string prsql = $"select * from Dispatchlist where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}' ";
                                //string prsql = $"select a.*,b.cInvName from Dispatchlist a,Inventory b where DLID = '{dataRow["DLID"]}' and a.cInvStd = b.tuno";
                                var tableMx = db.GetDataSet(prsql, CommandType.Text);
                                foreach (DataRow dataRowmx in tableMx.Rows)
                                {
                                    string cpq = $"select id from customize where belongId = 100588408 AND name ='{dataRowmx["tuno"]}';";
                                    string cpid = sqlSelectOutId(cpq);


                                    #region prjson

                                    var prjson = "{\"belongId\":\"100593574\",\"record\":{"
                                    + $"\"name\":\"{dataRowmx["tuno"]}\""      //产品图号
                                    + $",\"ownerId\":\"{ownerId}\""       //所有人
                                    + $",\"customItem1__c\":\"{dataRowmx["cInvName"]}\""       //产品名称
                                    + $",\"customItem14__c\":\"{(int)Convert.ToSingle(dataRowmx["iQuantity"].ToString())}\""       //数量(int)Convert.ToSingle(dataRowmx["iQuantity"].ToString())
                                    + $",\"customItem9__c\":\"{dataRowmx["dw"]}\""       //单位
                                    + $",\"customItem4__c\":\"{dataRowmx["cInvStd"]}\""       //产品型号
                                    + $",\"customItem5__c\":\"{id}\""       //发货单id
                                    + $",\"customItem6__c\":\"{dataRowmx["cWhName"]}\""       //仓储地址
                                    + $",\"customItem7__c\":\"{dataRowmx["cWhAddress"]}\""       //客户指定仓储地址
                                    + $",\"customItem8__c\":\"{dataRowmx["dlxqid"]}\""       //明细id    
                                    + $",\"customItem10__c\":\"{dataRowmx["cinvcname"]}\""     //大类
                                    + $",\"customItem15__c\":\"{dataRowmx["fromerpno"]}\""
                                    + $",\"customItem12__c\":\"{dataRowmx["bk"]}\""     //板块
                                    + $",\"customItem13__c\":\"{cpid}\""     //产品关联id
                                    + "}}";
                                    #endregion

                                    string pid = string.Empty;
                                    var res = CustomCreate(prjson, out pid);
                                    if (pid.Contains("数据重复"))
                                    {
                                        string q = $"select id from customize where belongId = 100593574 AND customItem8__c ='{dataRowmx["dlxqid"].ToString()}' and customItem15__c = '{dataRowmx["fromerpno"]}';";
                                        string psid = sqlSelectOutId(q);

                                        pid = psid;
                                        if (id.Equals("") || id.Length > 12)
                                        {
                                            res = false;
                                        }
                                        else
                                        {
                                            res = true;
                                            pid = psid;
                                        }

                                    }

                                    if (res)
                                    {
                                        successIds += $"{dataRowmx["dlxqid"]} + {dataRow["fromerpno"]},";
                                        //updateSqls += $"update Dispatchlist set CreateStatus = 0,zid = '{id}',id = '{pid}' where dlxqid = '{dataRowmx["dlxqid"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                        db.ExecuteNonQuery($"update Dispatchlist set CreateStatus = 0,zid = '{id}',id = '{pid}' where dlxqid = '{dataRowmx["dlxqid"]}' and fromerpno = '{dataRow["fromerpno"]}';");
                                    }
                                    else
                                    {
                                        failedIds += $"{dataRow["DLID"]}+ {dataRow["fromerpno"]},";
                                        //updateSqls += $"update Dispatchlist set CreateStatus = CreateStatus + 1 where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                        db.ExecuteNonQuery($"update Dispatchlist set CreateStatus = CreateStatus + 1 where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}';");
                                        logMsg += pid;
                                    }


                                }


                            }
                            else
                            {
                                failedIds += $"{dataRow["DLID"]},";
                                updateSqls += $"update Dispatchlist set CreateStatus = CreateStatus + 1 where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            }


                            #endregion
                        }
                        else
                        {
                            LogHelper.Info($"查找不到对应客户:{dataRow["ccusname"]}，无法上传发货信息!");
                            updateSqls += $"update Dispatchlist set CreateStatus = CreateStatus + 1 where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}';";

                        }

                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["DLID"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"发货信息同步名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"发货信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"发货信息同步,失败发货信息名称为{failedIds}。";
                }

                //更新本地数据
                if (!string.IsNullOrEmpty(updateSqls))
                {
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);
                    }
                    catch (Exception exception)
                    {
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }


                }
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }


        //修改
        public void DispatchlistUpdate()
        {
            try
            {
                logMsg = string.Empty;
                var dataStr = $"select top 5 dlid,dDate,ccusname,zid,fromerpno from Dispatchlist where (zid <> '' and zid is not null) and dlxqid in(select dlxqid from Dispatchlist where UpdateStatus > 0  and UpdateStatus < 10) "
                            + "and ccusname in (select ccusname from Customer where id <> '' and id is not null) and (ccusname is not null and ccusname <> '') and ddate >= '2018-07-01' " 
                            + "group by dlid,dDate,ccusname,zid,fromerpno;";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);
               
                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要修改的发货信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                DateTime dt = DateTime.Now;
                //当前年份
                string nyear = dt.Year.ToString();
                //当前月份
                string nmo = dt.Month.ToString();
                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        ////查询客户id
                        //string cusidsql = $"select id from account where accountName = '{dataRow["ccusname"]}' ";
                        //string custid = sqlSelectOutId(cusidsql);
                        //查询客户所有人
                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["ccusname"]}'");


                        string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                        //string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);



                        #region json

                        var json = "{"
                        + $"\"id\":\"{dataRow["zid"]}\""
                            + $",\"name\":\"{dataRow["DLID"]}\""      //发货单号
                            //+ $",\"ownerId\":\"{ownerId}\""       //所有人
                            + $",\"customItem2__c\":\"{dataRow["dDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //发货日期                                                                                               
                            + $",\"customItem5__c\":\"{custid}\""       //客户
                            + $",\"customItem10__c\":\"{dataRow["fromerpno"]}\""       //账套
                        + "}";
                        #endregion

                        var result = CustomUpdate(json);//调用销售易同步接口同步数据

                        #region 销售易上传结果处理
                        if (result)
                        {
                            successIds += $"{dataRow["DLID"]},";
                            //select a.*,b.cInvName from Dispatchlist a,Inventory b where DLID = '1000028922' and a.cInvCode = b.cInvCode
                            string prsql = $"select * from Dispatchlist where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}' ";
                            var tableMx = db.GetDataSet(prsql, CommandType.Text);
                            foreach (DataRow dataRowmx in tableMx.Rows)
                            {
                                string cpq = $"select id from customize where belongId = 100588408 AND name ='{dataRowmx["tuno"]}';";
                                string cpid = sqlSelectOutId(cpq);


                                #region prjson

                                var prjson = "{"
                                        + $"\"id\":\"{dataRowmx["id"]}\""
                                    + $",\"name\":\"{dataRowmx["tuno"]}\""      //产品图号
                                    + $",\"customItem1__c\":\"{dataRowmx["cInvName"]}\""       //产品名称
                                    + $",\"customItem14__c\":\"{(int)Convert.ToSingle(dataRowmx["iQuantity"].ToString())}\""       //数量dataRowmx["iQuantity"].ToString().Substring(0, dataRowmx["iQuantity"].ToString().IndexOf("."))
                                    + $",\"customItem9__c\":\"{dataRowmx["dw"]}\""       //单位
                                    + $",\"customItem4__c\":\"{dataRowmx["cInvStd"]}\""       //产品型号
                                    + $",\"customItem5__c\":\"{dataRow["zid"]}\""       //发货单id
                                    + $",\"customItem6__c\":\"{dataRowmx["cWhName"]}\""       //仓储地址
                                    + $",\"customItem7__c\":\"{dataRowmx["cWhAddress"]}\""       //客户指定仓储地址
                                    + $",\"customItem8__c\":\"{dataRowmx["dlxqid"]}\""       //明细id
                                    + $",\"customItem10__c\":\"{dataRowmx["cinvcname"]}\""     //大类
                                    + $",\"customItem11__c\":\"{dataRowmx["cInvStd"].ToString() + "<" + dataRow["ccusname"].ToString() + "<" + nyear + "-" + nmo}\""
                                    + $",\"customItem12__c\":\"{dataRowmx["bk"]}\""     //板块
                                    + $",\"customItem13__c\":\"{cpid}\""     //关联产品
                                    + $",\"customItem15__c\":\"{dataRow["fromerpno"]}\""
                                    + "}";
                                #endregion

                                var res = CustomUpdate(prjson);//调用销售易同步接口同步数据

                                if (res)
                                {
                                    successIds += $"{dataRowmx["dlxqid"]} + {dataRow["fromerpno"]},";
                                    updateSqls += $"update Dispatchlist set Updatestatus = 0 where dlxqid = '{dataRowmx["dlxqid"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                }
                                else
                                {
                                    failedIds += $"{dataRowmx["dlxqid"]} + {dataRow["fromerpno"]},";
                                    updateSqls += $"update Dispatchlist set Updatestatus = Updatestatus + 1 where dlxqid = '{dataRowmx["dlxqid"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            failedIds += $"{dataRow["DLID"]},";
                            updateSqls += $"update Dispatchlist set Updatestatus = Updatestatus + 1 where DLID = '{dataRow["DLID"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                        }



                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["DLID"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg += $"发货信息修改名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"发货信息修改失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"发货信息修改,失败发货信息名称为{failedIds}。";
                }

                //更新本地数据
                if (!string.IsNullOrEmpty(updateSqls))
                {
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);
                    }
                    catch (Exception exception)
                    {
                        logMsg += $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }


                }


                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        #endregion

        #region 开票信息
        //新增
        public void SaleBillVouchCreate()
        {
            try
            {
                logMsg = string.Empty;
                var dataStr = $"select top 5 cSBVCode,ccusname,dDate,cVouchName,fromerpno,sum(iSum) as sumje from SaleBillVouch  where  samxid in(select samxid from SaleBillVouch where CreateStatus > 0  and CreateStatus < 10) and ccusname "
                            + "in (select ccusname from Customer where id <> '' and id is not null) and ddate >= '2018-07-01 00:00:00.000'  group by cSBVCode,ccusname,dDate,cVouchName,fromerpno order by ddate desc;";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);
               
                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无新的开票信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        //查询客户所有人
                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["ccusname"]}'");

                            if (!string.IsNullOrEmpty(ownerIdJson) && ownerIdJson.Contains("id"))
                            {
                                string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                                string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);

                                #region json

                                var json = "{\"belongId\":\"100539205\",\"record\":{"
                                + $"\"name\":\"{dataRow["cSBVCode"]}\""      //发票号
                                + $",\"customItem17__c\":\"{custid}\""       //客户
                                + $",\"ownerId\":\"{ownerId}\""       //所有人
                                + $",\"customItem18__c\":\"{dataRow["dDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //开票日期
                                + $",\"customItem19__c\":\"{dataRow["cVouchName"]}\""       //开票类型
                                + $",\"customItem20__c\":\"{dataRow["sumje"]}\""       //开票总额
                                + $",\"customItem21__c\":\"{dataRow["fromerpno"]}\""       //账套
                                + "}}";

                                #endregion
                                string id;
                                var result = CustomCreate(json, out id);//调用销售易同步接口同步数据
                                
                                if (id.Contains("数据重复"))
                                {
                                    string q = $"select id from customize where belongId = 100539205 AND name ='{dataRow["cSBVCode"].ToString()}' and customItem21__c = '{dataRow["fromerpno"]}' limit 0,1;";
                                    string sid = sqlSelectOutId(q);

                                    id = sid;
                                    if (id.Equals("") || id.Length > 12)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        result = true;
                                    }

                                }

                                #region 销售易上传结果处理
                                if (result)
                                {
                                    successIds += $"{dataRow["cSBVCode"]},";
                                    string prsql = $"select * from SaleBillVouch where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                    var tableMx = db.GetDataSet(prsql, CommandType.Text);
                                    foreach (DataRow dataRowmx in tableMx.Rows)
                                    {
                                        #region prjson

                                        var prjson = "{\"belongId\":\"100598601\",\"record\":{"
                                        + $"\"name\":\"{dataRowmx["samxid"]}\""      //开票明细编号
                                        + $",\"ownerId\":\"{ownerId}\""       //所有人
                                        + $",\"customItem2__c\":\"{dataRowmx["iSum"]}\""       //开票金额
                                        + $",\"customItem3__c\":\"{dataRowmx["cinvstd"]}\""       //产品型号
                                        + $",\"customItem4__c\":\"{dataRowmx["cInvName"]}\""       //产品名称
                                        + $",\"customItem8__c\":\"{id}\""       //发票id
                                        + $",\"customItem5__c\":\"{dataRowmx["iQuantity"].ToString().Split('.')[0]}\""       //产品数量
                                        + $",\"customItem6__c\":\"{dataRowmx["iUnitPrice"]}\""       //不含税单价
                                        + $",\"customItem7__c\":\"{dataRowmx["iTaxUnitPrice"]}\""       //单价
                                        + $",\"customItem9__c\":\"{dataRowmx["dw"]}\""       //计量单位
                                        + $",\"customItem10__c\":\"{dataRowmx["fromerpno"]}\""       //账套
                                        + "}}";
                                        #endregion

                                        string pid = string.Empty;
                                        var res = CustomCreate(prjson, out pid);
                                         
                                        if (pid.Contains("数据重复"))
                                        {
                                            string q = $"select id from customize where belongId = 100598601 AND name ='{dataRowmx["samxid"].ToString()}' and customItem10__c = '{dataRow["fromerpno"]}';";
                                            string psid = sqlSelectOutId(q);

                                            pid = psid;
                                            if (id.Equals("") || id.Length > 12)
                                            {
                                                res = false;
                                            }
                                            else
                                            {
                                                res = true;
                                                pid = psid;
                                            }

                                        }

                                        if (res)
                                        {
                                        //updateSqls +=
                                        //    $"update SaleBillVouch set CreateStatus = 0,id = '{pid}',zid = '{id}' where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}';";

                                        db.ExecuteNonQuery($"update SaleBillVouch set CreateStatus = 0, id = '{pid}', zid = '{id}' where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}'; ");
                                        }
                                        else
                                        {

                                        //updateSqls += $"update SaleBillVouch set CreateStatus = CreateStatus + 1 where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                        db.ExecuteNonQuery($"update SaleBillVouch set CreateStatus = CreateStatus + 1 where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';");
                                        }


                                    }


                                }
                                else
                                {
                                    failedIds += $"{dataRow["cSBVCode"]},";
                                    updateSqls += $"update SaleBillVouch set CreateStatus = CreateStatus + 1 where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                }


                                #endregion
                            }
                            else
                            {
                            LogHelper.Info($"查找不到对应客户:{dataRow["ccusname"]}，无法上传开票信息!");
                                updateSqls += $"update SaleBillVouch set CreateStatus = CreateStatus + 1 where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
 

                            }

                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["cSBVCode"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg += $"开票信息同步名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"开票信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"开票信息同步失败送名称为{failedIds}。";
                }

                //更新本地数据
                if (!string.IsNullOrEmpty(updateSqls))
                {
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);

                    }
                    catch (Exception exception)
                    {
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }


                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }


        //修改
        public void SaleBillVouchUpdate()
        {
            try
            {
                logMsg = string.Empty;
                var dataStr = $"select top 15 cSBVCode,ccusname,dDate,cVouchName,zid,fromerpno,sum(iSum) as sumje from SaleBillVouch  where  samxid in(select samxid from SaleBillVouch where updatestatus > 0  and updatestatus < 10 and id is not null and id <> '') and ccusname "
                          + "in (select ccusname from Customer where id <> '' and id is not null) and ddate >= '2018-07-01' group by cSBVCode,ccusname,dDate,cVouchName,zid,fromerpno;";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要修改的开票信息！");
                    return;
                }



                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {
                        //查询客户所有人
                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["ccusname"]}'");

                        string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                        string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);

                        #region json

                        var json = "{"
                        + $"\"id\":\"{dataRow["zid"]}\""
                                + $",\"name\":\"{dataRow["cSBVCode"]}\""      //发票号
                                + $",\"customItem17__c\":\"{custid}\""       //客户
                                //+ $",\"ownerId\":\"{ownerId}\""       //所有人
                                + $",\"customItem18__c\":\"{dataRow["dDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //开票日期
                                + $",\"customItem19__c\":\"{dataRow["cVouchName"]}\""       //客户
                                + $",\"customItem20__c\":\"{dataRow["sumje"]}\""       //开票总额
                                + $",\"customItem21__c\":\"{dataRow["fromerpno"]}\""       //账套
                        + "}";
                        #endregion

                        var result = CustomUpdate(json);//调用销售易同步接口同步数据

                        #region 销售易上传结果处理
                        if (result)
                        {
                            successIds += $"{dataRow["cSBVCode"]},";
                            //select a.*,b.cInvName from Dispatchlist a,Inventory b where DLID = '1000028922' and a.cInvCode = b.cInvCode
                            string prsql = $"select * from SaleBillVouch where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            var tableMx = db.GetDataSet(prsql, CommandType.Text);
                            foreach (DataRow dataRowmx in tableMx.Rows)
                            {
                                #region prjson

                                var prjson = "{"
                                        + $"\"id\":\"{dataRowmx["id"]}\""
                                        + $",\"name\":\"{dataRowmx["samxid"]}\""      //开票明细编号
                                        + $",\"customItem2__c\":\"{dataRowmx["iSum"]}\""       //开票金额
                                        + $",\"customItem3__c\":\"{dataRowmx["cinvstd"]}\""       //产品型号
                                        + $",\"ownerId\":\"{ownerId}\""       //所有人
                                        + $",\"customItem4__c\":\"{dataRowmx["cInvName"]}\""       //产品名称
                                        + $",\"customItem8__c\":\"{dataRow["zid"]}\""       //发票id
                                        + $",\"customItem5__c\":\"{dataRowmx["iQuantity"].ToString().Split('.')[0]}\""       //产品数量
                                        + $",\"customItem6__c\":\"{dataRowmx["iUnitPrice"]}\""       //不含税单价
                                        + $",\"customItem7__c\":\"{dataRowmx["iTaxUnitPrice"]}\""       //单价
                                        + $",\"customItem9__c\":\"{dataRowmx["dw"]}\""       //计量单位
                                        + $",\"customItem10__c\":\"{dataRowmx["fromerpno"]}\""       //账套
                                        + "}";
                                #endregion

                                var res = CustomUpdate(prjson);//调用销售易同步接口同步数据

                                if (res)
                                {
                                    successIds += $"{dataRowmx["samxid"]},";
                                    //updateSqls += $"update SaleBillVouch set Updatestatus = 0 where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                    db.ExecuteNonQuery($"update SaleBillVouch set Updatestatus = 0 where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}';");
                                }
                                else
                                {
                                    failedIds += $"{dataRowmx["samxid"]},";
                                    //updateSqls += $"update SaleBillVouch set Updatestatus = Updatestatus + 1 where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                                    db.ExecuteNonQuery($"update SaleBillVouch set Updatestatus = Updatestatus + 1 where samxid = '{dataRowmx["samxid"]}' and fromerpno = '{dataRow["fromerpno"]}';");
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            failedIds += $"{dataRow["cSBVCode"]},";
                            updateSqls += $"update SaleBillVouch set Updatestatus = Updatestatus + 1 where cSBVCode = '{dataRow["cSBVCode"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                        }



                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["cSBVCode"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"开票信息修改名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"开票信息修改失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"开票信息修改失败名称为{failedIds}。";
                }

                //更新本地数据
                if (!string.IsNullOrEmpty(updateSqls))
                {
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);
                    }
                    catch (Exception exception)
                    {
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }


                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        #endregion 

        #region 回款信息
        //新增
        public void closebillCreate()
        {
            try
            {
                logMsg = string.Empty;
                var dataStr = $"select top {countOneTime} * from closebill  where CreateStatus > 0  and CreateStatus < 10 and  dVouchDate >= '2018-07-01 00:00:00.000'"
                                + "and ccusname in(select ccusname from Customer where id <> '' and id is not null)";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要上传的回款信息！");
                    return;
                }




                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {
                        string accountName = string.Empty;
                        if (dataRow["ccusname"].ToString().Length < 4 && dataRow["ccusname"].ToString().Length > 0)
                        {
                            accountName = "个人";
                        }
                        else
                        {
                            accountName = dataRow["ccusname"].ToString();

                        }

                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{accountName}'");

                        if (!string.IsNullOrEmpty(ownerIdJson) && ownerIdJson.Contains("id"))
                        {
                            string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                            string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);


                            #region json

                            var json = "{\"belongId\":\"100538750\",\"record\":{"
                            + $"\"name\":\"{dataRow["cvouchid"]}\""      //单据编号
                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                            + $",\"customItem7__c\":\"{custid}\""       //客户名称
                            + $",\"customItem2__c\":\"{dataRow["iAmount"]}\""       //付款金额
                            + $",\"customItem3__c\":\"{dataRow["dVouchDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //付款日期
                            + $",\"customItem4__c\":\"{repalcePayType(dataRow["cssname"].ToString())}\""       //付款方式 
                            + $",\"customItem10__c\":\"{dataRow["iId"]}\""       //主键erp判重        
                            + $",\"customItem5__c\":\"{dataRow["cdigest"]}\""       //摘要
                            + $",\"customItem11__c\":\"{dataRow["fromerpno"]}\""       //账套
                            + "}}";
                            #endregion


                            string id;
                            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据
                            if (id.Contains("数据重复"))
                            {
                                string q = $"select id from customize  where belongId = 100538750 AND customItem10__c ='{dataRow["iId"]}' and customItem11__c = '{dataRow["fromerpno"]}';";
                                string sid = sqlSelectOutId(q);

                                id = sid;
                                if (string.IsNullOrEmpty(id) || id.Length > 12)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }

                            }

                            #region 销售易上传结果处理
                            if (result)
                            {
                                successIds += $"{dataRow["iId"]}-{dataRow["fromerpno"]},";
                                updateSqls +=  $"update closebill set CreateStatus = 0,id = '{id}'  where iId = '{dataRow["iId"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            }
                            else
                            {
                                failedIds += $"{dataRow["iId"]}-{dataRow["fromerpno"]},";
                                updateSqls += $"update closebill set CreateStatus = CreateStatus + 1 where iId = '{dataRow["iId"]}' and fromerpno = '{dataRow["fromerpno"]}';";
                            }



                            #endregion
                        }
                        else
                        {
                            logMsg = $"查找不到对应客户:{dataRow["ccusname"]}，无法上传回款信息!";
                            updateSqls += $"update closebill set CreateStatus = CreateStatus + 9 where iId = '{dataRow["iId"]}' and fromerpno = '{dataRow["fromerpno"]}';";


                        }

                        }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["iId"]},";
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"回款信息同步名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"回款信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"回款信息同步失败名称为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        //修改
        public void closebillUpdate()
        {
            try
            {
                logMsg = string.Empty;
                var dataStr = $"select top {countOneTime} * from closebill  where updateStatus > 0  and updateStatus < 10 and id is not null and id <> '' and  dVouchDate >= '2018-01-01'"
                                + "and cdwcode in(select customerno from Customer where id <> '' and id is not null)";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无需要修改的回款信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {
                        string accountName = string.Empty;
                        if (dataRow["ccusname"].ToString().Length < 4 && dataRow["ccusname"].ToString().Length > 0)
                        {
                            accountName = "个人";
                        }
                        else
                        {
                            accountName = dataRow["ccusname"].ToString();

                        }

                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{accountName}'");

                        if (!string.IsNullOrEmpty(ownerIdJson) && ownerIdJson.Contains("id"))
                        {
                            string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                            string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);


                            #region json

                            var json = "{"
                                + $"\"id\":\"{dataRow["id"]}\""      //
                                + $",\"name\":\"{dataRow["cvouchid"]}\""      //单据编号
                                + $",\"customItem7__c\":\"{custid}\""       //客户名称
                                + $",\"customItem2__c\":\"{dataRow["iAmount"]}\""       //付款金额
                                + $",\"customItem3__c\":\"{dataRow["dVouchDate"].ToString().Substring(0, 10).Replace('/', '-')}\""       //付款日期
                                + $",\"customItem4__c\":\"{repalcePayType(dataRow["cssname"].ToString())}\""       //付款方式 
                                + $",\"customItem10__c\":\"{dataRow["iId"]}\""       //主键erp判重        
                                + $",\"customItem5__c\":\"{dataRow["cdigest"]}\""       //摘要
                                + $",\"customItem11__c\":\"{dataRow["fromerpno"]}\""       //账套
                                + "}";

                            #endregion



                            var result = CustomUpdate(json);//调用销售易同步接口同步数据

                            #region 销售易上传结果处理
                            if (result)
                            {
                                successIds += $"{dataRow["iId"]}-{dataRow["fromerpno"]},";
                                updateSqls += $"update closebill set updatestatus = 0  where id = '{dataRow["id"]}' ;";
                            }
                            else
                            {
                                failedIds += $"{dataRow["iId"]},";
                                updateSqls += $"update closebill set updatestatus = updatestatus + 1 where id = '{dataRow["id"]}' ;";
                            }


                            #endregion
                        }
                        else
                        {
                            logMsg = $"查找不到对应客户，无法上传开票信息!";
                            updateSqls += $"update closebill set updatestatus = updatestatus + 9 where id = '{dataRow["id"]}';";

                        }

                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["iId"]},";
                        logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"回款信息同步名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"回款信息修改失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"回款信息修改失败名称为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }


        #endregion

        #region  回款信息（客户） -----李姐的模块
        //上传客户字段
        public void BackMoneyOfCus()
        {
            try
            {
                DateTime dt = DateTime.Now;
                string today = dt.ToString("yyyy-MM-dd"); //当前日期     
                string toym = dt.ToString("yyyyMM"); //当前日期                                 
                string nmo = dt.Month.ToString();   //当前月份
                string nextmo = dt.AddMonths(+1).Month.ToString();;   //计划月份
                string nextMon = dt.AddMonths(1).ToString("yyyyMM"); //计划年份
                string nextmfday = dt.AddDays(1 - dt.Day).AddMonths(+1).ToString("yyyy-MM-dd");//下个月的第一天
                string logMes = string.Empty;

                string semsSql = "select customItem1__c,customItem1__c.accountName,customItem1__c.ownerId,customItem2__c,customItem4__c from customEntity32__c";
                string rejson = xoqlGet(semsSql);
                if (rejson.Contains("\"code\":\"200\""))
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(rejson);
                    int count = Convert.ToInt32(jo["data"]["count"].ToString());
                    //string records = jo["data"]["records"].ToString();
                    if (count > 0)
                    {
                        for (int a = 0; a < count; a++)
                        {
                            string id = jo["data"]["records"][a]["customItem1__c"].ToString(); //客户id
                            string cusname = jo["data"]["records"][a]["customItem1__c.accountName"].ToString(); //客户名称
                            string hkqujian = jo["data"]["records"][a]["customItem2__c"].ToString(); //客户回款区间
                            string ownerId = jo["data"]["records"][a]["customItem1__c.ownerId"].ToString(); //客户所有人
                            string zbj = jo["data"]["records"][a]["customItem4__c"].ToString(); //质保金

                            //string startdata = "201801";
                            int m = Convert.ToInt32(hkqujian) + 1;
                            double dzbj = Convert.ToDouble(zbj);
                            //string enddata = dt.AddMonths(-m).Date.ToString("yyyyMM");  //结束日期
                            string startdata = dt.AddMonths(-m).Month.ToString();  //结束月份 

                            string hkqj = $"{nextmo}月回{startdata}月及之前货款";


                            double je = 0;
                            string mbsql = "select sum(mb) from UFDATA_009_2018.dbo.GL_accass a,UFDATA_009_2018.dbo.customer b "
                                    + $"where iYPeriod = '{toym}' and ccode = '1122' and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';";
                            string mb = db.ExecuteScalar(mbsql).ToString();
                            if (!string.IsNullOrEmpty(mb))
                            {
                                string mdcsql = "select sum(md)-sum(mc) from UFDATA_009_2018.dbo.GL_accass a,UFDATA_009_2018.dbo.customer b "
                                            + $"where iYPeriod >= '{startdata}' and iYPeriod <= '{toym}' and ccode = '1122' "
                                            + $"and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';"; 
                                string mbc = db.ExecuteScalar(mdcsql).ToString();

                                //string jgsql = "select sum(mc) from UFDATA_009_2018.dbo.GL_accass a,UFDATA_009_2018.dbo.customer b "
                                //              + $"where iYPeriod >= '{enddata}' and iYPeriod <= '{toym}' and ccode = '1122' "
                                //              + $"and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';";
                                //string jgk = db.ExecuteScalar(jgsql).ToString();

                                je = Convert.ToDouble(mb) + Convert.ToDouble(mbc)- dzbj;
                            }
                            else
                            {
                                mb = db.ExecuteScalar($"select sum(mb) from UFDATA_888_2016.dbo.GL_accass a, UFDATA_888_2016.dbo.customer b "
                                    + $"where iYPeriod = '{toym}' and ccode = '112202' and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';").ToString();
                                if (!string.IsNullOrEmpty(mb))
                                {
                                    string mbc = db.ExecuteScalar("select sum(md)-sum(mc) from UFDATA_888_2016.dbo.GL_accass a,UFDATA_888_2016.dbo.customer b "
                                            + $"where iYPeriod >= '{startdata}' and iYPeriod <= '{toym}' and ccode = '112202' "
                                            + $"and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';").ToString();



                                    //string jgsql = "select sum(mc) from UFDATA_888_2016.dbo.GL_accass a,UFDATA_009_2018.dbo.customer b "
                                    //          + $"where iYPeriod >= '{enddata}' and iYPeriod <= '{toym}' and ccode = '112202' "
                                    //          + $"and a.ccus_id = b.ccuscode and b.ccusname = '{cusname}';";
                                    //string jgk = db.ExecuteScalar(jgsql).ToString();

                                    je = Convert.ToDouble(mb) + Convert.ToDouble(mbc) - dzbj;

                                }
                                else
                                {
                                    continue;
                                }
                            }

                            var json = "{\"belongId\":\"100635973\",\"record\":{"
                                    + $"\"name\":\"{nextMon}\""      //主键 //.Substring(0,7)
                                    + $",\"ownerId\":\"{ownerId}\""       //所有人     
                                    + $",\"customItem1__c\":\"{id}\""      //客户
                                    + $",\"customItem4__c\":\"{je}\""      //n月计划回款金额
                                    //+ $",\"customItem7__c\":\"{customItem3__c}\""      //回款说明
                                    + $",\"customItem11__c\":\"{hkqj}\""      //回款区间
                                    + $",\"customItem13__c\":\"{nextmfday}\""      //计划月份（n月）
                                    + "}}";

                            string reid = string.Empty;
                            bool fa = CustomCreate(json,out reid);
                            if (fa)
                            {
                                logMes += $"{cusname}回款生成成功！";
                            }
                            else
                            {
                                logMes += $"{cusname}回款生成失败！";
                            }

                        }
                        LogHelper.Info(logMes);
                    }
                     LogHelper.Info("无需要生成回款计划的客户！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.ToString());

            }
     

        }
        #endregion

        #region  预测和实际 -----孔经理的模块

        //预测
        public void MonNowYc(string mr)
        {

            try
            {

                DateTime dt = Convert.ToDateTime(mr);
                #region    常量
                logMsg = string.Empty;
                string todaydata = dt.ToString("yyyy-MM-dd");
                //string nowtime = DateTime.Now.ToString("yyyy-MM-dd");            // 2008-9-4 20:02:10
                //DateTime dt = DateTime.Now;
                //本月第一天时间   
                string dt_First = dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd");
                long dt_Firstsjc = ConvertDateTimeToInt(dt.AddDays(-(dt.Day) + 1));
                //当月最后一天时间
                string dt_lastt = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                long dt_lasttsjc = ConvertDateTimeToInt(dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1));
                //当前日期
                string nowtime = dt.ToString("yyyy-MM-dd");
                //当前年月
                string nowmd = dt.ToString("yyyy-MM");
                //下个月日期
                string nexttime = dt.AddMonths(1).ToString("yyyy-MM-dd");
                //当前年份
                string nyear = dt.Year.ToString();
                //当前月份
                string nmo = dt.Month.ToString();
                //上个月月份 
                string bmo = dt.AddMonths(-1).Month.ToString();
                //上上个月月份 
                string bbmo = dt.AddMonths(-2).Month.ToString();


                //ConvertDateTimeToInt 下个月
                //下个月第一天
                string nextmfday = dt.AddDays(1 - dt.Day).AddMonths(+1).ToString("yyyy-MM-dd");
                long nextmfdaysjc = ConvertDateTimeToInt(dt.AddDays(1 - dt.Day).AddMonths(+1));
                //下个月最后一天
                string nextmlday = dt.AddMonths(+1).AddDays(1 - dt.AddMonths(+1).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                long nextmldaysjc = ConvertDateTimeToInt(dt.AddMonths(+1).AddDays(1 - dt.AddMonths(+1).Day).AddMonths(1).AddDays(-1));

                //当月最后一天
                string lastday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //当月一共天数
                int daysOfonm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.ToString("MM")));
                //次月一共天数
                int daysOfnexm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.AddMonths(1).ToString("MM")));
                //上月一共天数
                int daysOflastm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.AddMonths(-1).ToString("MM")));
                //当前天数
                int daysnow = dt.Day;

                //上个月第一天
                string befMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-1).ToString("yyyy-MM-dd");
                //上个月最后一天
                string befMflastDay = dt.AddMonths(-1).AddDays(1 - dt.AddMonths(-1).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //上个月的今天
                string befMonDay = dt.AddMonths(-1).ToString("yyyy-MM-dd");
                //上个月的年月
                string befmd = dt.AddMonths(-1).ToString("yyyy-MM");

                //上上个月的年月
                string bbefmd = dt.AddMonths(-2).ToString("yyyy-MM");
                //上上个月第一天
                string bbefMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-2).ToString("yyyy-MM-dd");
                //上上个月最后一天  datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1)
                string bbefMflastDay = dt.AddMonths(-2).AddDays(1 - dt.AddMonths(-2).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //上上个月的今天
                string bbefMonDay = dt.AddMonths(-2).ToString("yyyy-MM-dd");
                #endregion
                //db.ExecuteNonQuery("update Dispatchlist set txh = cInvStd; update Dispatchlist set txh = SUBSTRING(txh, 1, (CHARINDEX('-', txh) - 1))  where txh like '%-%' ; ");


                #region  补充发货信息（预测所没有的）
                //查询有区间的客户
                string yqkh = $"select accountName,id,ownerId from account where dbcSelect6 != '3' limit 300;";
                string yqkhjson = sqlSelect(yqkh);
                if (yqkhjson.Contains("records"))
                {
                    JObject yqkhjo = (JObject)JsonConvert.DeserializeObject(yqkhjson);
                    int count = Convert.ToInt32(yqkhjo["count"].ToString());
                    if (count > 0)
                    {
                        for (int a = 0; a < count; a++)
                        {
                            string cusid = yqkhjo["records"][a]["id"].ToString();
                            string cusname = yqkhjo["records"][a]["accountName"].ToString();
                            string ownerId = yqkhjo["records"][a]["ownerId"].ToString();

                            var dataStr = $"select ccusname,cInvStd from [dbo].[Dispatchlist] where dDate BETWEEN '{bbefMfirDay}' and '{befMflastDay}' and ccusname = '{cusname}'"
                                           + " group by ccusname,cInvStd";
                            var table = db.GetDataSet(dataStr, CommandType.Text);

                            foreach (DataRow dataRow in table.Rows)
                            {
                                string cpidsql = $"select id from customize  where belongId = 100588408 and customItem3__c = '{dataRow["cInvStd"]}' limit 0,1";
                                string cpid = sqlSelectOutId(cpidsql);

                                if (!string.IsNullOrEmpty(cpid))
                                {
                                    string yzycsql = $"select id from customEntity26__c where customItem2__c = '{cusid}' and customItem6__c.name = '{dataRow["cInvStd"]}'";
                                    string yzycjson = xoqlGet(yzycsql);

                                    if (yzycjson.Contains("\"code\":\"200\""))
                                    {
                                        JObject yzycjo = (JObject)JsonConvert.DeserializeObject(yzycjson);
                                        int yzyccount = Convert.ToInt32(yzycjo["data"]["count"].ToString());
                                        if (yzyccount < 1)
                                        {
                                            var json = "{\"belongId\":\"100767745\",\"record\":{"
                                                        + $"\"customItem2__c\":\"{cusid}\""      //客户id
                                                        + $",\"ownerId\":\"{ownerId}\""       //所有人
                                                        + $",\"customItem9__c\":\"{0}\""      //预测月计划发货量
                                                        + $",\"customItem12__c\":\"{todaydata}\""      //预测月
                                                        + $",\"customItem6__c\":\"{cpid}\""      //产品
                                                        + $",\"customItem16__c\":\"{"生成主营预测用，由程序生成"}\""      //备注
                                                        + "}}";

                                            string id = string.Empty;
                                            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                            if (result)
                                            {
                                                logMsg += $"客户：{cusname}，产品：{dataRow["cInvStd"]}生成机器预测成功！\r\n ";
                                            }
                                            else
                                            {
                                                logMsg += $"客户：{cusname}，产品：{dataRow["cInvStd"]}生成机器预测失败！\r\n ";

                                            }



                                        }
                                       

                                    }
                                    else
                                    {
                                        logMsg += $"{cusname}{dataRow["cInvStd"]}发生错误！";
                                    }
                                    

                                }
                                else
                                {
                                    logMsg += $"CRM没有上传对应产品！===={dataRow["cInvStd"]}";

                                }
                                
                            }
                        }
                    }
                    else
                    {
                        LogHelper.Info("所有客户无开票区间！");
                    }
                }
                else
                {
                    LogHelper.Error("完善客户和产品对应关系错误！");
                }




                #endregion

                //取预测的客户
                string ycsql = $"select customItem2__c,customItem2__c.accountName,customItem6__c,customItem6__c.name,avg(customItem9__c) from customEntity26__c where "
                + $"customItem12__c >= '{dt_Firstsjc}' and customItem12__c <= '{dt_lasttsjc}'  group by customItem2__c,customItem2__c.accountName,customItem6__c,customItem6__c.name limit 1000;";
                string ycjson = xoqlGet(ycsql);

                if (ycjson.Contains("\"code\":\"200\""))
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(ycjson);
                    int count = Convert.ToInt32(jo["data"]["count"].ToString());
                    //string records = jo["data"]["records"].ToString();
                    if (count > 0)
                    {
                        for (int a = 0; a < count; a++)
                        {
                            string cusid = jo["data"]["records"][a]["customItem2__c"].ToString(); //客户id
                            string cusname = jo["data"]["records"][a]["customItem2__c.accountName"].ToString(); //客户名称
                            string cpid = jo["data"]["records"][a]["customItem6__c"].ToString(); //产品id
                            string cpname = jo["data"]["records"][a]["customItem6__c.name"].ToString(); //产品id
                            int fhiq = Convert.ToInt32(jo["data"]["records"][a]["avg(customItem9__c)"].ToString()); //计划发货量

                            //查询产品价格
                            string jgSql = $"select customItem3__c from customEntity27__c where customItem4__c = {cpid} and customItem2__c = {cusid} and customItem3__c is not null order by customItem3__c desc limit 1;";
                            string jgjson = xoqlGet(jgSql);
                            string je = "0";
                            if (jgjson.Contains("\"code\":\"200\""))
                            {
                                JObject jgjo = (JObject)JsonConvert.DeserializeObject(jgjson);
                                int jgcount = Convert.ToInt32(jgjo["data"]["count"].ToString());
                                if (jgcount > 0)
                                {
                                    je = jgjo["data"]["records"][0]["customItem3__c"].ToString();
                                }
                                else
                                {
                                    logMsg += $"查询不到{cusname}的产品{cpname}的价格！";
                                }
                            }
                            else
                            {

                                logMsg += $"查询{cusname}的产品{cpname}的价格失败！";

                            }

                            //根据客户id查询对应的开票区间
                            string qjsql = $"select customItem158__c,customItem159__c,accountName,ownerId from account where id = '{cusid}';";
                            string qjjson = xoqlGet(qjsql);
                            if (qjjson.Contains("\"code\":\"200\""))
                            {
                                JObject qjjo = (JObject)JsonConvert.DeserializeObject(qjjson);
                                int kpqjcount = Convert.ToInt32(qjjo["data"]["count"].ToString());
                                if (kpqjcount > 0)
                                {

                                    string kpy = qjjo["data"]["records"][0]["customItem158__c"][0].ToString(); //开票月
                                    string kpqstr = qjjo["data"]["records"][0]["customItem159__c"].ToString();
                                    int kpqsr = string.IsNullOrEmpty(kpqstr) ? 0 : Convert.ToInt32(kpqstr);
                                    //int kpqsr = Convert.ToInt32(qjjo["data"]["records"][0]["customItem159__c"].ToString()); //开票起始日
                                    //string cusname = qjjo["data"]["records"][0]["accountName"].ToString(); //客户名称
                                    string ownerId = qjjo["data"]["records"][0]["ownerId"].ToString(); //客户所有人
                                    string qjName = string.Empty;
                                    string jvkpqj = string.Empty;
                                    int b1 = 0;
                                    int b2 = 0;
                                    string lastiqsql = "select a.iqt from(select ccusname, cInvStd, sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                + $"where dDate BETWEEN '{befMfirDay}' and '{befMflastDay}' and ccusname = '{cusname}' and cInvStd = '{cpname}' "
                                                + $"GROUP BY ccusname,cInvStd) a";
                                    int lastiq = Convert.ToInt32(db.ExecuteScalar(lastiqsql));
                                    switch (kpy)
                                    {

                                        case "上月": //上月
                                            if (kpqsr <= 1)
                                            {
                                                qjName = "(n-1)整月";
                                                b1 = lastiq / daysnow * daysOflastm;
                                                jvkpqj = $"{befmd}月";
                                            }
                                            else
                                            {
                                                                                    //string bedata = $"{befmd}-{kpqsr}";
                                                //b1 = Convert.ToInt32(db.ExecuteScalar("select a.iqt from(select ccusname, cInvStd, sum(iQuantity) as iqt FROM[dbo].[Dispatchlist] "
                                                //                + $"where dDate BETWEEN '{bedata}' and '{befMflastDay}' and ccusname = '{cusname}' and cInvStd = '{cpname}' "
                                                //                + $"GROUP BY ccusname,cInvStd) a"));
                                                b1 = lastiq / daysnow * (daysOflastm - kpqsr);
                                                b1 = b1 > 0 ? b1 : 1;
                                                b2 = fhiq / daysOfonm * (kpqsr - 1);
                                                qjName = $"(n-1).{kpqsr}-n.{kpqsr - 1}";
                                                jvkpqj = $"{befmd}-{kpqsr}到{nowmd}-{kpqsr - 1}";

                                            }

                                            break;
                                        case "当月": //当月(预测月)
                                            if (kpqsr <= 1)
                                            {
                                                b2 = fhiq;
                                                qjName = $"n月";
                                                jvkpqj = $"{nowmd}月";
                                            }
                                            else
                                            {
                                                logMsg += $"{cusname}开票区间填写错误！";
                                                continue;
                                            }

                                            break;
                                        case "无": //无
                                            logMsg += $"{cusname}没有填写开票区间，请添加该客户的开票区间后再运行程序！\r\n ";
                                            continue;
                                            break;

                                        case "": // 空
                                            logMsg += $"{cusname}没有填写开票区间，请添加该客户的开票区间后再运行程序！\r\n ";
                                            continue;
                                            break;
                                        case "上上月": //上上月
                                            if (kpqsr <= 1)
                                            {
                                                qjName = "(n-2)整月";
                                                b1 = Convert.ToInt32(db.ExecuteScalar("select a.iqt from(select ccusname, cInvStd, sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                + $"where dDate BETWEEN '{bbefMfirDay}' and '{bbefMflastDay}' and ccusname = '{cusname}' and cInvStd = '{cpname}' "
                                                + $"GROUP BY ccusname,cInvStd) a"));
                                                jvkpqj = $"{bbefmd}月";

                                            }
                                            else
                                            {
                                                string bbda = $"{bbefmd}-{kpqsr}";
                                                b1 = Convert.ToInt32(db.ExecuteScalar("select a.iqt from(select ccusname, cInvStd, sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                + $"where dDate BETWEEN '{bbda}' and '{bbefMflastDay}' and ccusname = '{cusname}' and cInvStd = '{cpname}' "
                                                + $"GROUP BY ccusname,cInvStd) a"));
                                                string beda = $"{befmd}-{kpqsr - 1}";
                                                b2 = Convert.ToInt32(db.ExecuteScalar("select a.iqt from(select ccusname, cInvStd, sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                + $"where dDate BETWEEN '{befMfirDay}' and '{beda}' and ccusname = '{cusname}' and cInvStd = '{cpname}' "
                                                + $"GROUP BY ccusname,cInvStd) a"));
                                                qjName = $"(n-2).{kpqsr}-(n-1).{kpqsr - 1}";
                                                jvkpqj = $"{bbda}到{beda}";

                                            }
                                            break;

                                    }




                                    var json = "{\"belongId\":\"100842635\",\"record\":{"
                                            + $"\"name\":\"{mr.Substring(0, 8)}{"预测"}\""      //主键 //.Substring(0,7)
                                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                                            + $",\"customItem1__c\":\"{cusid}\""      //客户
                                            + $",\"customItem3__c\":\"{qjName}\""      //开票区间
                                            + $",\"customItem4__c\":\"{je}\""      //单价
                                            + $",\"customItem5__c\":\"{lastiq}\""      //当月累计发货
                                            + $",\"customItem6__c\":\"{b1}\""      //当月月开票区间发货
                                            + $",\"customItem2__c\":\"{cpid}\""      //产品
                                            + $",\"customItem7__c\":\"{fhiq}\""      //次月发货计划
                                            + $",\"customItem8__c\":\"{b2}\""      //次月开票区间内发货计划
                                            + $",\"customItem11__c\":\"{dt.ToString("yyyy-MM-dd")}\""       //计划月份
                                            + $",\"customItem13__c\":\"{jvkpqj}\""       //计划月份
                                            + "}}";


                                    string id = string.Empty;
                                    var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                    if (result)
                                    {
                                        logMsg += $"客户：{cusname}，产品：{cpname}生成主营预测成功！\r\n ";
                                    }
                                    else
                                    {
                                        logMsg += $"客户：{cusname}，产品：{cpname}生成主营预测失败！\r\n ";

                                    }
                                    }


                            }
                            else
                            {

                                logMsg += $"{cusname}没有填写开票区间，请添加该客户的开票区间后再运行程序！\r\n ";

                            }
                        }
                    }
                }
            }

            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            LogHelper.Info(logMsg);


        }


        //实际
        public void MonNowsj()
        {
            try
            {
                #region    常量
                logMsg = string.Empty;
                //string nowtime = DateTime.Now.ToString("yyyy-MM-dd");            // 2008-9-4 20:02:10

                DateTime dt = DateTime.Now;
                //本月第一天时间   
                string dt_First = dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd");
                //当前日期
                string nowtime = dt.ToString("yyyy-MM-dd");
                //下个月日期
                string nexttime = dt.AddMonths(1).ToString("yyyy-MM-dd");
                //当前年份
                string nyear = dt.Year.ToString();
                //当前月份
                string nmo = dt.Month.ToString();
                //上个月月份 
                string bmo = dt.AddMonths(-1).Month.ToString();
                //上上个月月份 
                string bbmo = dt.AddMonths(-2).Month.ToString();


                //ConvertDateTimeToInt 下个月
                //下个月第一天
                string nextmfday = dt.AddDays(1 - dt.Day).AddMonths(+1).ToString("yyyy-MM-dd");
                long nextmfdaysjc = ConvertDateTimeToInt(dt.AddDays(1 - dt.Day).AddMonths(+1));
                //下个月最后一天
                string nextmlday = dt.AddMonths(+1).AddDays(1 - dt.AddMonths(+1).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                long nextmldaysjc = ConvertDateTimeToInt(dt.AddMonths(+1).AddDays(1 - dt.AddMonths(+1).Day).AddMonths(1).AddDays(-1));

                //当月最后一天
                string lastday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //当月一共天数
                int daysOfonm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.ToString("MM")));
                //次月一共天数
                int daysOfnexm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.AddMonths(1).ToString("MM")));
                //当前天数
                int daysnow = dt.Day;

                //上个月第一天
                string befMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-1).ToString("yyyy-MM-dd");
                //上个月最后一天
                string befMflastDay = dt.AddMonths(-1).AddDays(1 - dt.AddMonths(-1).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //上个月的今天
                string befMonDay = dt.AddMonths(-1).ToString("yyyy-MM-dd");

                //上上个月第一天
                string bbefMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-2).ToString("yyyy-MM-dd");
                //上上个月最后一天  datetime.AddDays(1 - datetime.Day).AddMonths(1).AddDays(-1)
                string bbefMflastDay = dt.AddMonths(-2).AddDays(1 - dt.AddMonths(-2).Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //上上个月的今天
                string bbefMonDay = dt.AddMonths(-2).ToString("yyyy-MM-dd");
                #endregion
                //db.ExecuteNonQuery("update Dispatchlist set txh = cInvStd; update Dispatchlist set txh = SUBSTRING(txh, 1, (CHARINDEX('-', txh) - 1))  where txh like '%-%' ; ");

                //提取这当月的发货信息
                var dataStr = $"select ccusname,cInvStd FROM [dbo].[Dispatchlist] "
                               + $" where dDate BETWEEN '{dt_First}' and '{nowtime}' and ccusname <> '' and ccusname is not null group BY ccusname,cInvStd";

                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无发货信息！\r\n");
                    return;
                }
                foreach (DataRow dataRow in tableSt.Rows)
                {
                    //查询客户信息
                    var customerJson = sqlSelect($"select id,dbcSelect6,dbcInteger1,ownerId,accountName from account where (dbcSelect6 = '1' or dbcSelect6 = '2' or dbcSelect6 = '4') and accountName = '{dataRow["ccusname"]}'");

                    if (!string.IsNullOrEmpty(customerJson) && customerJson.Contains("dbcSelect6"))
                    {

                        JObject jo = (JObject)JsonConvert.DeserializeObject(customerJson);
                        string records = jo["records"].ToString();
                        JArray jry = (JArray)JsonConvert.DeserializeObject(records);
                        string cusid = jry[0]["id"].ToString();
                        string quM = jry[0]["dbcSelect6"].ToString();
                        string quD = jry[0]["dbcInteger1"].ToString();
                        string ownerId = jry[0]["ownerId"].ToString();
                        string accountName = jry[0]["accountName"].ToString();
                        //查询是否有开票区间（是否需要统计）如果不需要就跳过
                        if (!string.IsNullOrEmpty(quM) && !quM.Equals("3"))
                        {
                            //查询产品id
                            string cpidsql = $"select id from customize  where belongId = 100588408 and customItem3__c = '{dataRow["cInvStd"]}' limit 0,1";
                            string cpid = sqlSelectOutId(cpidsql);
                            if (string.IsNullOrEmpty(cpid))
                            {
                                LogHelper.Info($"CRM没有上传对应产品！===={dataRow["cInvStd"]}");
                                continue;
                            }

                            //查重
                            string bdsql = $"select id from customize  where belongId = 100671930 AND customItem1__c = '{cusid}' and name = '{nowtime.Substring(0, 7)}' and customItem2__c = '{cpid}' ";
                            // + $" and customItem1__c = '{dataRow["cInvStd"]}'";
                            string salplanid = sqlSelectOutId(bdsql);

                            //如果不存在
                            if (string.IsNullOrEmpty(salplanid))
                            {
                                //查询对应的单价
                                string jgsql = $"select customItem3__c from customize  where belongId = 100821247 and customItem2__c = {cusid} and customItem4__c = {cpid} limit 0,1";
                                string customItem3__c = "0";
                                string jgjson = sqlSelect(jgsql);
                                if (!string.IsNullOrEmpty(jgjson) && jgjson.Contains("customItem3__c"))
                                {

                                    JObject jojg = (JObject)JsonConvert.DeserializeObject(jgjson);
                                    int jgcount = Convert.ToInt32(jojg["count"].ToString());

                                    if (jgcount > 0)
                                    {
                                        string jgrecords = jojg["records"].ToString();
                                        JArray jgry = (JArray)JsonConvert.DeserializeObject(jgrecords);
                                        customItem3__c = jgry[0]["customItem3__c"].ToString();
                                    }
                                }
                                //当月累计发货
                                string ljsql = "select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                            + $"where dDate BETWEEN '{dt_First}' and '{nowtime}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                            + $"GROUP BY ccusname,cInvStd) a";
                                var nowmon = Convert.ToInt32(db.ExecuteScalar(ljsql));

                                //当月开票区间内发货
                                #region    根据开票区间拆分算出日期 //1上月，2当月，3无，4上上月
                                string begdate = string.Empty;
                                string enddate = string.Empty;
                                string qjName = string.Empty;
                                //上月开票区间内发货
                                int shangyueqty = 0;
                                //此月开票区间发货   分两种情况
                                int ciyueqty = 0;
                                //上月整月
                                if (quM.Equals("1") && (quD.Equals("1") || quD.Equals("0")))
                                {
                                    begdate = befMfirDay;
                                    enddate = befMflastDay;
                                    shangyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                            + $"where dDate BETWEEN '{befMfirDay}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                            + $"GROUP BY ccusname,cInvStd) a"));
                                    ciyueqty =0;

                                    qjName = "(n-1)整月";

                                }//上月有区间
                                else if (quM.Equals("1") && !quD.Equals("1") && !quD.Equals("0"))
                                {
                                    begdate = $"{nyear}-{bmo}-{quD}";
                                    enddate = $"{nyear}-{nmo}-{Convert.ToInt32(quD) - 1}";
                                    qjName = $"(n-1).{quD}--n.{Convert.ToInt32(quD) - 1}";

                                    //if (Convert.ToInt32(quD) <= daysnow)
                                    //{
                                        shangyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                        + $"where dDate BETWEEN '{begdate}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                        + $"GROUP BY ccusname,cInvStd) a"));

                                        ciyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                        + $"where dDate BETWEEN '{dt_First}' and '{enddate}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                        + $"GROUP BY ccusname,cInvStd) a"));

                                    //}
                                    //else
                                    //{

                                        




                                    //}

                                }//当月
                                else if (quM.Equals("2"))
                                {
                                    begdate = dt_First;
                                    enddate = nowtime;
                                    qjName = "n月整月";
                                    shangyueqty = 0;

                                    ciyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                    + $"where dDate BETWEEN '{dt_First}' and '{enddate}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                    + $"GROUP BY ccusname,cInvStd) a"));



                                }//上上月整月
                                else if (quM.Equals("4") && (quD.Equals("1") || quD.Equals("0")))
                                {
                                    begdate = bbefMfirDay;
                                    enddate = bbefMflastDay;
                                    qjName = "n-2月(整月)";

                                    shangyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                    + $"where dDate BETWEEN '{bbefMfirDay}' and '{bbefMflastDay}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                    + $"GROUP BY ccusname,cInvStd) a"));

                                    ciyueqty = 0;


                                }//上上月有区间
                                else if (quM.Equals("4") && !quD.Equals("1") && !quD.Equals("0"))
                                {
                                    begdate = $"{nyear}-{bbmo}-{quD}";
                                    enddate = $"{nyear}-{bmo}-{Convert.ToInt32(quD) - 1}";
                                    qjName = $"（n-2).{quD}--(n-1).{Convert.ToInt32(quD) - 1}";

                                    shangyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                    + $"where dDate BETWEEN '{begdate}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                    + $"GROUP BY ccusname,cInvStd) a"));

                                    ciyueqty = Convert.ToInt32(db.ExecuteScalar("select a.iqt from  (select ccusname,cInvStd,sum(iQuantity) as iqt FROM [dbo].[Dispatchlist] "
                                                    + $"where dDate BETWEEN '{befMfirDay}' and '{enddate}' and ccusname = '{dataRow["ccusname"]}' and cInvStd = '{dataRow["cInvStd"]}' "
                                                    + $"GROUP BY ccusname,cInvStd) a"));
                                }
                                else
                                {
                                    LogHelper.Info($"客户：{accountName}的开票区间填写错误");
                                    continue;
                                }
                                #endregion



                                ////此月开票区间有多少天，算出多少量
                                //int dayqty = Convert.ToInt32(nowmon) / daysOfonm * (daysOfonm - daysnow);

                                ////次月计划
                                //string jhsql = $"select customItem9__c from customize  where belongId = 100767745 and customItem2__c = '{cusid}' and customItem6__c = '{cpid}' and customItem12__c like '{nexttime.ToString().Substring(0, 7)}' limit 0,1";

                                //string jhjson = sqlSelect(jhsql);
                                //JObject jojh = (JObject)JsonConvert.DeserializeObject(jhjson);
                                //string recordjh = jojh["records"].ToString();
                                //int count = Convert.ToInt32(jojh["count"].ToString());
                                //string customItem9__c = "0";
                                //if (count > 0)
                                //{
                                //    JArray jrjh = (JArray)JsonConvert.DeserializeObject(recordjh);
                                //    customItem9__c = jrjh[0]["customItem9__c"].ToString();
                                //}
                                ////次月开票区间内

                                //int nextjhqj = Convert.ToInt32(customItem9__c) / Convert.ToInt32(daysOfnexm) * (Convert.ToInt32(quD) - 1);

                                string jhsql = $"select id from customize  where belongId = 100842635 and customItem1__c = '{cusid}' and customItem1__c = '{cpid}' and name = '{befMonDay.Substring(0, 7)}' limit 0,1;";
                                string jhid = sqlSelectOutId(jhsql);



                                var json = "{\"belongId\":\"100671930\",\"record\":{"
                                            + $"\"name\":\"{nowtime.Substring(0, 7)}\""      //主键 //.Substring(0,7)
                                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                                            //+ $",\"entityType\":\"{100850324}\""       //所有人
                                            + $",\"customItem1__c\":\"{cusid}\""      //客户
                                            + $",\"customItem2__c\":\"{qjName}\""      //开票区间
                                            + $",\"customItem4__c\":\"{customItem3__c}\""      //单价
                                            + $",\"customItem6__c\":\"{nowmon}\""      //当月累计发货
                                            + $",\"customItem16__c\":\"{ciyueqty}\""      //当月月开票区间发货
                                            + $",\"customItem13__c\":\"{cpid}\""      //产品
                                            //+ $",\"customItem7__c \":\"{customItem9__c}\""      //次月发货计划
                                            + $",\"customItem5__c\":\"{shangyueqty}\""      //上月开票区间内发货计划
                                            + $",\"customItem11__c\":\"{nowtime}\""       //月份
                                            + $",\"customItem25__c\":\"{jhid}\""       //预测关联的
                                            + "}}";
                                string id = string.Empty;
                                var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                if (result)
                                {
                                    salplanid = id;
                                }
                                else
                                {
                                    logMsg += $"{dataRow["ccusname"]}写入错误！\\n";
                                }
                            }

                        }
                        else
                        {
                            logMsg += $"{dataRow["ccusname"]}不用统计开票区间内信息！ \r\n ";

                        }


                    }
                    else
                    {

                        logMsg += $"{dataRow["ccusname"]}在CRM上不存在！ \r\n ";


                    }


                }


            }

            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            LogHelper.Info(logMsg);


        }

        #endregion
        
         

        #region //其他
        /// <summary>
        /// 接口测试
        /// </summary>
        public string inText()
        {

            return inTextBygetoke();

        }

        /// <summary>
        /// 测试
        /// </summary>
        public void infoShow()
        {
             try
            {
                var dataStr = $"select * from ADW_CRM_2008.dbo.export";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {

                        string gidq = $"select id from customize  where belongId = 100588408 AND name ='{dataRow["goodid"].ToString()}';";
                        string goodid = sqlSelectOutId(gidq);

                        string cusql = $"select id,ownerId from account where accountName = '{dataRow["cusname"].ToString()}';";
                        string cujson = xoqlGet(cusql);

                        JObject jo = (JObject)JsonConvert.DeserializeObject(cujson);
                        int count = Convert.ToInt32(jo["data"]["count"].ToString());
                        //string records = jo["data"]["records"].ToString();
                        if (count > 0)
                        { 
                            //string hkqujian = jo["data"]["records"][a]["customItem2__c"].ToString();
                            #region json
                            var json = "{\"belongId\":\"100821247\",\"record\":{"
                            + $"\"name\":\"{dataRow["goodname"].ToString()}\""      //产品名称
                            + $",\"ownerId\":\"{jo["data"]["records"][0]["ownerId"].ToString()}\""       //所有人
                            + $",\"customItem2__c\":\"{jo["data"]["records"][0]["id"].ToString()}\""       //产品型号
                            + $",\"customItem3__c\":\"{dataRow["jg"].ToString()}\""       //可用
                            + $",\"customItem4__c\":\"{goodid}\""       //待入库数量
                            + $",\"customItem5__c\":\"{dataRow["bz"].ToString()}\""       //待发货数量
                            + "}}";
                            #endregion


                            string id;
                            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据
                            if (result || id.Contains("数据重复"))
                            {
                                db.ExecuteNonQuery($"update ADW_CRM_2008.dbo.export set status = 100 where cusname = '{dataRow["cusname"].ToString()}' and goodid = '{dataRow["goodid"].ToString()}';");
                            }

                        }


                    }
                    catch (Exception exception)
                    {
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }

        }


        //新增
        public void jgsy()
        {
            try
            {
                var dataStr = $"select top 50 * from jg where createstatus = 1";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无新的开票信息！");
                    return;
                }

                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in tableSt.Rows)
                {

                    #region 需要传入的参数
                    try
                    {


                        //查询客户所有人
                        var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["cusname"]}'");

                        if (!string.IsNullOrEmpty(ownerIdJson) && ownerIdJson.Contains("id"))
                        {
                            string custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                            string ownerId = !ownerIdJson.Contains("ownerId") ? "" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);


                            //查询产品id
                            string cpq = $"select id from customize where belongId = 100588408 AND name ='{dataRow["std"]}';";
                            string cpid = sqlSelectOutId(cpq);

                            if (!string.IsNullOrEmpty(cpid))
                            {

                                #region json

                                var json = "{\"belongId\":\"100821247\",\"record\":{"
                                + $"\"name\":\"{dataRow["cm"]}\""      //产品名称
                                + $",\"customItem2__c\":\"{custid}\""       //客户
                                + $",\"ownerId\":\"{ownerId}\""       //所有人
                                + $",\"customItem4__c\":\"{cpid}\""       //开票日期
                                + $",\"customItem5__c\":\"{dataRow["jg"]}\""       //客户
                                //+ $",\"customItem5__c\":\"{dataRow["E"]}\""       //开票总额
                                + "}}";

                                #endregion
                                string id;
                                var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                if (id.Contains("数据重复"))
                                {
                                    string q = $"select id from customize where belongId = 100821247 AND customItem2__c ='{custid}' and customItem4__c = '{cpid}';";
                                    string sid = sqlSelectOutId(q);

                                    id = sid;
                                    if (id.Equals("") || id.Length > 12)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        result = true;
                                    }

                                }


                                if (result)
                                {
                                    successIds += $"{dataRow["std"]},";
                                    updateSqls += $"update jg set CreateStatus = 0 where std = '{dataRow["std"]}' and cusname = '{dataRow["cusname"]}';";


                                }
                                else
                                {
                                    failedIds += $"{dataRow["std"]},";
                                    updateSqls += $"update jg set CreateStatus = CreateStatus+1 where std = '{dataRow["std"]}' and cusname = '{dataRow["cusname"]}';";


                                }


                            }
                            else
                            {
                                LogHelper.Info($"查找不到对应产品，无法上传开票信息!{dataRow["std"]};=======");
                                updateSqls += $"update jg set CreateStatus = CreateStatus+9 where std = '{dataRow["std"]}' and cusname = '{dataRow["cusname"]}';";
                            }
                        }
                        else
                        {
                            LogHelper.Info($"查找不到对应客户，无法上传开票信息!{dataRow["cusname"]};=======");
                            updateSqls += $"update jg set CreateStatus = CreateStatus+9 where std = '{dataRow["std"]}' and cusname = '{dataRow["cusname"]}';";

                        }

                    }
                    catch (Exception exception)
                    {
                        failedIds += $"{dataRow["std"]},";
                        logMsg = $"失败原因：{exception.Message}";
                    }
                    #endregion
                }

                logMsg = $"开票信息同步名称为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != tableSt.Rows.Count
                       ? $"开票信息同步失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{tableSt.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                       : $"开票信息同步失败送名称为{failedIds}。";
                }

                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg = $"但在更新数据库时失败，失败原因：{exception.Message}";
                }

                //Log记录
                LogHelper.Info(logMsg);
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }


        }

        //新增
        public string xiugai()
        {

            try
            {
                CustomerInterfaceSelect();
                var dataStr = $"select id,dbcSelect6 from account where dbcSelect6 is null limit 0,300";
                string res = sqlSelect(dataStr);
                if (res.Contains("error"))
                {
                    LogHelper.Error("错误");
                    return "错误";
                }


                JObject jo = (JObject)JsonConvert.DeserializeObject(res);
                string records = jo["records"].ToString();
                if (records.Equals("0"))
                {

                    return "已完成";
                }
                JArray jArray = (JArray)JsonConvert.DeserializeObject(records);//jsonArrayText必须是带[]数组格式字符串
                int count = jArray.Count;
                for (int a = 0; a < count; a++)
                {
                    string dataid = jArray[a]["id"].ToString();


                    var json = "{"
                   + $"\"id\":\"{dataid}\""      //id
                    + $",\"dbcSelect6\":\"{"3"}\""      //产品编号
                   + "}";

                    var result = CustomerUpdate(json);//调用销售易同步接口同步数据



                }




            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            return "null";



        }

        //上传客户字段
        public void PlanOfSal()
        {
            try
            {
                #region    常量
                logMsg = string.Empty;
                //string nowtime = DateTime.Now.ToString("yyyy-MM-dd");            // 2008-9-4 20:02:10

                DateTime dt = DateTime.Now;
                //本月第一天时间   
                string dt_First = dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd");
                //当前日期
                string nowtime = dt.ToString("yyyy-MM-dd");
                //当前年份
                string nyear = dt.Year.ToString();
                //当前月份
                string nmo = dt.Month.ToString();
                //上个月月份 
                string bmo = dt.AddMonths(-1).Month.ToString();
                //上上个月月份 
                string bbmo = dt.AddMonths(-2).Month.ToString();

                //当月最后一天
                string lastday = dt.AddDays(1 - dt.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                //当月一共天数
                int daysOfonm = DateTime.DaysInMonth(dt.Year, int.Parse(DateTime.Now.ToString("MM")));
                //当前天数
                int daysnow = dt.Day;

                //上个月第一天
                string befMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-1).ToString("yyyy-MM-dd");
                //上个月最后一天
                string befMflastDay = dt.AddDays(1 - dt.Day).AddDays(-1).ToString("yyyy-MM-dd");
                //上个月的今天
                string befMonDay = dt.AddMonths(-1).ToString("yyyy-MM-dd");

                //上上个月第一天
                string bbefMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-2).ToString("yyyy-MM-dd");
                //上上个月最后一天
                string bbefMflastDay = dt.AddDays(1 - dt.Day).AddDays(-2).ToString("yyyy-MM-dd");
                //上上个月的今天
                string bbefMonDay = dt.AddMonths(-2).ToString("yyyy-MM-dd");
                #endregion

                ////提取需要统计的客户信息 （客户表中开票月不为空和不是无的）
                //var customerJson = sqlSelect("select id,accountName from account where dbcSelect6 = '1' or dbcSelect6 = '2' or dbcSelect6 <> '4'");

                // db.ExecuteNonQuery("update Dispatchlist set txh = cInvStd; update Dispatchlist set
                // txh = SUBSTRING(txh, 1, (CHARINDEX('-', txh) - 1)) where txh like '%-%' ; ");

                //提取当月的发货信息
                var dataStr = $"select ccusname,cInvStd,iQuantity as ondayiq,ddate FROM [dbo].[Dispatchlist] "
                               + $" where dDate BETWEEN '{dt_First}' and '{nowtime}' and ccusname <> '' and ccusname is not null ORDER BY ccusname";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("无发货信息！\r\n");
                    return;
                }

                foreach (DataRow dataRow in tableSt.Rows)
                {
                    //查询平台有无此条数据

                    var customerJson = sqlSelect($"select id,dbcSelect6,dbcInteger1,ownerId from account where (dbcSelect6 = '1' or dbcSelect6 = '2' or dbcSelect6 = '4') and accountName = '{dataRow["ccusname"]}'");

                    if (!string.IsNullOrEmpty(customerJson) && customerJson.Contains("dbcSelect6"))
                    {

                        JObject jo = (JObject)JsonConvert.DeserializeObject(customerJson);
                        string records = jo["records"].ToString();
                        JArray jry = (JArray)JsonConvert.DeserializeObject(records);
                        string cusid = jry[0]["id"].ToString();
                        string quM = jry[0]["dbcSelect6"].ToString();
                        string quD = jry[0]["dbcInteger1"].ToString();
                        string ownerId = jry[0]["ownerId"].ToString();

                        //查询是否有开票区间（是否需要统计）如果不需要就跳过
                        if (!string.IsNullOrEmpty(quM) && !quM.Equals("3"))
                        {
                            //查重
                            string bdsql = $"select id from customize  where belongId = 100671930 AND customItem22__c = '{cusid}' and name = '{nowtime.ToString()}' "
                                + $" and customItem1__c = '{dataRow["cInvStd"]}'";
                            string salplanid = sqlSelectOutId(bdsql);

                            //产品id
                            string cpq = $"select id from customize where belongId = 100588408 AND customItem3__c ='{dataRow["tuno"]}';";
                            string cpid = sqlSelectOutId(cpq);

                            //上传无参架构、、
                            if (string.IsNullOrEmpty(salplanid))
                            {
                                var json = "{\"belongId\":\"100671930\",\"record\":{"
                                            + $"\"name\":\"{nowtime.ToString()}\""      //主键 //.Substring(0,7)
                                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                                            + $",\"customItem3__c\":\"{dataRow["tuno"]}\""      //产品型号
                                            + $",\"customItem13__c\":\"{cpid}\""      //产品关联
                                            + $",\"customItem1__c \":\"{cusid}\""      //客户
                                            + "}}";
                                string id = string.Empty;
                                var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                if (result)
                                {
                                    salplanid = id;
                                }
                                else
                                {
                                    logMsg += $"{dataRow["ccusname"]}写入错误！\\n";
                                }
                            }


                            #region    根据开票区间拆分算出日期 //1上月，2当月，3无，4上上月

                            string qty001 = string.Empty;
                            string qty002 = string.Empty;
                            string begdate = string.Empty;
                            string enddate = string.Empty;
                            string qjName = string.Empty;
                            //上月整月
                            if (quM.Equals("1") && quD.Equals("1"))
                            {

                                qty001 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                                        + $"where dDate BETWEEN '{befMfirDay}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]} and tuno = '{dataRow["tuno"]}' "
                                                        + $"GROUP BY ccusname, tuno "
                                                        + $" ORDER BY ccusname) a").ToString();


                                qty002 = "0";
                                qjName = "(n-1)月(整月)";

                            }//上月有区间
                            else if (quM.Equals("1") && !quD.Equals("1"))
                            {
                                //+ Convert.ToInt32(quD)

                                begdate = $"{nyear}-{bmo}-{quD}";
                                qty001 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                        + $"where dDate BETWEEN '{begdate}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}' "
                                        + $"GROUP BY ccusname, tuno "
                                        + $" ORDER BY ccusname) a").ToString();

                                enddate = $"{nyear}-{nmo}-{Convert.ToInt32(quD) - 1}";
                                qty002 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                        + $"where  dDate BETWEEN '{befMfirDay}' and '{enddate}' and ccusname = '{dataRow["ccusname"]}'  and tuno = '{dataRow["tuno"]}'"
                                        + $"GROUP BY ccusname, tuno "
                                        + $" ORDER BY ccusname) a").ToString();
                                qjName = $"(n-1).{quD}--n{Convert.ToInt32(quD) - 1}";

                            }//当月
                            else if (quM.Equals("2"))
                            {
                                qty002 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                        + $"where  dDate BETWEEN '{dt_First}' and '{lastday}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}'"
                                        + $"GROUP BY ccusname, tuno "
                                        + $" ORDER BY ccusname) a").ToString();


                                qty001 = "0";
                                qjName = "n月(整月)";


                            }//上上月整月
                            else if (quM.Equals("4") && quD.Equals("1"))
                            {
                                qty001 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                            + $"where dDate BETWEEN '{bbefMfirDay}' and '{bbefMflastDay}' and ccusname = '{dataRow["ccusname"]} and tuno = '{dataRow["tuno"]}'"
                                            + $"GROUP BY ccusname, tuno "
                                            + $" ORDER BY ccusname) a").ToString();


                                qty002 = "0";
                                qjName = "n-2月(整月)";


                            }//上上月有区间
                            else if (quM.Equals("4") && !quD.Equals("1"))
                            {
                                begdate = $"{nyear}-{bbmo}-{quD}";
                                qty001 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                        + $"where dDate BETWEEN '{begdate}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}' "
                                        + $"GROUP BY ccusname, tuno "
                                        + $" ORDER BY ccusname) a").ToString();

                                enddate = $"{nyear}-{bmo}-{Convert.ToInt32(quD) - 1}";
                                qty002 = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                        + $"where dDate BETWEEN '{befMfirDay}' and '{enddate}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}' "
                                        + $"GROUP BY ccusname, tuno "
                                        + $" ORDER BY ccusname) a").ToString();
                                qjName = $"（n-2).{quD}--(n-1){Convert.ToInt32(quD) - 1}";

                            }
                            else
                            {
                                qty001 = "0";
                                qty002 = "0";
                                qjName = $"其他{quM}-{quD}";
                            }

                            #endregion



                            //查询当月的量
                            var onmoniq = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                                        + $"where dDate BETWEEN '{dt_First}' and '{lastday}' and ccusname = '{dataRow["ccusname"]} and tuno = '{dataRow["tuno"]}' "
                                                        + $"GROUP BY ccusname, tuno "
                                                        + $" ORDER BY ccusname) a");
                            //上月整月的量
                            var pasmoniq = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                            + $"where dDate BETWEEN '{befMfirDay}' and '{befMflastDay}' and ccusname = '{dataRow["ccusname"]} and tuno = '{dataRow["tuno"]}'"
                                            + $"GROUP BY ccusname, tuno "
                                            + $"ORDER BY ccusname) a");

                            ////当天
                            //var ondayiq = db.ExecuteScalar("select a.sumiq from  (select ccusname,cInvStd,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                            //                + $"where dDate = {nowtime} and ccusname = '{dataRow["ccusname"]}' "
                            //                + $"GROUP BY ccusname, cInvStd "
                            //                + $"ORDER BY ccusname) a");

                            //上月累计的量
                            var bljiq = db.ExecuteScalar("select a.sumiq from  (select ccusname,tuno,sum(iQuantity) as onmoniq FROM [dbo].[Dispatchlist] "
                                            + $"where dDate BETWEEN '{befMfirDay}' and '{befMonDay}' and ccusname = '{dataRow["ccusname"]} and tuno = '{dataRow["tuno"]}'"
                                            + $"GROUP BY ccusname, tuno "
                                            + $"ORDER BY ccusname) a");




                            //预测的查出随意一条如果没有就空着
                            ////上月预测的查出随意一条如果没有就空着
                            //string ymsql = $"select customItem9__c from customize  where belongId = 100767745 AND customItem2__c = '{custid}' and customItem20__c = '{cpid}' and(customItem9__c <> '' and customItem9__c is not null and customItem9__c <> '0') and name like '{nowtime.ToString().Substring(0, 7)}%'";
                            //var ycnjson = sqlSelect(ymsql);
                            //string ym = !ycnjson.Contains("customItem9__c") ? "0" : ycnjson.Substring(ycnjson.IndexOf("_c\":", StringComparison.Ordinal) + 4, ycnjson.IndexOf("}", StringComparison.Ordinal) - ycnjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

                            //当月预测的查出随意一条如果没有就空着
                            string nmsql = $"select customItem9__c from customize  where belongId = 100767745 AND customItem2__c = '{cusid}' and customItem20__c = '{cpid}' and(customItem9__c <> '' and customItem9__c is not null and customItem9__c <> '0') and name like '{befMonDay.Substring(0, 7)}%'";
                            var nmjson = sqlSelect(nmsql);
                            string nm = !nmjson.Contains("customItem9__c") ? "0" : nmjson.Substring(nmjson.IndexOf("_c\":", StringComparison.Ordinal) + 4, nmjson.IndexOf("}", StringComparison.Ordinal) - nmjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

                            //单价的查出随意一条如果没有就空着
                            string djsql = $"select customItem4__c from customize  where belongId = 100671930 AND customItem3__c = '{dataRow["tuno"]}' and customItem1__c = '{cusid}' and(customItem4__c <> '' and customItem4__c is not null and customItem4__c <> '0') and name like '{nyear}-{bmo}%'";
                            var djjson = sqlSelect(djsql);

                            string dj = !djjson.Contains("customItem4__c") ? "0" : djjson.Substring(djjson.IndexOf("_c\":", StringComparison.Ordinal) + 4, djjson.IndexOf("}", StringComparison.Ordinal) - djjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;


                            var updatejson = "{"
                                + $"\"id\":\"{salplanid}\""
                                + $",\"customItem2__c\":\"{qjName ?? "0"}\""      //开票区间
                                + $",\"customItem4__c\":\"{dj ?? "0"}\""      //单价       ？？
                                + $",\"customItem5__c\":\"{pasmoniq ?? "0"}\""      //上月全月发货数量
                                + $",\"customItem6__c\":\"{qty001 ?? "0"}\""      //上月开票区间内发货
                                                                                  //+ $",\"customItem7__c\":\"{nm ?? "0"}\""      //当月计划 --------------------------------------------
                                                                                  //+ $",\"customItem8__c\":\"{qty002 ?? "0"}\""      //最后一周预测
                                                                                  //+ $",\"customItem9__c\":\"{qty002 ?? "0"}\""      //开票数量合计----------------------------------
                                + $",\"customItem16__c \":\"{onmoniq ?? "0"}\""      //上月整月发货量
                                + $",\"customItem14__c \":\"{daysOfonm}\""      //此月多少天
                                + $",\"customItem15__c \":\"{quD}\""      //此月开票区间天数
                                                                          //+ $",\"customItem11__c\":\"{qty001 ?? "0"}\""      //上月开票区间内发货
                                                                          //+ $",\"customItem12__c\":\"{pasmoniq ?? "0"}\""      //当月计划发货量  手打
                                                                          //+ $",\"customItem13__c\":\"{onmoniq ?? "0"}\""      //当月累计发货量

                                + "}";

                            //客户 customItem1__c 关联关系
                            //开票区间 customItem2__c 文本类型
                            //产品简称（选填） customItem3__c 文本类型
                            //单价 customItem4__c 货币类型
                            //当月累计发货 customItem5__c 整数类型
                            //当月月开票区间发货 customItem6__c 整数类型
                            //产品 customItem13__c 关联关系
                            //次月发货计划 customItem16__c 整数类型
                            //3月开票区间内发货计划 customItem20__c 整数类型
                            //开票数量合计 customItem21__c 浮点类型
                            //销售收入 customItem22__c 计算型字段
                        }
                        else
                        {
                            logMsg += $"{dataRow["ccusname"]}不用统计开票区间内信息！ \r\n ";

                        }


                    }
                    else
                    {

                        logMsg += $"{dataRow["ccusname"]}在CRM上不存在！ \r\n ";


                    }


                    // var res = CustomUpdate(updatejson);

                    // if (res) { logMsg += $"{dataRow["ccusname"]}计划更新成功！\r\n "; } else { logMsg +=
                    // $"{dataRow["ccusname"]}计划更新失败！\r\n "; }


                    //}
                    //else
                    //{
                    //    logMsg += $"{dataRow["ccusname"]}在CRM上不存在！ \r\n ";

                    //}
                }


            }

            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            LogHelper.Info(logMsg);

        }

        /// <summary>
        /// 当月销售预测 --------------胡经理的模块
        /// </summary>
        public void PlanOfFh()
        {

            try
            {
                logMsg = string.Empty;
                DateTime dt = DateTime.Now;

                #region 列出所有时间区间
                ///需要三个范围  分别是此月   上个月 和去年此月
                //本月第一天时间   
                string dt_First = dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd");
                //当前日期
                string nowtime = dt.ToString("yyyy-MM-dd");


                //上个月第一天
                string befMfirDay = dt.AddDays(1 - dt.Day).AddMonths(-1).ToString("yyyy-MM-dd");
                //上个月的今天
                string befMonDay = dt.AddMonths(-1).ToString("yyyy-MM-dd");

                //去年此月的第一天
                string befYfirDay = dt.AddYears(-1).AddDays(1 - dt.Day).ToString("yyyy-MM-dd");
                //去年此月的今天
                string befYerDay = dt.AddYears(-1).ToString("yyyy-MM-dd");


                #endregion

                //按照型号客户分组查询本月的发货信息 板块暂且不分组（因为可能有板块没找到匹配的）
                var dataStr = $"SELECT tuno,ccusname,sum(iQuantity) as iq FROM Dispatchlist where dDate BETWEEN '{dt_First}' and '{nowtime}' and ccusname is not null and ccusname <> '' GROUP BY tuno,ccusname";
                var tableSt = db.GetDataSet(dataStr, CommandType.Text);

                if (tableSt.Rows.Count <= 0)
                {
                    LogHelper.Info("写入当月销售预测模块中无最新发货信息！\r\n");
                    return;
                }
                //查询平台有无此条数据
                //string bdsql = "select id from account where dbcVarchar5 = '{}' ";
                foreach (DataRow dataRow in tableSt.Rows)
                {
                    //查询客户所有人业务员id和客户在crm上的id（如果CRM不存在此客户就无法上传）
                    var ownerIdJson = sqlSelect($"select id,ownerId from account where accountName = '{dataRow["ccusname"]}'");

                    string custid = "";
                    string ownerId = "847069";
                    if (!ownerIdJson.Contains("records\":[]"))
                    {
                        custid = ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4, ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 - ownerIdJson.IndexOf("id") - 4);

                        //查出所有人id，此时如果为空归给孔经理
                        ownerId = !ownerIdJson.Contains("ownerId") ? "847069" : ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9, ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) - 9);


                        //产品id
                        string cpq = $"select id from customize where belongId = 100588408 AND name ='{dataRow["tuno"]}';";
                        string cpid = sqlSelectOutId(cpq);
                        //查重

                        if (!string.IsNullOrEmpty(cpid))
                        {
                            string bdsql = $"select id from customize  where belongId = 100672402 AND customItem20__c = '{cpid}' and name = '{nowtime.ToString()}' and customItem17__c = '{custid}'";
                            string salplanid = sqlSelectOutId(bdsql);

                            if (string.IsNullOrEmpty(salplanid))
                            {
                                //string cpq = $"select id from customize where belongId = 100588408 AND name ='{dataRow["tuno"]}';";
                                //string cpid = sqlSelectOutId(cpq);

                                var json = "{\"belongId\":\"100672402\",\"record\":{"
                                            + $"\"name\":\"{nowtime.ToString()}\""      //主键 //.Substring(0,7)
                                            + $",\"ownerId\":\"{ownerId}\""       //所有人
                                                                                  //+ $",\"customItem2__c\":\"{dataRow["bk"]??"无对应,需手动填入"}\""       //板块
                                            + $",\"customItem20__c\":\"{cpid}\""      //产品id
                                            + $",\"customItem17__c\":\"{custid}\""      //客户id
                                            + $",\"customItem5__c\":\"{dataRow["ccusname"]}\""      //客户简称
                                            + $",\"customItem7__c\":\"{Convert.ToInt32(dataRow["iq"])}\""       //当月累计
                                            + "}}";
                                string id = string.Empty;
                                var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

                                if (result)
                                {
                                    salplanid = id;
                                }
                                else
                                {
                                    logMsg += $"{dataRow["tuno"]}写入错误！\\n";
                                }
                            }

                            //查询上月和当月预测的任何一条，没有则为0

                            //上月预测的查出随意一条如果没有就空着
                            string ymsql = $"select customItem9__c from customize  where belongId = 100767745 AND customItem2__c = '{custid}' and customItem20__c = '{cpid}' and(customItem9__c <> '' and customItem9__c is not null and customItem9__c <> '0') and name like '{nowtime.ToString().Substring(0, 7)}%'";
                            var ycnjson = sqlSelect(ymsql);
                            string ym = !ycnjson.Contains("customItem9__c") ? "0" : ycnjson.Substring(ycnjson.IndexOf("_c\":", StringComparison.Ordinal) + 4, ycnjson.IndexOf("}", StringComparison.Ordinal) - ycnjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

                            //当月预测的查出随意一条如果没有就空着
                            string nmsql = $"select customItem9__c from customize  where belongId = 100767745 AND customItem2__c = '{custid}' and customItem20__c = '{cpid}' and(customItem9__c <> '' and customItem9__c is not null and customItem9__c <> '0') and name like '{befMonDay.Substring(0, 7)}%'";
                            var nmjson = sqlSelect(nmsql);
                            string nm = !ycnjson.Contains("customItem9__c") ? "0" : ycnjson.Substring(ycnjson.IndexOf("_c\":", StringComparison.Ordinal) + 4, ycnjson.IndexOf("}", StringComparison.Ordinal) - ycnjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

                            //去年此月至此日累计发货量
                            string befyeaiqsql = "select sum(iQuantity) as befyeaiq FROM [dbo].[Dispatchlist] "
                                    + $"where dDate BETWEEN '{befYfirDay}' and '{befYerDay}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}'";
                            var befyeaiq = db.ExecuteScalar(befyeaiqsql);
                            string befyeaiqz = string.IsNullOrEmpty(befyeaiq.ToString()) ? "0" : Convert.ToInt32(befyeaiq).ToString();

                            //上月至此日累计发货量
                            string bljiqsql = "select sum(iQuantity) as bljiq FROM [dbo].[Dispatchlist] "
                                    + $"where dDate BETWEEN '{befMfirDay}' and '{befMonDay}' and ccusname = '{dataRow["ccusname"]}' and tuno = '{dataRow["tuno"]}'";
                            var bljiq = db.ExecuteScalar(bljiqsql);
                            string bljiqz = string.IsNullOrEmpty(bljiq.ToString()) ? "0" : Convert.ToInt32(bljiq).ToString();


                            string bksql = "select top 1 bk,cinvname,cinvcname FROM [dbo].[Dispatchlist] "
                                    + $"where tuno = '{dataRow["tuno"]}' and ccusname = '{dataRow["ccusname"]}' ";

                            var ccSt = db.GetDataDataSet(bksql, CommandType.Text);



                            var updatejson = "{"
                                    + $"\"id\":\"{salplanid}\""
                                    + $",\"customItem14__c\":\"{befyeaiqz}\""       //去年此月至此日累计发货量
                                    + $",\"customItem1__c\":\"{ccSt.Tables[0].Rows[0]["cinvcname"]}\""       //大类
                                    + $",\"customItem2__c\":\"{ccSt.Tables[0].Rows[0]["bk"]}\""       //板块
                                    + $",\"customItem4__c\":\"{ccSt.Tables[0].Rows[0]["cinvname"]}\""       //板块
                                    + $",\"customItem8__c\":\"{ym}\""       //上月预测
                                    + $",\"customItem9__c\":\"{nm}\""       //当月预测
                                    + $",\"customItem15__c\":\"{bljiqz}\""       //上月至此日累计发货量Convert.ToInt32(bljiq)
                                    + "}";
                            //String.Format("{0:N2}",Convert.ToDecimal("0.333333").ToString());

                            var res = CustomUpdate(updatejson);

                            if (res)
                            {
                                logMsg += $"{dataRow["tuno"]}计划更新成功！\r\n ";
                            }
                            else
                            {
                                logMsg += $"{dataRow["tuno"]}计划更新失败！\r\n ";
                            }

                        }
                        else
                        {
                            logMsg += $"产品:{dataRow["tuno"]}在CRM上不存在！ \r\n ";
                        }
                    }
                    else
                    {
                        logMsg += $"客户:{dataRow["ccusname"]}在CRM上不存在！ \r\n ";

                    }
                }


            }


            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            LogHelper.Info(logMsg);




            //try
            //{
            //    logMsg = string.Empty;
            //    DateTime dt = DateTime.Now;
            //    //本月第一天时间   
            //    string dt_First = dt.AddDays(-(dt.Day) + 1).ToString("yyyy-MM-dd");
            //    //当前日期
            //    string nowtime = dt.ToString("yyyy-MM-dd");
            //    //上个月的今天
            //    string befMonDay = dt.AddMonths(-1).ToString("yyyy-MM-dd");

            // //db.ExecuteNonQuery("update Dispatchlist set tuno = cInvStd where tuno = '' or tuno
            // is null ; " // + "update Dispatchlist set fhstatus = '1001' where cWhAddress like
            // '%成品库%' and cInvCcode <> '14'and cWhAddress not like '%零件%' and tuno <> '油泵';");

            // db.ExecuteNonQuery("update Dispatchlist set txh = cInvStd; update Dispatchlist set txh
            // = SUBSTRING(txh, 1, (CHARINDEX('-', txh) - 1)) where txh like '%-%' ; "
            // + " update Dispatchlist set bk = b.dybm from Dispatchlist a INNER JOIN dbo.bkb b on
            //   a.cInvCcode = b.lb and a.fromerpno = b.fromerpno; "); //提取发货信息中的产品型号 var dataStr =
            // $"SELECT txh,bk,ccusname,sum(iQuantity) as iq FROM Dispatchlist where dDate BETWEEN
            // '{dt_First}' and '{nowtime}' GROUP BY txh,bk,ccusname"; var tableSt =
            // db.GetDataTable(dataStr, CommandType.Text);

            // if (tableSt.Rows.Count <= 0) { LogHelper.Info("无发货信息！\r\n"); return; } //查询平台有无此条数据
            // //string bdsql = "select id from account where dbcVarchar5 = '{}' "; foreach (DataRow
            // dataRow in tableSt.Rows) { //查询客户所有人 var ownerIdJson = sqlSelect($"select id,ownerId
            // from account where accountName = '{dataRow["ccusname"]}'");

            // if (!ownerIdJson.Contains("records\":[]")) { string custid =
            // ownerIdJson.Substring(ownerIdJson.IndexOf("id\":", StringComparison.Ordinal) + 4,
            // ownerIdJson.IndexOf("ownerId", StringComparison.Ordinal) - 2 -
            // ownerIdJson.IndexOf("id") - 4);

            // //如果为空归给孔经理 string ownerId = !ownerIdJson.Contains("ownerId") ? "847069" :
            // ownerIdJson.Substring(ownerIdJson.IndexOf("ownerId\":", StringComparison.Ordinal) + 9,
            // ownerIdJson.IndexOf("}", StringComparison.Ordinal) - ownerIdJson.IndexOf("ownerId\":",
            // StringComparison.Ordinal) - 9);

            // string bdsql = $"select id from customize where belongId = 100672402 AND
            // customItem1__c = '{dataRow["txh"]}' and name = '{nowtime.ToString()} and
            // customItem3__c = {custid}'"; string salplanid = sqlSelectOutId(bdsql);

            // if (string.IsNullOrEmpty(salplanid)) {


            // var json = "{\"belongId\":\"100672402\",\"record\":{"
            // + $"\"name\":\"{nowtime.ToString()}\"" //主键 //.Substring(0,7)
            // + $",\"ownerId\":\"{ownerId}\"" //所有人
            // + $",\"customItem2__c\":\"{dataRow["bk"]}\"" //板块
            // + $",\"customItem1__c\":\"{dataRow["txh"]}\"" //产品型号
            // + $",\"customItem3__c\":\"{custid}\"" //客户
            // + $",\"customItem5__c\":\"{dataRow["ccusname"]}\"" //客户简称
            // + "}}"; string id = string.Empty; var result = CustomCreate(json, out id);//调用销售易同步接口同步数据

            // if (result) { salplanid = id; } else { logMsg += $"{dataRow["txh"]}写入错误！\\n";
            // continue; } }

            // //查询上月和当月预测的任何一条，没有则为0

            // //上月预测的查出随意一条如果没有就空着 string ymsql = $"select customItem8__c from customize where
            // belongId = 100635973 AND customItem1__c = '{dataRow["txh"]}' and(customItem8__c <> ''
            // and customItem8__c is not null and customItem8__c <> '0') and name like
            // '{nowtime.ToString().Substring(0, 7)}%'"; var ycnjson = sqlSelect(ymsql); string ym =
            // !ycnjson.Contains("customItem8__c") ? "0" : ycnjson.Substring(ycnjson.IndexOf("_c\":",
            // StringComparison.Ordinal) + 4, ycnjson.IndexOf("}", StringComparison.Ordinal) -
            // ycnjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

            // //当月预测的查出随意一条如果没有就空着 string nmsql = $"select customItem9__c from customize where
            // belongId = 100635973 AND customItem1__c = '{dataRow["txh"]}' and(customItem9__c <> ''
            // and customItem9__c is not null and customItem9__c <> '0') and name like
            // '{befMonDay.Substring(0, 7)}%'"; var nmjson = sqlSelect(nmsql); string nm =
            // !ycnjson.Contains("customItem9__c") ? "0" : ycnjson.Substring(ycnjson.IndexOf("_c\":",
            // StringComparison.Ordinal) + 4, ycnjson.IndexOf("}", StringComparison.Ordinal) -
            // ycnjson.IndexOf("_c\":", StringComparison.Ordinal) - 4); ;

            // var updatejson = "{"
            // + $"\"id\":\"{salplanid}\""
            // + $",\"customItem7__c\":\"{Convert.ToInt32(dataRow["iq"])}\"" //累计
            // + $",\"customItem8__c\":\"{ym}\"" //上月预测
            // + $",\"customItem9__c\":\"{nm}\"" //当月预测
            // + "}"; //String.Format("{0:N2}",Convert.ToDecimal("0.333333").ToString());

            // var res = CustomUpdate(updatejson);

            // if (res) { logMsg += $"{dataRow["txh"]}计划更新成功！\r\n "; } else { logMsg +=
            // $"{dataRow["txh"]}计划更新失败！\r\n "; }

            // } else { logMsg += $"{dataRow["ccusname"]}在CRM上不存在！ \r\n ";

            //        }
            //}


            //}

            //catch (Exception exception)
            //{
            //    var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
            //    LogHelper.Error(msg);
            //}
            //LogHelper.Info(logMsg);






        }




        #endregion
    }
}
