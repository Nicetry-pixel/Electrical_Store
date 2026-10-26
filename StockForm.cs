using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class StockForm : Form
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords = 0;

        // Конструктор (убрал лишние привязки событий)
        public StockForm()
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

            // Настройка внешнего вида кнопок пагинации
            try
            {
                buttonPrev.Text = "<";
                buttonPrev.BackColor = Color.Black;
                buttonPrev.ForeColor = Color.White;
                buttonPrev.FlatStyle = FlatStyle.Flat;

                buttonNext.Text = ">";
                buttonNext.BackColor = Color.Black;
                buttonNext.ForeColor = Color.White;
                buttonNext.FlatStyle = FlatStyle.Flat;
            }
            catch { }

            LoadStock();
        }

        private void LoadStock()
        {
            try
            {
                DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM stock");
                totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);

                int offset = (currentPage - 1) * pageSize;
                string query = $@"SELECT s.id, p.name as product_name, s.quantity, 
                                         s.reserved_quantity, s.min_stock_level, s.location 
                                  FROM stock s 
                                  JOIN products p ON s.product_id = p.id 
                                  LIMIT {pageSize} OFFSET {offset}";

                DataTable dt = DbHelper.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                    FormatGrid();
                }
                else
                {
                    dataGridView1.DataSource = null;
                }

                UpdatePageButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки склада: " + ex.Message, "Ошибка SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dataGridView1.Columns.Count == 0) return;

            if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;
            if (dataGridView1.Columns["product_name"] != null) dataGridView1.Columns["product_name"].HeaderText = "Товар";
            if (dataGridView1.Columns["quantity"] != null) dataGridView1.Columns["quantity"].HeaderText = "Количество";
            if (dataGridView1.Columns["reserved_quantity"] != null) dataGridView1.Columns["reserved_quantity"].HeaderText = "В резерве";
            if (dataGridView1.Columns["min_stock_level"] != null) dataGridView1.Columns["min_stock_level"].HeaderText = "Мин. остаток";
            if (dataGridView1.Columns["location"] != null) dataGridView1.Columns["location"].HeaderText = "Место (ячейка)";
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (totalPages == 0) totalPages = 1;

            label1.Text = $"Общее количество записей: {totalRecords}";
            label2.Text = $"{currentPage} / {totalPages}";

            buttonPrev.Enabled = currentPage > 1;
            buttonNext.Enabled = currentPage < totalPages;

            buttonPrev.BackColor = buttonPrev.Enabled ? Color.Black : Color.DimGray;
            buttonNext.BackColor = buttonNext.Enabled ? Color.Black : Color.DimGray;
        }

        // События пагинации (они должны быть привязаны в дизайнере!)
        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                LoadStock();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadStock();
            }
        }

        // Кнопка Выход
        private void buttonExit_Click(object sender, EventArgs e)
        {
            // Блокируем кнопку, чтобы предотвратить случайный двойной клик
            buttonExit.Enabled = false;

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

            this.Hide();
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string productName = dataGridView1.Rows[e.RowIndex].Cells["product_name"].Value.ToString();

                if (MessageBox.Show($"Вы точно хотите удалить запись склада для товара:\n{productName}?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        DbHelper.ExecuteNonQuery($"DELETE FROM stock WHERE id = {id}");
                        MessageBox.Show("Запись удалена!");
                        LoadStock(); // Обновляем таблицу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}