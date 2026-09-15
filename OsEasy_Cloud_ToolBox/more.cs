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


        private const int thread_suspend_resume_access = 0x0002;

        //private bool student_suspended = false; // 记录当前是否已挂起
        bool student_suspended = Main.student_process_suspended;
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
            Logger.Info("更多工具窗口加载，student_suspended=" + student_suspended);
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            
            // 为按钮添加右键帮助事件
            this.button_1.MouseDown += button_mouse_down;
            this.button_2.MouseDown += button_mouse_down;
            this.button_3.MouseDown += button_mouse_down;
            this.button_4.MouseDown += button_mouse_down;

            // 关闭窗口时记录日志
            this.FormClosing += more_form_closing;

            if (student_suspended)
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
                Main.toolbox_is_hidden ? Main.WDA_EXCLUDEFROMCAPTURE : Main.WDA_NONE);
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

            this.button_4.Text = Main.toolbox_is_hidden ? "显示本程序" : "隐藏本程序";
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

            student_suspended = suspended;
            this.button_1.Text = suspended ? "恢复学生端" : "挂起学生端";
        }

        private void button_1_click(object sender, EventArgs e)
        {
            // 点击后先显示警告信息框，确认后才继续
            if (!student_suspended)
            {
                DialogResult suspend_confirm_result = MessageBox.Show(
                                "点击后程序会隐藏5秒\n然后恢复\n此时教师端看你不是下线\n而是一直卡在隐藏的那个界面\n可以有效规避点名等功能\n此外如果教师端发了文件/发了消息/发起点名\n你也可以再次点击以正常运行\n\n基于Windows API接口实现\n\n是否继续？",
                                "警告",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                if (suspend_confirm_result != DialogResult.Yes)
                {
                    Logger.Info("挂起/恢复学生端: 用户取消");
                    return;
                }
            }
                

            Logger.Info("挂起/恢复学生端: 开始，当前 student_suspended=" + student_suspended);

            // 禁用按钮，防止重复点击
            this.button_1.Enabled = false;

            // 在后台线程上执行挂起/恢复操作，避免阻塞UI
            Thread backgroundThread = new Thread(() =>
            {
                try
                {
                    // 获取目标进程（例如 Student.exe）
                    var student_processes = Process.GetProcessesByName("Student");
                    if (student_processes.Length == 0)
                    {
                        Logger.Warn("挂起/恢复学生端: 未找到 Student 进程");
                        this.Invoke(new Action(() =>
                        {
                            MessageBox.Show("未找到学生端进程！\n请先启动学生端然后稍等一会", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }));
                        return;
                    }
                    Logger.Info("挂起/恢复学生端: 找到 " + student_processes.Length + " 个 Student 进程，PID=" + student_processes[0].Id);

                    // 根据当前状态决定挂起或恢复
                    if (!student_suspended)
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
                        foreach (ProcessThread thread in student_processes[0].Threads)
                        {
                            IntPtr thread_handle = OpenThread(thread_suspend_resume_access, false, (uint)thread.Id);
                            if (thread_handle == IntPtr.Zero) continue;

                            SuspendThread(thread_handle);
                            suspended_count++;
                            CloseHandle(thread_handle);
                        }

                        Logger.Info("挂起学生端: 已挂起 " + suspended_count + " 个线程");
                        student_suspended = true; // 更新状态
                        this.Invoke(new Action(() =>
                        {
                            this.button_1.Text = "恢复学生端";
                        }));
                    }
                    else
                    {
                        // 恢复所有线程
                        int resumed_count = 0;
                        foreach (ProcessThread thread in student_processes[0].Threads)
                        {
                            IntPtr thread_handle = OpenThread(thread_suspend_resume_access, false, (uint)thread.Id);
                            if (thread_handle == IntPtr.Zero) continue;

                            while (ResumeThread(thread_handle) > 0) { } // 确保完全恢复
                            resumed_count++;
                            CloseHandle(thread_handle);
                        }

                        Logger.Info("恢复学生端: 已恢复 " + resumed_count + " 个线程");
                        student_suspended = false; // 更新状态
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


            ProcessStartInfo process_start_info = new ProcessStartInfo
            {
                FileName = $"{Main.student_install_dir}\\Student.exe", // 获取当前程序的路径
                WorkingDirectory = Main.student_install_dir,
                Arguments = "",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process process = Process.Start(process_start_info);
                Logger.Info("启动学生端: " + process_start_info.FileName + " PID=" + (process != null ? process.Id.ToString() : "unknown"));
            }
            catch (Exception ex)
            {
                Logger.Error("启动学生端失败: " + process_start_info.FileName, ex);
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button_3_click(object sender, EventArgs e)
        {
            // 学生端正在运行时，先调用“关学生端”逻辑结束它
            if (Process.GetProcessesByName("Student").Length > 0)
            {
                Logger.Info("启动教师端: 检测到学生端正在运行，先调用关学生端逻辑");

                Main main_form = null;
                foreach (Form form in Application.OpenForms)
                {
                    if (form is Main)
                    {
                        main_form = (Main)form;
                        break;
                    }
                }

                if (main_form != null)
                {
                    main_form.button1_click(sender, e);
                }
                else
                {
                    Logger.Warn("启动教师端: 未找到主窗口，跳过关闭学生端");
                }
            }

            ProcessStartInfo process_start_info = new ProcessStartInfo
            {
                FileName = $"{Main.student_install_dir}\\Teacher.exe", // 获取当前程序的路径
                WorkingDirectory = Main.student_install_dir,
                Arguments = "",
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            try
            {
                Process process = Process.Start(process_start_info);
                Logger.Info("启动教师端: " + process_start_info.FileName + " PID=" + (process != null ? process.Id.ToString() : "unknown"));
            }
            catch (Exception ex)
            {
                Logger.Error("启动教师端失败: " + process_start_info.FileName, ex);
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button_4_click(object sender, EventArgs e)
        {
            Logger.Info("更多工具: 切换显示/隐藏，当前 toolbox_is_hidden=" + Main.toolbox_is_hidden);
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

            if (Main.toolbox_is_hidden)
            {
                if (main_form != null)
                {
                    main_form.show_toolbox();
                }
                else
                {
                    Main.toolbox_is_hidden = false;
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
                    Main.toolbox_is_hidden = true;
                    Main.set_all_windows_display_affinity(Main.WDA_EXCLUDEFROMCAPTURE);
                }
            }

            this.UpdateHideButton();
        }



    }


}
