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
            Logger.StartSession();

            // System.Diagnostics.Debugger.Launch();

            // 先检查并尝试提权
            bool is_admin = is_run_as_admin();
            Logger.Info("程序启动，管理员权限=" + is_admin + "，日志文件=" + Logger.LogPath);
            if (!is_admin)
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
                    Logger.Info("正在以管理员权限重启: " + Application.ExecutablePath);
                }
                catch (Exception ex)
                {
                    Logger.Error("以管理员权限重启失败", ex);
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
                    Logger.Info("检测到已有实例，切换到已运行窗口");
                    bring_existing_instance_to_front();
                    return;
                }

                // 如果Mutex是新创建的，说明是第一个实例，正常启动程序
                Logger.Info("创建并运行主窗口");

                Application.ApplicationExit += (s, args) => Logger.Info("应用程序退出（ApplicationExit）");
                Application.ThreadException += (s, args) => Logger.Error("未处理的UI线程异常", args.Exception);
                AppDomain.CurrentDomain.UnhandledException += (s, args) =>
                    Logger.Error("未处理的异常（UnhandledException）", args.ExceptionObject as Exception);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());

                Logger.Info("程序主循环结束，进程即将退出");
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

