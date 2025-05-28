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

            this.Load += CitizenForm_Load;
        }

        private async void CitizenForm_Load(object sender, EventArgs e)
        {
            labelWelcome.Text = $"Добро пожаловать, {_currentUser.FullName}!";
            await LoadUserApplicationsAsync();
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
                Button btnCancel = new Button() { Text = "Отмена", Left = 200, Top = 110, DialogResult = DialogResult.Cancel, AutoSize = true };

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
                paramForm.Text = "Добавление параметра";
                paramForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                paramForm.StartPosition = FormStartPosition.CenterParent;
                paramForm.ClientSize = new Size(400, 200);
                paramForm.MaximizeBox = false;
                paramForm.MinimizeBox = false;
                paramForm.ShowInTaskbar = false;

                // Метка и комбобокс для типа параметра
                Label lblType = new Label() { Text = "Тип параметра:", Left = 20, Top = 20, AutoSize = true };
                ComboBox cmbTypes = new ComboBox() { Left = 150, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

                // Получение типов параметров
                List<ParameterTypeDTO> types;
                try
                {
                    types = await _citizenController.LoadParameterTypesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки типов параметров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                cmbTypes.DataSource = types;
                cmbTypes.DisplayMember = "Name";
                cmbTypes.ValueMember = "Id";

                // Метка и поле ввода значения
                Label lblValue = new Label() { Text = "Значение:", Left = 20, Top = 60, AutoSize = true };
                TextBox txtValue = new TextBox() { Left = 150, Top = 60, Width = 200 };

                // Кнопки
                Button btnOk = new Button() { Text = "Добавить", Left = 150, Top = 110, DialogResult = DialogResult.OK, AutoSize = true };
                Button btnCancel = new Button() { Text = "Отмена", Left = 250, Top = 110, DialogResult = DialogResult.Cancel, AutoSize = true };

                paramForm.Controls.AddRange(new Control[] { lblType, cmbTypes, lblValue, txtValue, btnOk, btnCancel });
                paramForm.AcceptButton = btnOk;
                paramForm.CancelButton = btnCancel;

                if (paramForm.ShowDialog(this) == DialogResult.OK)
                {
                    if (cmbTypes.SelectedItem is ParameterTypeDTO selectedType)
                    {
                        string value = txtValue.Text.Trim();
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            MessageBox.Show("Значение не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        await AddParameterAsync(selectedType.Id, value);
                    }
                }
            }
        }


        private async Task AddParameterAsync(int typeId, string value)
        {
            try
            {
                await _citizenController.AddParameterAsync(typeId, value);
                MessageBox.Show("Параметр успешно добавлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Можно обновить список параметров на форме, если он есть
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Ошибка при сохранении изменений:");

                Exception current = ex;
                while (current != null)
                {
                    sb.AppendLine(current.Message);
                    current = current.InnerException;
                }

                MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Width = 150
                };

                paramForm.Controls.AddRange(new Control[] { lblService, comboBoxServices, buttonApplyApplication });

                // Загрузим услуги асинхронно (await внутри Invoke, чтобы не блокировать UI)
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

                // Обработчик нажатия на кнопку в дочерней форме
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

                    MessageBox.Show("User Type Ids: " + string.Join(", ", userTypeIds));
                    MessageBox.Show("Required Type Ids: " + string.Join(", ", requiredTypeIds));


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
                        Deadline = DateTime.Now.AddMonths(1)

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
                    ApplicationId = app.ApplicationId, // <-- добавляем Id для удаления
                    Услуга = serviceMap.ContainsKey(app.ServiceId) ? serviceMap[app.ServiceId] : "Неизвестно",
                    Создано = app.CreationDate.ToString("dd.MM.yyyy"),
                    Дедлайн = app.Deadline.ToString("dd.MM.yyyy"),
                    Статус = app.Status.ToString(),
                    Результат = app.Result ?? "—"
                }).ToList();

                dataGridViewApplications.DataSource = displayList;

                // Сделать столбец ApplicationId скрытым
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

            // Получаем Id выбранного заявления
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
                // Вызываем метод удаления из контроллера
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
