using System;
using System.Data;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class InputUserForm : Form
    {
        public InputUserForm()
        {
            InitializeComponent();

            // Привязка ролей к ComboBox
            DataTable dtRoles = DbHelper.ExecuteQuery("SELECT id, name FROM roles");
            comboBox1.DataSource = dtRoles;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";

            // Настраиваем DateTimePicker на текущую дату по умолчанию
            dateTimePicker1.Value = DateTime.Now;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string fullName = textBox1.Text.Trim();
            string login = textBox3.Text.Trim();
            string password = textBox2.Text.Trim();
            string email = textBox4.Text.Trim();
            string phone = textBox5.Text.Trim();
            int roleId = Convert.ToInt32(comboBox1.SelectedValue);

            // Получаем дату из DateTimePicker (формат для MySQL: yyyy-MM-dd)
            string hireDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            int isActive = checkBox1.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните обязательные поля (ФИО, Логин, Пароль)!");
                return;
            }

            // Подтверждение
            if (MessageBox.Show("Точно ли вы хотите добавить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // В SQL-запросе НЕ указываем created_at и updated_at, так как БД сделает это сама
            string query = $@"INSERT INTO users (login, password, full_name, email, phone, role_id, hire_date, is_active) 
                              VALUES ('{login}', '{password}', '{fullName}', '{email}', '{phone}', {roleId}, '{hireDate}', {isActive})";

            try
            {
                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Пользователь успешно добавлен!");
                this.DialogResult = DialogResult.OK; // Сообщаем главной форме об успехе
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}