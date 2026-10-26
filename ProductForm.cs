using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class ProductForm : Form
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;

        private int _roleId;

        // Объявляем переменную на уровне всего класса
        private bool _canEdit;

        // Конструктор с параметром (по умолчанию true)
        public ProductForm(bool canEdit = true, int roleId = 1)
        {
            InitializeComponent();

            // Присваиваем значение
            _canEdit = canEdit;
            _roleId = roleId;

            // --- НАСТРОЙКА DataGridView ---
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 30;

            // --- ПРОВЕРКА ПРАВ (Скрываем кнопки для продавца) ---
            if (!_canEdit)
            {
                button1.Visible = false; // Кнопка "Добавить"
                button2.Visible = false; // Кнопка "Изменить"
                                         // Если кнопки называются иначе (например, buttonAdd, buttonEdit), замените их имена здесь

                dataGridView1.ContextMenuStrip = null; // Отключаем меню ПКМ (удаление)
            }

            // Настройка кнопок пагинации (напоминаю, проверьте их имена в дизайнере)
            try
            {
                button4.Text = "Вперёд >"; button4.ForeColor = Color.White; button4.BackColor = Color.Black;
                button6.Text = "< Назад"; button6.ForeColor = Color.White; button6.BackColor = Color.Black;
            }
            catch { }

            LoadProducts();
            _roleId = roleId;
        }

        private void LoadProducts()
        {
            try
            {
                DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM products");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT p.id, p.sku, p.name, c.name as category, p.brand, p.retail_price, p.warranty_months 
                                  FROM products p 
                                  LEFT JOIN categories c ON p.category_id = c.id 
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);
                dataGridView1.DataSource = dt;

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["sku"].HeaderText = "Артикул";
                    dataGridView1.Columns["name"].HeaderText = "Наименование";
                    dataGridView1.Columns["category"].HeaderText = "Категория";
                    dataGridView1.Columns["brand"].HeaderText = "Бренд";
                    dataGridView1.Columns["retail_price"].HeaderText = "Цена";
                    dataGridView1.Columns["warranty_months"].HeaderText = "Гарантия";
                }

                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (totalPages == 0) totalPages = 1;
                label1.Text = $"Всего записей: {totalRecords} | Стр. {currentPage} из {totalPages}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        // События пагинации
        private void button4_Click(object sender, EventArgs e) // Вперёд
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages) { currentPage++; LoadProducts(); }
        }

        private void button6_Click(object sender, EventArgs e) // Назад
        {
            if (currentPage > 1) { currentPage--; LoadProducts(); }
        }

        // Событие кнопки Добавить
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Проверка на всякий случай (хотя кнопка скрыта для продавца)
            if (!_canEdit) return;

            if (new InputProductForm().ShowDialog() == DialogResult.OK)
            {
                currentPage = 1;
                LoadProducts();
            }
        }

        // Событие кнопки Изменить
        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (!_canEdit) return;

            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertProductForm(id).ShowDialog() == DialogResult.OK) LoadProducts();
            }
            else MessageBox.Show("Выберите товар.");
        }

        // Кнопка Выход (оставляем только ОДИН этот метод, чтобы не было ошибки CS0111)
        private void buttonExit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Текущая роль в сессии: " + Session.CurrentUserRoleId.ToString(), "Отладка");
            // -------------------

            if (Session.CurrentUserRoleId == 1)
            {
                AdminForm admin = new AdminForm();
                admin.Show();
            }
            else if (Session.CurrentUserRoleId == 2)
            {
                MechendaizerForm merch = new MechendaizerForm();
                merch.Show();
            }
            else if (Session.CurrentUserRoleId == 4)
            {
                BuyerForm buyer = new BuyerForm();
                buyer.Show();
            }

            this.Close();
        }

        // Кнопка Корзина
        private void button5_Click(object sender, EventArgs e)
        {
            new BukketForm().ShowDialog();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Если у пользователя нет прав на редактирование (продавец), блокируем удаление
            if (!_canEdit) return;

            if (e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string productName = dataGridView1.Rows[e.RowIndex].Cells["name"].Value.ToString();

                if (MessageBox.Show($"Вы точно хотите удалить товар:\n{productName}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DbHelper.ExecuteNonQuery($"DELETE FROM products WHERE id = {id}");
                        MessageBox.Show("Товар удален!");
                        LoadProducts(); // Обновляем таблицу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Невозможно удалить товар (он есть на складе или в заказах).\nОшибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}