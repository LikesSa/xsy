using Microsoft.Win32;
using System;
using System.Windows.Forms;
using xsy.likes.Db;
using XsyAdw;
using System.Timers;
using xsy.likes.Log;
using System.Diagnostics;

namespace xsy.likes.WinFormTest
{
    public partial class FormAdw : Form
    {
        public FormAdw()
        {
            InitializeComponent();
        }

        #region
        private AdwData rd = new AdwData();       //通用接口操作类
        private IDbOperate db = new DbOperate();         //数据库连接
        private static readonly System.Timers.Timer t1 = new System.Timers.Timer(); //设置时间间隔为time1 分
        private static readonly System.Timers.Timer t2 = new System.Timers.Timer(); //设置时间间隔为time2 分
        private string workingtimeofen2 = string.Empty;   //测试库存每次同步耗时
        private int status1 = 1;               //标记，当为1.。。计时器1继续进行 ，置为2 停止运行。
        private int status2 = 1;                //标记，当为1.。。计时器2继续进行，置为2 停止运行。




        #endregion

        #region  窗体类
        /// <summary>
        /// 接口连接测试
        /// </summary>
        private void button_text1_Click(object sender, EventArgs e)
        {
            string cc = "77888";
            cc = rd.inText();
            if (string.IsNullOrEmpty(cc) || cc.Equals("77888"))
            {
                MessageBox.Show("接口未连接，请检查网络和配置文件！");
            }
            else
            {
                MessageBox.Show("接口已成功连接，可以进行下一步操作。");
            }
        }

        /// <summary>
        /// 数据库连接测试
        /// </summary>
        private void button_text2_Click(object sender, EventArgs e)
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

        /// <summary>
        /// 打开日志
        /// </summary>
        private void button_openLog_Click(object sender, EventArgs e)
        {
            //获取路径
            string path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string logpath = path + @"\Log";
            if (!System.IO.Directory.Exists(logpath))
            {
                System.IO.Directory.CreateDirectory(logpath);//不存在就创建目录 
            }
            System.Diagnostics.Process.Start(logpath);
        }

        /// <summary>
        /// 开机启动
        /// </summary>
        private void button_openStart_Click(object sender, EventArgs e)
        {
            try
            {
                //string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string path = this.GetType().Assembly.Location; //获取路径
                string ShortFileName = Application.ProductName;      //获取程序名
                                                                     //string strAssName = Application.StartupPath + @"\" + Application.ProductName + @".exe";
                RegistryKey HKLM = Registry.LocalMachine;
                RegistryKey Run = HKLM.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

                if (Run == null)
                {
                    Run = HKLM.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                }

                //Run.SetValue(ShortFileName, path);
                Run.SetValue("Autorun", path);
                HKLM.Close();
                MessageBox.Show("设置开机启动成功！");
            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message.ToString());
            }
        }

        /// <summary>
        /// 任务栏图标
        /// </summary>
        private void ntfi_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;  //显示在系统任务栏
            this.WindowState = FormWindowState.Normal;  //还原窗体
        }


        /// <summary>
        /// 完全退出
        /// </summary>
        private void button_exit_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要退出吗?", "此操作是程序完全退出", messButton);
            if (dr == DialogResult.OK)//如果点击“确定”按钮

            {
                Dispose();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 关闭改为最小化
        /// </summary>
        private void FormAdw_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            // 将窗体变为最小化
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// 打开窗口立马加载
        /// </summary>
        private void FormAdw_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 最小化去掉任务栏图标
        /// </summary>
        private void FormAdw_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)  //判断是否最小化
            {
                this.ShowInTaskbar = false;  //不显示在系统任务栏
                ntfi.Visible = true;  //托盘图标可见
            }
        }

        #endregion


        /// <summary>
        /// 定时器1
        /// </summary>
        private void button_start_Click(object sender, EventArgs e)
        {
            string time1 = textBox_time1.Text;
            double a = 5;
            if (string.IsNullOrWhiteSpace(time1))
            {
                MessageBox.Show("请输入同步的间隔时间！");
            }
            else
            {
                button_start.Enabled = false;
                textBox_time1.ReadOnly = true;
                try
                {
                    a = Convert.ToDouble(time1);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message.ToString());
                }

                t1.Interval = a * 60 * 1000; //设置时间间隔为time1 分


                t1.Elapsed += T1_Elapsed;
                t1.Start();
                t1.AutoReset = true;
            }
        }


        /// <summary>
        /// 定时器1 的内容
        /// </summary>
        private void T1_Elapsed(object sender, ElapsedEventArgs e)
        {
            status1 = 1;
            LogHelper.Info("启动本轮同步");
            t1.Stop();
            try
            {
                //ntfi.Text = "程序正在进行增客户信息。。。。";
                rd.CustomerBackToERP();
                //ntfi.Text = "程序正在进行修改客户信息。。。。";
                rd.CustomerBackToERPUpdate();

                //ntfi.Text = "程序正在进行增添产品信息。。。。";
                rd.GoodsCreate();
                ////ntfi.Text = "程序正在进行修改产品信息。。。。";
                rd.GoodsUpdate();

                //ntfi.Text = "程序正在进行增加库存信息。。。。";
                rd.StockCreate();
                ////ntfi.Text = "程序正在进行修改库存信息。。。。";
                //rd.StockUpdate();

                ////ntfi.Text = "程序正在进行新增发货信息。。。。";
                rd.DispatchlistCreate();
                ////ntfi.Text = "程序正在进行修改发货信息。。。。";
                rd.DispatchlistUpdate();

                //ntfi.Text = "程序正在进行新增开票信息。。。。";
                rd.SaleBillVouchCreate();
                //ntfi.Text = "程序正在进行修改开票信息。。。。";
                rd.SaleBillVouchUpdate();


                //ntfi.Text = "程序正在进行新增回款信息。。。。";
                rd.closebillCreate();




            }
            catch (Exception exception)
            {
                LogHelper.Error(exception.Message);
            }
            finally
            {
                LogHelper.Info("本轮同步结束");
                if (status1 != 2)
                {
                    t1.Start();
                }
                else
                {
                    t1.Stop();
                }
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        private void button_Stop_Click(object sender, EventArgs e)
        {
            status1 = 2;
            t1.Stop();
            button_start.Enabled = true;
            textBox_time1.ReadOnly = false;
        }

        #region
        /// <summary>
        /// 手动强制回写客户信息
        /// </summary>
        private void button_sdkh_Click(object sender, EventArgs e)
        {
            //rd.CustomerBackAllTotwo();
            //rd.CustomerBackAllToERP();
            //MessageBox.Show("完成！");
            ////rd.CustomerBackAllToERP();
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        //产品
        private void button_yuce01_Click(object sender, EventArgs e)
        {
            //rd.PlanOfFh();
            //MessageBox.Show("完成");
        }


        //客户
        private void button_yuce02_Click(object sender, EventArgs e)
        {
            //rd.saPlanOfCus();
            //MessageBox.Show("完成");
        }

        //业务主管
        private void button_yuce03_Click(object sender, EventArgs e)
        {
            //rd.saPlanOfSz();
            //MessageBox.Show("完成");
        }

        #endregion


        /// <summary>
        /// 同步库存
        /// </summary>
        private void button_kucunsta_Click(object sender, EventArgs e)
        {
            status2 = 1;

            string time2 = TextBox_time2.Text;
            double b = 5;
            if (string.IsNullOrWhiteSpace(time2))
            {
                MessageBox.Show("请输入库存同步的间隔时间！");
            }
            else
            {
                button_kucunsta.Enabled = false;
                TextBox_time2.ReadOnly = true;
                try
                {
                    b = Convert.ToDouble(time2);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message.ToString());
                }

                t2.Interval = b * 60 * 1000; //设置时间间隔为time1 分


                t2.Elapsed += T2_Elapsed;
                t2.Start();
                t2.AutoReset = true;
            }
        }

        private void T2_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.Info("启动库存同步");
            t2.Stop();
            try
            {
                Stopwatch sth = new Stopwatch();
                sth.Start();
                //ntfi.Text = "程序正在进行修改库存信息。。。。";
                rd.StockUpdate();

                //for (int c = 1; c< 20;c++)
                //{
                //    int a = c + 12;
                //}
                sth.Stop();
                workingtimeofen2 = (sth.ElapsedMilliseconds / 1000).ToString();
                ntfi.Text = "本轮库存同步结束。。。。";
            }
            catch (Exception exception)
            {
                LogHelper.Error(exception.Message);
            }
            finally
            {

                LogHelper.Info("本轮库存同步结束。耗时" + workingtimeofen2 + "秒");
                if (status2 != 2)
                {
                    t2.Start();

                }
                else
                {
                    t2.Stop();
                }
            }
        }

        /// <summary>
        /// 库存同步停止
        /// </summary>
        private void button_kuncunend_Click(object sender, EventArgs e)
        {
            status2 = 2;
            t2.Stop();
            button_kucunsta.Enabled = true;
            TextBox_time2.ReadOnly = false;
        }

        private void label_timePast_Click(object sender, EventArgs e)
        {

        }
    }
}
