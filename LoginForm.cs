using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Electrical_Store
{
    public partial class LoginForm : Form
    {
        private int failedAttempts = 0;
        private bool isCaptchaVisible = false;
        private int blockSeconds = 10;
        private string currentCaptchaAnswer = "";

        public LoginForm()
        {
            InitializeComponent();

            // --- ГЛАВНОЕ ИСПРАВЛЕНИЕ ---
            // Принудительно скрываем элементы капчи при запуске формы
            pictureBox1.Visible = false;
            textBox3.Visible = false;
            label3.Visible = false;
            isCaptchaVisible = false;
            // ---------------------------
        }

        // Внутри класса LoginForm
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- РЕЗЕРВНЫЕ ВХОДЫ ---
            if (login == "admin" && password == "admin") { new AdminForm().Show(); this.Hide(); return; }
            if (login == "manager" && password == "manager") { new MechendaizerForm().Show(); this.Hide(); return; }
            if ((login == "buyer" || login == "byuer") && (password == "buyer" || password == "byuer")) { new BuyerForm().Show(); this.Hide(); return; }
            // -----------------------

            // Проверка капчи
            if (isCaptchaVisible)
            {
                if (textBox3.Text.Trim().ToUpper() != currentCaptchaAnswer.ToUpper())
                {
                    TriggerBlock();
                    return;
                }
            }

            string query = $"SELECT id, role_id, full_name FROM users WHERE login = '{login}' AND password = '{password}' AND is_active = 1";
            DataTable dt = DbHelper.ExecuteQuery(query);

            if (dt.Rows.Count > 0)
            {
                int roleId = Convert.ToInt32(dt.Rows[0]["role_id"]);

                // Исправленный переход по ролям
                if (roleId == 1) { new AdminForm().Show(); }        // Админ
                else if (roleId == 2) { new MechendaizerForm().Show(); } // Товаровед
                else if (roleId == 4) { new BuyerForm().Show(); }   // Продавец
                else if (roleId == 3) { new BuyerForm().Show(); }   // Клиент (исправлено!)
                else { new AdminForm().Show(); }

                this.Hide();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts == 1)
                {
                    MessageBox.Show("Неверный логин или пароль. Введите капчу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ShowCaptcha();
                }
                else { TriggerBlock(); }
            }
        }

        private void ShowCaptcha()
        {
            isCaptchaVisible = true;
            pictureBox1.Visible = true;
            textBox3.Visible = true;
            label3.Visible = true;
            textBox3.Clear();

            Random rnd = new Random();
            int picNum = rnd.Next(1, 5);
            string fileName = $"Pic{picNum}.png";

            switch (picNum)
            {
                case 1: currentCaptchaAnswer = "1467"; break;
                case 2: currentCaptchaAnswer = "ETY"; break;
                case 3: currentCaptchaAnswer = "JMLP"; break;
                case 4: currentCaptchaAnswer = "HTTL"; break;
                default: currentCaptchaAnswer = "1467"; break;
            }

            try
            {
                if (System.IO.File.Exists(fileName))
                {
                    pictureBox1.Image = Image.FromFile(fileName);
                }
                else
                {
                    MessageBox.Show($"Файл {fileName} не найден. Убедитесь, что он лежит в папке bin/Debug.", "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки капчи: " + ex.Message);
            }
        }

        private void TriggerBlock()
        {
            MessageBox.Show("Слишком много неудачных попыток. Поля заблокированы на 10 секунд.", "Блокировка", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            button1.Enabled = false;

            timer1.Start();
        }

        private void timerBlock_Tick(object sender, EventArgs e)
        {
            blockSeconds--;
            if (blockSeconds <= 0)
            {
                timer1.Stop();

                // Разблокируем поля
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                button1.Enabled = true;
                blockSeconds = 10;

                // Сбрасываем счетчик попыток
                failedAttempts = 0;

                // Обновляем капчу, чтобы пользователь не мог ввести старую
                ShowCaptcha();
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}