namespace OsEasy_Cloud_ToolBox
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label_1 = new System.Windows.Forms.Label();
            this.button_1 = new System.Windows.Forms.Button();
            this.button_2 = new System.Windows.Forms.Button();
            this.button_3 = new System.Windows.Forms.Button();
            this.button_4 = new System.Windows.Forms.Button();
            this.picture_box_1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picture_box_1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_1
            // 
            this.label_1.AutoSize = true;
            this.label_1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_1.Location = new System.Drawing.Point(18, 13);
            this.label_1.Name = "label_1";
            this.label_1.Size = new System.Drawing.Size(39, 16);
            this.label_1.TabIndex = 0;
            this.label_1.Text = "一言";
            this.label_1.Click += new System.EventHandler(this.label_1_click);
            // 
            // button_1
            // 
            this.button_1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_1.Location = new System.Drawing.Point(23, 43);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(225, 50);
            this.button_1.TabIndex = 1;
            this.button_1.Text = "关学生端";
            this.button_1.UseVisualStyleBackColor = true;
            this.button_1.Click += new System.EventHandler(this.button1_click);
            // 
            // button_2
            // 
            this.button_2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_2.Location = new System.Drawing.Point(254, 43);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(225, 50);
            this.button_2.TabIndex = 2;
            this.button_2.Text = "解禁网络";
            this.button_2.UseVisualStyleBackColor = true;
            this.button_2.Click += new System.EventHandler(this.button2_click);
            // 
            // button_3
            // 
            this.button_3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_3.Location = new System.Drawing.Point(23, 99);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(225, 50);
            this.button_3.TabIndex = 3;
            this.button_3.Text = "解禁U盘";
            this.button_3.UseVisualStyleBackColor = true;
            this.button_3.Click += new System.EventHandler(this.button3_click);
            // 
            // button_4
            // 
            this.button_4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_4.Location = new System.Drawing.Point(254, 99);
            this.button_4.Name = "button_4";
            this.button_4.Size = new System.Drawing.Size(225, 50);
            this.button_4.TabIndex = 4;
            this.button_4.Text = "更多工具";
            this.button_4.UseVisualStyleBackColor = true;
            this.button_4.Click += new System.EventHandler(this.button4_click);
            // 
            // pictureBox1
            // 
            this.picture_box_1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.picture_box_1.Location = new System.Drawing.Point(432, 6);
            this.picture_box_1.Name = "picture_box_1";
            this.picture_box_1.Size = new System.Drawing.Size(34, 35);
            this.picture_box_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picture_box_1.TabIndex = 5;
            this.picture_box_1.TabStop = false;
            this.picture_box_1.Click += new System.EventHandler(this.picture_box1_click);
            // 
            // Main
            // 
            this.AcceptButton = this.button_1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 170);
            this.Controls.Add(this.picture_box_1);
            this.Controls.Add(this.button_4);
            this.Controls.Add(this.button_3);
            this.Controls.Add(this.button_2);
            this.Controls.Add(this.button_1);
            this.Controls.Add(this.label_1);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "OsEasy_Cloud_ToolBox";
            this.Load += new System.EventHandler(this.main_form_load);
            ((System.ComponentModel.ISupportInitialize)(this.picture_box_1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_1;
        private System.Windows.Forms.Button button_1;
        private System.Windows.Forms.Button button_2;
        private System.Windows.Forms.Button button_3;
        private System.Windows.Forms.Button button_4;
        private System.Windows.Forms.PictureBox picture_box_1;
    }
}

