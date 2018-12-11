namespace xsy.likes.WinFormTest
{
    partial class FormText1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button_tokeGet = new System.Windows.Forms.Button();
            this.textBox_show = new System.Windows.Forms.TextBox();
            this.button_text = new System.Windows.Forms.Button();
            this.button_mm = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button_dataceshi = new System.Windows.Forms.Button();
            this.button_jiekouceshi = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_tokeGet
            // 
            this.button_tokeGet.Location = new System.Drawing.Point(1098, 12);
            this.button_tokeGet.Name = "button_tokeGet";
            this.button_tokeGet.Size = new System.Drawing.Size(145, 22);
            this.button_tokeGet.TabIndex = 0;
            this.button_tokeGet.Text = "获取code";
            this.button_tokeGet.UseVisualStyleBackColor = true;
            this.button_tokeGet.Click += new System.EventHandler(this.button_tokeGet_Click);
            // 
            // textBox_show
            // 
            this.textBox_show.Location = new System.Drawing.Point(27, 315);
            this.textBox_show.Multiline = true;
            this.textBox_show.Name = "textBox_show";
            this.textBox_show.Size = new System.Drawing.Size(999, 300);
            this.textBox_show.TabIndex = 1;
            // 
            // button_text
            // 
            this.button_text.Location = new System.Drawing.Point(449, 190);
            this.button_text.Name = "button_text";
            this.button_text.Size = new System.Drawing.Size(137, 39);
            this.button_text.TabIndex = 2;
            this.button_text.Text = "测试";
            this.button_text.UseVisualStyleBackColor = true;
            this.button_text.Click += new System.EventHandler(this.button_text_Click);
            // 
            // button_mm
            // 
            this.button_mm.Location = new System.Drawing.Point(799, 13);
            this.button_mm.Name = "button_mm";
            this.button_mm.Size = new System.Drawing.Size(86, 25);
            this.button_mm.TabIndex = 3;
            this.button_mm.Text = "Play";
            this.button_mm.UseVisualStyleBackColor = true;
            this.button_mm.Click += new System.EventHandler(this.button_mm_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 98);
            this.button1.TabIndex = 4;
            this.button1.Text = "开始";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_dataceshi
            // 
            this.button_dataceshi.Location = new System.Drawing.Point(200, 72);
            this.button_dataceshi.Name = "button_dataceshi";
            this.button_dataceshi.Size = new System.Drawing.Size(104, 50);
            this.button_dataceshi.TabIndex = 5;
            this.button_dataceshi.Text = "数据库测试";
            this.button_dataceshi.UseVisualStyleBackColor = true;
            this.button_dataceshi.Click += new System.EventHandler(this.button_dataceshi_Click);
            // 
            // button_jiekouceshi
            // 
            this.button_jiekouceshi.Location = new System.Drawing.Point(200, 24);
            this.button_jiekouceshi.Name = "button_jiekouceshi";
            this.button_jiekouceshi.Size = new System.Drawing.Size(104, 41);
            this.button_jiekouceshi.TabIndex = 6;
            this.button_jiekouceshi.Text = "接口测试";
            this.button_jiekouceshi.UseVisualStyleBackColor = true;
            this.button_jiekouceshi.Click += new System.EventHandler(this.button_jiekouceshi_Click);
            // 
            // FormText1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 615);
            this.Controls.Add(this.button_jiekouceshi);
            this.Controls.Add(this.button_dataceshi);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button_mm);
            this.Controls.Add(this.button_text);
            this.Controls.Add(this.textBox_show);
            this.Controls.Add(this.button_tokeGet);
            this.Name = "FormText1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FormText1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_tokeGet;
        private System.Windows.Forms.TextBox textBox_show;
        private System.Windows.Forms.Button button_text;
        private System.Windows.Forms.Button button_mm;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_dataceshi;
        private System.Windows.Forms.Button button_jiekouceshi;
    }
}

