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
        /// 

        // 检测是否传入 --debug 参数
        public static string[] args = Environment.GetCommandLineArgs();
        public static string argsString = string.Join(" ", args.Skip(1)); // 跳过第一个元素（应用程序路径）
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
