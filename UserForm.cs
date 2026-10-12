using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class UserForm : Form
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;

        public UserForm()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            // Получаем общее количество
            DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM users");
            totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);
            label1.Text = $"Общее количество записей: {totalRecords}";

            // Пагинация
            int offset = (currentPage - 1) * pageSize;
            string query = $@"SELECT u.id, u.login, u.full_name, u.email, u.phone, r.name as role_name, u.is_active 
                              FROM users u 
                              JOIN roles r ON u.role_id = r.id 
                              LIMIT {pageSize} OFFSET {offset}";

            DataTable dt = DbHelper.ExecuteQuery(query);
            dataGridView1.DataSource = dt;

            // Условное форматирование
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToInt32(row.Cells["is_active"].Value) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral; // Неактивные - красные
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }

            // Скрываем ненужные колонки
            if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;

            UpdatePageButtons();
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            label2.Text = $"{currentPage} / {totalPages}";
            button1.Enabled = currentPage > 1;
            button2.Enabled = currentPage < totalPages;
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadUsers(); }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages) { currentPage++; LoadUsers(); }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            new InputUserForm().ShowDialog();
            LoadUsers(); // Обновить после добавления
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                //new InputUserForm(id).ShowDialog();
                LoadUsers();
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
