using System;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class BuyerForm : Form
    {
        public BuyerForm()
        {
            InitializeComponent();

            // Принудительная привязка событий к кнопкам (на случай если в дизайнере забыли)
            //this.button4.Click += new EventHandler(button4_Click); // Заказы
            //this.button3.Click += new EventHandler(button3_Click); // Товары
            //this.button5.Click += new EventHandler(button5_Click); // Склад
            //this.button1.Click += new EventHandler(button1_Click); // Выйти
        }

        // 1. Переход на форму Заказов
        private void button4_Click(object sender, EventArgs e)
        {
            OrderForm orderForm = new OrderForm();
            orderForm.Show();
            this.Hide(); // Скрываем меню продавца
        }

        // 2. Переход на форму Товаров (с запретом на редактирование)
        private void button3_Click(object sender, EventArgs e)
        {
            // Передаем false, чтобы скрыть кнопки Добавить/Изменить
            ProductForm productForm = new ProductForm(false);
            productForm.Show();
            this.Hide();
        }

        // 3. Переход на форму Склада
        private void button5_Click(object sender, EventArgs e)
        {
            StockForm stockForm = new StockForm();
            stockForm.Show();
            this.Hide();
        }

        // 4. Выход из аккаунта (возврат на форму входа)
        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide(); // Закрываем меню продавца
        }
    }
}