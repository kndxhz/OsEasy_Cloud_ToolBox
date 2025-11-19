using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace OsEasy_Cloud_ToolBox
{
    public partial class Main : Form
    {
        // 存储一系列的句子
        private string[] sentences = new string[]
        {
            "《机课时间管理》",
            "开源造福人类",
            "锟斤拷烫烫烫",
            "Hello World!",
            "世界...遗忘我...",
            "Python(屁眼通红)",
            "C#(C++ *2)",
            "你要来杯Jvav吗",
            "人生苦短，我用C#",
            "第一项=0",
            "互联网大厂都是草台班子"
        };

        private int currentSentenceIndex = 0; // 当前显示的句子索引
        private int currentCharIndex = 0; // 当前句子的字符索引
        private Timer typingTimer;

        public Main()
        {
            InitializeComponent();
        }
        public static string directory1;
        [STAThread]
        private void Form1_Load(object sender, EventArgs e)
        {
            // 这里不再进行自提权，统一由 Program.Main 处理

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            this.label1.Text = "";
            try
            {
                var process = Process.GetProcessesByName("Student").FirstOrDefault();
                if (process != null && process.MainModule != null)
                {
                    directory1 = System.IO.Path.GetDirectoryName(process.MainModule.FileName);
                }
                else
                {
                    throw new InvalidOperationException("Student 进程未找到");
                }
            }
            catch (Exception)
            {
                directory1 = "C:\\Program Files (x86)\\Os-Easy\\multimedia network teaching System";
            }
            // 初始化定时器
            typingTimer = new Timer();
            typingTimer.Interval = 100; // 设置每次显示字符的间隔（100毫秒）
            typingTimer.Tick += TypingTimer_Tick;

            // 开始打字效果
            typingTimer.Start();
        }

        // 定时器事件：每次触发时，逐个显示字符
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // 判断当前句子是否已显示完
            if (currentCharIndex < sentences[currentSentenceIndex].Length)
            {
                // 将下一个字符添加到 label1
                this.label1.Text += sentences[currentSentenceIndex][currentCharIndex];
                currentCharIndex++; // 移动到下一个字符
            }
            else
            {
                // 当前句子显示完后，暂停一段时间
                typingTimer.Stop();

                // 设置间隔时间后切换到下一个句子
                Timer switchSentenceTimer = new Timer();
                switchSentenceTimer.Interval = 3000; // 切换句子的间隔（3000毫秒）
                switchSentenceTimer.Tick += (s, args) =>
                {
                    // 重新设置定时器，显示下一个句子
                    switchSentenceTimer.Stop();
                    currentCharIndex = 0; // 重置字符索引
                    currentSentenceIndex++; // 切换到下一个句子
                    if (currentSentenceIndex >= sentences.Length)
                    {
                        currentSentenceIndex = 0; // 如果到了最后一条，重新从头开始
                    }

                    // 清空 label 并开始新的打字效果
                    this.label1.Text = "";
                    typingTimer.Start();
                };
                switchSentenceTimer.Start(); // 启动切换句子的定时器
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 获取当前应用程序的临时目录路径
            string tempDir = Path.GetTempPath();

            // 设定文件路径
            string filePath = Path.Combine(tempDir, "killer.bat");

            // 将 Resources 中的 "killer" 文件写入到临时目录
            File.WriteAllBytes(filePath, Encoding.Default.GetBytes(Properties.Resources.killer));

            // 以管理员权限运行该文件
            RunAsAdmin(filePath);
        }

        public static void RunAsAdmin(string path)
        {
            // 检查文件是否存在
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("指定的文件不存在", path);
            }

            // 设置 ProcessStartInfo 来以管理员权限运行
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = path,        // 要执行的文件路径
                Verb = "runas",         // 以管理员权限运行
                UseShellExecute = true  // 使用外部 shell 启动
            };

            // 启动进程
            Process.Start(startInfo);
        }

        private string force_get_teacher_ip()
        {
            string hostName = Dns.GetHostName();

            // 获取本地主机的 IP 地址信息
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            // 从地址列表中找到第一个以 192.168 开头的 IPv4 地址
            IPAddress localIp = hostEntry.AddressList.FirstOrDefault(ip =>
                ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                ip.ToString().StartsWith("192.168"));

            if (localIp != null)
            {
                // 获取 IP 地址的字节数组（IPv4 为 4 个字节）
                byte[] ipBytes = localIp.GetAddressBytes();
                if (ipBytes.Length == 4)
                {
                    // 将主机部分（第四个字节）设置为 250
                    ipBytes[3] = 250;

                    // 使用新的字节数组构造目标 IP 地址
                    IPAddress targetIp = new IPAddress(ipBytes);

                    // 输出结果
                    return targetIp.ToString();
                }
                else
                {
                    MessageBox.Show("本地 IP 不是 IPv4 地址。");
                    return "";
                }
            }
            else
            {
                MessageBox.Show("未找到 192.168 开头的 IPv4 地址。");
                return "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string get_ip_way = "";
            DialogResult chose_unlock_net = MessageBox.Show(
    "请选择你要解禁的方式\n是：软解禁（推荐）\n否：硬解禁（不推荐）",
    "选择方式",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

            if (chose_unlock_net == DialogResult.Yes)
            {
                string filePath = $"{directory1}\\vdi_channel.log";
                string lastTeacherIp = null;
                // 调整正则表达式更严格的IP格式验证
                Regex ipRegex = new Regex(@"teacher_ip:((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))", RegexOptions.Compiled);

                try
                {
                    foreach (string line in File.ReadLines(filePath))
                    {
                        MatchCollection matches = ipRegex.Matches(line);
                        if (matches.Count > 0)
                        {
                            // 修改为兼容C# 7.3的写法
                            lastTeacherIp = matches[matches.Count - 1].Groups[1].Value;
                        }
                    }
                    get_ip_way = "读取日志匹配正则";

                }
                catch (FileNotFoundException)
                {
                    lastTeacherIp = force_get_teacher_ip();
                    get_ip_way = "直接读取ip";
                }
                catch (Exception)
                {
                    lastTeacherIp = force_get_teacher_ip();
                    get_ip_way = "直接读取ip";
                }

                if (lastTeacherIp == null)
                {
                    MessageBox.Show("ip两种方式都获取失败，请手动输入教师机ip！！！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    string input = Microsoft.VisualBasic.Interaction.InputBox("请输入教师机IP地址:", "输入IP地址", "", -1, -1);
                    if (!string.IsNullOrEmpty(input))
                    {
                        lastTeacherIp = input;
                    }
                }
                else
                {
                    //MessageBox.show
                    DialogResult confirm_ip = MessageBox.Show(
            $"获取到的教师机ip：{lastTeacherIp}\n读取方式：{get_ip_way}\n是否正确？",  // 消息内容
            "IP",                          // 标题
            MessageBoxButtons.YesNo,          // 显示"是"和"否"按钮
            MessageBoxIcon.Question);          // 警告图标
                    if (confirm_ip == DialogResult.Yes)
                    {
                        // do nothing
                    }
                    else
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox("请输入教师机IP地址:", "输入IP地址", "", -1, -1);
                        if (!string.IsNullOrEmpty(input))
                        {
                            lastTeacherIp = input;
                        }
                    }
                }
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = $"{directory1}\\devicecontrol_x64\\DeviceControl_x64.exe", // 获取当前程序的路径
                    WorkingDirectory = $"{directory1}\\devicecontrol_x64",
                    Arguments = $"--type net --operation 0 --extend {lastTeacherIp}",
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
                MessageBox.Show("执行成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (chose_unlock_net == DialogResult.No)
            {
                DialogResult confirm_status = MessageBox.Show(
            "此操作将会重启系统\n你确定吗？",  // 消息内容
            "警告",                          // 标题
            MessageBoxButtons.YesNo,          // 显示"是"和"否"按钮
            MessageBoxIcon.Warning);          // 警告图标

                // 根据用户选择处理
                if (confirm_status == DialogResult.Yes)
                {
                    // 获取当前应用程序的临时目录路径
                    string tempDir = Path.GetTempPath();

                    // 设定文件路径
                    string filePath = Path.Combine(tempDir, "task.bat");

                    // 将 Resources 中的 "task" 文件写入到临时目录
                    File.WriteAllBytes(filePath, Encoding.Default.GetBytes(Properties.Resources.task));

                    // 以管理员权限运行该文件
                    RunAsAdmin(filePath);
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = $"{directory1}\\devicecontrol_x64\\DeviceControl_x64.exe", // 获取当前程序的路径
                WorkingDirectory = $"{directory1}\\devicecontrol_x64",
                Arguments = $"--type usb --operation 0 --extend 0.0.0.0",
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
            MessageBox.Show("执行成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/kndxhz/OsEasy_Cloud_ToolBox") { UseShellExecute = true });
        }


        private More form2Instance = null;
        private void button4_Click(object sender, EventArgs e)
        {


            // 检查实例是否存在且未被释放
            if (form2Instance == null || form2Instance.IsDisposed)
            {
                form2Instance = new More();
                // 窗体关闭时置空实例
                form2Instance.FormClosed += (s, args) => form2Instance = null;
                form2Instance.Show();
            }
            else
            {
                // 恢复最小化的窗体
                if (form2Instance.WindowState == FormWindowState.Minimized)
                    form2Instance.WindowState = FormWindowState.Normal;

                // 激活并前置窗体
                form2Instance.BringToFront();
                form2Instance.Activate();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://kndxhz.cn/") { UseShellExecute = true });
        }
    }
}
