﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace OsEasy_Cloud_ToolBox
{
    public partial class Main : Form
    {
        // 存储一系列的句子
        private string[] sentences = new string[]
        {
            "《机课时间管理》",
            "开源造福人类",
            "锟斤拷烫烫烫",
            "if...else...仙人",
            "While True仙人",
            "Fuck you Linus",
            "Hello World!",
            "世界...遗忘我...",
            "MAGA!",
            "你打cs吗",
            "Python(屁眼通红)",
            "C#(C++ *2)",
            "你要来杯Jvav吗",
            "尝试使用--debug参数或debug环境打开debuger！"

        };

        private int currentSentenceIndex = 0; // 当前显示的句子索引
        private int currentCharIndex = 0; // 当前句子的字符索引
        private Timer typingTimer;

        public Main()
        {
            InitializeComponent();
        }
        public static string directory1;
        [STAThread]
        private void Form1_Load(object sender, EventArgs e)
        {

            

            // 检查是否以管理员权限运行
            if (!IsRunAsAdmin())
            {
                // 如果没有管理员权限，尝试以管理员权限重新启动程序
                try
                {
                    // 以管理员权限重新启动当前程序
                    RestartAsAdmin();
                }
                catch (Exception)
                {
                    // 提权失败，弹出信息框并退出程序
                    MessageBox.Show("本程序需要以管理员权限运行", "需要管理员权限", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                return; // 程序已被管理员权限启动，终止当前加载
            }

            // 弹出程序信息框
            MessageBox.Show("本程序由可耐的小伙纸开发\n以GPL V3协议开源\n感谢使用",
                "程序信息",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            this.label1.Text = "";
            try { 
                var process = Process.GetProcessesByName("Student").FirstOrDefault();
                directory1 = System.IO.Path.GetDirectoryName(process.MainModule.FileName);
                
            }
            catch (Exception)
            {
                directory1 = "C:\\Program Files (x86)\\Os-Easy\\multimedia network teaching System";
            }
            // 初始化定时器
            typingTimer = new Timer();
            typingTimer.Interval = 100; // 设置每次显示字符的间隔（100毫秒）
            typingTimer.Tick += TypingTimer_Tick;

            // 开始打字效果
            typingTimer.Start();


            
        }

        // 定时器事件：每次触发时，逐个显示字符
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            // 判断当前句子是否已显示完
            if (currentCharIndex < sentences[currentSentenceIndex].Length)
            {
                // 将下一个字符添加到 label1
                this.label1.Text += sentences[currentSentenceIndex][currentCharIndex];
                currentCharIndex++; // 移动到下一个字符
            }
            else
            {
                // 当前句子显示完后，暂停一段时间
                typingTimer.Stop();

                // 设置间隔时间后切换到下一个句子
                Timer switchSentenceTimer = new Timer();
                switchSentenceTimer.Interval = 3000; // 切换句子的间隔（3000毫秒）
                switchSentenceTimer.Tick += (s, args) =>
                {
                    // 重新设置定时器，显示下一个句子
                    switchSentenceTimer.Stop();
                    currentCharIndex = 0; // 重置字符索引
                    currentSentenceIndex++; // 切换到下一个句子
                    if (currentSentenceIndex >= sentences.Length)
                    {
                        currentSentenceIndex = 0; // 如果到了最后一条，重新从头开始
                    }

                    // 清空 label 并开始新的打字效果
                    this.label1.Text = "";
                    typingTimer.Start();
                };
                switchSentenceTimer.Start(); // 启动切换句子的定时器
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 获取当前应用程序的临时目录路径
            string tempDir = Path.GetTempPath();

            // 设定文件路径
            string filePath = Path.Combine(tempDir, "killer.bat");

            // 将 Resources 中的 "killer" 文件写入到临时目录
            File.WriteAllBytes(filePath, Encoding.Default.GetBytes(Properties.Resources.killer));

            // 以管理员权限运行该文件
            RunAsAdmin(filePath);
        }

        public static void RunAsAdmin(string path)
        {
            // 检查文件是否存在
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("指定的文件不存在", path);
            }

            // 设置 ProcessStartInfo 来以管理员权限运行
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = path,        // 要执行的文件路径
                Verb = "runas",         // 以管理员权限运行
                UseShellExecute = true  // 使用外部 shell 启动
            };

            // 启动进程
            Process.Start(startInfo);
        }

        // 检查当前进程是否有管理员权限
        private bool IsRunAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // 以管理员权限重新启动当前程序
        private void RestartAsAdmin()
        {
            
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Application.ExecutablePath, // 获取当前程序的路径
                Arguments = Program.argsString, // 传递命令行参数
                Verb = "runas",                         // 以管理员权限运行
                UseShellExecute = true                  // 使用外部 shell 启动
            };
            Process.Start(startInfo);
            Application.Exit(); // 退出当前程序
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
            "此操作将会重启系统\n你确定吗？",  // 消息内容
            "警告",                          // 标题
            MessageBoxButtons.YesNo,          // 显示"是"和"否"按钮
            MessageBoxIcon.Warning);          // 警告图标

            // 根据用户选择处理
            if (result == DialogResult.Yes)
            {
                // 获取当前应用程序的临时目录路径
                string tempDir = Path.GetTempPath();

                // 设定文件路径
                string filePath = Path.Combine(tempDir, "task.bat");

                // 将 Resources 中的 "task" 文件写入到临时目录
                File.WriteAllBytes(filePath, Encoding.Default.GetBytes(Properties.Resources.task));

                // 以管理员权限运行该文件
                RunAsAdmin(filePath);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://cn-sy1.rains3.com/xhz/ToolBox%20(1).zip") { UseShellExecute = true });
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        
            Process.Start(new ProcessStartInfo("https://github.com/kndxhz/OsEasy_Cloud_ToolBox") { UseShellExecute = true });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            More form2 = new More();
            form2.Show();  // 使用 Show() 使新窗口非阻塞
        }


    }
   }
