using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AppWnForm.ProductsForm;

namespace AppWnForm
{
    public partial class Empleado : Form
    {
        private readonly HttpClient _httpClient;

        public Empleado()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") };
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }
        private async Task<List<EmpleadoModel>> GetTodosLosProductosAsync()
        {
            var response = await _httpClient.GetAsync("Empleado/GetTodosLosEmpleados");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EmpleadoModel>>();
            }
            return null;
        }
        public class EmpleadoModel
        {
            public int idEmpleado { get; set; }
            public string nombre { get; set; }
            public string puesto { get; set; }
            public double salario { get; set; }
            public int status { get; set; }
        }
        private async void LoadDataAsync()
        {
            var productos = await GetTodosLosProductosAsync();
            if (productos != null)
            {
                // Filtrar los productos donde status != 0
                var productosFiltrados = productos.Where(p => p.status != 0).ToList();

                // Cargar los datos filtrados en el DataGridView
                dataGridView1.DataSource = productosFiltrados;

                // Ocultar las columnas idProducto y status
                dataGridView1.Columns["idEmpleado"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los Empleados.");
            }
        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Ocultar las filas donde status = 0
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((int)row.Cells["status"].Value == 0)
                {
                    row.Visible = false;
                }
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView1.SelectedRows[0];
                txtid.Text = selectedRow.Cells["idEmpleado"].Value.ToString();
                txtNombre.Text = selectedRow.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = selectedRow.Cells["puesto"].Value.ToString();
                txtPrecio.Text = selectedRow.Cells["salario"].Value.ToString();
            }
        }

        private void btnGetAllProducts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idEmpleado"].Value);

                EmpleadoModel productoActualizado = new EmpleadoModel
                {
                    idEmpleado = idProducto,
                    nombre = txtNombre.Text,
                    puesto = txtDescripcion.Text,
                    salario = float.Parse(txtPrecio.Text),
                    status = 1, // Cambiar el estado a 1
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Empleado actualizado correctamente.");
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el Empleado. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un Empleado para actualizar.");
            }
        }
        private HttpResponseMessage ActualizarProductoSync(int idProducto, EmpleadoModel producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PutAsJsonAsync($"Empleado/ActualizarCliente/Update/{idProducto}", producto));
            return task.Result;
        }
        private void LoadDataSync()
        {
            var productos = GetTodosLosProductosSync();
            if (productos != null)
            {
                // Filtrar los productos donde status != 0
                var productosFiltrados = productos.Where(p => p.status != 0).ToList();

                // Cargar los datos filtrados en el DataGridView
                dataGridView1.DataSource = productosFiltrados;

                // Ocultar las columnas idProducto y status
                dataGridView1.Columns["idEmpleado"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los Empleados.");
            }
        }

        private List<EmpleadoModel> GetTodosLosProductosSync()
        {
            Task<List<EmpleadoModel>> task = Task.Run(() => GetTodosLosProductosAsync());
            return task.Result;
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TextBox
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            string precio = txtPrecio.Text;

            // Crear el objeto Producto con los valores obtenidos
            EmpleadoModel nuevoProducto = new EmpleadoModel
            {
                nombre = nombre,
                puesto = descripcion,
                salario = double.Parse(precio),
                status = 1, // Puedes establecer el estado como desees
            };

            // Realizar la solicitud POST al servidor para insertar el nuevo producto
            HttpResponseMessage response = InsertarProducto(nuevoProducto);

            // Verificar si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Empleados insertado correctamente.");
                LoadDataSync(); // Actualizar el DataGridView con los datos actualizados
            }
            else
            {
                MessageBox.Show("Error al insertar el Empleados. Por favor, inténtalo de nuevo.");
            }

        }

        private HttpResponseMessage InsertarProducto(EmpleadoModel producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PostAsJsonAsync("Empleado/CrearEmpleado/Create", producto));
            return task.Result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idEmpleado"].Value);

                EmpleadoModel productoActualizado = new EmpleadoModel
                {
                    idEmpleado = idProducto,
                    nombre = txtNombre.Text,
                    puesto = txtDescripcion.Text,
                    salario = float.Parse(txtPrecio.Text),
                    status = 0,
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Empleado eliminado correctamente.");
                    LoadDataSync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el Empleado. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para actualizar.");
            }
        }






        private void Cliente_Load(object sender, EventArgs e)
        {
            LoadDataAsync();

        }
    }
}
