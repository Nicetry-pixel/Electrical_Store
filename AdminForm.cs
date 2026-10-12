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
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
            this.Close();
        }

      

        private void button2_Click(object sender, EventArgs e)
        {
            UserForm usr = new UserForm();
            usr.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProductForm product = new ProductForm();
            product.Show();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OrderForm order = new OrderForm();
            order.Show();
            this.Close();
        }
    }
}
