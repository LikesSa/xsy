namespace xsy.likes.WinFormTest
{
    partial class FormPub
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
            this.button_jvinterwrong = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_iftext = new System.Windows.Forms.Button();
            this.button_jvstop = new System.Windows.Forms.Button();
            this.button_exit = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_jvinterwrong
            // 
            this.button_jvinterwrong.Location = new System.Drawing.Point(354, 27);
            this.button_jvinterwrong.Name = "button_jvinterwrong";
            this.button_jvinterwrong.Size = new System.Drawing.Size(76, 30);
            this.button_jvinterwrong.TabIndex = 0;
            this.button_jvinterwrong.Text = "开启";
            this.button_jvinterwrong.UseVisualStyleBackColor = true;
            this.button_jvinterwrong.Click += new System.EventHandler(this.button_jvinterwrong_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "聚源断网提醒：";
            // 
            // button_iftext
            // 
            this.button_iftext.Location = new System.Drawing.Point(142, 27);
            this.button_iftext.Name = "button_iftext";
            this.button_iftext.Size = new System.Drawing.Size(171, 30);
            this.button_iftext.TabIndex = 2;
            this.button_iftext.Text = "聚源接口通断测试";
            this.button_iftext.UseVisualStyleBackColor = true;
            this.button_iftext.Click += new System.EventHandler(this.button_iftext_Click);
            // 
            // button_jvstop
            // 
            this.button_jvstop.Location = new System.Drawing.Point(466, 27);
            this.button_jvstop.Name = "button_jvstop";
            this.button_jvstop.Size = new System.Drawing.Size(75, 30);
            this.button_jvstop.TabIndex = 3;
            this.button_jvstop.Text = "停止";
            this.button_jvstop.UseVisualStyleBackColor = true;
            this.button_jvstop.Click += new System.EventHandler(this.button_jvstop_Click);
            // 
            // button_exit
            // 
            this.button_exit.Location = new System.Drawing.Point(457, 380);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(75, 23);
            this.button_exit.TabIndex = 4;
            this.button_exit.Text = "退出";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(298, 160);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 23);
            this.button_start.TabIndex = 5;
            this.button_start.Text = "运行";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // FormPub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 440);
            this.Controls.Add(this.button_start);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_jvstop);
            this.Controls.Add(this.button_iftext);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_jvinterwrong);
            this.Name = "FormPub";
            this.Text = "FormPub";
            this.Load += new System.EventHandler(this.FormPub_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_jvinterwrong;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_iftext;
        private System.Windows.Forms.Button button_jvstop;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Button button_start;
    }
}