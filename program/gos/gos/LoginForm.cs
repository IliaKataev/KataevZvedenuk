using gos.controllers;
using gos.models;
using gos.services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gos
{
    public partial class LoginForm : Form
    {
        private readonly AuthController _authController;
        private readonly IServiceProvider _serviceProvider;

        public LoginForm(AuthController authController, IServiceProvider serviceProvider)
        {
            _authController = authController;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            var login = txtLogin.Text.Trim();
            var password = txtPassword.Text;

            lblError.Visible = false;

            // Выполняем логин
            var (isSuccess, userRole) = await _authController.Login(login, password);

            if (isSuccess && userRole.HasValue)
            {
                Hide(); // Скрываем форму логина

                switch (userRole.Value)
                {
                    case UserRole.ADMIN:
                        var adminForm = _serviceProvider.GetRequiredService<AdminForm>();
                        adminForm.Show();
                        break;
                    case UserRole.CIVILSERVANT:
                        var civilServantForm = _serviceProvider.GetRequiredService<CivilServantForm>();
                        civilServantForm.Show();
                        break;
                    case UserRole.CITIZEN:
                        var citizenForm = _serviceProvider.GetRequiredService<CitizenForm>();
                        citizenForm.Show();
                        break;
                    default:
                        lblError.Text = "Неизвестная роль пользователя.";
                        lblError.Visible = true;
                        Show(); // Показываем снова форму
                        break;
                }
            }
            else
            {
                lblError.Text = "Неверный логин или пароль.";
                lblError.Visible = true;
            }
        }
    }

}
