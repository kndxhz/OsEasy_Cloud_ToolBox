using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace OsEasy_Cloud_ToolBox
{
    public partial class Debuger : Form
    {

        public Debuger()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // 设置窗体样式
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 不允许调整大小

            // 初始化 DataGridView
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = false; // 允许编辑

            // 添加列
            dataGridView1.Columns.Add("VariableName", "变量名");
            dataGridView1.Columns.Add("Type", "类型");
            dataGridView1.Columns.Add("Value", "值");

            // 初始加载
            RefreshVariables();

            // 绑定 CellValueChanged 事件
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 手动刷新表格
            RefreshVariables();
        }

        private void RefreshVariables()
        {
            // 清空表格
            dataGridView1.Rows.Clear();

            // 获取 Form1、Form2 和 Form3 的所有变量
            var form1Variables = GetAllVariables(typeof(Main));
            var form2Variables = GetAllVariables(typeof(More));
            var form3Variables = GetAllVariables(typeof(Debuger));
            var form4Variables = GetAllVariables(typeof(Program));

            // 添加数据到 DataGridView
            foreach (var variable in form1Variables)
            {
                dataGridView1.Rows.Add("Main." + variable.Name, variable.Type, variable.Value);
            }

            foreach (var variable in form2Variables)
            {
                dataGridView1.Rows.Add("More." + variable.Name, variable.Type, variable.Value);
            }

            foreach (var variable in form3Variables)
            {
                dataGridView1.Rows.Add("Debuger." + variable.Name, variable.Type, variable.Value);
            }
            
            foreach (var variable in form4Variables)
            {
                dataGridView1.Rows.Add("Program." + variable.Name, variable.Type, variable.Value);
            }

            // 自适应行宽和行高
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 确保只在编辑第三列（值列）时更新变量
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                string variableName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string newValue = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                // 查找对应的字段并更新值
                UpdateVariable(variableName, newValue);
            }
        }

        private void UpdateVariable(string variableName, string newValue)
        {
            // 查找变量名对应的字段，更新字段的值
            if (variableName.StartsWith("Main."))
            {
                var fieldName = variableName.Substring(5); // 去除 "Form1."
                var form1Variable = typeof(Main).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.NonPublic);
                if (form1Variable != null)
                {
                    UpdateFieldValue(form1Variable, newValue);
                }
            }
            else if (variableName.StartsWith("More."))
            {
                var fieldName = variableName.Substring(5); // 去除 "Form2."
                var form2Variable = typeof(Main).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.NonPublic);
                if (form2Variable != null)
                {
                    UpdateFieldValue(form2Variable, newValue);
                }
            }
            else if (variableName.StartsWith("Debuger."))
            {
                var fieldName = variableName.Substring(8); // 去除 "Form3."
                var form3Variable = typeof(Main).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.NonPublic);
                if (form3Variable != null)
                {
                    UpdateFieldValue(form3Variable, newValue);
                }
            }
            else if (variableName.StartsWith("Program."))
            {
                var fieldName = variableName.Substring(8); // 去除 "Form3."
                var form4Variable = typeof(Main).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.NonPublic);
                if (form4Variable != null)
                {
                    UpdateFieldValue(form4Variable, newValue);
                }
            }
        }

        private void UpdateFieldValue(FieldInfo field, string newValue)
        {
            // 根据字段类型更新值
            try
            {
                if (field.FieldType == typeof(int))
                {
                    field.SetValue(this, int.Parse(newValue));
                }
                else if (field.FieldType == typeof(string))
                {
                    field.SetValue(this, newValue);
                }
                else if (field.FieldType == typeof(bool))
                {
                    field.SetValue(this, bool.Parse(newValue));
                }
                // 可根据需要继续添加其他类型的处理
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新变量时发生错误：{ex.Message}");
            }
        }

        private static (string Name, string Type, object Value)[] GetAllVariables(Type type)
        {
            // 获取所有字段（实例和静态，公有和私有）
            foreach (BindingFlags flag in Enum.GetValues(typeof(BindingFlags)))
            {
                Console.WriteLine(flag);
            }
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.CreateInstance | BindingFlags.NonPublic);

            // 创建一个实例（用于读取实例字段）
            object instance = null;
            if (fields.Any(f => !f.IsStatic)) // 只有存在实例字段时才创建
            {
                instance = Activator.CreateInstance(type);
            }

            // 获取字段信息
            return fields
                .Select(field =>
                {
                    object value = null;

                    try
                    {
                        // 读取字段值（区分静态和实例字段）
                        value = field.IsStatic ? field.GetValue(null) : field.GetValue(instance);
                    }
                    catch
                    {
                        value = "无法访问";
                    }

                    return (field.Name, field.FieldType.Name, value ?? "null");
                })
                .ToArray();
        }
    }
}
