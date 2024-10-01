using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Principal;

namespace Let_s_happy
{
    public partial class Form1 : Form
    {
        // 创建全局变量
        string[] one_sentence = { "欢迎来到工业学校！",
            "学习要注意劳逸结合",
            "请确保你已经会了上课的内容再使用本软件",
            "懒惰是人类科技进步的唯一动力",
            "如果你觉得本软件不错，请分享给其他人"};
        int i = 0; // 用于滚动的计数器

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 显示作者信息
            MessageBox.Show("本程序由可耐的小伙纸开发\n邮箱kndxhz@163.com\n感谢广大工业学子使用",
                            "作者信息",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            timer1.Start(); // 启动计时器
            static bool IsRunningAsAdministrator()
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (IsRunningAsAdministrator() == false)
            {
                MessageBox.Show("请以管理员身份运行本程序");
                Application.Exit();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 滚动显示one_sentence的内容
            label1.Text = one_sentence[i]; // 将当前one_sentence中的值显示在label1上

            // 递增i，使其指向下一个句子
            i++;

            // 如果i超出数组长度，则重置为0，实现循环
            if (i >= one_sentence.Length)
            {
                i = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 获取应用程序的运行目录
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string outputPath = Path.Combine(appDirectory, "ntsd.exe");

            // 将资源文件保存为物理文件
            using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Let_s_happy.Resources.ntsd.exe"))
            {
                if (resourceStream != null)
                {
                    using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        resourceStream.CopyTo(fileStream);
                    }
                }


            }
            // 通过进程名获取所有匹配的进程
            Process[] processes = Process.GetProcessesByName("StudentMain");
            string pid = "";
            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    MessageBox.Show($"进程名: {process.ProcessName}, PID: {process.Id}");
                    pid = process.Id.ToString();
                }
            }
            else
            {
                MessageBox.Show("未找到极域进程");
                return;
            }
            // 定义要执行的命令和参数
            string command = "ntsd.exe"; // 替换为要执行的命令
            string arguments = $"-c q -p {pid}"; // 替换为命令的参数

            // 创建一个新的进程
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe", // 使用 cmd.exe 执行命令
                Arguments = $"/c {command} {arguments}", // /c 参数表示在执行完命令后关闭命令窗口
                UseShellExecute = true, // 使用系统外壳程序启动
                Verb = "runas", // 以管理员身份运行
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory // 设置当前工作目录为程序运行目录
            };

            try
            {
                // 启动进程
                Process process = Process.Start(processStartInfo);
                process.WaitForExit(); // 等待命令执行完成
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
