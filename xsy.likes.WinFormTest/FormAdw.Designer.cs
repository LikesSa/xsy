namespace xsy.likes.WinFormTest
{
    partial class FormAdw
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdw));
            this.button_start = new System.Windows.Forms.Button();
            this.button_text1 = new System.Windows.Forms.Button();
            this.button_text2 = new System.Windows.Forms.Button();
            this.button_openStart = new System.Windows.Forms.Button();
            this.label_time = new System.Windows.Forms.Label();
            this.label_timePsatTitle = new System.Windows.Forms.Label();
            this.label_timePast = new System.Windows.Forms.Label();
            this.textBox_time1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_exit = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button_openLog = new System.Windows.Forms.Button();
            this.button_Stop = new System.Windows.Forms.Button();
            this.button_sdkh = new System.Windows.Forms.Button();
            this.ntfi = new System.Windows.Forms.NotifyIcon(this.components);
            this.button_yuce01 = new System.Windows.Forms.Button();
            this.button_yuce02 = new System.Windows.Forms.Button();
            this.button_yuce03 = new System.Windows.Forms.Button();
            this.button_kucunsta = new System.Windows.Forms.Button();
            this.button_kuncunend = new System.Windows.Forms.Button();
            this.TextBox_time2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(38, 176);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(146, 35);
            this.button_start.TabIndex = 0;
            this.button_start.Text = "开始";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_text1
            // 
            this.button_text1.Location = new System.Drawing.Point(37, 114);
            this.button_text1.Name = "button_text1";
            this.button_text1.Size = new System.Drawing.Size(146, 35);
            this.button_text1.TabIndex = 1;
            this.button_text1.Text = "接口接通测试";
            this.button_text1.UseVisualStyleBackColor = true;
            this.button_text1.Click += new System.EventHandler(this.button_text1_Click);
            // 
            // button_text2
            // 
            this.button_text2.Location = new System.Drawing.Point(212, 114);
            this.button_text2.Name = "button_text2";
            this.button_text2.Size = new System.Drawing.Size(146, 35);
            this.button_text2.TabIndex = 2;
            this.button_text2.Text = "数据库接通测试";
            this.button_text2.UseVisualStyleBackColor = true;
            this.button_text2.Click += new System.EventHandler(this.button_text2_Click);
            // 
            // button_openStart
            // 
            this.button_openStart.Location = new System.Drawing.Point(131, 427);
            this.button_openStart.Name = "button_openStart";
            this.button_openStart.Size = new System.Drawing.Size(146, 35);
            this.button_openStart.TabIndex = 3;
            this.button_openStart.Text = "开机启动";
            this.button_openStart.UseVisualStyleBackColor = true;
            this.button_openStart.Visible = false;
            this.button_openStart.Click += new System.EventHandler(this.button_openStart_Click);
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(29, 34);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(90, 15);
            this.label_time.TabIndex = 4;
            this.label_time.Text = "间隔时间 ：";
            // 
            // label_timePsatTitle
            // 
            this.label_timePsatTitle.AutoSize = true;
            this.label_timePsatTitle.Location = new System.Drawing.Point(36, 465);
            this.label_timePsatTitle.Name = "label_timePsatTitle";
            this.label_timePsatTitle.Size = new System.Drawing.Size(112, 15);
            this.label_timePsatTitle.TabIndex = 5;
            this.label_timePsatTitle.Text = "当前运行时间：";
            this.label_timePsatTitle.Visible = false;
            // 
            // label_timePast
            // 
            this.label_timePast.Location = new System.Drawing.Point(185, 437);
            this.label_timePast.Name = "label_timePast";
            this.label_timePast.Size = new System.Drawing.Size(150, 25);
            this.label_timePast.TabIndex = 6;
            this.label_timePast.Visible = false;
            this.label_timePast.Click += new System.EventHandler(this.label_timePast_Click);
            // 
            // textBox_time1
            // 
            this.textBox_time1.Location = new System.Drawing.Point(169, 24);
            this.textBox_time1.Name = "textBox_time1";
            this.textBox_time1.Size = new System.Drawing.Size(136, 25);
            this.textBox_time1.TabIndex = 7;
            this.textBox_time1.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(329, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "分";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(400, 447);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "分";
            this.label2.Visible = false;
            // 
            // button_exit
            // 
            this.button_exit.Location = new System.Drawing.Point(212, 285);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(146, 32);
            this.button_exit.TabIndex = 10;
            this.button_exit.Text = "彻底退出";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(133, 505);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(321, 35);
            this.button1.TabIndex = 11;
            this.button1.Text = "客户货币收入达成";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_openLog
            // 
            this.button_openLog.Location = new System.Drawing.Point(37, 288);
            this.button_openLog.Name = "button_openLog";
            this.button_openLog.Size = new System.Drawing.Size(146, 29);
            this.button_openLog.TabIndex = 12;
            this.button_openLog.Text = "打开日志";
            this.button_openLog.UseVisualStyleBackColor = true;
            this.button_openLog.Click += new System.EventHandler(this.button_openLog_Click);
            // 
            // button_Stop
            // 
            this.button_Stop.Location = new System.Drawing.Point(212, 176);
            this.button_Stop.Name = "button_Stop";
            this.button_Stop.Size = new System.Drawing.Size(146, 35);
            this.button_Stop.TabIndex = 13;
            this.button_Stop.Text = "停止";
            this.button_Stop.UseVisualStyleBackColor = true;
            this.button_Stop.Click += new System.EventHandler(this.button_Stop_Click);
            // 
            // button_sdkh
            // 
            this.button_sdkh.Location = new System.Drawing.Point(62, 511);
            this.button_sdkh.Name = "button_sdkh";
            this.button_sdkh.Size = new System.Drawing.Size(321, 29);
            this.button_sdkh.TabIndex = 14;
            this.button_sdkh.Text = "强制手动写入客户";
            this.button_sdkh.UseVisualStyleBackColor = true;
            this.button_sdkh.Visible = false;
            this.button_sdkh.Click += new System.EventHandler(this.button_sdkh_Click);
            // 
            // ntfi
            // 
            this.ntfi.Icon = ((System.Drawing.Icon)(resources.GetObject("ntfi.Icon")));
            this.ntfi.Text = "销售易ERP同步程序";
            this.ntfi.Visible = true;
            this.ntfi.DoubleClick += new System.EventHandler(this.ntfi_DoubleClick);
            // 
            // button_yuce01
            // 
            this.button_yuce01.Location = new System.Drawing.Point(133, 505);
            this.button_yuce01.Name = "button_yuce01";
            this.button_yuce01.Size = new System.Drawing.Size(255, 35);
            this.button_yuce01.TabIndex = 15;
            this.button_yuce01.Text = "销售计划收入达成—产品";
            this.button_yuce01.UseVisualStyleBackColor = true;
            this.button_yuce01.Visible = false;
            this.button_yuce01.Click += new System.EventHandler(this.button_yuce01_Click);
            // 
            // button_yuce02
            // 
            this.button_yuce02.Location = new System.Drawing.Point(112, 505);
            this.button_yuce02.Name = "button_yuce02";
            this.button_yuce02.Size = new System.Drawing.Size(255, 35);
            this.button_yuce02.TabIndex = 16;
            this.button_yuce02.Text = "销售计划收入达成—客户";
            this.button_yuce02.UseVisualStyleBackColor = true;
            this.button_yuce02.Visible = false;
            this.button_yuce02.Click += new System.EventHandler(this.button_yuce02_Click);
            // 
            // button_yuce03
            // 
            this.button_yuce03.Location = new System.Drawing.Point(112, 505);
            this.button_yuce03.Name = "button_yuce03";
            this.button_yuce03.Size = new System.Drawing.Size(255, 35);
            this.button_yuce03.TabIndex = 17;
            this.button_yuce03.Text = "销售计划收入达成—销售主管";
            this.button_yuce03.UseVisualStyleBackColor = true;
            this.button_yuce03.Visible = false;
            this.button_yuce03.Click += new System.EventHandler(this.button_yuce03_Click);
            // 
            // button_kucunsta
            // 
            this.button_kucunsta.Location = new System.Drawing.Point(38, 228);
            this.button_kucunsta.Name = "button_kucunsta";
            this.button_kucunsta.Size = new System.Drawing.Size(146, 35);
            this.button_kucunsta.TabIndex = 18;
            this.button_kucunsta.Text = "库存开始";
            this.button_kucunsta.UseVisualStyleBackColor = true;
            this.button_kucunsta.Click += new System.EventHandler(this.button_kucunsta_Click);
            // 
            // button_kuncunend
            // 
            this.button_kuncunend.Location = new System.Drawing.Point(212, 228);
            this.button_kuncunend.Name = "button_kuncunend";
            this.button_kuncunend.Size = new System.Drawing.Size(146, 35);
            this.button_kuncunend.TabIndex = 19;
            this.button_kuncunend.Text = "库存结束";
            this.button_kuncunend.UseVisualStyleBackColor = true;
            this.button_kuncunend.Click += new System.EventHandler(this.button_kuncunend_Click);
            // 
            // TextBox_time2
            // 
            this.TextBox_time2.Location = new System.Drawing.Point(169, 65);
            this.TextBox_time2.Name = "TextBox_time2";
            this.TextBox_time2.Size = new System.Drawing.Size(136, 25);
            this.TextBox_time2.TabIndex = 20;
            this.TextBox_time2.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(29, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 15);
            this.label3.TabIndex = 21;
            this.label3.Text = "库存间隔时间 ：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(329, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 15);
            this.label4.TabIndex = 22;
            this.label4.Text = "分";
            // 
            // FormAdw
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 367);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TextBox_time2);
            this.Controls.Add(this.button_kuncunend);
            this.Controls.Add(this.button_kucunsta);
            this.Controls.Add(this.button_yuce03);
            this.Controls.Add(this.button_yuce02);
            this.Controls.Add(this.button_yuce01);
            this.Controls.Add(this.button_sdkh);
            this.Controls.Add(this.button_Stop);
            this.Controls.Add(this.button_openLog);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label_timePsatTitle);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_time1);
            this.Controls.Add(this.label_timePast);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.button_openStart);
            this.Controls.Add(this.button_text2);
            this.Controls.Add(this.button_text1);
            this.Controls.Add(this.button_start);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAdw";
            this.Text = " ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAdw_FormClosing);
            this.Load += new System.EventHandler(this.FormAdw_Load);
            this.SizeChanged += new System.EventHandler(this.FormAdw_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_text1;
        private System.Windows.Forms.Button button_text2;
        private System.Windows.Forms.Button button_openStart;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label_timePsatTitle;
        private System.Windows.Forms.Label label_timePast;
        private System.Windows.Forms.TextBox textBox_time1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_openLog;
        private System.Windows.Forms.Button button_Stop;
        private System.Windows.Forms.Button button_sdkh;
        private System.Windows.Forms.NotifyIcon ntfi;
        private System.Windows.Forms.Button button_yuce01;
        private System.Windows.Forms.Button button_yuce02;
        private System.Windows.Forms.Button button_yuce03;
        private System.Windows.Forms.Button button_kucunsta;
        private System.Windows.Forms.Button button_kuncunend;
        private System.Windows.Forms.TextBox TextBox_time2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}