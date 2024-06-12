﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppWnForm
{
    public partial class ProductsForm : Form
    {
        private readonly HttpClient _httpClient;

        public ProductsForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") };
            dataGridView1.DataBindingComplete += dataGridView1_DataBindingComplete;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private async Task<List<Producto>> GetTodosLosProductosAsync()
        {
            var response = await _httpClient.GetAsync("Producto/GetTodasLasProducto");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Producto>>();
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
                dataGridView1.Columns["idProducto"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los productos.");
            }
        }

        public class Producto
        {
            public int idProducto { get; set; }
            public string nombre { get; set; }
            public string descripcion { get; set; }
            public double precio { get; set; }
            public int status { get; set; }
        }

        private void ProductsForm_Load(object sender, EventArgs e)
        {
            LoadDataAsync();
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
                txtid.Text = selectedRow.Cells["idProducto"].Value.ToString();
                txtNombre.Text = selectedRow.Cells["nombre"].Value.ToString();
                txtDescripcion.Text = selectedRow.Cells["descripcion"].Value.ToString();
                txtPrecio.Text = selectedRow.Cells["precio"].Value.ToString();
            }
        }

        private void btnGetAllProducts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProducto"].Value);

                Producto productoActualizado = new Producto
                {
                    idProducto = idProducto,
                    nombre = txtNombre.Text,
                    descripcion = txtDescripcion.Text,
                    precio = Convert.ToDouble(txtPrecio.Text),
                    status = 1, // Cambiar el estado a 1
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Producto actualizado correctamente.");
                    LoadDataSync();
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

        private HttpResponseMessage ActualizarProductoSync(int idProducto, Producto producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PutAsJsonAsync($"Producto/ActualizarProducto/Update/{idProducto}", producto));
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
                dataGridView1.Columns["idProducto"].Visible = false;
                dataGridView1.Columns["status"].Visible = false;
            }
            else
            {
                MessageBox.Show("No se pudieron obtener los productos.");
            }
        }

        private List<Producto> GetTodosLosProductosSync()
        {
            Task<List<Producto>> task = Task.Run(() => GetTodosLosProductosAsync());
            return task.Result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los TextBox
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;
            double precio = Convert.ToDouble(txtPrecio.Text);

            // Crear el objeto Producto con los valores obtenidos
            Producto nuevoProducto = new Producto
            {
                nombre = nombre,
                descripcion = descripcion,
                precio = precio,
                status = 1, // Puedes establecer el estado como desees
            };

            // Realizar la solicitud POST al servidor para insertar el nuevo producto
            HttpResponseMessage response = InsertarProducto(nuevoProducto);

            // Verificar si la solicitud fue exitosa
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Producto insertado correctamente.");
                LoadDataSync(); // Actualizar el DataGridView con los datos actualizados
            }
            else
            {
                MessageBox.Show("Error al insertar el producto. Por favor, inténtalo de nuevo.");
            }

        }

        private HttpResponseMessage InsertarProducto(Producto producto)
        {
            Task<HttpResponseMessage> task = Task.Run(() => _httpClient.PostAsJsonAsync("Producto/CrearProduto/Create", producto));
            return task.Result;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int idProducto = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["idProducto"].Value);

                Producto productoActualizado = new Producto
                {
                    idProducto = idProducto,
                    nombre = txtNombre.Text,
                    descripcion = txtDescripcion.Text,
                    precio = Convert.ToDouble(txtPrecio.Text),
                    status = 0, 
                };

                HttpResponseMessage response = ActualizarProductoSync(idProducto, productoActualizado);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Producto eliminado correctamente.");
                    LoadDataSync();
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

       
    }
    }

