using System;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using xsy.jvif;
using XsyAdw;


namespace xsy.likes.WinFormTest
{
    public partial class FormPub : Form
    {
        public FormPub()
        {
            InitializeComponent();
        }
        private JvData jd = new JvData();
        System.Timers.Timer t = new System.Timers.Timer(Convert.ToDouble(1) * 60 * 1000);
        private void button_jvinterwrong_Click(object sender, EventArgs e)
        {
            button_jvinterwrong.Enabled = false;
            t.Enabled = true; //是否触发Elapsed事件
            t.Start();

        }

        private void button_iftext_Click(object sender, EventArgs e)
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

        private void button_jvstop_Click(object sender, EventArgs e)
        {
            t.Stop();
            button_jvinterwrong.Enabled = true;
             
        }

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

        private AdwData ad = new AdwData();
        private void button_start_Click(object sender, EventArgs e)
        {
            while (true)
            {
                ad.DispatchlistCreate();
                Thread.Sleep(1000); 
            }

            


        }

        private void FormPub_Load(object sender, EventArgs e)
        {
            t.Elapsed += new System.Timers.ElapsedEventHandler(Timer_TimesUp);
            t.AutoReset = false; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
        }

        private void Timer_TimesUp(object sender, ElapsedEventArgs e)
        {
            jd.dwtx();
        }
    }
}
