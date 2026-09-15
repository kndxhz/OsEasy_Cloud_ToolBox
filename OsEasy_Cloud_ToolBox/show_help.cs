using System.Collections.Generic;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    internal class show_help
    {
        // Main.cs 按钮帮助信息
        public static readonly Dictionary<string, HelpInfo> main_button_helps = new Dictionary<string, HelpInfo>
        {
            {
                "button_1",
                new HelpInfo(
                    "关学生端",
                    "自动循环三次结束学生端相关进程\n过程中会弹出进度条显示进度\n执行完成后会弹窗提示\n此时学生端已经被关闭"
                )
            },
            {
                "button_2",
                new HelpInfo(
                    "解禁网络",
                    "借用学生端的解禁网络功能\n需要注意：需要先运行 关学生端\n否则解禁了之后又会被教师端下发重新禁用网络"
                )
            },
            {
                "button_3",
                new HelpInfo(
                    "解禁U盘",
                    "借用学生端的解禁U盘功能\n需要注意：需要先运行 关学生端\n否则解禁了之后又会被教师端下发重新禁用U盘"
                )
            },
            {
                "button_4",
                new HelpInfo(
                    "更多工具",
                    "baka！zako！\n这还要解释吗？！\n就是弹出一个新窗口\n里面有更多功能而已啊（"
                )
            }
        };

        // more.cs 按钮帮助信息
        public static readonly Dictionary<string, HelpInfo> more_button_helps = new Dictionary<string, HelpInfo>
        {
            {
                "button_1",
                new HelpInfo(
                    "挂起/恢复学生端",
                    "重磅功能：\n点击后程序会隐藏5秒\n然后恢复\n此时教师端看你不是下线\n而是一直卡在隐藏的那个界面\n可以有效规避点名等功能\n此外如果教师端发了文件/发了消息/发起点名\n你也可以再次点击以正常运行\n\n基于Windows API接口实现"
                )
            },
            {
                "button_2",
                new HelpInfo(
                    "启动学生端",
                    "字面意思\n点击后可以启动学生端程序\n记得把关闭学生端的cmd关了\n要不然一启动就自动被关闭了"
                )
            },
            {
                "button_3",
                new HelpInfo(
                    "启动教师端",
                    "重磅功能：\n可以在学生端没有运行的时候启动教师端\n理论上来说\n只要你赶在教师机系统启动之前就把教师机的ip和频道占了\n那么你也可以像教师机一样掌管整个教室"
                )
            },
            {
                "button_4",
                new HelpInfo(
                    "显示/隐藏本程序",
                    "可以让教师端看不到本程序\n（如果系统版本小于Windows 10 2004则看本程序是黑屏）"
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
        public static void ShowHelp(string button_name, bool is_more_form = false)
        {
            Dictionary<string, HelpInfo> helps = is_more_form ? more_button_helps : main_button_helps;

            if (helps.ContainsKey(button_name))
            {
                HelpInfo help = helps[button_name];
                MessageBoxHelper.Show(
                    help.Content,
                    $"帮助 - {help.Title}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
    }
}
