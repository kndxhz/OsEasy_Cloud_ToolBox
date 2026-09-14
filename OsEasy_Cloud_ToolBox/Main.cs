using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace OsEasy_Cloud_ToolBox
{
    public partial class Main : Form
    {
        // 存储一系列的句子
        private string[] typing_sentences = new string[]
        {
            "你知道吗：在按钮上右键可以查看帮助",
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

        private int sentence_index = 0; // 当前显示的句子索引
        private int char_index = 0; // 当前句子的字符索引
        private System.Windows.Forms.Timer typing_timer;
        private System.Windows.Forms.Timer process_check_timer;
        private int process_check_running = 0;
        public static bool student_process_suspended = false; // 记录学生端是否被挂起

        public static bool toolbox_is_hidden = true; // 记录工具箱是否被隐藏

        // SetWindowDisplayAffinity 相关常量
        public const uint WDA_NONE = 0x00000000;
        public const uint WDA_EXCLUDEFROMCAPTURE = 0x00000011;

        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint dwAffinity);

        // 关学生端需要结束的进程列表
        private string[] student_process_names = new string[]
        {
            "Ctsc_Multi.exe", "DeviceControl_x64.exe", "HRMon.exe", "MultiClient.exe",
            "OActiveII-Client.exe", "OEClient.exe", "OELogSystem.exe", "OEUpdate.exe",
            "OEProtect.exe", "ProcessProtect.exe", "RunClient.exe", "ServerOSS.exe",
            "Student.exe", "wfilesvr.exe", "tvnserver.exe", "updatefilesvr.exe",
            "ScreenRender.exe"
        };

        public Main()
        {
            InitializeComponent();
        }
        public static string student_install_dir;

        private void button_mouse_down(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btn = sender as Button;
                if (btn != null)
                {
                    show_help.ShowHelp(btn.Name, false);
                }
            }
        }
        [STAThread]
        private void main_form_load(object sender, EventArgs e)
        {
            // 这里不再进行自提权，统一由 Program.Main 处理

            Logger.Info("主窗口加载");
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            this.label_1.Text = "";
            try
            {
                var process = Process.GetProcessesByName("Student").FirstOrDefault();
                if (process != null && process.MainModule != null)
                {
                    student_install_dir = System.IO.Path.GetDirectoryName(process.MainModule.FileName);
                }
                else
                {
                    throw new InvalidOperationException("学生端进程未找到");
                }
            }
            catch (Exception ex)
            {
                student_install_dir = "C:\\Program Files (x86)\\Os-Easy\\multimedia network teaching System";
                Logger.Warn("未找到学生端进程，使用默认目录（" + ex.Message + "）");
            }
            Logger.Info("工作目录 student_install_dir = " + student_install_dir);

            // 为按钮添加右键帮助事件
            this.button_1.MouseDown += button_mouse_down;
            this.button_2.MouseDown += button_mouse_down;
            this.button_3.MouseDown += button_mouse_down;
            this.button_4.MouseDown += button_mouse_down;

            // 关闭窗口时记录日志
            this.FormClosing += main_form_closing;

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

            hide_toolbox();

        }

        private void main_form_closing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("主窗口关闭（原因: " + e.CloseReason + "），程序即将退出");
        }

        // 将显示关联应用到本程序的所有窗口（含“更多工具”窗口）
        public static void set_all_windows_display_affinity(uint affinity)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form == null || !form.IsHandleCreated)
                {
                    continue;
                }
                try
                {
                    if (!SetWindowDisplayAffinity(form.Handle, affinity))
                    {
                        Logger.Warn("设置窗口显示关联失败: " + form.GetType().Name + " affinity=0x" + affinity.ToString("X8"));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("设置窗口显示关联异常: " + form.GetType().Name + " - " + ex.Message);
                }
            }
        }

        public void hide_toolbox()
        {
            this.Hide();
            toolbox_is_hidden = true;
            set_all_windows_display_affinity(WDA_EXCLUDEFROMCAPTURE);
            if (more_form_instance != null && !more_form_instance.IsDisposed)
            {
                more_form_instance.UpdateHideButton();
            }
            Logger.Info("隐藏工具箱（排除屏幕捕获）");
        }

        public void show_toolbox()
        {
            this.Show();
            toolbox_is_hidden = false;
            set_all_windows_display_affinity(WDA_NONE);
            if (more_form_instance != null && !more_form_instance.IsDisposed)
            {
                more_form_instance.UpdateHideButton();
            }
            Logger.Info("显示工具箱（恢复屏幕捕获）");
        }

        private void process_check_timer_tick(object sender, EventArgs e)
        {
            // 防止上一次检查尚未完成就再次启动
            if (Interlocked.Exchange(ref process_check_running, 1) == 1)
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
                        if (student_process_suspended != detected_suspended)
                        {
                            Logger.Info("学生端挂起状态变化: " + detected_suspended);
                        }
                        student_process_suspended = detected_suspended;

                        // 如果 More 窗体已打开，更新其按钮文本
                        try
                        {
                            if (more_form_instance != null && !more_form_instance.IsDisposed)
                            {
                                more_form_instance.UpdateSuspendButton(detected_suspended);
                            }
                        }
                        catch { }
                    }));
                }
                catch { }

                Interlocked.Exchange(ref process_check_running, 0);
            });
        }
        
        // 定时器事件：每次触发时，逐个显示字符
        private void typing_timer_tick(object sender, EventArgs e)
        {
            // 判断当前句子是否已显示完
            if (char_index < typing_sentences[sentence_index].Length)
            {
                // 将下一个字符添加到 label1
                this.label_1.Text += typing_sentences[sentence_index][char_index];
                char_index++; // 移动到下一个字符
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
                    char_index = 0; // 重置字符索引
                    sentence_index++; // 切换到下一个句子
                    if (sentence_index >= typing_sentences.Length)
                    {
                        sentence_index = 0; // 如果到了最后一条，重新从头开始
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
            this.button_1.Enabled = false;

            // 弹出进度条，异步循环三次结束学生端相关进程
            ProgressForm progress_form = new ProgressForm("正在关闭学生端", "正在准备...");
            progress_form.Show(this);
            set_all_windows_display_affinity(
                toolbox_is_hidden ? WDA_EXCLUDEFROMCAPTURE : WDA_NONE);

            Logger.Info("开始关闭学生端（循环 3 次结束进程）");
            Task.Run(() =>
            {
                const int kill_rounds = 3;
                int total = student_process_names.Length * kill_rounds;
                int completed = 0;
                string error_message = null;

                try
                {
                    for (int round = 0; round < kill_rounds; round++)
                    {
                        foreach (string process_name in student_process_names)
                        {
                            try
                            {
                                string kill_output;
                                run_and_wait(
                                    Path.Combine(Environment.SystemDirectory, "taskkill.exe"),
                                    "/f /IM " + process_name,
                                    null,
                                    out kill_output);
                            }
                            catch (Exception ex)
                            {
                                // 单个进程结束失败不影响整体进度
                                Logger.Warn("结束进程 " + process_name + " 失败: " + ex.Message);
                            }

                            completed++;
                            int percent = completed * 100 / total;
                            progress_form.SetProgress(percent, $"正在关闭：{process_name}（{completed}/{total}）");
                        }
                    }
                }
                catch (Exception ex)
                {
                    error_message = ex.Message;
                    Logger.Error("关闭学生端失败", ex);
                }

                this.BeginInvoke(new Action(() =>
                {
                    progress_form.SetProgress(100, "完成");
                    progress_form.Close();
                    progress_form.Dispose();
                    this.button_1.Enabled = true;

                    if (error_message == null)
                    {
                        Logger.Info("学生端相关进程已关闭");
                        MessageBox.Show(this, "学生端相关进程已关闭", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Logger.Error("关闭学生端失败: " + error_message);
                        MessageBox.Show(this, "关闭学生端失败：\n" + error_message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }));
            });
        }

        // 运行程序并等待结束，返回退出码（0 表示成功），同时输出 stdout/stderr
        private int run_and_wait(string file_name, string arguments, string working_directory, out string output)
        {
            ProcessStartInfo process_start_info = new ProcessStartInfo
            {
                FileName = file_name,
                Arguments = arguments,
                WorkingDirectory = working_directory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            output = "";
            StringBuilder output_builder = new StringBuilder();
            Logger.Info("启动外部程序: " + file_name + " " + arguments + "（工作目录: " + working_directory + "）");
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = process_start_info;
                    process.OutputDataReceived += (s, args) =>
                    {
                        if (args.Data != null) output_builder.AppendLine(args.Data);
                    };
                    process.ErrorDataReceived += (s, args) =>
                    {
                        if (args.Data != null) output_builder.AppendLine(args.Data);
                    };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    output = output_builder.ToString().Trim();
                    int exit_code = process.ExitCode;
                    Logger.LogProcessResult(file_name, arguments, exit_code, output);
                    return exit_code;
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
                Logger.Error("外部程序调用失败: " + file_name + " " + arguments, ex);
                throw;
            }
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
            string teacher_ip_source = "";
            DialogResult unlock_net_choice = MessageBox.Show(
    "请选择你要解禁的方式\n是：软解禁（推荐）\n否：硬解禁（不推荐）",
    "选择方式",
    MessageBoxButtons.YesNo,
    MessageBoxIcon.Question);

            Logger.Info("解禁网络: 选择=" + (unlock_net_choice == DialogResult.Yes ? "软解禁" : "硬解禁"));

            if (unlock_net_choice == DialogResult.Yes)
            {
                string file_path = $"{student_install_dir}\\vdi_channel.log";
                string teacher_ip = null;
                // 调整正则表达式更严格的IP格式验证
                Regex teacher_ip_regex = new Regex(@"teacher_ip:((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?))", RegexOptions.Compiled);

                try
                {
                    foreach (string line in File.ReadLines(file_path))
                    {
                        MatchCollection matches = teacher_ip_regex.Matches(line);
                        if (matches.Count > 0)
                        {
                            // 修改为兼容C# 7.3的写法
                            teacher_ip = matches[matches.Count - 1].Groups[1].Value;
                        }
                    }
                    teacher_ip_source = "读取日志匹配正则";

                }
                catch (FileNotFoundException)
                {
                    teacher_ip = force_get_teacher_ip();
                    teacher_ip_source = "直接读取ip";
                }
                catch (Exception)
                {
                    teacher_ip = force_get_teacher_ip();
                    teacher_ip_source = "直接读取ip";
                }

                if (teacher_ip == null)
                {
                    MessageBox.Show("ip两种方式都获取失败，请手动输入教师机ip！！！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    string manual_ip_input = Microsoft.VisualBasic.Interaction.InputBox("请输入教师机IP地址:", "输入IP地址", "", -1, -1);
                    if (!string.IsNullOrEmpty(manual_ip_input))
                    {
                        teacher_ip = manual_ip_input;
                    }
                }
                else
                {
                    //MessageBox.show
                    DialogResult ip_confirm_result = MessageBox.Show(
            $"获取到的教师机ip：{teacher_ip}\n读取方式：{teacher_ip_source}\n是否正确？",  // 消息内容
            "IP",                          // 标题
            MessageBoxButtons.YesNo,          // 显示"是"和"否"按钮
            MessageBoxIcon.Question);          // 警告图标
                    if (ip_confirm_result == DialogResult.Yes)
                    {
                        // do nothing
                    }
                    else
                    {
                        string manual_ip_input = Microsoft.VisualBasic.Interaction.InputBox("请输入教师机IP地址:", "输入IP地址", "", -1, -1);
                        if (!string.IsNullOrEmpty(manual_ip_input))
                        {
                            teacher_ip = manual_ip_input;
                        }
                    }
                }
                Logger.Info("解禁网络: 教师机IP=" + teacher_ip + "，获取方式=" + teacher_ip_source);
                if (string.IsNullOrEmpty(teacher_ip))
                {
                    Logger.Warn("解禁网络: 未获取到教师机IP，操作已取消");
                    MessageBox.Show("未获取到教师机IP，操作已取消。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string device_control_path = $"{student_install_dir}\\devicecontrol_x64\\DeviceControl_x64.exe";
                string device_control_dir = $"{student_install_dir}\\devicecontrol_x64";
                try
                {
                    string output;
                    int exit_code = run_and_wait(
                        device_control_path,
                        $"--type net --operation 0 --extend {teacher_ip}",
                        device_control_dir,
                        out output);

                    if (exit_code == 0)
                    {
                        Logger.Info("解禁网络成功");
                        MessageBox.Show("执行成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Logger.Warn("解禁网络失败，返回码=" + exit_code);
                        MessageBox.Show(
                            $"解禁网络失败\n返回码：{exit_code}\n程序输出：\n{output}",
                            "错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("解禁网络失败", ex);
                    MessageBox.Show("解禁网络失败：\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (unlock_net_choice == DialogResult.No)
            {
                DialogResult reboot_confirm_result = MessageBox.Show(
            "此操作将会重启系统\n你确定吗？",  // 消息内容
            "警告",                          // 标题
            MessageBoxButtons.YesNo,          // 显示"是"和"否"按钮
            MessageBoxIcon.Warning);          // 警告图标

                // 根据用户选择处理
                if (reboot_confirm_result == DialogResult.Yes)
                {
                    // 直接调用“关学生端”按钮的逻辑结束相关进程
                    Logger.Info("硬解禁: 直接调用关学生端逻辑");
                    button1_click(sender, e);

                    // 重启系统
                    try
                    {
                        Process.Start(new ProcessStartInfo(
                            Path.Combine(Environment.SystemDirectory, "shutdown.exe"),
                            "/r /f /t 0")
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true
                        });
                        Logger.Info("硬解禁: 已执行 shutdown /r /f /t 0");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("硬解禁: 重启系统失败", ex);
                        MessageBox.Show("重启系统失败：\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }


        private void button3_click(object sender, EventArgs e)
        {
            Logger.Info("解禁U盘: 开始");
            string device_control_path = $"{student_install_dir}\\devicecontrol_x64\\DeviceControl_x64.exe";
            string device_control_dir = $"{student_install_dir}\\devicecontrol_x64";
            try
            {
                string output;
                int exit_code = run_and_wait(
                    device_control_path,
                    "--type usb --operation 0 --extend 0.0.0.0",
                    device_control_dir,
                    out output);

                if (exit_code == 0)
                {
                    Logger.Info("解禁U盘成功");
                    MessageBox.Show("执行成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Logger.Warn("解禁U盘失败，返回码=" + exit_code);
                    MessageBox.Show(
                        $"解禁U盘失败\n返回码：{exit_code}\n程序输出：\n{output}",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("解禁U盘失败", ex);
                MessageBox.Show("解禁U盘失败：\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void picture_box1_click(object sender, EventArgs e)
        {
            Logger.Info("打开网址: https://github.com/kndxhz/OsEasy_Cloud_ToolBox");
            Process.Start(new ProcessStartInfo("https://github.com/kndxhz/OsEasy_Cloud_ToolBox") { UseShellExecute = true });
        }


        private More more_form_instance = null;
        private void button4_click(object sender, EventArgs e)
        {


            // 检查实例是否存在且未被释放
            if (more_form_instance == null || more_form_instance.IsDisposed)
            {
                Logger.Info("打开更多工具窗口");
                more_form_instance = new More();
                // 窗体关闭时置空实例
                more_form_instance.FormClosed += (s, args) => more_form_instance = null;
                more_form_instance.Show();
            }
            else
            {
                // 恢复最小化的窗体
                if (more_form_instance.WindowState == FormWindowState.Minimized)
                    more_form_instance.WindowState = FormWindowState.Normal;

                // 激活并前置窗体
                more_form_instance.BringToFront();
                more_form_instance.Activate();
                Logger.Info("激活更多工具窗口");
            }
        }

        private void label_1_click(object sender, EventArgs e)
        {
            Logger.Info("打开网址: https://www.kndxhz.cn/");
            Process.Start(new ProcessStartInfo("https://www.kndxhz.cn/") { UseShellExecute = true });
        }

        // 简单的弹出式进度条窗口，用于异步任务显示进度
        private class ProgressForm : Form
        {
            private ProgressBar progress_bar;
            private Label progress_label;

            public ProgressForm(string title, string text)
            {
                this.Text = title;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.StartPosition = FormStartPosition.CenterParent;
                this.MinimizeBox = false;
                this.MaximizeBox = false;
                this.ControlBox = false;
                this.ShowInTaskbar = false;
                this.ClientSize = new System.Drawing.Size(360, 100);

                this.progress_label = new Label
                {
                    Text = text,
                    Location = new System.Drawing.Point(12, 15),
                    AutoSize = true
                };
                this.progress_bar = new ProgressBar
                {
                    Location = new System.Drawing.Point(12, 45),
                    Size = new System.Drawing.Size(336, 23),
                    Minimum = 0,
                    Maximum = 100
                };

                this.Controls.Add(this.progress_label);
                this.Controls.Add(this.progress_bar);
            }

            public void SetProgress(int value, string text)
            {
                if (this.IsDisposed) return;
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => SetProgress(value, text)));
                    return;
                }

                if (value < this.progress_bar.Minimum) value = this.progress_bar.Minimum;
                if (value > this.progress_bar.Maximum) value = this.progress_bar.Maximum;
                this.progress_bar.Value = value;
                if (text != null) this.progress_label.Text = text;
            }
        }
    }
}
