using gos.controllers;
using gos.models;
using gos.models.DTO;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace gos
{
    public partial class CitizenForm : Form
    {
        private readonly CitizenController _citizenController;
        private readonly UserDTO _currentUser;

        public CitizenForm(CitizenController citizenController, UserDTO currentUser)
        {
            InitializeComponent();
            _citizenController = citizenController;
            _currentUser = currentUser;

            Load += CitizenForm_Load;
            FormClosed += CitizenForm_Closed;
        }

        private async void CitizenForm_Load(object sender, EventArgs e)
        {
            labelWelcome.Text = $"Добро пожаловать, {_currentUser.FullName}!";
            await LoadUserApplicationsAsync();
        }

        private void CitizenForm_Closed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void BtnEditPersonalData_Click(object sender, EventArgs e)
        {
            using (Form editForm = new Form())
            {
                editForm.Text = "Редактирование персональных данных";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(400, 200);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                // ФИО
                Label lblName = new Label() { Text = "ФИО:", Left = 20, Top = 20, AutoSize = true };
                TextBox txtName = new TextBox() { Left = 100, Top = 20, Width = 250 };

                // Пароль
                Label lblPassword = new Label() { Text = "Пароль:", Left = 20, Top = 60, AutoSize = true };
                TextBox txtPassword = new TextBox() { Left = 100, Top = 60, Width = 250, PasswordChar = '*' };

                // Кнопки
                Button btnOk = new Button() { Text = "Сохранить", Left = 100, Top = 110, DialogResult = DialogResult.OK, AutoSize = true };
                Button btnCancel = new Button() { Text = "Отмена", Left = 210, Top = 110, DialogResult = DialogResult.Cancel, AutoSize = true };

                editForm.Controls.AddRange(new Control[] { lblName, txtName, lblPassword, txtPassword, btnOk, btnCancel });
                editForm.AcceptButton = btnOk;
                editForm.CancelButton = btnCancel;

                if (editForm.ShowDialog(this) == DialogResult.OK)
                {
                    string fullName = txtName.Text.Trim();
                    string password = txtPassword.Text;

                    if (string.IsNullOrWhiteSpace(fullName))
                    {
                        MessageBox.Show("ФИО не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    UpdatePersonalData(fullName, password);
                }
            }
        }

        private async void UpdatePersonalData(string fullName, string password)
        {
            try
            {
                await _citizenController.UpdatePersonalData(fullName, password);
                MessageBox.Show("Данные успешно обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private async void buttonAddParameters_Click(object sender, EventArgs e)
        {
            using (Form paramForm = new Form())
            {
                paramForm.Text = "Управление параметрами";
                paramForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                paramForm.StartPosition = FormStartPosition.CenterParent;
                paramForm.ClientSize = new Size(500, 300);
                paramForm.MaximizeBox = false;
                paramForm.MinimizeBox = false;
                paramForm.ShowInTaskbar = false;

                // Список параметров
                ListView listViewParams = new ListView()
                {
                    View = View.Details,
                    FullRowSelect = true,
                    Location = new Point(10, 10),
                    Size = new Size(480, 180)
                };
                listViewParams.Columns.Add("Тип", 200);
                listViewParams.Columns.Add("Значение", 240);

                // Загрузка параметров пользователя
                List<ParameterDTO> parameters = await _citizenController.LoadParameters();

                // Загрузка типов
                List<ParameterTypeDTO> types = await _citizenController.LoadParameterTypesAsync();

                void RefreshList()
                {
                    listViewParams.Items.Clear();
                    foreach (var param in parameters)
                    {
                        string typeName = types.FirstOrDefault(t => t.Id == param.TypeId)?.Name ?? "Неизвестно";
                        listViewParams.Items.Add(new ListViewItem(new[] { typeName, param.Value }) { Tag = param });
                    }
                }

                RefreshList();

                // Кнопки
                Button btnAdd = new Button() { Text = "Добавить", Location = new Point(10, 210), Width = 110, Height = 40 };
                Button btnEdit = new Button() { Text = "Изменить", Location = new Point(120, 210), Width = 110, Height = 40 };
                Button btnDelete = new Button() { Text = "Удалить", Location = new Point(230, 210), Width = 110, Height = 40 };
                Button btnClose = new Button() { Text = "Закрыть", Location = new Point(380, 250), Width = 110, Height = 40, DialogResult = DialogResult.Cancel };

                // Добавление параметра
                btnAdd.Click += async (s, ev) =>
                {
                    if (ShowParameterDialog(types, out var selectedTypeId, out var value))
                    {
                        await _citizenController.AddParameterAsync(selectedTypeId, value);
                        parameters = await _citizenController.LoadParameters();
                        RefreshList();
                    }
                };

                // Редактирование
                btnEdit.Click += async (s, ev) =>
                {
                    if (listViewParams.SelectedItems.Count == 0) return;

                    var param = (ParameterDTO)listViewParams.SelectedItems[0].Tag;

                    if (ShowParameterDialog(types, out var newTypeId, out var newValue, param.TypeId, param.Value))
                    {
                        await _citizenController.UpdateParameterAsync(param.Id, newTypeId, newValue);
                        parameters = await _citizenController.LoadParameters();
                        RefreshList();
                    }
                };

                // Удаление
                btnDelete.Click += async (s, ev) =>
                {
                    if (listViewParams.SelectedItems.Count == 0) return;

                    var param = (ParameterDTO)listViewParams.SelectedItems[0].Tag;

                    if (MessageBox.Show("Удалить параметр?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        await _citizenController.DeleteParameterAsync(param.Id);
                        parameters = await _citizenController.LoadParameters();
                        RefreshList();
                    }
                };

                paramForm.Controls.AddRange(new Control[] { listViewParams, btnAdd, btnEdit, btnDelete, btnClose });

                paramForm.ShowDialog(this);
            }
        }

        private bool ShowParameterDialog(List<ParameterTypeDTO> types, out int selectedTypeId, out string value, int? defaultTypeId = null, string defaultValue = "")
        {
            selectedTypeId = -1;
            value = "";

            using (Form dialog = new Form())
            {
                dialog.Text = "Параметр";
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(400, 160);
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;

                Label lblType = new Label() { Text = "Тип параметра:", Left = 20, Top = 20, AutoSize = true };
                ComboBox cmbTypes = new ComboBox() { Left = 160, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbTypes.DataSource = types;
                cmbTypes.DisplayMember = "Name";
                cmbTypes.ValueMember = "Id";

                if (defaultTypeId.HasValue)
                {
                    var existingItem = types.FirstOrDefault(t => t.Id == defaultTypeId.Value);

                    if (existingItem == null)
                    {
                        existingItem = new ParameterTypeDTO { Id = defaultTypeId.Value, Name = $"Удалённый тип ({defaultTypeId.Value})" };
                        types.Add(existingItem);
                    }

                    cmbTypes.DataSource = null;

                    cmbTypes.DisplayMember = "Name";
                    cmbTypes.ValueMember = "Id";

                    cmbTypes.DataSource = types;

                    dialog.Shown += (s, e) =>
                    {
                        cmbTypes.SelectedValue = existingItem.Id;
                    };


                    cmbTypes.SelectedValue = existingItem.Id;
                    MessageBox.Show($"Ожидали Id: {existingItem.Id}, выбранный: {cmbTypes.SelectedValue}");

                }
                else
                {
                    cmbTypes.SelectedIndex = -1;
                }

                Label lblValue = new Label() { Text = "Значение:", Left = 20, Top = 60, AutoSize = true };
                TextBox txtValue = new TextBox() { Left = 160, Top = 60, Width = 200, Text = defaultValue };

                Button btnOk = new Button() { Text = "ОК", Left = 160, Top = 100, Height = 40, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Отмена", Left = 250, Top = 100, Width = 110, Height = 40, DialogResult = DialogResult.Cancel };

                dialog.Controls.AddRange(new Control[] { lblType, cmbTypes, lblValue, txtValue, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (cmbTypes.SelectedItem is ParameterTypeDTO type && !string.IsNullOrWhiteSpace(txtValue.Text))
                    {
                        selectedTypeId = type.Id;
                        value = txtValue.Text.Trim();
                        return true;
                    }
                    MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                return false;
            }
        }

        private async void buttonAddApplication_Click(object sender, EventArgs e)
        {
            using (Form paramForm = new Form())
            {
                paramForm.Text = "Новое заявление";
                paramForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                paramForm.StartPosition = FormStartPosition.CenterParent;
                paramForm.ClientSize = new Size(400, 200);
                paramForm.MaximizeBox = false;
                paramForm.MinimizeBox = false;
                paramForm.ShowInTaskbar = false;

                Label lblService = new Label()
                {
                    Text = "Выберите услугу:",
                    Left = 20,
                    Top = 20,
                    AutoSize = true
                };

                ComboBox comboBoxServices = new ComboBox()
                {
                    Left = 20,
                    Top = 50,
                    Width = 350,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                Button buttonApplyApplication = new Button()
                {
                    Text = "Подать заявление",
                    Left = 20,
                    Top = 100,
                    Width = 150,
                    Height = 40

                };

                paramForm.Controls.AddRange(new Control[] { lblService, comboBoxServices, buttonApplyApplication });

                try
                {
                    var services = await _citizenController.LoadAvailableServices();

                    if (services == null || services.Count == 0)
                    {
                        MessageBox.Show("Нет доступных услуг.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    comboBoxServices.DataSource = services;
                    comboBoxServices.DisplayMember = "Name";
                    comboBoxServices.ValueMember = "Id";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки услуг: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                buttonApplyApplication.Click += async (s, ev) =>
                {
                    if (comboBoxServices.SelectedItem is not ServiceDTO selectedService)
                    {
                        MessageBox.Show("Пожалуйста, выберите услугу.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var userParams = await _citizenController.LoadParameters();
                    var userTypeIds = userParams.Select(p => p.TypeId).ToHashSet();

                    var requiredTypes = await _citizenController.LoadServiceRequirements(selectedService.Id);
                    var requiredTypeIds = requiredTypes.Select(t => t.Id).ToHashSet();

                    var missingTypeIds = requiredTypeIds.Except(userTypeIds).ToList();

                    //MessageBox.Show("User Type Ids: " + string.Join(", ", userTypeIds));
                    //MessageBox.Show("Required Type Ids: " + string.Join(", ", requiredTypeIds));


                    if (missingTypeIds.Any())
                    {
                        var allTypes = await _citizenController.LoadParameterTypesAsync();
                        var missingTypes = allTypes
                            .Where(t => missingTypeIds.Contains(t.Id))
                            .Select(t => $"• {t.Name}")
                            .ToList();

                        var message = "Невозможно создать заявление.\nДобавьте недостающие параметры:\n\n" +
                                      string.Join("\n", missingTypes);

                        MessageBox.Show(message, "Недостающие параметры", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var application = new ApplicationDTO
                    {
                        UserId = _currentUser.Id,
                        ServiceId = selectedService.Id,
                        Status = ApplicationStatus.IN_PROGRESS,
                        CreationDate = DateTime.Now,
                    };

                    MessageBox.Show(application.Deadline.ToString());

                    await _citizenController.CreateNewApplication(_currentUser.Id, application);
                    MessageBox.Show("Заявление успешно создано.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadUserApplicationsAsync();

                    paramForm.DialogResult = DialogResult.OK;
                    paramForm.Close();
                };

                paramForm.ShowDialog();
            }
        }

        private async Task LoadUserApplicationsAsync()
        {
            try
            {
                var applications = await _citizenController.ViewMyApplications();

                var services = await _citizenController.LoadAvailableServices();
                var serviceMap = services.ToDictionary(s => s.Id, s => s.Name);

                var displayList = applications.Select(app => new
                {
                    app.ApplicationId,
                    Услуга = serviceMap.ContainsKey(app.ServiceId) ? serviceMap[app.ServiceId] : "Неизвестно",
                    Создано = app.CreationDate.ToString("dd.MM.yyyy"),
                    Дедлайн = app.Deadline.HasValue ? app.Deadline.Value.ToString("dd.MM.yyyy") : "—",
                    Статус = app.Status.ToString(),
                    Результат = app.Result ?? "—"
                }).ToList();

                dataGridViewApplications.DataSource = displayList;

                if (dataGridViewApplications.Columns["ApplicationId"] != null)
                    dataGridViewApplications.Columns["ApplicationId"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявлений: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonCancelApplication_Click(object sender, EventArgs e)
        {
            if (dataGridViewApplications.CurrentRow == null)
            {
                MessageBox.Show("Выберите заявление для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idCell = dataGridViewApplications.CurrentRow.Cells["ApplicationId"];
            if (idCell == null || idCell.Value == null)
            {
                MessageBox.Show("Не удалось получить ID выбранного заявления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int applicationId = Convert.ToInt32(idCell.Value);

            var confirm = MessageBox.Show("Вы действительно хотите удалить выбранное заявление?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            try
            {
                await _citizenController.CancelMyApplication(applicationId);

                // Обновляем таблицу после удаления
                await LoadUserApplicationsAsync();

                MessageBox.Show("Заявление успешно удалено.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении заявления: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
