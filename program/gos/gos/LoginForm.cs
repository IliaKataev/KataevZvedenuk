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

            var user = await _authController.Login(login, password);

            if (user != null)
            {
                Hide();

                switch (user.Role)
                {
                    case UserRole.ADMIN:
                        var adminForm = ActivatorUtilities.CreateInstance<AdminForm>(_serviceProvider, user);
                        adminForm.Show();
                        break;

                    case UserRole.CIVILSERVANT:
                        var civilServantForm = ActivatorUtilities.CreateInstance<CivilServantForm>(_serviceProvider, user);
                        civilServantForm.Show();
                        break;

                    case UserRole.CITIZEN:
                        var citizenForm = ActivatorUtilities.CreateInstance<CitizenForm>(_serviceProvider, user);
                        citizenForm.Show();
                        break;

                    default:
                        lblError.Text = "Неизвестная роль пользователя.";
                        lblError.Visible = true;
                        Show();
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
