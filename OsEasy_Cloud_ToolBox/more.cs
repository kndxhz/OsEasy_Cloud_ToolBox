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
using System.IO;

namespace OsEasy_Cloud_ToolBox
{
    public partial class more : Form
    {
        public more()
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
            Process.Start(new ProcessStartInfo("https://kndxhz.cn/") { UseShellExecute = true });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = $"{Main.directory1}\\Student.exe", // 获取当前程序的路径
                Arguments = "",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://ipw.cn/") { UseShellExecute = true });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string result = RunCmdCommand("ipconfig /all");
            string tempDir = Path.GetTempPath();
            using (StreamWriter writer = new StreamWriter(tempDir + "ipconfig.txt"))
            {
                writer.WriteLine(result);  // 写入字符串内容
            }
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "notepad.exe",           
                Arguments = $"{tempDir}\\ipconfig.txt",
                
            };
            Process process = new Process { StartInfo = processStartInfo };
            process.Start();
        }
        static string RunCmdCommand(string command)
        {
            // 创建一个新的进程启动 cmd 命令
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",           // 指定启动 cmd
                Arguments = $"/c {command}",    // /c 参数表示执行完命令后关闭 cmd
                RedirectStandardOutput = true,  // 重定向标准输出
                UseShellExecute = false,       // 必须设置为 false，才能重定向输出
                CreateNoWindow = true          // 不显示命令窗口
            };

            Process process = new Process { StartInfo = processStartInfo };

            try
            {
                process.Start();  // 启动进程

                // 获取命令的输出
                string output = process.StandardOutput.ReadToEnd();

                process.WaitForExit();  // 等待命令执行完毕

                return output;
            }
            catch (Exception ex)
            {
                return "执行命令时出错: " + ex.Message;
            }
        }
    }
   
    
}
