using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xsy.likes.Db;
using xsy.likes.Log;
using xsy.likes.pwmd;

namespace xsy.likes.myif
{
    public class mydata : pwapi
    {

        #region 配置文件和参数
        private const string UserId = "503253134@qq.com";     //用户名
        private const string Pwd = "my002714";
        private const string Code = "Gwhhu4UA";          //安全令牌
        private const string TokenClientId = "18d7c6425428c22d31f9e7d3e47a0fe6";
        private const string TokenClientSecret = "f5376bc0df49a84966684b44510e88ee";
        private const string TokenRedirectUrl = "https://www.xsy.my.com";

        DbOperate db = new DbOperate();
        private const int countOneTime = 30;//每次取的个数

        public string logMsg = string.Empty;//全局Log对象，用于记录错误日志

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public mydata() : base(UserId, Pwd, Code, TokenClientId, TokenClientSecret, TokenRedirectUrl)
        {

        }


        //新增
        public string xiugai()
        {
            try
            {
                var dataStr = $"select id,entityType,ownerId from customize where belongId = 100822253 and approvalStatus = '0' limit 0,300";
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
                    string entityType = jArray[a]["entityType"].ToString();
                    string ownerId = jArray[a]["ownerId"].ToString();
                    string dy = approvalsLcGet("100822253", entityType);

                    JObject jody = (JObject)JsonConvert.DeserializeObject(dy);
                    string dyrecords = jody["records"].ToString();
                    JArray jArraydy = (JArray)JsonConvert.DeserializeObject(dyrecords);//jsonArrayText必须是带[]数组格式字符串
                    string dyid = jArraydy[0]["id"].ToString();

                    //获取下级id

                    //string nextsp = approvalsNextGet("100822253", dataid, null, dyid);


                    var json = "{"
                            + $"\"defineId\":\"{dyid}\""
                                + $",\"dataId\":\"{dataid}\""      //发货单号
                                + $",\"approvalUserId\":\"{"995708"}\""
                            + "}";

                    bool fa = approvalsLcPut(json);

                    //if (fa)
                    //{
                    //            var tjson = "{"
                    //            + $"\"approvalId\":\"{dyid}\""
                    //            + $",\"approvalUserId\":\"{"995708"}\""      //发货单号
                    //            + $",\"comments\":\"{"历史数据通过"}\""
                    //                 + "}";


                    //    approvalsLcPass(tjson);

                    //}
                    



                }




            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            return "null";

        }


        public void tg()
        {
            bool fa = true;
            while (fa)
            {
                string jsonlist = approvalsGet("1", "100", "995708");
                JObject jo = (JObject)JsonConvert.DeserializeObject(jsonlist);
                string records = jo["records"].ToString();
                if (records.Equals("0"))
                {

                    fa = false;
                    return;
                }
                JArray jArray = (JArray)JsonConvert.DeserializeObject(records);//jsonArrayText必须是带[]数组格式字符串
                int count = jArray.Count;
                for (int a = 0; a < count; a++)
                {
                    string id = jArray[a]["id"].ToString();
                    string dataid = jArray[a]["dataId"].ToString();
                    if (!dataid.Equals("122086193") || !dataid.Equals("122215136"))
                    {
                        var tjson = "{"
                        + $"\"approvalId\":\"{id}\""
                        + $",\"approvalUserId\":\"{"995708"}\""
                        + $",\"comments\":\"{"历史数据"}\""
                             + "}";


                        bool hj = approvalsLcPass(tjson);

                    }




                }

            }

        }




        public bool sql()
        {
            //string sqls = "select id from customize where belongId = 100822253 and name = 'CRM005469' limit 0,1;";
            //string pd = sqlSelectOutId(sqls);// 你                                       


            //return customizeGetById("customEntity2__c",pd);
            return picDown("8027202");
    
        }



        //新增
        public string zht()
        {
            try
            {
                var dataStr = $"select id,entityType,ownerId from customize where belongId = 100822253 and customItem79__c is null or customItem79__c = '' limit 0,300";
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
                    string entityType = jArray[a]["entityType"].ToString();
                    string ownerId = jArray[a]["ownerId"].ToString();

                    if (!dataid.Equals("122086193") || !dataid.Equals("122215136") || !dataid.Equals("122218349") )
                    {
                        #region json

                        var json = "{"
                        + $"\"id\":\"{dataid}\""
                        + $",\"customItem79__c\":\"{"3"}\""       //账套
                        + "}";
                        #endregion

                        var result = CustomUpdate(json);//调用销售易同步接口同步数据

                    }


                }




            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
            return "null";

        }












    }
}
