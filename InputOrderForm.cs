using System;
using System.Data;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Electrical_Store
{
    public partial class InputOrderForm : Form
    {
        public InputOrderForm()
        {
            InitializeComponent();

            // Привязка кнопок (если в дизайнере они не привязаны)
            this.button2.Click += new EventHandler(button2_Click); // Добавить
            this.button1.Click += new EventHandler(button1_Click); // Выйти

            LoadComboBoxes();
            SetupDatePickers();

            // Автоматическая подстановка телефона и email при выборе клиента
            this.comboSup.SelectedIndexChanged += new EventHandler(comboSup_SelectedIndexChanged);
        }

        private void LoadComboBoxes()
        {
            try
            {
                // 1. Загружаем клиентов (для comboSup, comboNumber, comboEmail)
                // Берем пользователей с role_id = 3 (клиенты)
                string queryClients = "SELECT id, full_name, phone, email FROM users WHERE role_id = 3";
                DataTable dtClients = DbHelper.ExecuteQuery(queryClients);

                comboSup.DataSource = dtClients;
                comboSup.DisplayMember = "full_name";
                comboSup.ValueMember = "id";
                comboSup.DropDownStyle = ComboBoxStyle.DropDownList;

                comboNumber.DataSource = dtClients;
                comboNumber.DisplayMember = "phone";
                comboNumber.ValueMember = "id";
                comboNumber.DropDownStyle = ComboBoxStyle.DropDownList;

                comboEmail.DataSource = dtClients;
                comboEmail.DisplayMember = "email";
                comboEmail.ValueMember = "id";
                comboEmail.DropDownStyle = ComboBoxStyle.DropDownList;

                // 2. Загружаем продавцов (comboBuyer)
                // Берем админов, продавцов и менеджеров
                string querySellers = "SELECT id, full_name FROM users WHERE role_id IN (1, 4, 5)";
                DataTable dtSellers = DbHelper.ExecuteQuery(querySellers);

                comboBuyer.DataSource = dtSellers;
                comboBuyer.DisplayMember = "full_name";
                comboBuyer.ValueMember = "id";
                comboBuyer.DropDownStyle = ComboBoxStyle.DropDownList;

                // 3. Статус заказа (comboStOrder) - строго по ENUM из БД
                comboStOrder.Items.Clear();
                comboStOrder.Items.AddRange(new object[] { "new", "processing", "shipped", "delivered", "cancelled" });
                comboStOrder.SelectedIndex = 0;
                comboStOrder.DropDownStyle = ComboBoxStyle.DropDownList;

                // 4. Способ оплаты (comboCashM) - строго по ENUM из БД
                comboCashM.Items.Clear();
                comboCashM.Items.AddRange(new object[] { "cash", "card", "online", "bank_transfer" });
                comboCashM.SelectedIndex = 1; // По умолчанию card
                comboCashM.DropDownStyle = ComboBoxStyle.DropDownList;

                // 5. Скидка (comboDiscount)
                comboDiscount.Items.Clear();
                comboDiscount.Items.AddRange(new object[] { "0", "5", "10", "15", "20", "25", "50" });
                comboDiscount.SelectedIndex = 0;
                comboDiscount.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных из БД: " + ex.Message);
            }
        }

        private void SetupDatePickers()
        {
            // Дата создания - текущая
            dateTimePicker1.Value = DateTime.Now;

            // Дата обновления блокируется (БД ставит сама)
            try
            {
                dateTimePicker2.Value = DateTime.Now;
                dateTimePicker2.Enabled = false;
            }
            catch { }
        }

        // Синхронизация ФИО, телефона и email
        private void comboSup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboSup.SelectedIndex >= 0)
            {
                comboNumber.SelectedIndex = comboSup.SelectedIndex;
                comboEmail.SelectedIndex = comboSup.SelectedIndex;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Точно ли вы хотите добавить заказ?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // Сбор данных
            string orderNumber = maskedTextBox2.Text.Trim(); // Номер заказа
            string customerName = comboSup.Text;        // ФИО
            string phone = comboNumber.Text;            // Телефон
            string email = comboEmail.Text;             // Email

            if (comboBuyer.SelectedValue == null) { MessageBox.Show("Выберите продавца!"); return; }
            int sellerId = Convert.ToInt32(comboBuyer.SelectedValue);

            string status = comboStOrder.Text;

            decimal.TryParse(textBox3.Text, out decimal totalAmount); // Сумма заказа
            decimal.TryParse(comboDiscount.Text, out decimal discount); // Скидка

            string paymentMethod = comboCashM.Text;
            string createdAt = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");

            if (string.IsNullOrEmpty(orderNumber))
            {
                MessageBox.Show("Введите номер заказа!"); return;
            }

            string query = $@"INSERT INTO orders (order_number, customer_name, customer_phone, customer_email, seller_id, status, total_amount, discount_amount, payment_method, created_at) 
                              VALUES ('{orderNumber}', '{customerName}', '{phone}', '{email}', {sellerId}, '{status}', {totalAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {discount.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '{paymentMethod}', '{createdAt}')";

            try
            {
                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Заказ успешно добавлен!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка при добавлении: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}