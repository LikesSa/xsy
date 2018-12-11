using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using xsy.likes.Base;
using xsy.likes.Log;
using XsyAdw;

namespace ADW.Protal.WindowsService
{
    public partial class Service1 : ServiceBase
    {

        private AdwData rd = new AdwData();       //通用接口操作类
        private string workingtimeofen2 = string.Empty;   //测试库存每次同步耗时
        private string t1 = ConfigAppSetting.AppSettingsGet("t1");
        //private string t2 = ConfigAppSetting.AppSettingsGet("t2");
        Timer timer = new Timer();
        //Timer timer2 = new Timer();

        public Service1()
        {
            InitializeComponent();
            try
            {
                timer.Interval = Convert.ToDouble(t1) * 60 * 1000; //设置时间间隔为time1 分
                timer.Elapsed += T1_Elapsed;

                //timer2.Interval = Convert.ToDouble(t2) * 60 * 1000; //设置时间间隔为time1 分
                //timer2.Elapsed += T2_Elapsed;


            }
            catch (Exception exception)
            {
                LogHelper.Error($"初始化启动XSYIF时错误：{exception.Message}");
            }
        }

        //private void T2_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    LogHelper.Info("执行T2");
        //    timer2.Stop();
        //    try
        //    {

        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Error($"timer中出现错误:{exception.Message}");
        //    }
        //    finally
        //    {
        //        timer2.Start();
        //    }
        //}

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.Info("执行Timer_Tick");
            timer.Stop();
            try
            {
                LogHelper.Info("同步客户到ERP");
                rd.CustomerBackToERP();
                rd.CustomerBackToERPUpdate();


                LogHelper.Info("正在处理产品信息");
                rd.GoodsCreate();
                rd.GoodsUpdate();

                LogHelper.Info("正在增加库存信息");
                rd.StockCreate();

                Stopwatch sth = new Stopwatch();
                sth.Start();
                LogHelper.Info("正在更新库存信息");
                rd.StockUpdate();
                sth.Stop();
                workingtimeofen2 = (sth.ElapsedMilliseconds / 1000).ToString();
                LogHelper.Info("本轮库存同步耗时" + workingtimeofen2 + "秒");

                LogHelper.Info("正在处理发货信息");
                rd.DispatchlistCreate();
                rd.DispatchlistUpdate();

                LogHelper.Info("正在处理开票信息");
                rd.SaleBillVouchCreate();
                rd.SaleBillVouchUpdate();


                LogHelper.Info("正在处理回款信息");
                rd.closebillCreate();
                rd.closebillUpdate();




            }
            catch (Exception exception)
            {
                LogHelper.Error($"timer中出现错误:{exception.Message}");
            }
            finally
            {
                timer.Start();
            }
        }



    protected override void OnStart(string[] args)
    {
        timer.Start();
        LogHelper.Info($"timer启动");
    }

    protected override void OnStop()
    {
        timer.Stop();
        LogHelper.Info($"timer关闭");
    }
}
}
