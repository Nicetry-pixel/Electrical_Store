using System;
using System.Data;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class InputProductForm : Form
    {
        public InputProductForm()
        {
            InitializeComponent();
            LoadComboBoxes();

            // Привязываем события напрямую в коде (на случай, если в дизайнере забыли)
            // Если у вас кнопка называется не button2, замените это имя здесь:
            this.button2.Click += new EventHandler(button2_Click);
            this.button1.Click += new EventHandler(button1_Click);
        }

        private void LoadComboBoxes()
        {
            try
            {
                DataTable dtCat = DbHelper.ExecuteQuery("SELECT id, name FROM categories");
                comboBox1.DataSource = dtCat;
                comboBox1.DisplayMember = "name";
                comboBox1.ValueMember = "id";

                DataTable dtUsers = DbHelper.ExecuteQuery("SELECT id, full_name FROM users");
                comboBox6.DataSource = dtUsers;
                comboBox6.DisplayMember = "full_name";
                comboBox6.ValueMember = "id";

                comboBox5.Items.Clear(); comboBox5.Items.AddRange(new object[] { "Да", "Нет" }); comboBox5.SelectedIndex = 1;
                comboBox7.Items.Clear(); comboBox7.Items.AddRange(new object[] { "0", "6", "12", "24", "36", "60" }); comboBox7.SelectedIndex = 2;
                comboBox2.Items.Clear(); comboBox2.Items.AddRange(new object[] { "Huter", "Ресанта", "Кабель-М", "Philips", "Osram", "IEK", "EKF" });
                comboBox3.Items.Clear(); comboBox3.Items.AddRange(new object[] { "220V", "380V", "12V", "24V" });

                comboBox4.Items.Clear();
                comboBox4.Items.AddRange(new object[] { "0", "5", "10", "16", "25", "50", "100", "500", "1000", "2000", "5000" });

                comboBox8.DropDownStyle = ComboBoxStyle.DropDown;
                comboBox8.Items.Clear();
                comboBox8.Items.AddRange(new object[] { "Генератор", "Стабилизатор", "Кабель", "Лампа" });
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(comboBox8.Text) || comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Заполните обязательные поля (Артикул, Наименование, Категория)!"); return;
            }

            if (MessageBox.Show("Точно ли вы хотите добавить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                string sku = textBox1.Text.Trim();
                string name = comboBox8.Text.Trim();
                int catId = Convert.ToInt32(comboBox1.SelectedValue);
                string brand = comboBox2.Text.Trim();
                string desc = textBox4.Text.Trim();

                decimal.TryParse(textBox6.Text, out decimal buyPrice);
                decimal.TryParse(textBox5.Text, out decimal sellPrice);
                int.TryParse(comboBox7.Text, out int warranty);
                string voltage = comboBox3.Text;
                int.TryParse(comboBox4.Text, out int power);
                int isCert = comboBox5.Text == "Да" ? 1 : 0;
                int creator = comboBox6.SelectedValue != null ? Convert.ToInt32(comboBox6.SelectedValue) : 1;

                string query = $@"INSERT INTO products (sku, name, category_id, brand, description, purchase_price, retail_price, warranty_months, voltage, power_watt, is_certified, created_by) 
                                  VALUES ('{sku}', '{name}', {catId}, '{brand}', '{desc}', {buyPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {sellPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {warranty}, '{voltage}', {power}, {isCert}, {creator})";

                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Товар добавлен!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка БД: " + ex.Message); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}