using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread] // 标明应用程序是单线程单元 (STA) 模型，通常在UI应用中使用
        static void Main()
        {
            // System.Diagnostics.Debugger.Launch();

            // 先检查并尝试提权
            if (!is_run_as_admin())
            {
                try
                {
                    var start_info = new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        Verb = "runas",
                        UseShellExecute = true
                    };
                    Process.Start(start_info);
                }
                catch
                {
                    MessageBox.Show("本程序需要以管理员权限运行", "需要管理员权限", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return; // 未提权或已启动提权实例，退出当前进程
            }

            // 定义一个唯一的Mutex名称，确保在全局范围内唯一
            string mutex_name = "a25keGh6LmNu";

            // 使用标准单例模式：初始拥有并检查是否新建
            bool created_new;
            using (Mutex mutex = new Mutex(true, mutex_name, out created_new))
            {
                if (!created_new)
                {
                    // 如果Mutex已经存在，说明程序已经在运行
                    // 将焦点切换到已运行的程序窗口
                    bring_existing_instance_to_front();
                    return;
                }

                // 如果Mutex是新创建的，说明是第一个实例，正常启动程序
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
        }

        // 检查当前进程是否有管理员权限
        private static bool is_run_as_admin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// 将已运行的程序窗口带到前台.
        /// </summary>
        private static void bring_existing_instance_to_front()
        {
            // 遍历所有打开的窗口，找到我们的应用程序窗口
            foreach (Process process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                {
                    // 获取窗口句柄
                    IntPtr handle = process.MainWindowHandle;

                    if (handle != IntPtr.Zero)
                    {
                        // 显示窗口并设置为前台
                        ShowWindow(handle, sw_show);
                        SetForegroundWindow(handle);
                        return;
                    }
                }
            }
        }

        // Windows API函数声明
        private const int sw_show = 5;

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}

