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

        public ProductForm()
        {
            InitializeComponent();

            // --- ЖЕСТКАЯ НАСТРОЙКА ТАБЛИЦЫ ---
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;

            // Настройка ширины колонок: AllCells подстраивает под текст и добавляет скролл, если не влезает
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.False; // Запрет переноса текста
            dataGridView1.RowTemplate.Height = 30;

            // --- ЖЕСТКАЯ НАСТРОЙКА КНОПОК ПАГИНАЦИИ ---
            // ВАЖНО: Если у вас в дизайнере кнопки называются по-другому (например, btnNext), 
            // замените button4 и button6 на ваши названия.
            button4.Text = "Вперёд >";
            button4.BackColor = Color.Black;
            button4.ForeColor = Color.White; // Белый текст
            button4.FlatStyle = FlatStyle.Flat;
            button4.Click += (s, e) => {
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (currentPage < totalPages) { currentPage++; LoadProducts(); }
            };

            button6.Text = "< Назад";
            button6.BackColor = Color.Black;
            button6.ForeColor = Color.White; // Белый текст
            button6.FlatStyle = FlatStyle.Flat;
            button6.Click += (s, e) => {
                if (currentPage > 1) { currentPage--; LoadProducts(); }
            };

            LoadProducts();
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

        // Кнопки "Добавить", "Изменить", "Выход", "Корзина" 
        // (Привязка через дизайнер, или через += как выше, если они не работают)
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (new InputProductForm().ShowDialog() == DialogResult.OK) { currentPage = 1; LoadProducts(); }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertProductForm(id).ShowDialog() == DialogResult.OK) LoadProducts();
            }
            else MessageBox.Show("Выберите товар.");
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            AdminForm admin = new AdminForm();
            admin.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e) => new BukketForm().ShowDialog();

        private void button4_Click(object sender, EventArgs e)
        {
            // Вычисляем общее количество страниц
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // Если мы не на последней странице, переключаем на следующую
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadProducts(); // Загружаем новые данные
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Если мы не на первой странице, переключаем на предыдущую
            if (currentPage > 1)
            {
                currentPage--;
                LoadProducts(); // Загружаем новые данные
            }
        }
    }
}