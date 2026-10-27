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
        private string searchTerm = "";
        private bool _canEdit;

        public ProductForm(bool canEdit = true)
        {
            InitializeComponent();
            _canEdit = canEdit;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (!_canEdit)
            {
                if (button1 != null) button1.Visible = false; // Добавить
                if (button2 != null) button2.Visible = false; // Изменить
                dataGridView1.ContextMenuStrip = null;
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                string whereClause = string.IsNullOrEmpty(searchTerm) ? "" : $"WHERE p.name LIKE '%{searchTerm}%' OR p.sku LIKE '%{searchTerm}%'";
                DataTable dtCount = DbHelper.ExecuteQuery($"SELECT COUNT(*) FROM products p {whereClause}");

                if (dtCount != null && dtCount.Rows.Count > 0)
                    totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT p.id, p.sku, p.name, c.name as category, p.brand, p.retail_price, p.warranty_months 
                                  FROM products p 
                                  LEFT JOIN categories c ON p.category_id = c.id 
                                  {whereClause}
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);

                if (dt != null)
                {
                    dataGridView1.DataSource = dt;

                    // Безопасная настройка заголовков
                    if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;
                    if (dataGridView1.Columns["sku"] != null) dataGridView1.Columns["sku"].HeaderText = "Артикул";
                    if (dataGridView1.Columns["name"] != null) dataGridView1.Columns["name"].HeaderText = "Наименование";
                    if (dataGridView1.Columns["category"] != null) dataGridView1.Columns["category"].HeaderText = "Категория";
                    if (dataGridView1.Columns["brand"] != null) dataGridView1.Columns["brand"].HeaderText = "Бренд";
                    if (dataGridView1.Columns["retail_price"] != null) dataGridView1.Columns["retail_price"].HeaderText = "Цена";
                    if (dataGridView1.Columns["warranty_months"] != null) dataGridView1.Columns["warranty_months"].HeaderText = "Гарантия";
                }
                else
                {
                    dataGridView1.DataSource = null;
                }

                UpdatePageButtons();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            if (label1 != null) label1.Text = $"Всего записей: {totalRecords} | Стр. {currentPage} из {totalPages}";

            try
            {
                button4.Enabled = currentPage < totalPages; // Вперёд
                button6.Enabled = currentPage > 1;          // Назад
            }
            catch { }
        }

        // Обработчики кнопок
        private void button4_Click(object sender, EventArgs e) { int t = (int)Math.Ceiling((double)totalRecords / pageSize); if (currentPage < t) { currentPage++; LoadProducts(); } }
        private void button6_Click(object sender, EventArgs e) { if (currentPage > 1) { currentPage--; LoadProducts(); } }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (new CreateForm("Product").ShowDialog() == DialogResult.OK) { currentPage = 1; LoadProducts(); }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells["id"] != null)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertForm("Product", id).ShowDialog() == DialogResult.OK) LoadProducts();
            }
            else MessageBox.Show("Выберите товар.");
        }

        private void buttonExit_Click(object sender, EventArgs e) => this.Close();
        private void button5_Click(object sender, EventArgs e) => new BukketForm().ShowDialog();
        // Метод для Двойного клика по таблице (Удаление)
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Если у пользователя нет прав на редактирование (продавец), блокируем удаление
            if (!_canEdit) return;

            // Проверяем, что кликнули по строке с данными, а не по заголовку
            if (e.RowIndex >= 0)
            {
                // Проверка на существование ячейки (защита от NullReference)
                if (dataGridView1.Rows[e.RowIndex].Cells["id"].Value == null) return;

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
                        // Проверяем на ошибку внешнего ключа (когда товар уже в заказе или на складе)
                        if (ex.Message.Contains("foreign key") || ex.Message.Contains("Cannot delete"))
                        {
                            MessageBox.Show("Невозможно удалить товар!\n\nЭтот товар уже присутствует в оформленных заказах или на складе. Сначала удалите связанные записи.",
                                            "Запрещено", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        else
                        {
                            MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }

}