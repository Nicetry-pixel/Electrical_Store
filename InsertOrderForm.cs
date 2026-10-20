using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Electrical_Store
{
    public partial class InsertOrderForm : Form
    {
        private int _orderId = 0;

        public InsertOrderForm(int orderId = 0)
        {
            InitializeComponent();
            _orderId = orderId;

            this.button2.Click += new EventHandler(button2_Click); // Изменить
            this.button1.Click += new EventHandler(button1_Click); // Выйти

            LoadComboBoxes();

            if (_orderId > 0)
            {
                this.Text = "Редактирование заказа";
                LoadOrderData();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                // 1. Клиенты
                string queryClients = "SELECT id, full_name, phone, email FROM users WHERE role_id = 3";
                DataTable dtClients = DbHelper.ExecuteQuery(queryClients);

                comboSup.DataSource = dtClients; comboSup.DisplayMember = "full_name"; comboSup.ValueMember = "id";
                comboNumber.DataSource = dtClients; comboNumber.DisplayMember = "phone"; comboNumber.ValueMember = "id";
                comboEmail.DataSource = dtClients; comboEmail.DisplayMember = "email"; comboEmail.ValueMember = "id";

                // 2. Продавцы
                string querySellers = "SELECT id, full_name FROM users WHERE role_id IN (1, 4, 5)";
                DataTable dtSellers = DbHelper.ExecuteQuery(querySellers);
                comboBuyer.DataSource = dtSellers; comboBuyer.DisplayMember = "full_name"; comboBuyer.ValueMember = "id";

                // 3. Статусы и оплата
                comboStOrder.Items.Clear(); comboStOrder.Items.AddRange(new object[] { "new", "processing", "shipped", "delivered", "cancelled" });
                comboCashM.Items.Clear(); comboCashM.Items.AddRange(new object[] { "cash", "card", "online", "bank_transfer" });

                // 4. Скидка
                comboDiscount.Items.Clear(); comboDiscount.Items.AddRange(new object[] { "0", "5", "10", "15", "20", "25", "50" });
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки данных: " + ex.Message); }
        }

        private void LoadOrderData()
        {
            DataTable dt = DbHelper.ExecuteQuery($"SELECT * FROM orders WHERE id = {_orderId}");
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

               maskedTextBox2.Text = row["order_number"].ToString(); // Номер заказа

                // Устанавливаем значения ComboBox. Если клиента нет в списке, ComboBox покажет текст.
                comboSup.Text = row["customer_name"].ToString();
                comboNumber.Text = row["customer_phone"].ToString();
                comboEmail.Text = row["customer_email"].ToString();

                if (row["seller_id"] != DBNull.Value)
                    comboBuyer.SelectedValue = row["seller_id"];

                comboStOrder.Text = row["status"].ToString();
                textBox3.Text = row["total_amount"].ToString(); // Сумма
                comboDiscount.Text = row["discount_amount"].ToString();
                comboCashM.Text = row["payment_method"].ToString();

                // Даты (только для чтения)
                if (row["created_at"] != DBNull.Value) dateTimePicker1.Value = Convert.ToDateTime(row["created_at"]);
                if (row["updated_at"] != DBNull.Value) dateTimePicker2.Value = Convert.ToDateTime(row["updated_at"]);

                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Точно ли вы хотите изменить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            string orderNumber = maskedTextBox2.Text.Trim();
            string customerName = comboSup.Text;
            string phone = comboNumber.Text;
            string email = comboEmail.Text;

            if (comboBuyer.SelectedValue == null) { MessageBox.Show("Выберите продавца!"); return; }
            int sellerId = Convert.ToInt32(comboBuyer.SelectedValue);

            string status = comboStOrder.Text;
            decimal.TryParse(textBox3.Text, out decimal totalAmount);
            decimal.TryParse(comboDiscount.Text, out decimal discount);
            string paymentMethod = comboCashM.Text;

            string query = $@"UPDATE orders SET 
                                order_number = '{orderNumber}', 
                                customer_name = '{customerName}', 
                                customer_phone = '{phone}', 
                                customer_email = '{email}', 
                                seller_id = {sellerId},
                                status = '{status}', 
                                total_amount = {totalAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                                discount_amount = {discount.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                                payment_method = '{paymentMethod}' 
                              WHERE id = {_orderId}";

            try
            {
                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Заказ успешно обновлен!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при обновлении: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}