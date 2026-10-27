using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class BukketForm : Form
    {
        // Класс для хранения позиции корзины
        public class CartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public decimal Total => Quantity * Price;
        }

        private List<CartItem> cart = new List<CartItem>();

        public BukketForm()
        {
            InitializeComponent();
            LoadProductsToCombo();
            UpdateGrid();
        }

        private void LoadProductsToCombo()
        {
            string query = "SELECT id, name, retail_price FROM products";
            DataTable dt = DbHelper.ExecuteQuery(query);
            cmbProducts.DataSource = dt;
            cmbProducts.DisplayMember = "name";
            cmbProducts.ValueMember = "id";
        }

        private void buttonAddToCart_Click(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null) return;

            DataRowView row = (DataRowView)cmbProducts.SelectedItem;
            int id = Convert.ToInt32(row["id"]);
            string name = row["name"].ToString();
            decimal price = Convert.ToDecimal(row["retail_price"]);
            int qty = (int)numQuantity.Value;

            var existing = cart.Find(c => c.ProductId == id);
            if (existing != null)
            {
                existing.Quantity += qty;
            }
            else
            {
                cart.Add(new CartItem { ProductId = id, ProductName = name, Quantity = qty, Price = price });
            }

            UpdateGrid();
        }

        private void UpdateGrid()
        {
            dgvCart.DataSource = null;
            dgvCart.DataSource = cart;

            decimal total = 0;
            foreach (var item in cart) total += item.Total;
            label1.Text = $"Итого: {total:F2} руб.";
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                CartItem item = (CartItem)dgvCart.SelectedRows[0].DataBoundItem;
                cart.Remove(item);
                UpdateGrid();
            }
        }

        private void buttonCheckout_Click(object sender, EventArgs e)
        {
            if (cart.Count == 0) { MessageBox.Show("Корзина пуста!"); return; }

            if (MessageBox.Show("Оформить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                // 1. Генерируем уникальный номер заказа
                string orderNumber = "ORD-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                decimal total = 0;
                foreach (var item in cart) total += item.Total;

                // 2. Создаем сам заказ (привязываем к текущему пользователю, если нужно, или ставим заглушку)
                // Предположим, что Session.CurrentUserId хранит ID того, кто оформляет (если вы это добавили)
                int sellerId = Session.CurrentUserRoleId == 4 ? 2 : 1; // Пример: если продавец, то ID 2, иначе 1

                string queryOrder = $@"INSERT INTO orders (order_number, customer_name, seller_id, status, total_amount) 
                               VALUES ('{orderNumber}', 'Розничный клиент', {sellerId}, 'new', {total.ToString(System.Globalization.CultureInfo.InvariantCulture)})";

                DbHelper.ExecuteNonQuery(queryOrder);

                // 3. Получаем ID только что созданного заказа
                DataTable dtId = DbHelper.ExecuteQuery("SELECT LAST_INSERT_ID()");
                int newOrderId = Convert.ToInt32(dtId.Rows[0][0]);

                // 4. Сохраняем позиции заказа
                foreach (var item in cart)
                {
                    string queryItem = $@"INSERT INTO order_items (order_id, product_id, quantity, unit_price, total_price) 
                                  VALUES ({newOrderId}, {item.ProductId}, {item.Quantity}, 
                                          {item.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                                          {item.Total.ToString(System.Globalization.CultureInfo.InvariantCulture)})";
                    DbHelper.ExecuteNonQuery(queryItem);
                }

                MessageBox.Show($"Заказ №{orderNumber} успешно оформлен!");

                // Очищаем корзину
                cart.Clear();
                UpdateGrid();
                this.Close(); // Закрываем корзину, возвращаемся к товарам
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
