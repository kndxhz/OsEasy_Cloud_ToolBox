using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

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

            // 检查管理员权限
            if (!IsRunningAsAdministrator())
            {
                MessageBox.Show("请以管理员身份运行本程序");
                Application.Exit();
            }
        }

        private static bool IsRunningAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
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
            Task.Run(() =>
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string outputPath = Path.Combine(appDirectory, "ntsd.exe");

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

                Process[] processes = Process.GetProcessesByName("StudentMain");
                string pid = "";
                if (processes.Length > 0)
                {
                    foreach (Process process in processes)
                    {
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show($"进程名: {process.ProcessName}, PID: {process.Id}");
                        }));
                        pid = process.Id.ToString();
                    }
                }
                else
                {
                    this.Invoke(new Action(() => { MessageBox.Show("未找到极域进程"); }));
                    return;
                }

                string command = "ntsd.exe";
                string arguments = $"-c q -p {pid}";

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command} {arguments}",
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                try
                {
                    Process process = Process.Start(processStartInfo);
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() => { MessageBox.Show($"发生错误: {ex.Message}"); }));
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string outputPath = Path.Combine(appDirectory, "sethc.exe");

                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Let_s_happy.Resources.sethc.exe"))
                {
                    if (resourceStream != null)
                    {
                        using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }

                    var processInfo = new ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = "C:\\Windows\\System32",
                        Verb = "runas",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    };

                    try
                    {
                        Process.Start(processInfo);
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() => { MessageBox.Show($"Error: {ex.Message}"); }));
                    }

                    this.Invoke(new Action(() => { MessageBox.Show("已释放，请手动替换"); }));
                }
            });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Let_s_happy.Resources.sethc.exe"))
                {
                    if (resourceStream != null)
                    {
                        using (var fileStream = new FileStream("C:\\system.exe", FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }
                this.Invoke(new Action(() => { MessageBox.Show("已释放至C盘根目录"); }));
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                DialogResult result = MessageBox.Show(
                    "本二级软件由@ZiHaoSaMa66开发\n我只是做了融合\n感谢这位大佬\n点击是将访问这个项目的GitHub主页",
                    "提示",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/ZiHaoSaMa66/OsEasy-ToolBox/") { UseShellExecute = true });
                    return;
                }

                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string outputPath = Path.Combine(appDirectory, "ToolBox 1.7 RC.exe");

                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Let_s_happy.Resources.ToolBox 1.7 RC.exe"))
                {
                    if (resourceStream != null)
                    {
                        using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }

                outputPath = Path.Combine(appDirectory, "ScreenRender_Helper.exe");

                using (var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Let_s_happy.Resources.ScreenRender_Helper.exe"))
                {
                    if (resourceStream != null)
                    {
                        using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }
                    }
                }

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "ToolBox 1.7 RC.exe",
                    UseShellExecute = true,
                    Verb = "runas",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                try
                {
                    Process process = Process.Start(processStartInfo);
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() => { MessageBox.Show($"发生错误: {ex.Message}"); }));
                }
            });
        }
    }
}
