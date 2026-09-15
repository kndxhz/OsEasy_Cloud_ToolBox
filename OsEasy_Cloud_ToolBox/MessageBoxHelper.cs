using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    // 消息框/输入框等模态弹窗的封装。
    // 当工具箱处于隐藏状态（Main.toolbox_is_hidden == true）时，
    // 自动为弹出的窗口加上反截屏属性，使其对屏幕捕获不可见。
    internal static class MessageBoxHelper
    {
        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadProc lpfn, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private delegate bool EnumThreadProc(IntPtr hWnd, IntPtr lParam);

        // 弹窗期间轮询设置显示关联的间隔（毫秒）
        private const int poll_interval = 30;

        // 在弹出模态窗口期间，持续为当前线程的顶层窗口设置反截屏属性。
        // 消息框、输入框本质都是独立顶层窗口，因此可以统一处理。
        private static T run_with_capture_protection<T>(Func<T> show_dialog)
        {
            if (show_dialog == null)
            {
                throw new ArgumentNullException(nameof(show_dialog));
            }

            // 工具箱未隐藏时无需处理，直接弹出
            if (!Main.toolbox_is_hidden)
            {
                return show_dialog();
            }

            // MessageBox/InputBox 的模态循环会继续分发消息，因此 WinForms Timer 能正常触发
            using (Timer timer = new Timer())
            {
                timer.Interval = poll_interval;
                timer.Tick += (sender, e) => apply_affinity_to_thread_windows();
                timer.Start();
                try
                {
                    return show_dialog();
                }
                finally
                {
                    timer.Stop();
                }
            }
        }

        // 将反截屏属性应用到当前线程的所有顶层窗口
        private static void apply_affinity_to_thread_windows()
        {
            uint affinity = Main.toolbox_is_hidden
                ? Main.WDA_EXCLUDEFROMCAPTURE
                : Main.WDA_NONE;

            uint thread_id = GetCurrentThreadId();
            EnumThreadWindows(thread_id, (h_wnd, l_param) =>
            {
                Main.apply_display_affinity(h_wnd, affinity);
                return true;
            }, IntPtr.Zero);
        }

        // 所有消息框的统一入口：记录日志、设置反截屏属性并返回用户选择
        private static DialogResult show_message_box(
            IWin32Window owner,
            string text,
            string caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton default_button)
        {
            Logger.Info("弹出消息框: [" + caption + "] " + text);

            DialogResult result = run_with_capture_protection(() =>
                owner != null
                    ? MessageBox.Show(owner, text, caption, buttons, icon, default_button)
                    : MessageBox.Show(text, caption, buttons, icon, default_button));

            Logger.Info("消息框关闭: [" + caption + "] 用户选择=" + result);
            return result;
        }

        public static DialogResult Show(string text)
        {
            return show_message_box(null, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption)
        {
            return show_message_box(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return show_message_box(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return show_message_box(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton default_button)
        {
            return show_message_box(null, text, caption, buttons, icon, default_button);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return show_message_box(owner, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return show_message_box(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return show_message_box(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return show_message_box(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton default_button)
        {
            return show_message_box(owner, text, caption, buttons, icon, default_button);
        }

        // 输入框封装：记录日志、设置反截屏属性并返回输入内容（取消时为空字符串）
        public static string ShowInputBox(string prompt, string title, string default_response, int x_pos, int y_pos)
        {
            Logger.Info("弹出输入框: [" + title + "] " + prompt);

            string result = run_with_capture_protection(
                () => Microsoft.VisualBasic.Interaction.InputBox(prompt, title, default_response, x_pos, y_pos));

            Logger.Info("输入框关闭: [" + title + "] 输入=" + (string.IsNullOrEmpty(result) ? "(空/已取消)" : result));
            return result;
        }
    }
}
