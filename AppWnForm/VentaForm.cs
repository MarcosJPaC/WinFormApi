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

namespace AppWnForm
{
    public partial class VentaForm : Form
    {
        private readonly HttpClient _httpClient;

        public VentaForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") };
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }
        private async Task<List<Venta>> GetTodasLasVentasAsync()
        {
            var response = await _httpClient.GetAsync("Venta/GetTodasLasVentas");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Venta>>();
            }
            return null;
        }
        public class Venta
        {
            public int idVenta { get; set; }
            public string fecha { get; set; }
            public decimal total { get; set; }
            public int status { get; set; }
        }
        private async void LoadDataAsync()
        {
            var ventas = await GetTodasLasVentasAsync();
            if (ventas != null)
            {
                // Filtrar las ventas donde status != 0
                var ventasFiltradas = ventas.Where(v => v.status != 0).ToList();

                // Cargar los datos filtrados en el DataGridView
                dataGridView1.DataSource = ventasFiltradas;

                // Ocultar las columnas idVenta y status
                dataGridView1.Columns["idVenta"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener las ventas.");
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
                txtid.Text = selectedRow.Cells["idVenta"].Value.ToString();
                txtNombre.Text = selectedRow.Cells["fecha"].Value.ToString();
                txtDescripcion.Text = selectedRow.Cells["total"].Value.ToString();
            }
        }

        private void btnGetAllProducts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idVenta = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVenta"].Value);

                Venta ventaActualizada = new Venta
                {
                    idVenta = idVenta,
                    fecha = txtNombre.Text,
                    total = decimal.Parse(txtDescripcion.Text),
                    status = 1, // Cambiar el estado a 1
                };

                HttpResponseMessage response = ActualizarVentaSync(idVenta, ventaActualizada);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Venta actualizada correctamente.");
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la venta. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona una venta para actualizar.");
            }
        }
        private HttpResponseMessage ActualizarVentaSync(int idVenta, Venta venta)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PutAsJsonAsync($"Venta/ActualizarCliente/Update/{idVenta}", venta));
            return task.Result;
        }
        private void LoadDataSync()
        {
            var ventas = GetTodasLasVentasSync();
            if (ventas != null)
            {
                // Filtrar las ventas donde status != 0
                var ventasFiltradas = ventas.Where(v => v.status != 0).ToList();

                // Cargar los datos filtrados en el DataGridView
                dataGridView1.DataSource = ventasFiltradas;

                // Ocultar las columnas idVenta y status
                dataGridView1.Columns["idVenta"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener las ventas.");
            }
        }

        private List<Venta> GetTodasLasVentasSync()
        {
            Task<List<Venta>> task = Task.Run(() => GetTodasLasVentasAsync());
            return task.Result;
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TextBox
            string fecha = txtNombre.Text;
            decimal total = decimal.Parse(txtDescripcion.Text);

            // Crear el objeto Venta con los valores obtenidos
            Venta nuevaVenta = new Venta
            {
                fecha = fecha,
                total = total,
                status = 1, // Puedes establecer el estado como desees
            };

            // Realizar la solicitud POST al servidor para insertar la nueva venta
            HttpResponseMessage response = InsertarVenta(nuevaVenta);

            // Verificar si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Venta insertada correctamente.");
                LoadDataSync(); // Actualizar el DataGridView con los datos actualizados
            }
            else
            {
                MessageBox.Show("Error al insertar la venta. Por favor, inténtalo de nuevo.");
            }

        }

        private HttpResponseMessage InsertarVenta(Venta venta)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PostAsJsonAsync("Venta/CrearEmpleado/Create", venta));
            return task.Result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idVenta = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idVenta"].Value);

                Venta ventaActualizada = new Venta
                {
                    idVenta = idVenta,
                    fecha = txtNombre.Text,
                    total = decimal.Parse(txtDescripcion.Text),
                    status = 0, // Cambiar el estado a 0
                };

                HttpResponseMessage response = ActualizarVentaSync(idVenta, ventaActualizada);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Venta eliminada correctamente.");
                    LoadDataSync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la venta. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona una venta para actualizar.");
            }
        }

        private void VentaForm_Load(object sender, EventArgs e)
        {
        }

        private void VentaForm_Load_1(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

       

       
    }
}
