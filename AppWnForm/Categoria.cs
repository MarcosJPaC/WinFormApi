using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static AppWnForm.ProductsForm;

namespace AppWnForm
{
    public partial class CategoriaModelo : Form
    {
        private readonly HttpClient _httpClient;

        public CategoriaModelo()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") };
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private async Task<List<Categoria>> GetTodosLosProductosAsync()
        {
            var response = await _httpClient.GetAsync("Categoria/GetTodosLasCategorias");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Categoria>>();
            }
            return null;
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
                dataGridView1.Columns["idCategoria"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los productos.");
            }
        }

        public class Categoria
        {
            public int idCategoria { get; set; }
            public string nombre { get; set; }
            public string descripcion { get; set; }
            public string estado { get; set; }
            public int status { get; set; }
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {
            LoadDataAsync();
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
                txtid.Text = selectedRow.Cells["idCategoria"].Value?.ToString();
                txtNombre.Text = selectedRow.Cells["nombre"].Value?.ToString();
                txtDescripcion.Text = selectedRow.Cells["descripcion"].Value?.ToString();
                txtPrecio.Text = selectedRow.Cells["estado"].Value?.ToString();
            }
        }

        private void CategoriaModelo_Load(object sender, EventArgs e)
        {
            LoadDataAsync();
        }

        private async void btnGetAllProducts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idCategoria"].Value);

                Categoria productoActualizado = new Categoria
                {
                    idCategoria = idProducto,
                    nombre = txtNombre.Text,
                    descripcion = txtDescripcion.Text,
                    estado = txtPrecio.Text,
                    status = 1, // Cambiar el estado a 1
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Producto actualizado correctamente.");
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el producto. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para actualizar.");
            }
        }
        private HttpResponseMessage ActualizarProductoSync(int idProducto, Categoria producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PutAsJsonAsync($"Categoria/ActualizarCategoria/Update/{idProducto}", producto));
            return task.Result;
        }








        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TextBox
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            string precio = txtPrecio.Text;

            // Crear el objeto Producto con los valores obtenidos
            Categoria nuevoProducto = new Categoria
            {
                nombre = nombre,
                descripcion = descripcion,
                estado = precio,
                status = 1, // Puedes establecer el estado como desees
            };

            // Realizar la solicitud POST al servidor para insertar el nuevo producto
            HttpResponseMessage response = InsertarProducto(nuevoProducto);

            // Verificar si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Producto insertado correctamente.");
                LoadDataAsync(); // Actualizar el DataGridView con los datos actualizados
            }
            else
            {
                MessageBox.Show("Error al insertar el producto. Por favor, inténtalo de nuevo.");
            }

        }

        private HttpResponseMessage InsertarProducto(Categoria producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PostAsJsonAsync("Categoria/CrearCategoria/Create", producto));
            return task.Result;
        }

        private async Task<HttpResponseMessage> InsertarCategoriaAsync(Categoria categoria)
        {
            try
            {
                return await _httpClient.PostAsJsonAsync("Categoria/CrearCategoria/Creat", categoria);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Error al insertar la categoría: {ex.Message}");
                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idCategoria"].Value);

                Categoria productoActualizado = new Categoria
                {
                    idCategoria = idProducto,
                    nombre = txtNombre.Text,
                    descripcion = txtDescripcion.Text,
                    estado = txtPrecio.Text,
                    status = 0, // Cambiar el estado a 1
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Producto actualizado correctamente.");
                    LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el producto. Por favor, inténtalo de nuevo.");
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para actualizar.");
            }
        }

        //    private async void btnEliminar_Click(object sender, EventArgs e)
        //    {
        //        if (dataGridView1.SelectedRows.Count > 0)
        //        {
        //            int idCategoria = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idCategoria"].Value);

        //            CategoriaModel categoriaEliminada = new CategoriaModel
        //            {
        //                idCategoria = idCategoria,
        //                nombre = txtNombre.Text,
        //                descripcion = txtDescripcion.Text,
        //                //estado = txtEstado.Text,
        //                status = 0 // Cambiar el estado a 0 para eliminar
        //            };

        //            var response = await ActualizarCategoriaAsync(idCategoria, categoriaEliminada);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                MessageBox.Show("Categoría eliminada correctamente.");
        //                LoadDataAsync();
        //            }
        //            else
        //            {
        //                MessageBox.Show("Error al eliminar la categoría. Por favor, inténtalo de nuevo.");
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Selecciona una categoría para eliminar.");
        //        }
        //    }

        //    private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //    {

        //    }
    }
}
