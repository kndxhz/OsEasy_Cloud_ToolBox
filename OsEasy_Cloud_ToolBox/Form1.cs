using System;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    public partial class Form1 : Form
    {
        // 存储一系列的句子
        private string[] sentences = new string[]
        {
            "《机课时间管理》",
            "开源造福人类",
            "锟斤拷烫烫烫",
            "if...else...仙人！",
            "While True仙人",
            "不要自己造轮子！"
        };

        private int currentSentenceIndex = 0; // 当前显示的句子索引
        private int currentCharIndex = 0; // 当前句子的字符索引
        private Timer typingTimer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 弹出程序信息框
            MessageBox.Show("本程序由可耐的小伙纸开发\n以GPL V3协议开源\n感谢使用",
                "程序信息",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小
            this.label1.Text = ""
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
                switchSentenceTimer.Interval = 1000; // 切换句子的间隔（1000毫秒）
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
    }
}
