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
            LoadProducts();
        }

        private void LoadProducts()
        {
            DataTable dtCount = DbHelper.ExecuteQuery("SELECT COUNT(*) FROM products");
            totalRecords = Convert.ToInt32(dtCount.Rows[0][0]);
            label1.Text = $"Общее количество записей: {totalRecords}";

            int offset = (currentPage - 1) * pageSize;
            string query = $@"SELECT p.id, p.sku, p.name, c.name as category, p.brand, p.retail_price, p.warranty_months 
                              FROM products p 
                              LEFT JOIN categories c ON p.category_id = c.id 
                              LIMIT {pageSize} OFFSET {offset}";

            DataTable dt = DbHelper.ExecuteQuery(query);
            dataGridView1.DataSource = dt;

            if (dataGridView1.Columns["id"] != null) dataGridView1.Columns["id"].Visible = false;

            UpdatePageButtons();
        }

        private void UpdatePageButtons()
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            label1.Text = $"{currentPage} / {totalPages}";
            button2.Enabled = currentPage > 1;
            button3.Enabled = currentPage < totalPages;
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadProducts(); }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            if (currentPage < totalPages) { currentPage++; LoadProducts(); }
        }

        private void dataGridViewProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Пример отображения изображения. Предполагаем, что картинки лежат в папке проекта и называются по SKU (например, GEN-5KW-01.jpg)
                string sku = dataGridView1.SelectedRows[0].Cells["sku"].Value.ToString();
                string imagePath = $"{sku}.jpg";

                if (System.IO.File.Exists(imagePath))
                {
                    //pictureBox1.Image = Image.FromFile(imagePath);
                }
                else
                {
                    //pictureBoxProduct.Image = null; // Или системная заглушка
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            new InputProductForm().ShowDialog();
            LoadProducts();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
               // new InsertProductForm(id).ShowDialog();
                LoadProducts();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
