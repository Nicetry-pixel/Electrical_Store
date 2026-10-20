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

            // --- НАСТРОЙКА DataGridView ---
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            // Растягивание колонок на всю ширину
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Фиксируем шрифт (чтобы он не плющился при растягивании формы)
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 30; // Высота строк

            // --- ИСПРАВЛЕНИЕ КНОПКИ "ДОБАВИТЬ" ---
            button1.Text = "Добавить";
            button1.BackColor = Color.Black;
            button1.ForeColor = Color.WhiteSmoke;

            // Скрываем второй лейбл, который создает кашу с текстом
            label2.Visible = false;

            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM users");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT u.id, u.login, u.full_name, u.email, u.phone, r.name as role_name, u.is_active 
                                  FROM users u 
                                  JOIN roles r ON u.role_id = r.id 
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);
                dataGridView1.DataSource = dt;

                // Форматирование (цвет неактивных)
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["is_active"].Value != DBNull.Value && Convert.ToInt32(row.Cells["is_active"].Value) == 0)
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                    else
                        row.DefaultCellStyle.BackColor = Color.White;
                }

                // Русские заголовки
                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["login"].HeaderText = "Логин";
                    dataGridView1.Columns["full_name"].HeaderText = "ФИО";
                    dataGridView1.Columns["email"].HeaderText = "Email";
                    dataGridView1.Columns["phone"].HeaderText = "Телефон";
                    dataGridView1.Columns["role_name"].HeaderText = "Роль";
                    dataGridView1.Columns["is_active"].HeaderText = "Активен";
                }

                UpdatePageButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке: " + ex.Message);
            }
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            label1.Text = $"Всего записей: {totalRecords} | Стр. {currentPage} из {totalPages}";
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            AdminForm admin = new AdminForm();
            admin.Show();
            this.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (new InputUserForm().ShowDialog() == DialogResult.OK)
            {
                currentPage = 1;
                LoadUsers();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertUserForm(id).ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}