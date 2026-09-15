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
        internal static T run_with_capture_protection<T>(Func<T> show_dialog)
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

        public static DialogResult Show(string text)
        {
            return run_with_capture_protection(() => MessageBox.Show(text));
        }

        public static DialogResult Show(string text, string caption)
        {
            return run_with_capture_protection(() => MessageBox.Show(text, caption));
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return run_with_capture_protection(() => MessageBox.Show(text, caption, buttons));
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return run_with_capture_protection(() => MessageBox.Show(text, caption, buttons, icon));
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton default_button)
        {
            return run_with_capture_protection(() => MessageBox.Show(text, caption, buttons, icon, default_button));
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            return run_with_capture_protection(() => MessageBox.Show(owner, text));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            return run_with_capture_protection(() => MessageBox.Show(owner, text, caption));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return run_with_capture_protection(() => MessageBox.Show(owner, text, caption, buttons));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return run_with_capture_protection(() => MessageBox.Show(owner, text, caption, buttons, icon));
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton default_button)
        {
            return run_with_capture_protection(() => MessageBox.Show(owner, text, caption, buttons, icon, default_button));
        }
    }
}
