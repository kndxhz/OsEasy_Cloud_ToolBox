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
        [STAThread] // 标明应用程序是单线程单元 (STA) 模型，通常在UI应用中使用
        static void Main()
        {
            Application.EnableVisualStyles(); // 启用视觉样式
            Application.SetCompatibleTextRenderingDefault(false); // 设置兼容文本渲染模式
            Application.Run(new Main()); // 启动主窗体
        }
    }
}

