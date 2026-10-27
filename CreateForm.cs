using System;
using System.Data;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class CreateForm : Form
    {
        private string _entityType;

        public CreateForm(string entityType)
        {
            InitializeComponent();
            _entityType = entityType;

            // Скрываем все панели по умолчанию
            panelUser.Visible = false;
            panelProduct.Visible = false;
            panelStock.Visible = false;
            panelOrder.Visible = false;

            // Показываем нужную панель и загружаем справочники
            if (_entityType == "User")
            {
                panelUser.Visible = true;
                this.Text = "Добавление пользователя";
                DataTable dtRoles = DbHelper.ExecuteQuery("SELECT id, name FROM roles");
                cmbUserRole.DataSource = dtRoles; cmbUserRole.DisplayMember = "name"; cmbUserRole.ValueMember = "id";
            }
            else if (_entityType == "Product")
            {
                panelProduct.Visible = true;
                this.Text = "Добавление товара";
                DataTable dtCat = DbHelper.ExecuteQuery("SELECT id, name FROM categories");
                cmbProdCategory.DataSource = dtCat; cmbProdCategory.DisplayMember = "name"; cmbProdCategory.ValueMember = "id";
            }
            else if (_entityType == "Stock")
            {
                panelStock.Visible = true;
                this.Text = "Добавление на склад";
                DataTable dtProd = DbHelper.ExecuteQuery("SELECT id, name FROM products");
                cmbStockProduct.DataSource = dtProd; cmbStockProduct.DisplayMember = "name"; cmbStockProduct.ValueMember = "id";
            }
            else if (_entityType == "Order")
            {
                panelOrder.Visible = true;
                this.Text = "Добавление заказа";
                cmbOrdStatus.Items.Clear(); cmbOrdStatus.Items.AddRange(new object[] { "new", "processing", "shipped", "delivered", "cancelled" }); cmbOrdStatus.SelectedIndex = 0;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_entityType == "User")
                {
                    string query = $"INSERT INTO users (login, password, full_name, email, phone, role_id) VALUES ('{txtUserLogin.Text}', '{txtUserPass.Text}', '{txtUserFIO.Text}', '{txtUserEmail.Text}', '{txtUserPhone.Text}', {cmbUserRole.SelectedValue})";
                    DbHelper.ExecuteNonQuery(query);
                }
                else if (_entityType == "Product")
                {
                    decimal.TryParse(txtProdPrice.Text, out decimal pPrice);
                    decimal.TryParse(txtProdRetail.Text, out decimal rPrice);
                    string query = $@"INSERT INTO products (sku, name, category_id, brand, purchase_price, retail_price) 
                                      VALUES ('{txtProdSKU.Text}', '{txtProdName.Text}', {cmbProdCategory.SelectedValue}, '{txtProdBrand.Text}', {pPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {rPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                    DbHelper.ExecuteNonQuery(query);
                }
                else if (_entityType == "Stock")
                {
                    int.TryParse(txtStockQty.Text, out int qty);
                    int.TryParse(txtStockMin.Text, out int min);
                    string query = $@"INSERT INTO stock (product_id, quantity, min_stock_level, location) 
                                      VALUES ({cmbStockProduct.SelectedValue}, {qty}, {min}, '{txtStockLoc.Text}')";
                    DbHelper.ExecuteNonQuery(query);
                }
                else if (_entityType == "Order")
                {
                    decimal.TryParse(txtOrdTotal.Text, out decimal total);
                    string query = $@"INSERT INTO orders (order_number, customer_name, customer_phone, status, total_amount) 
                                      VALUES ('{txtOrdNumber.Text}', '{txtOrdCust.Text}', '{txtOrdPhone.Text}', '{cmbOrdStatus.Text}', {total.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                    DbHelper.ExecuteNonQuery(query);
                }

                MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка БД: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnExit_Click(object sender, EventArgs e) { this.DialogResult = DialogResult.Cancel; this.Close(); }
    }
}