using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppWnForm
{

    public partial class Form1 : Form
    {
        private readonly HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") }; // Cambia la URL según sea necesario

        }
        private async Task LoginAsync()
        {
            try
            {
                var userCredentials = new
                {
                    usuario = txtUsuario.Text,
                    contraseña = txtPassword.Text
                };

                var response = await _httpClient.PostAsJsonAsync("Usuario/login", userCredentials);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("Inicio de sesion exitoso!");

                    // Aquí irías al formulario principal de tu aplicación
                    // Por ejemplo, podrías abrir otro formulario y cerrar este
                    MainForm mainMenu = new MainForm();
                    mainMenu.Show();
                    this.Hide();
                }
                else
                {
                    lblErrorMessage.Text = "Login failed. Please check your credentials.";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred during login: " + ex.Message;
            }
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = string.Empty; // Clear previous error message
            _ = LoginAsync();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
        }
    }
}
