using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace OsEasy_Cloud_ToolBox
{
    public partial class Main : Form
    {
        // 存储一系列的句子
        private string[] sentences_list = new string[]
        {
            "你知道吗：在按钮上右键就可以查看帮助了",
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

        private int current_sentence_index = 0; // 当前显示的句子索引
        private int current_char_index = 0; // 当前句子的字符索引
        private System.Windows.Forms.Timer typing_timer;
        private System.Windows.Forms.Timer process_check_timer;
        private int process_check_in_progress = 0;
        public static bool process_is_suspended = false; // 记录学生端是否被挂起

        public Main()
        {
            InitializeComponent();
        }
        public static string directory_1;
        [STAThread]
        private void main_form_load(object sender, EventArgs e)
        {
            // 这里不再进行自提权，统一由 Program.Main 处理

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            this.label_1.Text = "";
            try
            {
                var process = Process.GetProcessesByName("Student").FirstOrDefault();
                if (process != null && process.MainModule != null)
                {
                    directory_1 = System.IO.Path.GetDirectoryName(process.MainModule.FileName);
                }
                else
                {
                    throw new InvalidOperationException("学生端进程未找到");
                }
            }
            catch (Exception)
            {
                directory_1 = "C:\\Program Files (x86)\\Os-Easy\\multimedia network teaching System";
            }
            // 初始化定时器
            typing_timer = new System.Windows.Forms.Timer();
            typing_timer.Interval = 100; // 设置每次显示字符的间隔（100毫秒）
            typing_timer.Tick += typing_timer_tick;

            // 开始打字效果
            typing_timer.Start();


            process_check_timer = new System.Windows.Forms.Timer();
            process_check_timer.Interval = 1000; // 每1秒检查一次学生端进程
            process_check_timer.Tick += process_check_timer_tick;

            process_check_timer.Start();

        }

        private void process_check_timer_tick(object sender, EventArgs e)
        {
            // 防止上一次检查尚未完成就再次启动
            if (Interlocked.Exchange(ref process_check_in_progress, 1) == 1)
            {
                return;
            }
            // 在后台线程执行进程检查，避免在UI线程上阻塞
            Task.Run(() =>
            {
                string status;
                bool detected_suspended = false;
                try
                {
                    var student_process = Process.GetProcessesByName("Student").FirstOrDefault();
                    if (student_process == null)
                    {
                        status = "学生端状态：学生端未运行";
                        detected_suspended = false;
                    }
                    else
                    {
                        bool is_responding = false;
                        try
                        {
                            is_responding = student_process.Responding;
                        }
                        catch
                        {
                            is_responding = false;
                        }

                        // 检测进程是否被挂起
                        try
                        {
                            student_process.Refresh();
                            detected_suspended = false;
                            foreach (ProcessThread thread in student_process.Threads)
                            {
                                try
                                {
                                    // 检查线程状态是否为 Wait 且挂起原因
                                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                                        thread.WaitReason == System.Diagnostics.ThreadWaitReason.Suspended)
                                    {
                                        detected_suspended = true;
                                        break;   // 只要有一个线程被挂起即可判定
                                    }
                                }
                                catch
                                {
                                    // 忽略无法访问的线程
                                }
                            }
                        }
                        catch
                        {
                            detected_suspended = false;
                        }

                        status = detected_suspended ? "学生端状态：学生端被挂起" : "学生端状态：学生端正在运行";
                    }
                }
                catch
                {
                    status = "学生端状态：检查失败";
                    detected_suspended = false;
                }

                // 将结果回到UI线程显示，并同步 More 窗体的按钮文本
                try
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        this.Text = status;
                        // 更新静态状态，供 More 使用
                        process_is_suspended = detected_suspended;

                        // 如果 More 窗体已打开，更新其按钮文本
                        try
                        {
                            if (form2_instance != null && !form2_instance.IsDisposed)
                            {
                                form2_instance.UpdateSuspendButton(detected_suspended);
                            }
                        }
                        catch { }
                    }));
                }
                catch { }

                Interlocked.Exchange(ref process_check_in_progress, 0);
            });
        }
        
        // 定时器事件：每次触发时，逐个显示字符
        private void typing_timer_tick(object sender, EventArgs e)
        {
            // 判断当前句子是否已显示完
            if (current_char_index < sentences_list[current_sentence_index].Length)
            {
                // 将下一个字符添加到 label1
                this.label_1.Text += sentences_list[current_sentence_index][current_char_index];
                current_char_index++; // 移动到下一个字符
            }
            else
            {
                // 当前句子显示完后，暂停一段时间
                typing_timer.Stop();

                // 设置间隔时间后切换到下一个句子
                System.Windows.Forms.Timer switch_sentence_timer = new System.Windows.Forms.Timer();
                switch_sentence_timer.Interval = 3000; // 切换句子的间隔（3000毫秒）
                switch_sentence_timer.Tick += (s, args) =>
                {
                    // 重新设置定时器，显示下一个句子
                    switch_sentence_timer.Stop();
                    current_char_index = 0; // 重置字符索引
                    current_sentence_index++; // 切换到下一个句子
                    if (current_sentence_index >= sentences_list.Length)
                    {
                        current_sentence_index = 0; // 如果到了最后一条，重新从头开始
                    }

                    // 清空 label 并开始新的打字效果
                    this.label_1.Text = "";
                    typing_timer.Start();
                };
                switch_sentence_timer.Start(); // 启动切换句子的定时器
            }
        }

        private void button1_click(object sender, EventArgs e)
        {
            // 获取当前应用程序的临时目录路径
            string temp_dir = Path.GetTempPath();

            // 设定文件路径
            string file_path = Path.Combine(temp_dir, "killer.bat");

            // 将 Resources 中的 "killer" 文件写入到临时目录
            File.WriteAllBytes(file_path, Encoding.Default.GetBytes(Properties.Resources.killer));

            // 以管理员权限运行该文件
            run_as_admin(file_path);
        }

        public static void run_as_admin(string path)
        {
            // 检查文件是否存在
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("指定的文件不存在", path);
            }

            // 设置 ProcessStartInfo 来以管理员权限运行
            ProcessStartInfo start_info = new ProcessStartInfo
            {
                FileName = path,        // 要执行的文件路径
                Verb = "runas",         // 以管理员权限运行
                UseShellExecute = true  // 使用外部 shell 启动
            };

            // 启动进程
            Process.Start(start_info);
        }

        private string force_get_teacher_ip()
        {
            string host_name = Dns.GetHostName();

            // 获取本地主机的 IP 地址信息
            IPHostEntry host_entry = Dns.GetHostEntry(host_name);

            // 从地址列表中找到第一个以 192.168 开头的 IPv4 地址
            IPAddress local_ip = host_entry.AddressList.FirstOrDefault(ip =>
                ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                ip.ToString().StartsWith("192.168"));

            if (local_ip != null)
            {
                // 获取 IP 地址的字节数组（IPv4 为 4 个字节）
                byte[] ip_bytes = local_ip.GetAddressBytes();
                if (ip_bytes.Length == 4)
                {
                    // 将主机部分（第四个字节）设置为 250
                    ip_bytes[3] = 250;

                    // 使用新的字节数组构造目标 IP 地址
                    IPAddress target_ip = new IPAddress(ip_bytes);

                    // 输出结果
                    return target_ip.ToString();
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

        private void button2_click(object sender, EventArgs e)
        {
            string get_ip_way = "";
            DialogResult chose_unlock_net = MessageBox.Show(
    "请选择你要解禁的方式\n是：软解禁（推荐）\n否：硬解禁（不推荐）",
    "选择方式",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

            if (chose_unlock_net == DialogResult.Yes)
            {
                string file_path = $"{directory_1}\\vdi_channel.log";
                string last_teacher_ip = null;
                // 调整正则表达式更严格的IP格式验证
                Regex ip_regex = new Regex(@"teacher_ip:((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))", RegexOptions.Compiled);

                try
                {
                    foreach (string line in File.ReadLines(file_path))
                    {
                        MatchCollection matches = ip_regex.Matches(line);
                        if (matches.Count > 0)
                        {
                            // 修改为兼容C# 7.3的写法
                            last_teacher_ip = matches[matches.Count - 1].Groups[1].Value;
                        }
                    }
                    get_ip_way = "读取日志匹配正则";

                }
                catch (FileNotFoundException)
                {
                    last_teacher_ip = force_get_teacher_ip();
                    get_ip_way = "直接读取ip";
                }
                catch (Exception)
                {
                    last_teacher_ip = force_get_teacher_ip();
                    get_ip_way = "直接读取ip";
                }

                if (last_teacher_ip == null)
                {
                    MessageBox.Show("ip两种方式都获取失败，请手动输入教师机ip！！！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    string input = Microsoft.VisualBasic.Interaction.InputBox("请输入教师机IP地址:", "输入IP地址", "", -1, -1);
                    if (!string.IsNullOrEmpty(input))
                    {
                        last_teacher_ip = input;
                    }
                }
                else
                {
                    //MessageBox.show
                    DialogResult confirm_ip = MessageBox.Show(
            $"获取到的教师机ip：{last_teacher_ip}\n读取方式：{get_ip_way}\n是否正确？",  // 消息内容
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
                            last_teacher_ip = input;
                        }
                    }
                }
                ProcessStartInfo start_info = new ProcessStartInfo
                {
                    FileName = $"{directory_1}\\devicecontrol_x64\\DeviceControl_x64.exe", // 获取当前程序的路径
                    WorkingDirectory = $"{directory_1}\\devicecontrol_x64",
                    Arguments = $"--type net --operation 0 --extend {last_teacher_ip}",
                    Verb = "runas",                         // 以管理员权限运行
                    UseShellExecute = true                  // 使用外部 shell 启动
                };
                try
                {
                    Process.Start(start_info);
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
                    string temp_dir = Path.GetTempPath();

                    // 设定文件路径
                    string file_path = Path.Combine(temp_dir, "task.bat");

                    // 将 Resources 中的 "task" 文件写入到临时目录
                    File.WriteAllBytes(file_path, Encoding.Default.GetBytes(Properties.Resources.task));

                    // 以管理员权限运行该文件
                    run_as_admin(file_path);
                }
            }
        }


        private void button3_click(object sender, EventArgs e)
        {
            ProcessStartInfo start_info = new ProcessStartInfo
            {
                FileName = $"{directory_1}\\devicecontrol_x64\\DeviceControl_x64.exe", // 获取当前程序的路径
                WorkingDirectory = $"{directory_1}\\devicecontrol_x64",
                Arguments = $"--type usb --operation 0 --extend 0.0.0.0",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process.Start(start_info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
            MessageBox.Show("执行成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void picture_box1_click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/kndxhz/OsEasy_Cloud_ToolBox") { UseShellExecute = true });
        }


        private More form2_instance = null;
        private void button4_click(object sender, EventArgs e)
        {


            // 检查实例是否存在且未被释放
            if (form2_instance == null || form2_instance.IsDisposed)
            {
                form2_instance = new More();
                // 窗体关闭时置空实例
                form2_instance.FormClosed += (s, args) => form2_instance = null;
                form2_instance.Show();
            }
            else
            {
                // 恢复最小化的窗体
                if (form2_instance.WindowState == FormWindowState.Minimized)
                    form2_instance.WindowState = FormWindowState.Normal;

                // 激活并前置窗体
                form2_instance.BringToFront();
                form2_instance.Activate();
            }
        }

        private void label_1_click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://kndxhz.cn/") { UseShellExecute = true });
        }
    }
}
