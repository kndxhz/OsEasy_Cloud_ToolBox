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
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // 初始化 DataGridView
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = false;

            // 添加列
            dataGridView1.Columns.Add("VariableName", "变量名");
            dataGridView1.Columns.Add("Type", "类型");
            dataGridView1.Columns.Add("Value", "值");

            // 初始加载
            RefreshVariables();

            // 绑定事件
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

            // 获取所有需要检查的类型
            var types = new[] { typeof(Main), typeof(More), typeof(Debuger), typeof(Program) };

            foreach (var type in types)
            {
                var variables = GetAllVariables(type);
                foreach (var variable in variables)
                {
                    dataGridView1.Rows.Add($"{type.Name}.{variable.Name}", variable.Type, variable.Value);
                }
            }

            // 自适应行宽和行高
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            dataGridView1.AutoResizeRows(DataGridViewAutoSizeRowsMode.AllCells);
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0) // 只处理值列
            {
                string variableName = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string newValue = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                UpdateVariable(variableName, newValue);
            }
        }

        private void UpdateVariable(string variableName, string newValue)
        {
            try
            {
                // 解析变量名称，获取类型和字段名
                var parts = variableName.Split('.');
                if (parts.Length != 2) return;

                string typeName = parts[0];
                string fieldName = parts[1];

                var type = Type.GetType($"OsEasy_Cloud_ToolBox.{typeName}");
                if (type == null) throw new Exception($"未找到类型：{typeName}");

                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null) throw new Exception($"未找到字段：{fieldName}");

                // 更新字段值
                UpdateFieldValue(field, newValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新变量时发生错误：{ex.Message}");
            }
        }

        private void UpdateFieldValue(FieldInfo field, string newValue)
        {
            try
            {
                object instance = null;
                if (!field.IsStatic) // 如果字段是实例字段，创建实例
                {
                    instance = Activator.CreateInstance(field.DeclaringType);
                }

                object convertedValue = Convert.ChangeType(newValue, field.FieldType);
                field.SetValue(instance, convertedValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法更新字段：{ex.Message}");
            }
        }

        private static (string Name, string Type, object Value)[] GetAllVariables(Type type)
        {
            try
            {
                // 获取字段（包括静态和实例，公有和私有）
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                // 创建实例（用于读取实例字段）
                object instance = null;
                if (fields.Any(f => !f.IsStatic))
                {
                    instance = Activator.CreateInstance(type);
                }

                return fields
                    .Select(field =>
                    {
                        object value = null;
                        try
                        {
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
            catch
            {
                return Array.Empty<(string Name, string Type, object Value)>();
            }
        }
    }
}
