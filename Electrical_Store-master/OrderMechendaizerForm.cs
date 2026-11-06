using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class OrderMechendaizerForm : Form
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;
        private string searchTerm = "";

        public OrderMechendaizerForm()
        {
            InitializeComponent();

            // Настройка таблицы
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 9F, FontStyle.Regular);
            dataGridView1.RowTemplate.Height = 25;

            // Настройка ComboBox для сортировки (если он есть на форме)
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
                cmbSort.SelectedIndexChanged += (s, e) => { currentPage = 1; LoadOrders(); };
            }
            catch { }

            // --- НАСТРОЙКА ПОИСКА И АВТОДОПОЛНЕНИЯ ---
            SetupAutoComplete();

            // Привязка кнопки "Найти"
            if (btnSearch != null)
                btnSearch.Click += new EventHandler(btnSearch_Click);

            // Поиск по нажатию Enter в текстовом поле
            if (txtSearch != null)
            {
                txtSearch.KeyDown += (s, e) => {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.SuppressKeyPress = true; // Убираем звук "дзынь"
                        btnSearch_Click(s, e);
                    }
                };
            }

            LoadOrders();
        }

        // НОВЫЙ МЕТОД: Подгрузка номеров заказов для автодополнения
        private void SetupAutoComplete()
        {
            if (txtSearch == null) return;

            try
            {
                // Берем все существующие номера заказов из БД
                DataTable dt = DbHelper.ExecuteQuery("SELECT order_number FROM orders");

                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    collection.Add(row["order_number"].ToString());
                }

                // Привязываем подсказки к TextBox
                txtSearch.AutoCompleteCustomSource = collection;
                txtSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend; // Показывать список и дописывать
                txtSearch.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, просто не включаем автодополнение, чтобы не ломать форму
                Console.WriteLine("Ошибка автодополнения: " + ex.Message);
            }
        }

        // Обработчик нажатия на кнопку "Найти"
        private void btnSearch_Click(object sender, EventArgs e)
        {
            searchTerm = txtSearch.Text.Trim();
            currentPage = 1; // Сбрасываем на первую страницу
            LoadOrders();    // Перезагружаем данные
        }

        private void LoadOrders()
        {
            try
            {
                // 1. Поиск (по номеру заказа или клиенту)
                string whereClause = string.IsNullOrEmpty(searchTerm)
                    ? ""
                    : $"WHERE o.order_number LIKE '%{searchTerm}%' OR o.customer_name LIKE '%{searchTerm}%'";

                // 2. Сортировка
                string orderBy = "ORDER BY o.created_at DESC"; // По умолчанию (сначала новые)
                try
                {
                    switch (cmbSort.SelectedIndex)
                    {
                        case 1: orderBy = "ORDER BY o.created_at ASC"; break;
                        case 2: orderBy = "ORDER BY o.total_amount DESC"; break;
                        case 3: orderBy = "ORDER BY o.total_amount ASC"; break;
                    }
                }
                catch { }

                // 3. Подсчет записей
                DataTable dtCount = DbHelper.ExecuteQuery($"SELECT COUNT(*) FROM orders o {whereClause}");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                // 4. Загрузка данных
                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT o.id, o.order_number, o.customer_name, o.status, 
                                         o.total_amount, o.created_at
                                  FROM orders o 
                                  {whereClause}
                                  {orderBy}
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);

                if (dt != null)
                {
                    dataGridView1.DataSource = dt;

                    if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;
                    if (dataGridView1.Columns["order_number"] != null) dataGridView1.Columns["order_number"].HeaderText = "Номер заказа";
                    if (dataGridView1.Columns["customer_name"] != null) dataGridView1.Columns["customer_name"].HeaderText = "Клиент";
                    if (dataGridView1.Columns["status"] != null) dataGridView1.Columns["status"].HeaderText = "Статус";
                    if (dataGridView1.Columns["total_amount"] != null) dataGridView1.Columns["total_amount"].HeaderText = "Сумма";
                    if (dataGridView1.Columns["created_at"] != null) dataGridView1.Columns["created_at"].HeaderText = "Дата оформления";

                    // Маскировка ФИО клиента
                    dataGridView1.CellFormatting -= DataGridView1_CellFormatting;
                    dataGridView1.CellFormatting += DataGridView1_CellFormatting;
                }

                UpdatePageButtons();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.Value == null || dataGridView1.Columns.Count == 0) return;
            if (dataGridView1.Columns["customer_name"] != null && e.ColumnIndex == dataGridView1.Columns["customer_name"].Index)
                e.Value = Utils.MaskName(e.Value.ToString());
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            // Если у вас есть Label для вывода страниц (например label2), раскомментируйте:
            // label2.Text = $"{currentPage} / {totalPages}";

            try
            {
                button4.Enabled = currentPage > 1;           // Кнопка "<"
                button6.Enabled = currentPage < totalPages;  // Кнопка ">"
            }
            catch { }
        }

        // Кнопка "Назад" (<)
        private void button1_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadOrders(); }
        }

        // Кнопка "Вперед" (>)
        private void button2_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages) { currentPage++; LoadOrders(); }
        }

        // Кнопка "Выход"
        private void button3_Click(object sender, EventArgs e)
        {
            MechendaizerForm merch = new MechendaizerForm();
            merch.Show();
            this.Close();
        }
    }
}