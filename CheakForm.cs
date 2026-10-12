using System;
using System.Collections.Generic;
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

            dataGridView1.DataSource = items;
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
