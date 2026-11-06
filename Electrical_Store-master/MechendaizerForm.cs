using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Electrical_Store
{
    public partial class MechendaizerForm : Form
    {
        public MechendaizerForm()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OrderMechendaizerForm orders = new OrderMechendaizerForm();
            orders.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProductForm products = new ProductForm(true);
            products.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StockForm stock = new StockForm();
            stock.Show();
            this.Hide();
        }
    }
}
