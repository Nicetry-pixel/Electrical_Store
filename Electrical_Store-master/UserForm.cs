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

            // В конструкторе формы UserForm:
            dataGridView1.CellFormatting += (s, ev) => {
                if (ev.ColumnIndex == dataGridView1.Columns["full_name"].Index && ev.Value != null)
                    ev.Value = Utils.MaskName(ev.Value.ToString());
                if (ev.ColumnIndex == dataGridView1.Columns["phone"].Index && ev.Value != null)
                    ev.Value = Utils.MaskPhone(ev.Value.ToString());
            };

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

            try
            {
                cmbSort.Items.Clear();
                cmbSort.Items.AddRange(new object[] {
                    "Сначала новые",
                    "Сначала старые",
                    "Сумма (по убыванию)",
                    "Сумма (по возрастанию)"
                });
                cmbSort.SelectedIndex = 0;
                cmbSort.SelectedIndexChanged += (s, e) => { currentPage = 1; LoadUsers(); };
            }
            catch { }

            LoadUsers();
        }


        private string searchTerm = "";


        private void LoadUsers()
        {
            try
            {
                // 1. Поиск (по ФИО или логину)
                string whereClause = string.IsNullOrEmpty(searchTerm)
                    ? ""
                    : $"WHERE u.full_name LIKE '%{searchTerm}%' OR u.login LIKE '%{searchTerm}%'";

                // 2. Сортировка
                string orderBy = "ORDER BY u.full_name ASC";
                try
                {
                    if (cmbSort.SelectedIndex == 1) orderBy = "ORDER BY u.full_name DESC";
                    if (cmbSort.SelectedIndex == 2) orderBy = "ORDER BY u.is_active DESC";
                }
                catch { }

                // 3. Подсчет
                DataTable dtCount = DbHelper.ExecuteQuery($"SELECT COUNT(*) FROM users u {whereClause}");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                // 4. Загрузка
                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT u.id, u.login, u.full_name, u.email, u.phone, r.name as role_name, u.is_active 
                          FROM users u 
                          JOIN roles r ON u.role_id = r.id 
                          {whereClause}
                          {orderBy}
                          LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);
                dataGridView1.DataSource = dt;

                // Безопасная настройка заголовков
                if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;
                if (dataGridView1.Columns["login"] != null) dataGridView1.Columns["login"].HeaderText = "Логин";
                if (dataGridView1.Columns["full_name"] != null) dataGridView1.Columns["full_name"].HeaderText = "ФИО";
                if (dataGridView1.Columns["email"] != null) dataGridView1.Columns["email"].HeaderText = "Email";
                if (dataGridView1.Columns["phone"] != null) dataGridView1.Columns["phone"].HeaderText = "Телефон";
                if (dataGridView1.Columns["role_name"] != null) dataGridView1.Columns["role_name"].HeaderText = "Роль";
                if (dataGridView1.Columns["is_active"] != null) dataGridView1.Columns["is_active"].HeaderText = "Активен";

                // Форматирование (цвет + маскировка)
                dataGridView1.CellFormatting -= DataGridView1_CellFormatting;
                dataGridView1.CellFormatting += DataGridView1_CellFormatting;

                UpdatePageButtons();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.Value == null || dataGridView1.Columns.Count == 0) return;

            // Маскировка ФИО и Телефона
            if (dataGridView1.Columns["full_name"] != null && e.ColumnIndex == dataGridView1.Columns["full_name"].Index)
                e.Value = Utils.MaskName(e.Value.ToString());
            if (dataGridView1.Columns["phone"] != null && e.ColumnIndex == dataGridView1.Columns["phone"].Index)
                e.Value = Utils.MaskPhone(e.Value.ToString());

            // Подсветка неактивных
            if (dataGridView1.Columns["is_active"] != null && e.ColumnIndex == dataGridView1.Columns["is_active"].Index)
            {
                if (e.Value.ToString() == "0" || e.Value.ToString() == "False")
                    e.CellStyle.BackColor = Color.LightCoral;
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
            // Передаем тип "User"
            if (new CreateForm("User").ShowDialog() == DialogResult.OK)
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
                // Передаем тип "User" и ID
                if (new InsertForm("User", id).ShowDialog() == DialogResult.OK)
                {
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Выберите пользователя для редактирования.");
            }
        }

      

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что кликнули по строке с данными, а не по заголовку
            if (e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string fullName = dataGridView1.Rows[e.RowIndex].Cells["full_name"].Value.ToString();

                if (MessageBox.Show($"Вы точно хотите удалить пользователя:\n{fullName}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DbHelper.ExecuteNonQuery($"DELETE FROM users WHERE id = {id}");
                        MessageBox.Show("Пользователь удален!");
                        LoadUsers(); // Обновляем таблицу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно удалить пользователя (возможно, у него есть заказы).\nОшибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchTerm = txtSearch.Text.Trim();
            currentPage = 1; // Сбрасываем на первую страницу при поиске
            LoadUsers();
        }
    }
}