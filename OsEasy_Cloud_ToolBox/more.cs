using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;

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


        private const int THREAD_SUSPEND_RESUME = 0x0002;
        
        private bool isSuspended = false; // 记录当前是否已挂起
        public More()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
        }

        private void button1_Click(object sender, EventArgs e)
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
                if (!isSuspended)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        form.Hide();
                    }
                    Thread.Sleep(5000);
                    // 挂起所有线程
                    foreach (ProcessThread thread in processes[0].Threads)
                    {
                        IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)thread.Id);
                        if (pOpenThread == IntPtr.Zero) continue;

                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }

                    isSuspended = true; // 更新状态
                    this.button1.Text = "恢复学生端";
                    //MessageBox.Show("Student.exe 已成功挂起！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 恢复所有线程
                    foreach (ProcessThread thread in processes[0].Threads)
                    {
                        IntPtr pOpenThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)thread.Id);
                        if (pOpenThread == IntPtr.Zero) continue;

                        while (ResumeThread(pOpenThread) > 0) { } // 确保完全恢复
                        CloseHandle(pOpenThread);
                    }

                    isSuspended = false; // 更新状态
                    this.button1.Text = "挂起学生端";
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
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = $"{Main.directory1}\\Teacher.exe", // 获取当前程序的路径
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

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("数据（进程名）待收集\n暂时无法使用");
            //ProcessStartInfo startInfo = new ProcessStartInfo
            //{
            //    FileName = $"{Main.directory1}\\Teacher.exe", // 获取当前程序的路径
            //    Arguments = "",
            //    Verb = "runas",                         // 以管理员权限运行
            //    UseShellExecute = true                  // 使用外部 shell 启动
            //};
            //try
            //{
            //    Process.Start(startInfo);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("目录不存在：\n" + ex.Message);
            //}
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
