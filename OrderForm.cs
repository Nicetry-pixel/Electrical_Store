using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class OrderForm : Form
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;
        private string searchTerm = "";

        public OrderForm()
        {
            InitializeComponent();

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Привязываем событие маскировки ОДИН РАЗ при создании формы
            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(DataGridView1_CellFormatting);

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                string whereClause = string.IsNullOrEmpty(searchTerm) ? "" : $"WHERE o.order_number LIKE '%{searchTerm}%' OR o.customer_name LIKE '%{searchTerm}%'";
                DataTable dtCount = DbHelper.ExecuteQuery($"SELECT COUNT(*) FROM orders o {whereClause}");

                if (dtCount != null && dtCount.Rows.Count > 0)
                    totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT o.id, o.order_number, o.customer_name, o.customer_phone, 
                                         o.status, o.total_amount, u.full_name as seller_name 
                                  FROM orders o 
                                  LEFT JOIN users u ON o.seller_id = u.id 
                                  {whereClause}
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);

                if (dt != null)
                {
                    dataGridView1.DataSource = dt;

                    // Безопасная настройка заголовков
                    if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;
                    if (dataGridView1.Columns["order_number"] != null) dataGridView1.Columns["order_number"].HeaderText = "Номер заказа";
                    if (dataGridView1.Columns["customer_name"] != null) dataGridView1.Columns["customer_name"].HeaderText = "Клиент";
                    if (dataGridView1.Columns["customer_phone"] != null) dataGridView1.Columns["customer_phone"].HeaderText = "Телефон";
                    if (dataGridView1.Columns["status"] != null) dataGridView1.Columns["status"].HeaderText = "Статус";
                    if (dataGridView1.Columns["total_amount"] != null) dataGridView1.Columns["total_amount"].HeaderText = "Сумма";
                    if (dataGridView1.Columns["seller_name"] != null) dataGridView1.Columns["seller_name"].HeaderText = "Продавец";
                }
                else
                {
                    dataGridView1.DataSource = null;
                }

                UpdatePageButtons();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        // Безопасная маскировка
        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.Value == null || dataGridView1.Columns.Count == 0) return;

            if (dataGridView1.Columns["customer_name"] != null && e.ColumnIndex == dataGridView1.Columns["customer_name"].Index)
                e.Value = Utils.MaskName(e.Value.ToString());

            if (dataGridView1.Columns["customer_phone"] != null && e.ColumnIndex == dataGridView1.Columns["customer_phone"].Index)
                e.Value = Utils.MaskPhone(e.Value.ToString());
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            if (label1 != null) label1.Text = $"Всего записей: {totalRecords} | Стр. {currentPage} из {totalPages}";

            try
            {
                button2.Enabled = currentPage < totalPages; // Вперёд
                button3.Enabled = currentPage > 1;          // Назад
            }
            catch { }
        }

        private void button4_Click(object sender, EventArgs e) { int t = (int)Math.Ceiling((double)totalRecords / pageSize); if (currentPage < t) { currentPage++; LoadOrders(); } }
        private void button6_Click(object sender, EventArgs e) { if (currentPage > 1) { currentPage--; LoadOrders(); } }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (new CreateForm("Order").ShowDialog() == DialogResult.OK) { currentPage = 1; LoadOrders(); }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0 && dataGridView1.SelectedRows[0].Cells["id"] != null)
            {
                string status = dataGridView1.SelectedRows[0].Cells["status"].Value.ToString();
                if (status == "delivered" || status == "cancelled")
                {
                    MessageBox.Show("Этот заказ завершен или отменен. Редактирование запрещено!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertForm("Order", id).ShowDialog() == DialogResult.OK) LoadOrders();
            }
            else MessageBox.Show("Выберите заказ.");
        }

        private void button3_Click(object sender, EventArgs e) // Кнопка Выход
        {
            if (Session.CurrentUserRoleId == 1 || Session.CurrentUserRoleId == 5) { new AdminForm().Show(); }
            else if (Session.CurrentUserRoleId == 2) { new MechendaizerForm().Show(); }
            else { new BuyerForm().Show(); }
            this.Close();
        }

        // 1. Метод, которого не хватает для события Load (Загрузка формы)
        private void OrderForm_Load(object sender, EventArgs e)
        {
            // Если вы уже вызываете LoadOrders() в конструкторе, здесь можно оставить пусто.
            // Или продублировать вызов, если данные не грузятся.
        }

        // 2. Метод для Двойного клика по таблице (Удаление)
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что кликнули по строке с данными, а не по заголовку
            if (e.RowIndex >= 0)
            {
                // Получаем ID и номер заказа
                if (dataGridView1.Rows[e.RowIndex].Cells["id"].Value == null) return;

                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string orderNumber = dataGridView1.Rows[e.RowIndex].Cells["order_number"].Value.ToString();

                // Запрещаем удалять завершенные заказы (пункт 7 из вашего ТЗ)
                string status = dataGridView1.Rows[e.RowIndex].Cells["status"].Value.ToString();
                if (status == "delivered" || status == "cancelled")
                {
                    MessageBox.Show("Этот заказ завершен или отменен. Удаление запрещено!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Подтверждение
                if (MessageBox.Show($"Вы точно хотите удалить заказ №{orderNumber}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DbHelper.ExecuteNonQuery($"DELETE FROM orders WHERE id = {id}");
                        MessageBox.Show("Заказ удален!");
                        LoadOrders(); // Обновляем таблицу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}