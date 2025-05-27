using gos.controllers;
using gos.models;
using gos.models.DTO;
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
using System.Xml.Linq;

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

        private async void buttonAddPT_Click(object sender, EventArgs e)
        {
            using (Form editForm = new Form())
            {
                editForm.Text = "Тип параметра";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(600, 400);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                var listBoxTP = new ListBox() { Left = 20, Top = 20, Height = 180, Width = 540 };
                Label lblName = new Label() { Text = "Название параметра:", Left = 20, Top = 220, AutoSize = true };
                TextBox txtName = new TextBox() { Left = 210, Top = 218, Width = 220 };
                Label lblType = new Label() { Text = "Тип значения:", Left = 20, Top = 260, AutoSize = true };
                TextBox txtType = new TextBox() { Left = 210, Top = 258, Width = 220 };
                var btnAdd = new Button() { Text = "Добавить", Left = 440, Top = 215, Width = 110, Height = 40, Enabled = false };
                var btnUpdate = new Button() { Text = "Обновить", Left = 440, Top = 255, Width = 110, Height = 40, Enabled = false };
                var btnDelete = new Button() { Text = "Удалить", Left = 440, Top = 295, Width = 110, Height = 40, Enabled = false };
                var btnSave = new Button() { Text = "Сохранить и закрыть", Left = 200, Top = 340, Width = 200, Height = 40, DialogResult = DialogResult.OK };

                editForm.Controls.AddRange(new Control[] { listBoxTP, lblName, txtName, lblType, txtType, btnAdd, btnUpdate, btnDelete, btnSave });

                // Получаем DTO из контроллера
                var dtos = (await _adminController.GetParameterTypesAsync()).ToList();

                // Создаем список кортежей с Name и Type (без Id)
                List<(string Name, string Type)> parameters = dtos
                    .Select(d => (d.Name, d.Type))
                    .ToList();

                listBoxTP.Items.AddRange(parameters.Select(p => p.Name).ToArray());

                void UpdateAddButton()
                {
                    btnAdd.Enabled = !string.IsNullOrWhiteSpace(txtName.Text) &&
                                     !string.IsNullOrWhiteSpace(txtType.Text) &&
                                     listBoxTP.SelectedIndex < 0;
                }

                //при изменении кнопки изменить тип 
                void UpdateUpdateButton()
                {
                    int idx = listBoxTP.SelectedIndex;
                    btnUpdate.Enabled = idx >= 0 &&
                                        !string.IsNullOrWhiteSpace(txtName.Text) &&
                                        !string.IsNullOrWhiteSpace(txtType.Text);
                }

                listBoxTP.SelectedIndexChanged += (s, ev) =>
                {
                    int idx = listBoxTP.SelectedIndex;
                    if (idx >= 0)
                    {
                        var selected = parameters[idx];
                        txtName.Text = selected.Name;
                        txtType.Text = selected.Type;
                        btnDelete.Enabled = true;
                        btnAdd.Enabled = false;
                    }
                    else
                    {
                        txtName.Text = "";
                        txtType.Text = "";
                        btnDelete.Enabled = false;
                        btnAdd.Enabled = false;
                    }
                };

                txtName.TextChanged += (s, ev) =>
                {
                    UpdateAddButton();
                    UpdateUpdateButton();
                };

                txtType.TextChanged += (s, ev) =>
                {
                    UpdateAddButton();
                    UpdateUpdateButton();
                };

                btnAdd.Click += (s, ev) =>
                {
                    if (!string.IsNullOrWhiteSpace(txtName.Text) && !string.IsNullOrWhiteSpace(txtType.Text))
                    {
                        parameters.Add((txtName.Text.Trim(), txtType.Text.Trim()));
                        listBoxTP.Items.Add(txtName.Text.Trim());
                        txtName.Clear();
                        txtType.Clear();
                    }
                };

                btnUpdate.Click += (s, ev) =>
                {
                    int idx = listBoxTP.SelectedIndex;
                    if (idx >= 0)
                    {
                        var name = txtName.Text.Trim();
                        var type = txtType.Text.Trim();
                        parameters[idx] = (name, type);
                        listBoxTP.Items[idx] = name;
                        txtName.Clear();
                        txtType.Clear();
                        listBoxTP.ClearSelected();
                        btnUpdate.Enabled = false;
                        btnDelete.Enabled = false;
                    }
                };

                btnDelete.Click += (s, ev) =>
                {
                    int idx = listBoxTP.SelectedIndex;
                    if (idx >= 0)
                    {
                        parameters.RemoveAt(idx);
                        listBoxTP.Items.RemoveAt(idx);
                        txtName.Clear();
                        txtType.Clear();
                    }
                };

                btnSave.Click += async (s, ev) =>
                {
                    // Здесь вызываем контроллер, передавая DTO, которые он сформирует сам из параметров
                    await SaveParameterTypesThroughController(parameters);
                    editForm.Close();
                };

                // дважды вызывается один и тот же метод одновременно
                /*if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Можно тут тоже сохранить, если btnSave не нажали
                    await SaveParameterTypesThroughController(parameters);
                }*/

                editForm.ShowDialog(this);
            }
        }

        // Метод для передачи параметров контроллеру, где уже формируются DTO и сохраняются
        private async Task SaveParameterTypesThroughController(List<(string Name, string Type)> parameters)
        {
            try
            {
                // Передаем список в контроллер — контроллер создаст DTO и обновит через сервис
                await _adminController.ReplaceAllParameterTypesAsync(parameters);
                MessageBox.Show("Список успешно обновлён.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка при сохранении: {ex.Message}";
                if (ex.InnerException != null)
                    errorMessage += $"\nВнутреннее исключение: {ex.InnerException.Message}";

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonService_Click(object sender, EventArgs e)
        {
            using (Form editForm = new Form())
            {
                editForm.Text = "Редактирование услуг";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(700, 500);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                var listBoxServices = new ListBox() { Left = 20, Top = 20, Width = 300, Height = 300 };

                var labelName = new Label() { Text = "Название услуги:", Left = 340, Top = 30, AutoSize = true };
                var textBoxName = new TextBox() { Left = 460, Top = 28, Width = 200 };

                var labelDesc = new Label() { Text = "Описание:", Left = 340, Top = 70, AutoSize = true };
                var textBoxDesc = new TextBox() { Left = 460, Top = 68, Width = 200 };

                var btnAdd = new Button() { Text = "Добавить", Left = 460, Top = 110, Width = 100, Enabled = false };
                var btnUpdate = new Button() { Text = "Обновить", Left = 560, Top = 110, Width = 100, Enabled = false };
                var btnDelete = new Button() { Text = "Удалить", Left = 460, Top = 150, Width = 200, Enabled = false };
                var btnSave = new Button() { Text = "Сохранить и закрыть", Left = 250, Top = 400, Width = 200, DialogResult = DialogResult.OK };

                editForm.Controls.AddRange(new Control[]
                {
            listBoxServices, labelName, textBoxName, labelDesc, textBoxDesc,
            btnAdd, btnUpdate, btnDelete, btnSave
                });

                var services = (await _adminController.GetAllServicesAsync()).ToList();

                listBoxServices.Items.AddRange(services.Select(s => s.Name).ToArray());

                void UpdateButtons()
                {
                    bool hasText = !string.IsNullOrWhiteSpace(textBoxName.Text) && !string.IsNullOrWhiteSpace(textBoxDesc.Text);
                    btnAdd.Enabled = hasText && listBoxServices.SelectedIndex < 0;
                    btnUpdate.Enabled = hasText && listBoxServices.SelectedIndex >= 0;
                }

                listBoxServices.SelectedIndexChanged += (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        var selected = services[idx];
                        textBoxName.Text = selected.Name;
                        textBoxDesc.Text = selected.Description;
                        btnDelete.Enabled = true;
                    }
                    else
                    {
                        textBoxName.Clear();
                        textBoxDesc.Clear();
                        btnDelete.Enabled = false;
                    }
                    UpdateButtons();
                };

                textBoxName.TextChanged += (s, ev) => UpdateButtons();
                textBoxDesc.TextChanged += (s, ev) => UpdateButtons();

                btnAdd.Click += (s, ev) =>
                {
                    var name = textBoxName.Text.Trim();
                    var desc = textBoxDesc.Text.Trim();

                    var newService = new ServiceDTO
                    {
                        Id = 0,
                        Name = name,
                        Description = desc,
                        ActivationDate = DateOnly.FromDateTime(DateTime.Today)
                    };

                    services.Add(newService);
                    listBoxServices.Items.Add(name);

                    textBoxName.Clear();
                    textBoxDesc.Clear();
                    listBoxServices.ClearSelected();
                };

                btnUpdate.Click += (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        services[idx].Name = textBoxName.Text.Trim();
                        services[idx].Description = textBoxDesc.Text.Trim();

                        listBoxServices.Items[idx] = services[idx].Name;

                        textBoxName.Clear();
                        textBoxDesc.Clear();
                        listBoxServices.ClearSelected();
                    }
                };

                btnDelete.Click += (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        services.RemoveAt(idx);
                        listBoxServices.Items.RemoveAt(idx);
                        textBoxName.Clear();
                        textBoxDesc.Clear();
                    }
                };

                btnSave.Click += async (s, ev) =>
                {
                    await _adminController.ReplaceAllServicesAsync(services);
                    editForm.Close();
                };

                editForm.ShowDialog(this);
            }
        }



        private void buttonRule_Click(object sender, EventArgs e)
        {
            using (Form ruleForm = new Form())
            {

            }
        }
    }
}
