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

        public OrderForm()
        {
            InitializeComponent();

            // --- НАСТРОЙКА DataGridView ---
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 30;

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM orders");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT o.id, o.order_number, o.customer_name, o.customer_phone, 
                                         o.status, o.total_amount, u.full_name as seller_name 
                                  FROM orders o 
                                  LEFT JOIN users u ON o.seller_id = u.id 
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);
                dataGridView1.DataSource = dt;

                if (dataGridView1.Columns.Count > 0)
                {
                    dataGridView1.Columns["id"].Visible = false;
                    dataGridView1.Columns["order_number"].HeaderText = "Номер заказа";
                    dataGridView1.Columns["customer_name"].HeaderText = "Клиент";
                    dataGridView1.Columns["customer_phone"].HeaderText = "Телефон";
                    dataGridView1.Columns["status"].HeaderText = "Статус";
                    dataGridView1.Columns["total_amount"].HeaderText = "Сумма";
                    dataGridView1.Columns["seller_name"].HeaderText = "Продавец";
                }

                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (totalPages == 0) totalPages = 1;
                label1.Text = $"Всего записей: {totalRecords} | Стр. {currentPage} из {totalPages}";
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        // Логика пагинации (если вы привязали события в дизайнере)
        private void button4_Click(object sender, EventArgs e) // Вперёд
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages) { currentPage++; LoadOrders(); }
        }

        private void button6_Click(object sender, EventArgs e) // Назад
        {
            if (currentPage > 1) { currentPage--; LoadOrders(); }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (new InputOrderForm().ShowDialog() == DialogResult.OK)
            {
                currentPage = 1;
                LoadOrders();
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                if (new InsertOrderForm(id).ShowDialog() == DialogResult.OK)
                {
                    LoadOrders();
                }
            }
            else MessageBox.Show("Выберите заказ для редактирования.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void button3_Click(object sender, EventArgs e) // Кнопка Выход
        {
            AdminForm admin = new AdminForm();
            admin.Show();
            this.Close();
        }
    }
}