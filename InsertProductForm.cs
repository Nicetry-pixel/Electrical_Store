using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class InsertProductForm : Form
    {
        private int _productId = 0;

        public InsertProductForm(int productId = 0)
        {
            InitializeComponent();
            _productId = productId;

            // --- ПРИНУДИТЕЛЬНАЯ ПРИВЯЗКА КНОПОК ---
            // Если кнопки в дизайнере называются не button1 и button2, 
            // замените их имена здесь, иначе они так и не будут работать!
            this.button2.Click += new EventHandler(button2_Click);
            this.button1.Click += new EventHandler(button1_Click);

            LoadComboBoxes();

            if (_productId > 0)
            {
                this.Text = "Редактирование товара";
                button2.Text = "Изменить";
                LoadProductData();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                DataTable dtCat = DbHelper.ExecuteQuery("SELECT id, name FROM categories");
                comboBox1.DataSource = dtCat; comboBox1.DisplayMember = "name"; comboBox1.ValueMember = "id";

                DataTable dtUsers = DbHelper.ExecuteQuery("SELECT id, full_name FROM users");
                comboBox6.DataSource = dtUsers; comboBox6.DisplayMember = "full_name"; comboBox6.ValueMember = "id";

                comboBox5.Items.Clear(); comboBox5.Items.AddRange(new object[] { "Да", "Нет" });
                comboBox7.Items.Clear(); comboBox7.Items.AddRange(new object[] { "0", "6", "12", "24", "36", "60" });
                comboBox2.Items.Clear(); comboBox2.Items.AddRange(new object[] { "Huter", "Ресанта", "Кабель-М", "Philips", "Osram", "IEK", "EKF" });
                comboBox3.Items.Clear(); comboBox3.Items.AddRange(new object[] { "220V", "380V", "12V", "24V" });
                comboBox4.Items.Clear(); comboBox4.Items.AddRange(new object[] { "0", "5", "10", "16", "25", "50", "100", "500", "1000", "2000", "5000" });
                comboBox8.DropDownStyle = ComboBoxStyle.DropDown;
            }
            catch (Exception ex) { MessageBox.Show("Ошибка загрузки: " + ex.Message); }
        }

        private void LoadProductData()
        {
            DataTable dt = DbHelper.ExecuteQuery($"SELECT * FROM products WHERE id = {_productId}");
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                textBox1.Text = row["sku"].ToString();
                comboBox8.Text = row["name"].ToString();
                comboBox1.SelectedValue = row["category_id"];
                comboBox2.Text = row["brand"].ToString();
                textBox4.Text = row["description"].ToString();
                textBox6.Text = row["purchase_price"].ToString();
                textBox5.Text = row["retail_price"].ToString();
                comboBox7.Text = row["warranty_months"].ToString();
                comboBox3.Text = row["voltage"].ToString();
                comboBox4.Text = row["power_watt"].ToString();
                comboBox5.Text = Convert.ToInt32(row["is_certified"]) == 1 ? "Да" : "Нет";

                if (row["created_by"] != DBNull.Value) comboBox6.SelectedValue = row["created_by"];

                // --- ЖЕСТКОЕ ОТКЛЮЧЕНИЕ DATETIMEPICKER ---
                // Они становятся серыми, и пользователь не может их изменить (так и должно быть)
                try
                {
                    if (row["created_at"] != DBNull.Value) dateTimePicker1.Value = Convert.ToDateTime(row["created_at"]);
                    if (row["updated_at"] != DBNull.Value) dateTimePicker2.Value = Convert.ToDateTime(row["updated_at"]);

                    dateTimePicker1.Enabled = false;
                    dateTimePicker2.Enabled = false;
                }
                catch { }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Точно ли вы хотите изменить запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

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
                int creator = Convert.ToInt32(comboBox6.SelectedValue);

                string query = $@"UPDATE products SET 
                                    sku = '{sku}', name = '{name}', category_id = {catId}, 
                                    brand = '{brand}', description = '{desc}', 
                                    purchase_price = {buyPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, retail_price = {sellPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, 
                                    warranty_months = {warranty}, voltage = '{voltage}', 
                                    power_watt = {power}, is_certified = {isCert}, created_by = {creator} 
                                  WHERE id = {_productId}";

                DbHelper.ExecuteNonQuery(query);
                MessageBox.Show("Товар обновлен!");
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