namespace OsEasy_Cloud_ToolBox
{
    partial class More
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
            this.button_3 = new System.Windows.Forms.Button();
            this.button_2 = new System.Windows.Forms.Button();
            this.button_1 = new System.Windows.Forms.Button();
            this.label_1 = new System.Windows.Forms.Label();
            this.button_4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button_3
            // 
            this.button_3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_3.Location = new System.Drawing.Point(18, 68);
            this.button_3.Name = "button_3";
            this.button_3.Size = new System.Drawing.Size(225, 50);
            this.button_3.TabIndex = 7;
            this.button_3.Text = "启动教师端";
            this.button_3.UseVisualStyleBackColor = true;
            this.button_3.Click += new System.EventHandler(this.button_3_click);
            // 
            // button_2
            // 
            this.button_2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_2.Location = new System.Drawing.Point(249, 12);
            this.button_2.Name = "button_2";
            this.button_2.Size = new System.Drawing.Size(225, 50);
            this.button_2.TabIndex = 6;
            this.button_2.Text = "启动学生端";
            this.button_2.UseVisualStyleBackColor = true;
            this.button_2.Click += new System.EventHandler(this.button_2_click);
            // 
            // button_1
            // 
            this.button_1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_1.Location = new System.Drawing.Point(18, 12);
            this.button_1.Name = "button_1";
            this.button_1.Size = new System.Drawing.Size(225, 50);
            this.button_1.TabIndex = 5;
            this.button_1.Text = "挂起学生端";
            this.button_1.UseVisualStyleBackColor = true;
            this.button_1.Click += new System.EventHandler(this.button_1_click);
            // 
            // label_1
            // 
            this.label_1.AutoSize = true;
            this.label_1.Font = new System.Drawing.Font("微软雅黑", 16F);
            this.label_1.Location = new System.Drawing.Point(2, 131);
            this.label_1.Name = "label_1";
            this.label_1.Size = new System.Drawing.Size(488, 30);
            this.label_1.TabIndex = 9;
            this.label_1.Text = "Coryright © 2026 kndxhz. All rights reserved.";
            this.label_1.Click += new System.EventHandler(this.label_1_click);
            // 
            // button_4
            // 
            this.button_4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_4.Location = new System.Drawing.Point(249, 68);
            this.button_4.Name = "button_4";
            this.button_4.Size = new System.Drawing.Size(225, 50);
            this.button_4.TabIndex = 8;
            this.button_4.Text = "下载工具箱";
            this.button_4.UseVisualStyleBackColor = true;
            this.button_4.Click += new System.EventHandler(this.button_4_click);
            // 
            // More
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 170);
            this.Controls.Add(this.label_1);
            this.Controls.Add(this.button_4);
            this.Controls.Add(this.button_3);
            this.Controls.Add(this.button_2);
            this.Controls.Add(this.button_1);
            this.MaximizeBox = false;
            this.Name = "More";
            this.Text = "更多工具";
            this.Load += new System.EventHandler(this.more_form_load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_3;
        private System.Windows.Forms.Button button_2;
        private System.Windows.Forms.Button button_1;
        private System.Windows.Forms.Label label_1;
        private System.Windows.Forms.Button button_4;
    }
}