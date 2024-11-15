using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace OsEasy_Cloud_ToolBox
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "这是一个预留的按钮\n目前还不知道可以做什么\n有想法欢迎发issue",  // 消息内容
            "",                          // 标题
            MessageBoxButtons.OK,          
            MessageBoxIcon.Information);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("kndxhz.cn") { UseShellExecute = true });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = form1.directory+"Student.exe", // 获取当前程序的路径
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            Process.Start(startInfo);
        }
    }
   
    
}
