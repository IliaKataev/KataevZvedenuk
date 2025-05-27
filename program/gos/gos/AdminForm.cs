using gos.controllers;
using gos.models;
using Microsoft.VisualBasic.Logging;
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
    public partial class AdminForm : Form
    {
        private readonly AdminController _adminController;
        public AdminForm(AdminController adminController)
        {
            InitializeComponent();
            _adminController = adminController;
        }

        private void buttonAddUser_Click(object sender, EventArgs e)
        {
            using (Form editForm = new Form())
            {
                editForm.Text = "Добавление нового пользователя";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(400, 250);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                // ФИО
                Label lblName = new Label() { Text = "ФИО:", Left = 20, Top = 20, AutoSize = true };
                TextBox txtName = new TextBox() { Left = 100, Top = 20, Width = 250 };

                // Пароль
                Label lblPassword = new Label() { Text = "Пароль:", Left = 20, Top = 60, AutoSize = true };
                TextBox txtPassword = new TextBox() { Left = 100, Top = 60, Width = 250, PasswordChar = '*' };

                // Логин
                Label lblLogin = new Label() { Text = "Логин", Left = 20, Top = 100, AutoSize = true };
                TextBox txtLogin = new TextBox() { Left = 100, Top = 100, Width = 250 };

                // Выбор ЮзерРол
                Label lblUserRole = new Label() { Text = "Логин", Left = 20, Top = 140, AutoSize = true };
                ComboBox cmbUserRole = new ComboBox()
                {
                    Left = 100,
                    Top = 140,
                    Width = 250,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DataSource = Enum.GetValues(typeof(UserRole))
                };

                // Кнопки
                Button btnOk = new Button() { Text = "Сохранить", Left = 100, Top = 180, DialogResult = DialogResult.OK, AutoSize = true };
                Button btnCancel = new Button() { Text = "Отмена", Left = 200, Top = 180, DialogResult = DialogResult.Cancel, AutoSize = true };

                editForm.Controls.AddRange(new Control[] {
                    lblName, txtName,
                    lblLogin, txtLogin,
                    lblPassword, txtPassword,
                    lblUserRole, cmbUserRole,
                    btnOk, btnCancel
                });

                editForm.AcceptButton = btnOk;
                editForm.CancelButton = btnCancel;

                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    string fullName = txtName.Text.Trim();
                    string login = txtLogin.Text.Trim();
                    string password = txtPassword.Text;
                    var selectedRole = (UserRole)cmbUserRole.SelectedItem;

                    if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                    {
                        MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    CreateNewUser(fullName, login, password, selectedRole);
                }
            }
        }

        private async void CreateNewUser(string fullName, string login, string password, UserRole selectedRole)
        {
            try
            {
                await _adminController.CreateNewUser(fullName, login, password, selectedRole);
                MessageBox.Show("Пользователь успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка при обновлении: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nВнутреннее исключение: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAddPT_Click(object sender, EventArgs e)
        {
            using (Form editForm = new Form())
            {
                editForm.Text = "Добавление нового типа параметра";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(400, 200);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                // Тип
                Label lblType = new Label() { Text = "Тип:", Left = 20, Top = 20, AutoSize = true };
                TextBox txtType = new TextBox() { Left = 100, Top = 20, Width = 250 };

                // Наименование
                Label lblName = new Label() { Text = "Название:", Left = 20, Top = 60, AutoSize = true };
                TextBox txtName = new TextBox() { Left = 100, Top = 60, Width = 250};

                // Кнопки
                Button btnOk = new Button() { Text = "Сохранить", Left = 100, Top = 110, DialogResult = DialogResult.OK, AutoSize = true };
                Button btnCancel = new Button() { Text = "Отмена", Left = 200, Top = 110, DialogResult = DialogResult.Cancel, AutoSize = true };

                editForm.Controls.AddRange(new Control[] { lblName, txtName, lblType, txtType, btnOk, btnCancel });
                editForm.AcceptButton = btnOk;
                editForm.CancelButton = btnCancel;

                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    string name = txtName.Text.Trim();
                    string type = txtType.Text;

                    if (string.IsNullOrWhiteSpace(name) & string.IsNullOrWhiteSpace(type))
                    {
                        MessageBox.Show("Название и тип не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    AddParameterType(type, name);
                }
            }
        }

        private async Task AddParameterType(string type, string name)
        {
            try
            {
                await _adminController.CreateParameterType(type, name);
                MessageBox.Show("Параметр успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка при обновлении: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nВнутреннее исключение: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
