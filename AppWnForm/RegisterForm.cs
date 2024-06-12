using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppWnForm
{
    public partial class RegisterForm : Form
    {
        private readonly HttpClient _httpClient;

        public RegisterForm()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7267/api/") }; // URL base de la API

        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            var response = await _httpClient.GetAsync("Usuario"); // Ajusta la URL según sea necesario
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();
                return users?.Exists(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)) ?? false;
            }
            return false;
        }

        private async Task RegisterAsync()
        {
            try
            {
                if (!IsValidEmail(txtEmail.Text))
                {
                    lblErrorMessage.Text = "Invalid email format.";
                    return;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    lblErrorMessage.Text = "Passwords do not match.";
                    return;
                }

                if (await EmailExistsAsync(txtEmail.Text))
                {
                    lblErrorMessage.Text = "Email already exists.";
                    return;
                }

                var userCredentials = new
                {
                    usuario = txtEmail.Text,
                    contraseña = txtPassword.Text
                };

                var response = await _httpClient.PostAsJsonAsync("Usuario/register", userCredentials);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Registration successful!");

                    // Aquí podrías cerrar el formulario de registro y volver al formulario de inicio de sesión
                    this.Close();
                }
                else
                {
                    lblErrorMessage.Text = "Registration failed. Please check your inputs.";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred during registration: " + ex.Message;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = string.Empty; // Clear previous error message
            _ = RegisterAsync();
        }

        private class User
        {
            public string Email { get; set; }
        }
    }
}