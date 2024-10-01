using System;
using System.Windows.Forms;

namespace Let_s_happy
{
    public partial class Form1 : Form
    {
        // 创建全局变量
        string[] one_sentence = { "欢迎来到工业学校！", 
            "学习要注意劳逸结合", 
            "请确保你已经会了上课的内容再使用本软件", 
            "懒惰是人类科技进步的唯一动力",
            "如果你觉得本软件不错，请分享给其他人"};
        int i = 0; // 用于滚动的计数器

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 显示作者信息
            MessageBox.Show("本程序由可耐的小伙纸开发\n邮箱kndxhz@163.com\n感谢广大工业学子使用",
                            "作者信息",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
            timer1.Start(); // 启动计时器
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 滚动显示one_sentence的内容
            label1.Text = one_sentence[i]; // 将当前one_sentence中的值显示在label1上

            // 递增i，使其指向下一个句子
            i++;

            // 如果i超出数组长度，则重置为0，实现循环
            if (i >= one_sentence.Length)
            {
                i = 0;
            }
        }
    }
}
