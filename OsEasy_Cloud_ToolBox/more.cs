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

        private bool is_suspended = false; // 记录当前是否已挂起
        public More()
        {
            InitializeComponent();
        }

        private void more_form_load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
        }

        private void button_1_click(object sender, EventArgs e)
        {




            try
            {
                // 获取目标进程（例如 Student.exe）
                var processes = Process.GetProcessesByName("Student");
                if (processes.Length == 0)
                {
                    MessageBox.Show("未找到 Student.exe 进程！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 根据当前状态决定挂起或恢复
                if (!is_suspended)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        form.Hide();
                    }
                    Thread.Sleep(5000);
                    // 挂起所有线程
                    foreach (ProcessThread thread in processes[0].Threads)
                    {
                        IntPtr pOpenThread = OpenThread(thread_suspend_resume, false, (uint)thread.Id);
                        if (pOpenThread == IntPtr.Zero) continue;

                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }

                    is_suspended = true; // 更新状态
                    this.button_1.Text = "恢复学生端";
                    //MessageBox.Show("Student.exe 已成功挂起！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 恢复所有线程
                    foreach (ProcessThread thread in processes[0].Threads)
                    {
                        IntPtr pOpenThread = OpenThread(thread_suspend_resume, false, (uint)thread.Id);
                        if (pOpenThread == IntPtr.Zero) continue;

                        while (ResumeThread(pOpenThread) > 0) { } // 确保完全恢复
                        CloseHandle(pOpenThread);
                    }

                    is_suspended = false; // 更新状态
                    this.button_1.Text = "挂起学生端";
                    MessageBox.Show("Student.exe 已恢复运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            foreach (Form form in Application.OpenForms)
            {
                form.Show();
            }
        }

        private void label_1_click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://kndxhz.cn/") { UseShellExecute = true });
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
                Process.Start(start_info);
            }
            catch (Exception ex)
            {
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
                Process.Start(start_info);
            }
            catch (Exception ex)
            {
                MessageBox.Show("目录不存在：\n" + ex.Message);
            }
        }

        private void button_4_click(object sender, EventArgs e)
        {
            MessageBox.Show("此工具箱由 @ZiHaoSaMa66 开发\n这是适用于本地机房的\n由于版本不一样\n可能会有部分功能无法使用",
    "提示",
    MessageBoxButtons.OK,
    MessageBoxIcon.Warning);
            Process.Start(new ProcessStartInfo("https://cn-sy1.rains3.com/xhz/ToolBox%20(1).zip") { UseShellExecute = true });
        }



    }


}
