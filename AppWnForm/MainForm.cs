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

        private void button1_Click(object sender, EventArgs e)
        {
            Cliente prod = new Cliente();
            prod.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Proveedor prod = new Proveedor();
            prod.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Empleado prod = new Empleado();
            prod.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VentaForm prod = new VentaForm();
            prod.Show();
        }
    }
}
