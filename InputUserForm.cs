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
            LoadRoles();
        }

        private void LoadRoles()
        {
            string query = "SELECT id, name FROM roles";
            DataTable dt = DbHelper.ExecuteQuery(query);
            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "id";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            string fullName = textBox1.Text.Trim();
            string login = textBox3.Text.Trim();
            string password = textBox2.Text.Trim();
            string email = textBox4.Text.Trim();
            string phone = textBox5.Text.Trim();
            int roleId = Convert.ToInt32(comboBox1.SelectedValue);
            string hireDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            int isActive = checkBox1.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните обязательные поля!");
                return;
            }

            string query = $@"INSERT INTO users (login, password, full_name, email, phone, role_id, hire_date, is_active) 
                              VALUES ('{login}', '{password}', '{fullName}', '{email}', '{phone}', {roleId}, '{hireDate}', {isActive})";

            try
            {
                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Пользователь успешно добавлен!");
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
