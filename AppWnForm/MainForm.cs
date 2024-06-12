using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppWnForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            ProductsForm prod = new ProductsForm();
            prod.Show();
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            CategoriaModelo prod = new CategoriaModelo();
            prod.Show();
        }
    }
}
