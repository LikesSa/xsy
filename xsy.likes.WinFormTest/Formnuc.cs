using System;
using System.Windows.Forms;
using xsy.likes.Db;
using XsyAdw;

namespace xsy.likes.WinFormTest
{
    public partial class Formnuc : Form
    {
        public Formnuc()
        {
            InitializeComponent();
        }


        private AdwData rd = new AdwData();       //通用接口操作类
        private IDbOperate db = new DbOperate();         //数据库连接


        /// <summary>
        /// 接口测试
        /// </summary>
        private void button_textif_Click(object sender, EventArgs e)
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
        /// 数据库测试
        /// </summary>
        private void button_textsql_Click(object sender, EventArgs e)
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



        private void Button_xsjhcp_Click(object sender, EventArgs e)
        {
            //rd.CustomerInterfaceSelect();
            //rd.MonNowYc();
            



        }

        private void Formnuc_Load(object sender, EventArgs e)
        {
            string  dt = DateTime.Now.AddMonths(+1).ToString("yyyy-MM-dd");
            dateTimePicker1.Text = dt;
        }

        private void Button_fh_Click(object sender, EventArgs e)
        {
            rd.PlanOfFh();
            MessageBox.Show("完成！");
        }

        private void Button_srdckh_Click(object sender, EventArgs e)
        {
            try
            {
                Button_zysrdcandyc.Enabled = false;
                Button_srdckh.Enabled = false;
                skinButton1.Enabled = false;

                rd.BackMoneyOfCus();
                MessageBox.Show("完成！");

                Button_zysrdcandyc.Enabled = true;
                Button_srdckh.Enabled = true;
                skinButton1.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("发生错误" + exception.ToString());
            }

        }

        private void Button_xsjhkh_Click(object sender, EventArgs e)
        {
            //rd.saPlanOfCus();
            MessageBox.Show("完成！");
        }

        private void Button_xsjhywy_Click(object sender, EventArgs e)
        {
            //rd.saPlanOfSz();
            MessageBox.Show("完成！");
        }

        private void Button_khxg_Click(object sender, EventArgs e)
        {
            rd.CustomerBackToERPUpdate();
            MessageBox.Show("完成！");
        }

        private void Button_zysrdcandyc_Click(object sender, EventArgs e)
        {
            try
            {
                //rd.infoShow();
                Button_zysrdcandyc.Enabled = false;
                Button_srdckh.Enabled = false;
                skinButton1.Enabled = false;
                string data = dateTimePicker1.Text.ToString();
                rd.MonNowYc(data);
                MessageBox.Show("完成！");

                Button_zysrdcandyc.Enabled = true;
                Button_srdckh.Enabled = true;
                skinButton1.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("发生错误" + exception.ToString());
            }
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
           

            try
            {
                Button_zysrdcandyc.Enabled = false;
                Button_srdckh.Enabled = false;
                skinButton1.Enabled = false;

                rd.MonNowsj();
                MessageBox.Show("完成！");

                Button_zysrdcandyc.Enabled = true;
                Button_srdckh.Enabled = true;
                skinButton1.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("发生错误" + exception.ToString());
            }
        }

        private void skinButton2_Click(object sender, EventArgs e)
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

        private void skinButton3_Click(object sender, EventArgs e)
        {
            MessageBoxButtons messButton = MessageBoxButtons.OKCancel;
            DialogResult dr = MessageBox.Show("确定要退出吗?", "此操作是程序完全退出", messButton);
            if (dr == DialogResult.OK)//如果点击“确定”按钮

            {
                Dispose();
                Environment.Exit(0);
            }
        }
    }
}
