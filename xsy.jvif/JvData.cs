using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using xsy.likes.Base;
using xsy.likes.Db;
using xsy.likes.Log;
using xsy.likes.pwmd;

namespace xsy.jvif
{
    public class JvData : pwapi
    {
        #region 配置文件和参数
        private const string UserId = "13603715895";     //用户名
        private const string Pwd = "jasmine780925";
        private const string Code = "sTwwiu49";          //安全令牌
        private const string TokenClientId = "e2a23f172f9c686f7e2f4953577dee05";
        private const string TokenClientSecret = "609a1d958e2d623369b2cacc26c72ba8";
        private const string TokenRedirectUrl = "http://www.api.jy.com";
        private string dwh = ConfigAppSetting.AppSettingsGet("dwh");
        private string jsz1 = ConfigAppSetting.AppSettingsGet("fsz1");
        private string jsz = ConfigAppSetting.AppSettingsGet("jsz");

        DbOperate db = new DbOperate();
        private const int countOneTime = 30;//每次取的个数

        public string logMsg = string.Empty;//全局Log对象，用于记录错误日志

        #endregion

        /// <summary>
        /// 构造方法
        /// </summary>
        public JvData() : base(UserId, Pwd, Code, TokenClientId, TokenClientSecret, TokenRedirectUrl)
        {

        }

        //测试方法
        public string mesGet()
        {


                return ResultOfthebatch("606253e54780cbd0e7f355107d1a8584030d421b459248e796bd96051ef29c47");
            //string qwqwq = "select id from user where name = '孙涛'";   //2183308
            //DateTime dt = new DateTime();
            //string qq = $"select id,customItem26__c from customEntity33__c limit 3000; ";
            ////return xoqlGet(qq);
            ////return customizeGet("customEntity33__c");

            //var json = "{"
            //        + $"\"id\":\"{"126485782"}\""
            //        + $",\"lockStatus\":\"{2}\""       //开票总额
            //        + "}";
            // CustomUpdate(json);
            return null;
        }

        //指标项替换
        public string yclmcidGet(string yclmc)
        {
            switch (yclmc)
            {
                case "水泥": return "1";
                case "粉煤灰": return "2";
                case "粗砂": return "3";
                case "机制粗砂": return "3";
                case "细砂": return "5";
                case "河细砂": return "5";
                case "中砂": return "4";
                case "机制中砂": return "4";
                case "外加剂": return "6";
                case "抗裂纤维": return "7";
                case "聚丙烯抗裂纤维": return "7";
                case "小料": return "8";
                case "防冻剂": return "9";
                case "其他": return "10";
                case "包装袋": return "11";
                case "纤维素醚": return "10";
                default: return "10";
            }


        }

        ////采购入库数据采集
        //public void deliveryorderGet()
        //{
        //    try
        //    {
        //        logMsg = string.Empty;

        //        //提取发货信息中的客户信息
        //        var dataStr = $"SELECT top {countOneTime} * FROM [dbo].[zy_jl] where By1 = '进货' and pzsj >=  '2018-06-18 06:00'  and (status > 0  and status < 10) and pzsj > mzsj and sjzl > 0 and sjzl > 0.1";

        //        var table = db.GetDataSet(dataStr, CommandType.Text);

        //        if (table.Rows.Count <= 0)
        //        {
        //            LogHelper.Info("无信息！\r\n");
        //            return;
        //        }
        //        var successIds = string.Empty;
        //        var failedIds = string.Empty;
        //        var updateSqls = string.Empty;

        //        foreach (DataRow dataRow in table.Rows)
        //        {

        //            //付款对象
        //            string fkdx = string.Empty;
        //            if (dataRow["dwmc"].ToString().Contains("-"))
        //            {
        //                string[] str = dataRow["dwmc"].ToString().Split('-');

        //                fkdx = str[1];
        //            }
        //            else
        //            {
        //                fkdx = dataRow["dwmc"].ToString();
        //            }

        //            string fkdxid = sqlSelectOutId($"select id from customize where belongId = 100384191 AND name ='{fkdx}';", "utf-8");
        //            if (string.IsNullOrEmpty(fkdxid))
        //            {
        //                fkdxid = "120095086";
        //            }


        //            //统计报表日期，六点之前的都划到昨天。。。。
        //            long num = 3486599765000L;

        //            string thrq = string.Empty;
        //            string cksj = string.Empty;
        //            string rksj = string.Empty;
        //            bool flag4 = dataRow["pzsj"].ToString().ToString().Contains("/");
        //            if (flag4)
        //            {
        //                DateTime dateTime = Convert.ToDateTime(dataRow["mzsj"].ToString());
        //                DateTime time = Convert.ToDateTime(dataRow["pzsj"].ToString());
        //                rksj = dateTime.ToString("yyyy-MM-dd HH:mm");
        //                cksj = time.ToString("yyyy-MM-dd HH:mm");
        //                num = JvData.ConvertDateTimeToInt(time);
        //                int day = time.Day;
        //                int hour = time.Hour;
        //                bool flag5 = day != 1 && hour < 6;
        //                if (flag5)
        //                {
        //                    thrq = time.AddDays(-1.0).ToString("yyyy-MM-dd");
        //                }
        //                else
        //                {
        //                    thrq = time.ToString("yyyy-MM-dd");
        //                }
        //            }

        //            //查找原材料id
        //            string yclid = yclmcidGet(dataRow["wzmc"].ToString());
        //            if (string.IsNullOrEmpty(yclid))
        //            {
        //                LogHelper.Info("已有库中查不到原材料名称!");
        //            }
        //            //查找供应商id
        //            string gysid = sqlSelectOutId($"select id from customize where belongId = 100831304 AND name ='{dataRow["dwmc"].ToString()}';","utf-8");
        //            if (string.IsNullOrEmpty(gysid))
        //            {
        //                LogHelper.Info(string.Format("已有库中查不到供应商!===={0}", dataRow["dwmc"].ToString()));
        //            }

        //            //采购价格表
        //            string cgjgbid = sqlSelectOutId($"select id from customize where belongId = 100388784 AND customItem21__c ='{gysid}' and customItem9__c = {yclid} and customItem20__c < '{num}'order by customItem20__c desc limit 0,1;", "utf-8");
        //            if (string.IsNullOrEmpty(cgjgbid))
        //            {
        //                cgjgbid = string.Empty;
        //            }

        //            //重量
        //            double jz = Convert.ToDouble(dataRow["jzzl"]);
        //            double kz = Convert.ToDouble(dataRow["Kzzl"]);
                    
        //            //车主
        //            string czid = string.Empty;
        //            //查询车主id
        //            string czjson = sqlSelect($"select customItem2__c from customize where belongId = 100821904 AND name ='{dataRow["chmc"].ToString()}';", "utf-8");
        //            JObject jObject = (JObject)JsonConvert.DeserializeObject(czjson);
        //            if (czjson.Contains("count") && czjson.Contains("customItem2__c"))
        //            {
        //                string value = jObject["records"].ToString();
        //                JArray jArray = (JArray)JsonConvert.DeserializeObject(value);
        //                string customItem2c__c = jArray[0]["customItem2c__c"].ToString();
        //                if (customItem2c__c.Equals("0"))
        //                {
        //                    czid = null;
        //                }
        //                else
        //                {
        //                    czid = customItem2c__c;
        //                }
        //            }

        //            //查询运费单价
        //            if (string.IsNullOrEmpty(czid) && !string.IsNullOrEmpty(cgjgbid))
        //            {
        //                string sql = string.Format($"select customItem12__c from customize where belongId = 100388784 AND id = {cgjgbid};");
        //                string yfjson = this.sqlSelect(sql, "utf-8");
        //                JObject jObject2 = (JObject)JsonConvert.DeserializeObject(yfjson);
        //                if (yfjson.Contains("count") && yfjson.Contains("customItem12__c"))
        //                {
        //                    string value2 = jObject2["records"].ToString();
        //                    JArray jArray2 = (JArray)JsonConvert.DeserializeObject(value2);
        //                    string customItem12__c = jArray2[0]["customItem12__c"].ToString();
        //                    if (customItem12__c.Equals("0"))
        //                    {
        //                        czid = "120249115";
        //                    }
        //                    else
        //                    {
        //                        czid = "120249081";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (string.IsNullOrEmpty(czid) && string.IsNullOrEmpty(cgjgbid))
        //                {
        //                    czid = "120249081";
        //                }
        //            }

        //            var json = "{\"belongId\":\"100802765\",\"record\":{"
        //                        + $"\"name\":\"{"RKD-JY" + dataRow["czxh"]}\""      //主键
        //                        + $",\"customItem33__c\":\"{dataRow["by6"]}\""       //产地
        //                        + $",\"customItem22__c\":\"{dataRow["mzzl"]}\""      //毛重
        //                        + $",\"customItem23__c\":\"{dataRow["pzzl"]}\""       //皮重
        //                        + $",\"customItem24__c\":\"{jz-kz}\""      //净重
        //                        + $",\"customItem25__c\":\"{dataRow["chmc"]}\""       //车牌号
        //                        + $",\"customItem31__c\":\"{dataRow["by4"]}\""       //规格型号（车主）
        //                        + $",\"customItem45__c\":\"{czid}\""       //采购车队
        //                        + $",\"customItem14__c\":\"{cgjgbid}\""      //采购价格表名称
        //                        + $",\"customItem13__c\":\"{yclid}\""      //原材料名称
        //                        + $",\"customItem16__c\":\"{fkdx}\""      //磅房付款对象
        //                        + $",\"customItem41__c\":\"{fkdxid}\""      //付款对象
        //                        + $",\"customItem30__c\":\"{dataRow["szsby"]}\""      // 司磅员
        //                        + $",\"customItem8__c\":\"{dataRow["dwmc"]}\""      //磅房原始供应商名称
        //                        + $",\"customItem34__c\":\"{dataRow["wzmc"]}\""      //磅房原材料名称
        //                        + $",\"customItem17__c\":\"{rksj}\""       //入库时间
        //                        + $",\"customItem18__c\":\"{cksj}\""      //出库时间 
        //                        + $",\"customItem26__c\":\"{thrq}\""      //统计日期，无意义
        //                        + $",\"customItem27__c\":\"{dataRow["by8"]}\""      //对方出厂重量
        //                        + $",\"customItem47__c\":\"{cksj}\""      //具体出库时间
        //                        + "}}";
        //            string id = string.Empty;
        //            var result = CustomCreate(json, out id);//调用销售易同步接口同步数据


        //            if (id.Contains("数据重复"))
        //            {
        //                string sid = sqlSelectOutId(string.Format("select id from customize where belongId = 100802765 AND name ='RKD-JY{0}';", dataRow["czxh"].ToString()));

        //                id = sid;
        //                if (id.Equals("") || id.Length > 12)
        //                {
        //                    result = false;
        //                }
        //                else
        //                {
        //                    result = true;
        //                }

        //            }


        //            if (result)
        //            {
        //                successIds += $"{dataRow["czxh"]},";
        //            }
        //            else
        //            {
        //                failedIds += $"{dataRow["czxh"]},";
        //            }

        //            if (string.IsNullOrWhiteSpace(id))
        //            {
        //                updateSqls += $"update zy_jl set status = status + 1 where czxh = '{dataRow["czxh"]}';";
        //            }
        //            else
        //            {
        //                updateSqls += $"update zy_jl set status = 0,id = '{id}' where czxh = '{dataRow["czxh"]}';";
        //            }


        //        }
        //        logMsg = $"同步采购入库单号为{successIds}。";
        //        var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
        //        if (errorCount > 0)
        //        {
        //            logMsg = errorCount != table.Rows.Count
        //                ? $"同步采购入库单失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{table.Rows.Count}个。失败{failedIds}；成功{successIds}。"
        //                : $"同步采购入库单失败编号为{failedIds}。";
        //        }
        //        //更新本地数据
        //        try
        //        {
        //            db.ExecuteNonQuery(updateSqls, CommandType.Text);
        //        }
        //        catch (Exception exception)
        //        {
        //            logMsg += $"但在更新数据库时失败，失败原因：{exception.Message}";
        //        }

        //        //Log记录
        //        LogHelper.Info(logMsg);

        //    }
        //    catch (Exception exception)
        //    {
        //        var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
        //        LogHelper.Error(msg);
        //    }

        //}

        //日期转换为时间戳
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }

        //新的采购入库数据采集
        public void deliveryorderGetNew()
        {
            try
            {
                logMsg = string.Empty;


                //提取发货信息中的客户信息
                var dataStr = $"SELECT top {countOneTime} * FROM [dbo].[zy_jl] where By1 = '进货' and pzsj >=  '2018-06-18 06:00'  and (status > 0  and status < 10)  and sjzl > 0.1";

                var table = db.GetDataSet(dataStr, CommandType.Text);

                if (table.Rows.Count <= 0)
                {
                    LogHelper.Info("无信息！\r\n");
                    return;
                }
                var successIds = string.Empty;
                var failedIds = string.Empty;
                var updateSqls = string.Empty;

                foreach (DataRow dataRow in table.Rows)
                {

                    #region 付款对象
                    string fkdx = string.Empty;
                    if (dataRow["dwmc"].ToString().Contains("-"))
                    {
                        string[] str = dataRow["dwmc"].ToString().Split('-');

                        fkdx = str[1];
                    }
                    else
                    {
                        fkdx = dataRow["dwmc"].ToString();
                    }
                    string fkdxid = sqlSelectOutId($"select id from customize where belongId = 100384191 AND name ='{fkdx}';");//待审核付款对象
                    if (string.IsNullOrEmpty(fkdxid))
                    {
                        //如果找不到对应为了报表美观，分配一个虚拟的待审付款对象
                        fkdxid = "120095086";//120095086
                    }
                    #endregion

                    long cksjc = 3486599765000;
                    #region 统计报表日期，六点之前的都划到昨天。。。。
                    string thrq = string.Empty;
                    string cksj = string.Empty;
                    string rksj = string.Empty;
                    if (dataRow["pzsj"].ToString().ToString().Contains("/"))
                    {
                        DateTime d1 = Convert.ToDateTime(dataRow["mzsj"].ToString());
                        DateTime d2 = Convert.ToDateTime(dataRow["pzsj"].ToString());

                        rksj = d1.ToString("yyyy-MM-dd HH:mm");
                        cksj = d2.ToString("yyyy-MM-dd HH:mm");
                        cksjc = ConvertDateTimeToInt(d2); 
                        string[] str1 = dataRow["pzsj"].ToString().Remove(13, 3).Replace('/', '-').Split(' ');
                        string ymr = str1[0];
                        string[] nyr = ymr[0].ToString().Split('-');
                        string sj = str1[1];
                        string[] cf = str1[1].ToString().Split(':');
                        if (Convert.ToInt32(cf[0]) < 6)
                        {
                            DateTime date;

                            bool cc = DateTime.TryParse(dataRow["pzsj"].ToString().ToString(), out date);

                            thrq = date.AddDays(-1).ToString("yyyy-MM-dd");

                        }
                        else
                        {

                            thrq = d2.ToString("yyyy-MM-dd");


                        }

                    }
                    #endregion


                    //原材料价格表

                    string yclid = yclmcidGet(dataRow["wzmc"].ToString());

                    if (string.IsNullOrEmpty(yclid))
                    {
                        LogHelper.Info("已有库中查不到原材料名称!");

                    }
                    string gyid = sqlSelectOutId($"select id from customize where belongId = 100831304 AND name ='{dataRow["dwmc"].ToString()}';");
                    if (string.IsNullOrEmpty(yclid))
                    {
                        LogHelper.Info($"已有库中查不到供应商!===={dataRow["dwmc"].ToString()}");

                    }
                    string gysid = sqlSelectOutId($"select id from customize where belongId = 100388784 AND customItem21__c ='{gyid}' and customItem9__c = {yclid} and customItem20__c < '{cksjc}'order by createdAt desc limit 0,1;");
                    if (string.IsNullOrEmpty(gysid))
                    {
                        gysid = string.Empty;
                    }
                    double jz = Convert.ToDouble(dataRow["jzzl"]);
                    double kz = Convert.ToDouble(dataRow["Kzzl"]);

                    #region 车主分配
                    string chezhuid = string.Empty;
                    string chezhujson = sqlSelect($"select customItem2__c from customize where belongId = 100821904 AND name ='{dataRow["chmc"].ToString()}';");
                    JObject chzhjo = (JObject)JsonConvert.DeserializeObject(chezhujson);
                    if (chezhujson.Contains("count") && chezhujson.Contains("customItem2__c"))
                    {
                        string records = chzhjo["records"].ToString();
                        JArray jArray = (JArray)JsonConvert.DeserializeObject(records);//jsonArrayText必须是带[]数组格式字符串
                        string customItem2__c = jArray[0]["customItem2__c"].ToString();
                        if (customItem2__c.Equals("0"))
                        {
                            chezhuid = null;
                        }
                        else
                        {
                            chezhuid = customItem2__c;
                        }

                    }



                    if (string.IsNullOrEmpty(chezhuid) && !string.IsNullOrEmpty(gysid))
                    {
                        //查询运费是否为零把车队给分开   供应商承运120095444--------------待审核车队120095801
                        string sqlcyf = $"select customItem12__c from customize where belongId = 100388784 AND id = {gysid}";
                        string re = sqlSelect(sqlcyf);
                        JObject jo = (JObject)JsonConvert.DeserializeObject(re);
                        //string zone = jo["beijing"]["zone"].ToString();
                        if (re.Contains("count") && re.Contains("customItem12__c"))
                        {
                            string records = jo["records"].ToString();
                            JArray jArray = (JArray)JsonConvert.DeserializeObject(records);//jsonArrayText必须是带[]数组格式字符串
                            string customItem12 = jArray[0]["customItem12__c"].ToString();
                            if (customItem12.Equals("0"))
                            {
                                chezhuid = "120249115";

                            }
                            else
                            {
                                chezhuid = "120249081";

                            }

                        }
                    }
                    else if(string.IsNullOrEmpty(chezhuid) && string.IsNullOrEmpty(gysid))
                    {
                        chezhuid = "120249081";
                    }
                    #endregion

                    var json = "{\"belongId\":\"100802765\",\"record\":{"
                                + $"\"name\":\"{"RKD-JY" + dataRow["czxh"]}\""      //采购明细编号
                                + $",\"customItem33__c\":\"{dataRow["by6"]}\""       //产地
                                + $",\"customItem22__c\":\"{dataRow["mzzl"]}\""      //毛重
                                + $",\"customItem23__c\":\"{dataRow["pzzl"]}\""       //皮重
                                + $",\"customItem24__c\":\"{jz-kz}\""      //净重
                                + $",\"customItem25__c\":\"{dataRow["chmc"]}\""       //车牌号
                                + $",\"customItem31__c\":\"{dataRow["by4"]}\""       //规格型号（车主）
                                + $",\"customItem45__c\":\"{chezhuid}\""       //车主---------
                                + $",\"customItem14__c\":\"{gysid}\""      //供应商名称------------
                                + $",\"customItem13__c\":\"{yclid}\""      //原材料名称 -----------
                                + $",\"customItem16__c\":\"{fkdx}\""      //付款对象
                                + $",\"customItem41__c\":\"{fkdxid}\""      //付款对象id-------
                                + $",\"customItem30__c\":\"{dataRow["szsby"]}\""      //司磅员
                                + $",\"customItem8__c\":\"{dataRow["dwmc"]}\""      //磅房供应商名称
                                + $",\"customItem34__c\":\"{dataRow["wzmc"]}\""      //磅房原材料名称
                                + $",\"customItem17__c\":\"{rksj}\""       //入库时间
                                + $",\"customItem18__c\":\"{cksj}\""      //出库时间 
                                + $",\"customItem26__c\":\"{thrq}\""      //统计日期，无意义
                                + $",\"customItem27__c\":\"{dataRow["by8"]}\""      //对方出厂重量
                                + $",\"customItem47__c\":\"{cksj}\""      //文本格式的出库时间 
                                + "}}";

                    string id = string.Empty;
                    var result = CustomCreate(json, out id);//调用销售易同步接口同步数据


                    if (id.Contains("数据重复"))
                    {
                        string q = $"select id from customize where belongId = 100802765 AND name ='RKD-JY{dataRow["czxh"].ToString()}';";
                        string sid = sqlSelectOutId(q);

                        id = sid;
                        if (string.IsNullOrEmpty(id))
                        {
                            result = false;
                        }
                        else
                        {
                            if (id.Length > 12 )
                            {
                                result = false;
                            }
                            else
                            {
                                result = true;
                            }

                           
                        }

                    }


                    if (result)
                    {
                        successIds += $"{dataRow["czxh"]},";
                    }
                    else
                    {
                        failedIds += $"{dataRow["czxh"]},";
                    }

                    if (string.IsNullOrWhiteSpace(id))
                    {
                        updateSqls += $"update zy_jl set status = status + 1 where czxh = '{dataRow["czxh"]}';";
                    }
                    else
                    {
                        updateSqls += $"update zy_jl set status = 0,id = '{id}' where czxh = '{dataRow["czxh"]}';";
                    }


                }
                logMsg = $"同步采购入库单号为{successIds}。";
                var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                if (errorCount > 0)
                {
                    logMsg = errorCount != table.Rows.Count
                        ? $"同步采购入库单失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{table.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                        : $"同步采购入库单失败编号为{failedIds}。";
                }
                //更新本地数据
                try
                {
                    db.ExecuteNonQuery(updateSqls, CommandType.Text);
                }
                catch (Exception exception)
                {
                    logMsg += $"但在更新数据库时失败，失败原因：{exception.Message}";
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

        //订单发货重量回写
        public void orderBack()
        {
            try
            {
                logMsg = string.Empty;
                //提取发货信息中的客户信息
                var dataStr = $"SELECT top {countOneTime} * FROM [dbo].[zy_jl] where By1 = '发货' and szsj >=  '2018-11-03 06:00' and len(by7)>=11  and (status > 0  and status < 10);";

                var table = db.GetDataSet(dataStr, CommandType.Text);

                if (table.Rows.Count <= 0)
                {
                    LogHelper.Info("无订单数据信息！\r\n");
                    return;
                }
                else
                {
                    var successIds = string.Empty;
                    var failedIds = string.Empty;
                    var updateSqls = string.Empty;

                    foreach (DataRow dataRow in table.Rows)
                    {
                        string rbsj = string.Empty;
                        string cksj = string.Empty;
                        long cksjsjc = 3486599765000L;
                        bool flag2 = dataRow["szsj"].ToString().ToString().Contains("/");
                        if (flag2)
                        {
                            DateTime time = Convert.ToDateTime(dataRow["szsj"].ToString());
                            cksj = time.ToString("yyyy-MM-dd HH:mm");
                            cksjsjc = JvData.ConvertDateTimeToInt(time);
                            int day = time.Day;
                            int hour = time.Hour;
                            bool flag3 = day != 1 && hour < 6;
                            if (flag3)
                            {
                                rbsj = time.AddDays(-1.0).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                rbsj = time.ToString("yyyy-MM-dd");
                            }
                        }
                        double d = Convert.ToDouble(dataRow["by7"].ToString());
                        string ddbh = Math.Floor(d).ToString();
                        //查询订单 id
                        string ddid = sqlSelectOutId($"select id from customize where belongId = 100837560 AND name ='{ddbh}';", "utf-8");

                        //查询车牌号id不存在就创建
                        string cdid = sqlSelectOutId($"select id from customize where belongId = 100799845 AND name ='{dataRow["chmc"].ToString()}';", "utf-8");
                        if (string.IsNullOrEmpty(cdid))
                        {
                            var cdjson = "{\"belongId\":\"100799845\",\"record\":{"
                                        + $"\"entityType\":\"{"100806722"}\""      //
                                        + $",\"name\":\"{dataRow["chmc"].ToString()}\""
                                        + $",\"customItem10__c\":\"{dataRow["chmc"].ToString()}\""
                                        + "}}";
                            CustomCreate(cdjson, out cdid);
                        }

                        //查询产品，然后根据产品查询价格表id
                        string zxjg = string.Empty;
                        string xqjson = this.xoqlGet($"select customItem4__c,customItem26__c.customItem4__c.customItem2__c from customEntity45__c where id = {ddid}", "utf-8");
                        if (xqjson.Contains("\"code\":\"200\""))
                        {
                            JObject jObject = (JObject)JsonConvert.DeserializeObject(xqjson);
                            int xqcount = Convert.ToInt32(jObject["data"]["count"].ToString());
                            if (xqcount > 0)
                            {
                                string ldxmmc = jObject["data"]["records"][0]["customItem4__c"].ToString();
                                string cpid = jObject["data"]["records"][0]["customItem26__c.customItem4__c.customItem2__c"].ToString();
                                string zxjgsql = $"select customItem16__c from customEntity47__c where customItem4__c.customItem2__c = '{cpid}' and customItem1__c.customItem1__c ='{ldxmmc}' and  customItem1__c.customItem10__c  < {cksjsjc} order by customItem1__c.customItem10__c desc limit 1;";
                                string[] array = IdGetSql("customItem16__c", zxjgsql, "utf-8");
                                if (array != null && array.Length != 0)
                                {
                                    zxjg = array[0];
                                }
                            }
                        }

                        //如果存在订单
                        if (!string.IsNullOrEmpty(ddid))
                        {
                            var ddjson = "{"
                                       + $"\"id\":\"{ddid}\""
                                       + $",\"customItem39__c\":\"{dataRow["czxh"]}\""
                                       + $",\"customItem22__c\":\"{dataRow["chmc"]}\""
                                       + $",\"customItem15__c\":\"{dataRow["mzzl"]}\""
                                       + $",\"customItem18__c\":\"{dataRow["pzzl"]}\""
                                       + $",\"customItem16__c\":\"{dataRow["jzzl"]}\""
                                       + $",\"customItem40__c\":\"{rbsj}\""                                     //日报统计时间
                                       + $",\"customItem24__c\":\"{cksj}\""                                 //出库时间
                                       + $",\"customItem41__c\":\"{dataRow["wzmc"]}-{dataRow["by4"]}\""
                                       + $",\"customItem42__c\":\"{"1"}\""
                                       + $",\"customItem20__c\":\"{dataRow["szsby"]}\""
                                       + $",\"customItem46__c\":\"{cdid}\""
                                       + $",\"customItem47__c\":\"{zxjg}\""
                                       + "}";

                            bool fa = this.CustomUpdate(ddjson, "utf-8");

                            if (fa)
                            {
                                successIds += ddid;
                                updateSqls += $"update zy_jl set status = 0,id = '{ddid}' where czxh = '{dataRow["czxh"]}';";
                            }
                            else
                            {
                                failedIds += ddid;
                                updateSqls += $"update zy_jl set status = status + 1 where czxh = '{dataRow["czxh"]}';";
                            }
                        }
                        //如果不存在订单，放在疑问数据里
                        else
                        {
                            string ywddjson = string.Concat(new string[] {
                                      "{\"belongId\":\"101006185\",\"record\":{",
                                      string.Format("\"name\":\"{0}\"", ddbh),
                                      string.Format(",\"customItem1__c\":\"{0}\"", dataRow["czxh"]),
                                      string.Format(",\"customItem2__c\":\"{0}\"", dataRow["dwmc"]),
                                      string.Format(",\"customItem3__c\":\"{0}\"", dataRow["by2"]),
                                      string.Format(",\"customItem4__c\":\"{0}\"", dataRow["chmc"]),
                                      string.Format(",\"customItem8__c\":\"{0}\"", dataRow["mzzl"]),
                                      string.Format(",\"customItem9__c\":\"{0}\"", dataRow["pzzl"]),
                                      string.Format(",\"customItem10__c\":\"{0}\"", dataRow["jzzl"]),
                                      string.Format(",\"customItem5__c\":\"{0}\"", cksj),
                                      string.Format(",\"customItem6__c\":\"{0}-{1}\"", dataRow["wzmc"], dataRow["by4"]),
                                      string.Format(",\"customItem7__c\":\"{0}\"", dataRow["szsby"]),
                                      "}}"
                                    });
                            string ywid = string.Empty;
                            bool ywfa = this.CustomCreate(ywddjson, out ywid);
                            if (ywfa)
                            {
                                successIds += ywid;
                                updateSqls += string.Format("update zy_jl set status = 0 where czxh = '{0}';", dataRow["czxh"]);
                            }
                            else
                            {
                                failedIds += ddbh;
                                updateSqls += $"update zy_jl set status = status + 1 where czxh = '{dataRow["czxh"]}';";
                            }
                        }
                    }
                    logMsg = $"同步的订单号为{successIds}。";
                    var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (errorCount > 0)
                    {
                        logMsg = errorCount != table.Rows.Count
                            ? $"同步订单失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{table.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                            : $"同步订单失败编号为{failedIds}。";
                    }
                    //更新本地数据
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);
                    }
                    catch (Exception exception)
                    {
                        logMsg += $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }

                    //Log记录
                    LogHelper.Info(logMsg);
                }
            }
            catch (Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }




        }

        //订单客户接受回写
        public void orderBack1()
        {
            try
            {
                logMsg = string.Empty;
                string sql = $"SELECT top {countOneTime} * FROM [dbo].[zy_jl] where By1 = '发货' and szsj >=  '2018-11-03 06:00' and len(by7)>=11 and status = 0  and by18 is not null and by18 > 0;";
                DataTable table = db.GetDataSet(sql, CommandType.Text);
                if (table.Rows.Count <= 0)
                {
                    LogHelper.Info("无订单客户接受数量信息！\r\n");
                }
                else
                {
                    string successIds = string.Empty;
                    string failedIds = string.Empty;
                    string updateSqls = string.Empty;
                    foreach (DataRow dataRow in table.Rows)
                    {
                        double d = Convert.ToDouble(dataRow["by7"].ToString());
                        string ddbh = Math.Floor(d).ToString();
                        string ddid = sqlSelectOutId($"select id from customize where belongId = 100837560 AND name ='{ddbh}';");

                        if (!string.IsNullOrEmpty(ddid))
                        {
                            string json = string.Concat(new string[]
                            {
                                "{",
                                string.Format("\"id\":\"{0}\"", ddid),
                                string.Format(",\"customItem19__c\":\"{0}\"", dataRow["by18"]),
                                string.Format(",\"customItem37__c\":\"{0}\"", "1"),
                                "}"
                            });
                            bool fa = this.CustomUpdate(json);
                            if (fa)
                            {
                                successIds += ddid;
                                logMsg += $"{dataRow["czxh"]}确认数量为{dataRow["by18"]};";
                                updateSqls += $"update zy_jl set status = -1 where czxh = '{dataRow["czxh"]}';";
                            }
                            else
                            {
                                failedIds += ddid;
                                logMsg += $"{dataRow["czxh"]}确认数量为{dataRow["by18"]}回写失败！";
                            }
                        }
                        else
                        {
                            logMsg += $"CRM系统无此订单号:{dataRow["by7"]};";
                        }
                    }
                    logMsg = $"订单客户接受数量填写为{successIds}。";
                    var errorCount = failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (errorCount > 0)
                    {
                        logMsg = errorCount != table.Rows.Count
                            ? $"同步订单失败：{failedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length}/{table.Rows.Count}个。失败{failedIds}；成功{successIds}。"
                            : $"同步订单失败编号为{failedIds}。";
                    }
                    //更新本地数据
                    try
                    {
                        db.ExecuteNonQuery(updateSqls, CommandType.Text);
                    }
                    catch (Exception exception)
                    {
                        logMsg += $"但在更新数据库时失败，失败原因：{exception.Message}";
                    }

                    //Log记录
                    LogHelper.Info(logMsg);
                }
            }
            catch(Exception exception)
            {
                var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);
            }
        }

        //断网发送信息
        public void dwtx()
        {
            try
            {
                string lastTimeSql = "select MAX(customItem18__c) from customEntity33__c";
                var lastTimejson = xoqlGet(lastTimeSql);

                JObject jo = (JObject)JsonConvert.DeserializeObject(lastTimejson);
                string code = jo["code"].ToString();
                if (code.Equals("200"))
                {
                    string records = jo["data"]["records"].ToString();
                    JArray jArray = (JArray)JsonConvert.DeserializeObject(records);
                    long lastTime = (long)jArray[0]["MAX(customItem18__c)"];

                    DateTime nowTime = DateTime.Now;
                    var nowtimesjc = ConvertDateTimeToInt(nowTime);
                    long sjc = 60 * 60 * 1000 * Convert.ToInt32(dwh);
                    if (nowtimesjc - lastTime > sjc)
                    {
                        var content = $"已经{dwh}小时没有数据更新了，请检查是否是网络问题引起的！";


                        var tzjson = "{"
                            + $"\"sourceUserId\": \"{740333}\""   //通知发送方
                            + $",\"targetUserId\":\"{740341}\""   //通知接受方
                            + $",\"content\":\"{content}\""
                            +"}";

                        MessageSend(tzjson);

                        var tzjson1 = "{"
                            + $"\"sourceUserId\": \"{740333}\""   //通知发送方
                            + $",\"targetUserId\":\"{740333}\""   //通知接受方
                            + $",\"content\":\"{content}\""
                            + "}";
                        MessageSend(tzjson1);


                    }


                }




            }
            catch (Exception exception)
            {
                 var msg = $"方法{System.Reflection.MethodBase.GetCurrentMethod().Name}错误:{exception.Message}";
                LogHelper.Error(msg);    
            }           


        }

        /// <summary>
        /// 接口测试
        /// </summary>
        public string inText()
        {
            return CustomInterfaceSelect("100387750");
        }

        //新增
        public string xiugai()
        {
            try
            {
                var dataStr = $"select id from customize where belongId = 100797516  and customItem3__c is null or customItem3__c = ''";
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
                    string id = jArray[a]["id"].ToString();


                    //string sqlc = $"select customItem18__c from customize where belongId = 100802765 AND id = '{id}'";
                    //string jsonc = sqlSelect(sqlc);
                    //JObject jos = (JObject)JsonConvert.DeserializeObject(jsonc);
                    //JArray jArraya = (JArray)JsonConvert.DeserializeObject(jos["records"].ToString());
                    //string sl = jArraya[0]["customItem18__c"].ToString();


                    #region json

                    var json = "{"
                    + $"\"id\":\"{id}\""
                            + $",\"customItem3__c\":\"{3}\""       //开票总额
                    + "}";
                    #endregion

                    var result = CustomUpdate(json);//调用销售易同步接口同步数据
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
        