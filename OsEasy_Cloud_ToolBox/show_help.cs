using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    internal class show_help
    {
        // Main.cs 按钮帮助信息
        public static readonly Dictionary<string, HelpInfo> MainButtonHelps = new Dictionary<string, HelpInfo>
        {
            {
                "button_1",
                new HelpInfo(
                    "杀死进程",
                    "可以做什么：\n强制关闭学生端进程（Student.exe等），使其立即停止运行。\n\n怎么使用：\n点击按钮执行杀死进程操作，通过管理员权限运行killer.bat脚本。\n\n怎么恢复：\n需要重新启动系统或从\"更多\"窗口点击\"Student\"按钮重新启动学生端。\n\n原理：\n执行killer.bat批处理脚本文件，该脚本在临时目录中以管理员权限强制终止Student.exe进程。"
                )
            },
            {
                "button_2",
                new HelpInfo(
                    "解禁网络",
                    "可以做什么：\n解除学生端的网络限制，恢复互联网访问能力。\n\n怎么使用：\n点击按钮会弹窗询问解禁方式：\n• 是（软解禁，推荐）：需要获取教师机IP，通过DeviceControl工具移除限制\n• 否（硬解禁）：将重启系统以移除所有限制\n\n怎么恢复：\n软解禁可通过相同方式重新禁用；硬解禁后需要重新设置限制。\n\n原理：\n软解禁：从学生端日志文件读取教师机IP，调用DeviceControl_x64.exe工具以管理员权限移除网络限制。\n硬解禁：执行task.bat脚本重启系统以清除所有限制。"
                )
            },
            {
                "button_3",
                new HelpInfo(
                    "解禁USB",
                    "可以做什么：\n解除学生端的USB设备限制，允许使用USB存储设备等外部设备。\n\n怎么使用：\n点击按钮即可执行解禁操作，无需额外配置或参数。\n\n怎么恢复：\n需要重新禁用USB限制或重启系统以恢复限制。\n\n原理：\n调用DeviceControl_x64.exe工具以管理员权限执行命令：\n--type usb --operation 0 --extend 0.0.0.0\n该命令移除针对USB设备的所有限制。"
                )
            },
            {
                "button_4",
                new HelpInfo(
                    "打开更多窗口",
                    "可以做什么：\n打开\"更多功能\"窗口，提供学生端进程管理、启动教师端等高级功能。\n\n怎么使用：\n点击按钮打开新窗口。如果窗口已打开，点击会激活并前置该窗口。\n\n怎么恢复：\n关闭打开的\"更多\"窗口即可返回主窗口。\n\n原理：\n创建More窗体类的新实例并显示，若实例已存在则直接激活。窗口关闭时清空实例引用，下次点击可重新创建。"
                )
            }
        };

        // more.cs 按钮帮助信息
        public static readonly Dictionary<string, HelpInfo> MoreButtonHelps = new Dictionary<string, HelpInfo>
        {
            {
                "button_1",
                new HelpInfo(
                    "挂起/恢复学生端",
                    "可以做什么：\n挂起学生端进程的所有线程使其暂停运行，或恢复其运行。\n\n怎么使用：\n• 挂起学生端：点击\"挂起学生端\"按钮，窗口会隐藏5秒后恢复，按钮变为\"恢复学生端\"\n• 恢复学生端：点击\"恢复学生端\"按钮恢复进程运行\n\n怎么恢复：\n如果学生端被挂起，点击\"恢复学生端\"按钮即可恢复其正常运行。\n\n原理：\n使用Windows API接口（OpenThread、SuspendThread、ResumeThread）直接操作学生端进程的所有线程。挂起时暂停所有线程，恢复时完全唤醒所有线程。"
                )
            },
            {
                "button_2",
                new HelpInfo(
                    "启动学生端",
                    "可以做什么：\n以管理员权限启动学生端程序（Student.exe）。\n\n怎么使用：\n点击按钮以管理员权限启动学生端，若已运行则重新启动一个新实例。\n\n怎么恢复：\n如需关闭学生端，使用主窗口\"杀死进程\"功能或在任务管理器中结束进程。\n\n原理：\n使用ProcessStartInfo类以管理员权限（Verb=\"runas\"）启动Student.exe可执行文件。"
                )
            },
            {
                "button_3",
                new HelpInfo(
                    "启动教师端",
                    "可以做什么：\n以管理员权限启动教师端程序（Teacher.exe）。\n\n怎么使用：\n点击按钮以管理员权限启动教师端，若已运行则重新启动一个新实例。\n\n怎么恢复：\n在教师端窗口中选择退出或在任务管理器中结束Teacher.exe进程。\n\n原理：\n使用ProcessStartInfo类以管理员权限（Verb=\"runas\"）启动Teacher.exe可执行文件。"
                )
            },
            {
                "button_4",
                new HelpInfo(
                    "检查更新",
                    "可以做什么：\n显示当前工具箱版本信息，并提供下载最新版本的链接。\n\n怎么使用：\n点击按钮会显示版本信息和开发者信息，同时提供下载链接进行更新。\n\n怎么恢复：\n无需恢复操作。\n\n原理：\n显示信息对话框并打开浏览器访问下载链接（HTTPS），允许用户下载最新的工具箱。"
                )
            }
        };

        // 帮助信息类
        public class HelpInfo
        {
            public string Title { get; set; }
            public string Content { get; set; }

            public HelpInfo(string title, string content)
            {
                this.Title = title;
                this.Content = content;
            }
        }

        // 显示帮助信息的公共方法
        public static void ShowHelp(string buttonName, bool isMoreForm = false)
        {
            Dictionary<string, HelpInfo> helps = isMoreForm ? MoreButtonHelps : MainButtonHelps;

            if (helps.ContainsKey(buttonName))
            {
                HelpInfo help = helps[buttonName];
                MessageBox.Show(
                    help.Content,
                    $"帮助 - {help.Title}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
