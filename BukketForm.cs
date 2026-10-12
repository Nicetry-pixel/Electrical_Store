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
            if (cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста!");
                return;
            }

            // Здесь должна быть логика сохранения заказа в БД (таблицы orders и order_items)
            // Для примера просто откроем чек
            decimal total = 0;
            foreach (var item in cart) total += item.Total;

            CheakForm checkForm = new CheakForm("Козлов Кирилл Дмитриевич", new Random().Next(1000, 9999).ToString(), cart, total);
            checkForm.ShowDialog();

            cart.Clear();
            UpdateGrid();
        }
    }
}
