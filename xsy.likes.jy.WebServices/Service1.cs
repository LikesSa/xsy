using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using xsy.jvif;
using xsy.likes.Base;
using xsy.likes.Log;

namespace xsy.likes.jy.WebServices
{
    public partial class Service1 : ServiceBase
    {

        private JvData jd = new JvData();       //通用接口操作类
        private string workingtimeofen2 = string.Empty;   //测试库存每次同步耗时
        private string t1 = ConfigAppSetting.AppSettingsGet("t1");
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
            try
            {
                timer.Interval = Convert.ToDouble(t1) * 60 * 1000; //设置时间间隔为time1 分
                timer.Elapsed += T1_Elapsed;
                timer.AutoReset = true;
                
            }
            catch (Exception exception)
            {
                LogHelper.Error($"初始化启动XSYIF时错误：{exception.Message}");
            }



        }

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.Info("执行Timer_Tick");
            timer.Stop();
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                LogHelper.Info("采集采购入库数据");
                jd.deliveryorderGetNew();
                LogHelper.Info("订单发货数据");
                jd.orderBack();
                LogHelper.Info("订单客户接受数量");
                jd.orderBack1();
                stopwatch.Stop();
                workingtimeofen2 = (stopwatch.ElapsedMilliseconds / 1000L).ToString();
                LogHelper.Info("本轮同步耗时" + workingtimeofen2 + "秒");
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
