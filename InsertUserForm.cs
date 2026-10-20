using System;
using System.Data;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class InsertUserForm : Form
    {
        private int _userId = 0;

        public InsertUserForm(int userId = 0)
        {
            InitializeComponent();
            _userId = userId;

            // Привязка ролей
            DataTable dtRoles = DbHelper.ExecuteQuery("SELECT id, name FROM roles");
            comboBox1.DataSource = dtRoles;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";

            if (_userId > 0)
            {
                this.Text = "Изменение данных";
                label4.Text = "Изменение данных";
                button2.Text = "Изменить";
                LoadUserData();
            }
        }

        private void LoadUserData()
        {
            string query = $"SELECT * FROM users WHERE id = {_userId}";
            DataTable dt = DbHelper.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                textBox1.Text = row["full_name"].ToString();
                textBox3.Text = row["login"].ToString();
                textBox2.Text = row["password"].ToString();
                textBox4.Text = row["email"].ToString();
                textBox5.Text = row["phone"].ToString();
                comboBox1.SelectedValue = row["role_id"];

                if (row["hire_date"] != DBNull.Value)
                {
                    dateTimePicker1.Value = Convert.ToDateTime(row["hire_date"]);
                }

                checkBox1.Checked = Convert.ToInt32(row["is_active"]) == 1;
            }
        }

        // Кнопка "Изменить"
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Точно ли вы хотите изменить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Проверка на обязательные поля
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox3.Text) || comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Заполните обязательные поля (ФИО, Логин, Роль)!");
                return;
            }

            string fullName = textBox1.Text.Trim();
            string login = textBox3.Text.Trim();
            string password = textBox2.Text.Trim();
            string email = textBox4.Text.Trim();
            string phone = textBox5.Text.Trim();
            int roleId = Convert.ToInt32(comboBox1.SelectedValue);
            string hireDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            int isActive = checkBox1.Checked ? 1 : 0;

            string query = $@"UPDATE users SET 
                                login = '{login}', 
                                password = '{password}', 
                                full_name = '{fullName}', 
                                email = '{email}', 
                                phone = '{phone}', 
                                role_id = {roleId}, 
                                hire_date = '{hireDate}', 
                                is_active = {isActive} 
                              WHERE id = {_userId}";

            try
            {
                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Данные успешно обновлены!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        // Кнопка "Выход"
        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}