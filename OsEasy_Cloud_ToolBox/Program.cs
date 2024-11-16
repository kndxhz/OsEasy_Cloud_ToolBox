using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // 检测是否在调试模式
            bool isDebugMode = false;
#if DEBUG
            isDebugMode = true;
#endif

            // 检测是否传入 --debug 参数
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("--debug"))
            {
                isDebugMode = true;
            }

            // 如果在调试模式，打开 Form3
            if (isDebugMode)
            {
                Debuger debugForm = new Debuger();
                debugForm.Show();
            }
            Application.Run(new Main());
        }
    }
}
