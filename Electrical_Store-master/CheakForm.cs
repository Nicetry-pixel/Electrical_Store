using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class CheakForm : Form
    {
        public CheakForm(string clientName, string checkNumber, List<BukketForm.CartItem> items, decimal total)
        {
            InitializeComponent();

            label1.Text = $"ФИО Клиента: {clientName}";
            label3.Text = $"Номер чека: {checkNumber}";
            label8.Text = $"Общая сумма: {total:F2} рублей";

            // Привязываем данные
            dataGridView1.DataSource = items;

            // --- ФОРМАТИРОВАНИЕ ТАБЛИЦЫ ---
            FormatGrid();
        }

        private void FormatGrid()
        {
            // 1. Базовые настройки
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            // 2. Автоматическое растягивание колонок по ширине таблицы
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 3. Шрифты и высота строк
            dataGridView1.DefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Regular);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Verdana", 10F, FontStyle.Bold);
            dataGridView1.RowTemplate.Height = 30;

            // 4. Скрываем ID товара (в чеке он не нужен)
            if (dataGridView1.Columns["ProductId"] != null)
                dataGridView1.Columns["ProductId"].Visible = false;

            // 5. Переводим заголовки на русский и задаем пропорции ширины
            if (dataGridView1.Columns["ProductName"] != null)
            {
                dataGridView1.Columns["ProductName"].HeaderText = "Наименование";
                dataGridView1.Columns["ProductName"].FillWeight = 50; // Делаем колонку самой широкой
            }

            if (dataGridView1.Columns["Quantity"] != null)
            {
                dataGridView1.Columns["Quantity"].HeaderText = "Кол-во";
                dataGridView1.Columns["Quantity"].FillWeight = 15;
            }

            if (dataGridView1.Columns["Price"] != null)
            {
                dataGridView1.Columns["Price"].HeaderText = "Цена";
                dataGridView1.Columns["Price"].DefaultCellStyle.Format = "F2"; // Формат 2 знака после запятой
                dataGridView1.Columns["Price"].FillWeight = 15;
            }

            if (dataGridView1.Columns["Total"] != null)
            {
                dataGridView1.Columns["Total"].HeaderText = "Сумма";
                dataGridView1.Columns["Total"].DefaultCellStyle.Format = "F2"; // Формат 2 знака после запятой
                dataGridView1.Columns["Total"].FillWeight = 20;
            }
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Чек отправлен на печать!", "Печать", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}