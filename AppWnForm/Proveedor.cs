using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppWnForm
{
    public partial class Proveedor : Form
    {
        private readonly HttpClient _httpClient;

        public Proveedor()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") };
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private async Task<List<ProveedorModel>> GetTodosLosProveedoresAsync()
        {
            var response = await _httpClient.GetAsync("Proveedor/GetTodasLosProveedores");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ProveedorModel>>();
            }
            return null;
        }

        public class ProveedorModel
        {
            public int idProveedor { get; set; }
            public string nombre { get; set; }
            public string direccion { get; set; }
            public string telefono { get; set; }
            public int status { get; set; }
        }

        private async void LoadDataAsync()
        {
            var proveedores = await GetTodosLosProveedoresAsync();
            if (proveedores != null)
            {
                var proveedoresFiltrados = proveedores.Where(p => p.status != 0).ToList();
                dataGridView1.DataSource = proveedoresFiltrados;

                dataGridView1.Columns["idProveedor"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los proveedores.");
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
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
                txtid.Text = selectedRow.Cells["idProveedor"].Value.ToString();
                txtNombre.Text = selectedRow.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = selectedRow.Cells["direccion"].Value.ToString();
                txtPrecio.Text = selectedRow.Cells["telefono"].Value.ToString();
            }
        }

        private void btnGetAllProducts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProveedor = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProveedor"].Value);

                ProveedorModel proveedorActualizado = new ProveedorModel
                {
                    idProveedor = idProveedor,
                    nombre = txtNombre.Text,
                    direccion = txtDescripcion.Text,
                    telefono = txtPrecio.Text,
                    status = 1,
                };

                HttpResponseMessage response = ActualizarProveedorSync(idProveedor, proveedorActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Proveedor actualizado correctamente.");
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el proveedor. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un proveedor para actualizar.");
            }
        }

        private HttpResponseMessage ActualizarProveedorSync(int idProveedor, ProveedorModel proveedor)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PutAsJsonAsync($"Proveedor/ActualizarCliente/Update/{idProveedor}", proveedor));
            return task.Result;
        }

        private void LoadDataSync()
        {
            var proveedores = GetTodosLosProveedoresSync();
            if (proveedores != null)
            {
                var proveedoresFiltrados = proveedores.Where(p => p.status != 0).ToList();
                dataGridView1.DataSource = proveedoresFiltrados;

                dataGridView1.Columns["idProveedor"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los proveedores.");
            }
        }

        private List<ProveedorModel> GetTodosLosProveedoresSync()
        {
            Task<List<ProveedorModel>> task = Task.Run(() => GetTodosLosProveedoresAsync());
            return task.Result;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            string telefono = txtPrecio.Text;

            ProveedorModel nuevoProveedor = new ProveedorModel
            {
                nombre = nombre,
                direccion = descripcion,
                telefono = telefono,
                status = 1,
            };

            HttpResponseMessage response = InsertarProveedor(nuevoProveedor);

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Proveedor insertado correctamente.");
                LoadDataSync();
            }
            else
            {
                MessageBox.Show("Error al insertar el proveedor. Por favor, inténtalo de nuevo.");
            }
        }

        private HttpResponseMessage InsertarProveedor(ProveedorModel proveedor)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PostAsJsonAsync("Proveedor/CrearEmpleado/Create", proveedor));
            return task.Result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProveedor = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProveedor"].Value);

                ProveedorModel proveedorActualizado = new ProveedorModel
                {
                    idProveedor = idProveedor,
                    nombre = txtNombre.Text,
                    direccion = txtDescripcion.Text,
                    telefono = txtPrecio.Text,
                    status = 0,
                };

                HttpResponseMessage response = ActualizarProveedorSync(idProveedor, proveedorActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Proveedor eliminado correctamente.");
                    LoadDataSync();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el proveedor. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un proveedor para eliminar.");
            }
        }

        private void Proveedor_Load(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Proveedor_Load_1(object sender, EventArgs e)
        {
            LoadDataAsync();
        }
    }
}
