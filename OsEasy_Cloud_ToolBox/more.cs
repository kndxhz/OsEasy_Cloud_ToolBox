using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    public partial class More : Form
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        private static extern uint SuspendThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        private static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);


        private const int thread_suspend_resume = 0x0002;

        //private bool is_suspended = false; // 记录当前是否已挂起
        bool is_suspended = Main.process_is_suspended;
        public More()
        {
            InitializeComponent();
        }

        private void button_mouse_down(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Button btn = sender as Button;
                if (btn != null)
                {
                    show_help.ShowHelp(btn.Name, true);
                }
            }
        }

        private void more_form_load(object sender, EventArgs e)
        {
            Logger.Info("更多工具窗口加载，is_suspended=" + is_suspended);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            
            // 为按钮添加右键帮助事件
            this.button_1.MouseDown += button_mouse_down;
            this.button_2.MouseDown += button_mouse_down;
            this.button_3.MouseDown += button_mouse_down;
            this.button_4.MouseDown += button_mouse_down;

            // 关闭窗口时记录日志
            this.FormClosing += more_form_closing;

            if (is_suspended)
            {
                this.button_1.Text = "恢复学生端";
            }
            else
            {
                this.button_1.Text = "挂起学生端";
            }

            // 同步工具箱隐藏状态，并应用到本窗口
            this.UpdateHideButton();
            Main.set_all_windows_display_affinity(
                Main.toolbox_is_hide ? Main.WDA_EXCLUDEFROMCAPTURE : Main.WDA_NONE);
        }

        private void more_form_closing(object sender, FormClosingEventArgs e)
        {
            Logger.Info("更多工具窗口关闭（原因: " + e.CloseReason + "）");
        }

        // 允许外部线程安全地同步“显示/隐藏”按钮文本
        public void UpdateHideButton()
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateHideButton()));
                return;
            }

            this.button_4.Text = Main.toolbox_is_hide ? "显示本程序" : "隐藏本程序";
        }

        // 允许外部线程安全地更新按钮文本
        public void UpdateSuspendButton(bool suspended)
        {
            if (this.IsDisposed) return;
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateSuspendButton(suspended)));
                return;
            }

            is_suspended = suspended;
            this.button_1.Text = suspended ? "恢复学生端" : "挂起学生端";
        }

        private void button_1_click(object sender, EventArgs e)
        {
            // 点击后先显示警告信息框，确认后才继续
            DialogResult confirm_suspend = MessageBox.Show(
                "点击后程序会隐藏5秒\n然后恢复\n此时教师端看你不是下线\n而是一直卡在隐藏的那个界面\n可以有效规避点名等功能\n此外如果教师端发了文件/发了消息/发起点名\n你也可以再次点击以正常运行\n\n基于Windows API接口实现\n\n是否继续？",
                "警告",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm_suspend != DialogResult.Yes)
            {
                Logger.Info("挂起/恢复学生端: 用户取消");
                return;
            }

            Logger.Info("挂起/恢复学生端: 开始，当前 is_suspended=" + is_suspended);

            // 禁用按钮，防止重复点击
            this.button_1.Enabled = false;

            // 在后台线程上执行挂起/恢复操作，避免阻塞UI
            Thread backgroundThread = new Thread(() =>
            {
                try
                {
                    // 获取目标进程（例如 Student.exe）
                    var processes = Process.GetProcessesByName("Student");
                    if (processes.Length == 0)
                    {
                        Logger.Warn("挂起/恢复学生端: 未找到 Student 进程");
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("未找到学生端进程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }));
                        return;
                    }
                    Logger.Info("挂起/恢复学生端: 找到 " + processes.Length + " 个 Student 进程，PID=" + processes[0].Id);

                    // 根据当前状态决定挂起或恢复
                    if (!is_suspended)
                    {
                        // 隐藏所有窗口
                        this.Invoke(new Action(() =>
                        {
                            foreach (Form form in Application.OpenForms)
                            {
                                form.Hide();
                            }
                        }));

                        Logger.Info("挂起学生端: 已隐藏所有窗口，等待 5 秒");
                        Thread.Sleep(5000);

                        // 挂起所有线程
                        int suspended_count = 0;
                        foreach (ProcessThread thread in processes[0].Threads)
                        {
                            IntPtr pOpenThread = OpenThread(thread_suspend_resume, false, (uint)thread.Id);
                            if (pOpenThread == IntPtr.Zero) continue;

                            SuspendThread(pOpenThread);
                            suspended_count++;
                            CloseHandle(pOpenThread);
                        }

                        Logger.Info("挂起学生端: 已挂起 " + suspended_count + " 个线程");
                        is_suspended = true; // 更新状态
                        this.Invoke(new Action(() =>
                        {
                            this.button_1.Text = "恢复学生端";
                        }));
                    }
                    else
                    {
                        // 恢复所有线程
                        int resumed_count = 0;
                        foreach (ProcessThread thread in processes[0].Threads)
                        {
                            IntPtr pOpenThread = OpenThread(thread_suspend_resume, false, (uint)thread.Id);
                            if (pOpenThread == IntPtr.Zero) continue;

                            while (ResumeThread(pOpenThread) > 0) { } // 确保完全恢复
                            resumed_count++;
                            CloseHandle(pOpenThread);
                        }

                        Logger.Info("恢复学生端: 已恢复 " + resumed_count + " 个线程");
                        is_suspended = false; // 更新状态
                        this.Invoke(new Action(() =>
                        {
                            this.button_1.Text = "挂起学生端";
                            MessageBox.Show("学生端已恢复运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("挂起/恢复学生端失败", ex);
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                finally
                {
                    // 显示所有窗口并重新启用按钮
                    this.Invoke(new Action(() =>
                    {
                        foreach (Form form in Application.OpenForms)
                        {
                            form.Show();
                        }
                        this.button_1.Enabled = true;
                    }));
                }
            })
            {
                IsBackground = true // 设为后台线程
            };

            backgroundThread.Start();
        }

        private void label_1_click(object sender, EventArgs e)
        {
            Logger.Info("更多工具: 打开网址 https://www.kndxhz.cn/");
            Process.Start(new ProcessStartInfo("https://www.kndxhz.cn/") { UseShellExecute = true });
        }

        private void button_2_click(object sender, EventArgs e)
        {


            ProcessStartInfo start_info = new ProcessStartInfo
            {
                FileName = $"{Main.directory_1}\\Student.exe", // 获取当前程序的路径
                WorkingDirectory = Main.directory_1,
                Arguments = "",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process process = Process.Start(start_info);
                Logger.Info("启动学生端: " + start_info.FileName + " PID=" + (process != null ? process.Id.ToString() : "unknown"));
            }
            catch (Exception ex)
            {
                Logger.Error("启动学生端失败: " + start_info.FileName, ex);
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button_3_click(object sender, EventArgs e)
        {
            ProcessStartInfo start_info = new ProcessStartInfo
            {
                FileName = $"{Main.directory_1}\\Teacher.exe", // 获取当前程序的路径
                WorkingDirectory = Main.directory_1,
                Arguments = "",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process process = Process.Start(start_info);
                Logger.Info("启动教师端: " + start_info.FileName + " PID=" + (process != null ? process.Id.ToString() : "unknown"));
            }
            catch (Exception ex)
            {
                Logger.Error("启动教师端失败: " + start_info.FileName, ex);
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button_4_click(object sender, EventArgs e)
        {
            Logger.Info("更多工具: 切换显示/隐藏，当前 toolbox_is_hide=" + Main.toolbox_is_hide);
            // 找到主窗口，切换时保持所有窗口状态一致
            Main main_form = null;
            foreach (Form form in Application.OpenForms)
            {
                if (form is Main)
                {
                    main_form = (Main)form;
                    break;
                }
            }

            if (Main.toolbox_is_hide)
            {
                if (main_form != null)
                {
                    main_form.show_toolbox();
                }
                else
                {
                    Main.toolbox_is_hide = false;
                    Main.set_all_windows_display_affinity(Main.WDA_NONE);
                }
            }
            else
            {
                if (main_form != null)
                {
                    main_form.hide_toolbox();
                }
                else
                {
                    Main.toolbox_is_hide = true;
                    Main.set_all_windows_display_affinity(Main.WDA_EXCLUDEFROMCAPTURE);
                }
            }

            this.UpdateHideButton();
        }



    }


}
