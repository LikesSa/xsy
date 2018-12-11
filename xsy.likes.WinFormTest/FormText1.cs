using csapi;
using System;
using System.Windows.Forms;
using xsy.jvif;
using xsy.likes.Db;
using xsy.likes.pwmd;
using XsyAdw;
using XsyRlx;
using System.Timers;
using xsy.likes.Log;
using System.Diagnostics;
using xsy.likes.myif;

namespace xsy.likes.WinFormTest
{
    public partial class FormText1 : Form
    {
        public FormText1()
        {
            InitializeComponent();
        }

        private void button_tokeGet_Click(object sender, EventArgs e)
        {
            
        }

        private void button_text_Click(object sender, EventArgs e)
        {
            //mydata md = new mydata();
            //jd.xiugai();
            //cedata cd = new cedata();
            AdwData rd = new AdwData();
            //JvData jd = new JvData();
            //jd.deliveryorderGetNew();
            rd.CustomerBackAllTotwo();


            //textBox_show.Text = rd.infoShow();




        }

        private void button_mm_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            int days = DateTime.DaysInMonth(dt.Year,2);
            textBox_show.Text = days.ToString();
            //int days1 = DateTime.DaysInMonth(2012, 2);
            //textBox_show.Text += "kk" + days1.ToString();
        }

        private void FormText1_Load(object sender, EventArgs e)
        {

        }


        private JvData jd = new JvData();       //通用接口操作类
        private IDbOperate db = new DbOperate();         //数据库连接
        private static readonly System.Timers.Timer t1 = new System.Timers.Timer(); //设置时间间隔为time1 分
        private string workingtimeofen2 = string.Empty;   //测试库存每次同步耗时

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            t1.Interval = Convert.ToDouble(t1) * 60 * 1000; //设置时间间隔为time1 分


            t1.Elapsed += T1_Elapsed;
            t1.AutoReset = true;
            t1.Start();
        }

        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.Info("执行Timer_Tick");
            t1.Stop();
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
                t1.Start();
            }
        }

        private void button_dataceshi_Click(object sender, EventArgs e)
        {
            IDbOperate dbt = new DbOperate();         //数据库连接
            try
            {

                string result = string.Empty;
                bool fa = dbt.TestConnection(out result);
                if (fa)
                {
                    MessageBox.Show("连接成功，可以进行下一步操作！");
                }
                else
                {
                    MessageBox.Show("连接失败，请检查数据库配置！" + result);

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                dbt.Dispose();
            }
        }

        private void button_jiekouceshi_Click(object sender, EventArgs e)
        {
            string cc = "77888";
            cc = jd.inText();
            if (string.IsNullOrEmpty(cc) || cc.Equals("77888"))
            {

                MessageBox.Show("接口未连接，请检查网络和配置文件！");

            }
            else
            {
                MessageBox.Show("接口已成功连接，可以进行下一步操作。");
            }
        }
    }
}
