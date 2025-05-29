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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace gos
{
    public partial class AdminForm : Form
    {
        private readonly AdminController _adminController;
        private readonly UserDTO _currentUser;

        public AdminForm(AdminController adminController, UserDTO currentUser)
        {
            InitializeComponent();
            _adminController = adminController;
            _currentUser = currentUser;

            this.Load += AdminForm_Load; // Подписка на событие загрузки формы
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            labelWelcome.Text = $"Добро пожаловать, {_currentUser.FullName}!";
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
                Label lblUserRole = new Label() { Text = "Роль", Left = 20, Top = 140, AutoSize = true };
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

                var listBoxTP = new ListBox() { Left = 20, Top = 20, Height = 140, Width = 540 };
                Label lblName = new Label() { Text = "Название параметра:", Left = 20, Top = 220, AutoSize = true };
                TextBox txtName = new TextBox() { Left = 210, Top = 218, Width = 220 };
                Label lblType = new Label() { Text = "Тип значения:", Left = 20, Top = 260, AutoSize = true };
                TextBox txtType = new TextBox() { Left = 210, Top = 258, Width = 220 };

                var btnCancelType = new Button() { Text = "Отменить выбор", Left = 250, Top = 150, Width = 220, Height = 40, Enabled = false };

                var btnAdd = new Button() { Text = "Добавить", Left = 440, Top = 215, Width = 110, Height = 40, Enabled = false };
                var btnUpdate = new Button() { Text = "Обновить", Left = 440, Top = 255, Width = 110, Height = 40, Enabled = false };
                var btnDelete = new Button() { Text = "Удалить", Left = 440, Top = 295, Width = 110, Height = 40, Enabled = false };
                var btnSave = new Button() { Text = "Сохранить и закрыть", Left = 200, Top = 340, Width = 200, Height = 40, DialogResult = DialogResult.OK };

                editForm.Controls.AddRange(new Control[] { listBoxTP, lblName, txtName, lblType, txtType, btnAdd, btnUpdate, btnDelete, btnSave, btnCancelType });

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
                        btnCancelType.Enabled = true;
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

                btnCancelType.Click += (s, e) =>
                {
                    int idx = listBoxTP.SelectedIndex;
                    if (idx >= 0)
                    {
                        listBoxTP.ClearSelected();
                        btnCancelType.Enabled = false;
                    }
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
                    await SaveParameterTypes(parameters);
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
        private async Task SaveParameterTypes(List<(string Name, string Type)> parameters)
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
                editForm.Text = "Редактирование услуг и правил";
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.ClientSize = new Size(1000, 615);
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                editForm.ShowInTaskbar = false;

                var listBoxServices = new ListBox() { Left = 20, Top = 20, Width = 300, Height = 520 };
                var listBoxRules = new ListBox() { Left = 340, Top = 160, Width = 620, Height = 180 };

                var labelName = new Label() { Text = "Название услуги:", Left = 340, Top = 30, AutoSize = true };
                var textBoxName = new TextBox() { Left = 490, Top = 28, Width = 200 };

                var labelDesc = new Label() { Text = "Описание:", Left = 340, Top = 70, AutoSize = true };
                var textBoxDesc = new TextBox() { Left = 490, Top = 68, Width = 200 };

                var btnCancelService = new Button() { Text = "Отменить выбор услуги", Left = 100, Top = 540, Width = 220, Height = 40, Enabled = false };

                var btnUpdate = new Button() { Text = "Обновить услугу", Left = 700, Top = 28, Width = 220, Height = 40, Enabled = false };
                var btnAddService = new Button() { Text = "Добавить услугу", Left = 700, Top = 68, Width = 220, Height = 40 };
                var btnDeactService = new Button() { Text = "Деактивировать услугу", Left = 700, Top = 108, Width = 220, Height = 40, Enabled = false };

                var btnAddRule = new Button() { Text = "Добавить правило", Left = 340, Top = 350, Width = 220, Height = 40, Enabled = false };
                var btnEditRule = new Button() { Text = "Изменить правило", Left = 560, Top = 350, Width = 220, Height = 40, Enabled = false };
                var btnDeleteRule = new Button() { Text = "Удалить правило", Left = 760, Top = 350, Width = 220, Height = 40, Enabled = false };

                var btnOk = new Button() { Text = "Сохранить", Left = 560, Top = 555, Width = 110, Height = 40, DialogResult = DialogResult.OK };
                var btnCancel = new Button() { Text = "Закрыть", Left = 670, Top = 555, Width = 110, Height = 40, DialogResult = DialogResult.Cancel };

                var labelRuleValue = new Label() { Text = "Значение:", Left = 340, Top = 400, AutoSize = true, Visible = false };
                var textBoxRuleValue = new TextBox() { Left = 470, Top = 400, Width = 150, Visible = false };

                var labelOperator = new Label() { Text = "Оператор:", Left = 340, Top = 440, AutoSize = true, Visible = false };
                var comboBoxOperator = new ComboBox() { Left = 470, Top = 440, Width = 80, DropDownStyle = ComboBoxStyle.DropDownList, Visible = false };
                comboBoxOperator.Items.AddRange(new string[] { "=", "<", ">", "<=", ">=", "!=" });

                var labelRuleType = new Label() { Text = "Тип:", Left = 340, Top = 480, AutoSize = true, Visible = false };
                var comboBoxParameterTypes = new ComboBox()
                {
                    Left = 470,
                    Top = 480,
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DisplayMember = "Name",
                    ValueMember = "Id",
                    Visible = false
                };

                var labelDeadlineDays = new Label() { Text = "Срок (дней):", Left = 340, Top = 520, AutoSize = true, Visible = false };
                var textBoxDeadlineDays = new TextBox() { Left = 470, Top = 520, Width = 150, Visible = false };

                var btnSaveRule = new Button() { Text = "Сохранить правило", Left = 750, Top = 415, Width = 220, Height = 40, Visible = false };
                var btnCancelRule = new Button() { Text = "Отменить", Left = 750, Top = 455, Width = 220, Height = 40, Visible = false };


                editForm.Controls.AddRange(new Control[]
                {
                    listBoxServices, listBoxRules, labelName, textBoxName, labelDesc, textBoxDesc,
                    btnUpdate, btnAddRule, btnEditRule, btnDeleteRule, btnOk, btnCancel,
                    labelRuleValue, textBoxRuleValue, labelOperator, comboBoxOperator,
                    labelRuleType, comboBoxParameterTypes, btnSaveRule, btnAddService, btnDeactService, btnCancelService, btnCancelRule,
                    labelDeadlineDays, textBoxDeadlineDays
                });

                var parameterTypes = (await _adminController.GetParameterTypesAsync()).ToList();
                comboBoxParameterTypes.DataSource = parameterTypes;


                var services = (await _adminController.GetAllServicesAsync()).ToList();

                listBoxServices.Items.AddRange(services
                    .Select(s => s.DeactivationDate.HasValue
                        ? $"{s.Name} (деактивировано {s.DeactivationDate.Value:dd.MM.yyyy})"
                        : s.Name)
                    .ToArray());


                async Task RefreshRulesForSelectedService()
                {
                    listBoxRules.Items.Clear();
                    btnAddRule.Enabled = btnEditRule.Enabled = btnDeleteRule.Enabled = false;

                    if (listBoxServices.SelectedIndex >= 0)
                    {
                        int selectedServiceId = services[listBoxServices.SelectedIndex].Id;
                        var rules = (await _adminController.GetRulesForServiceAsync(selectedServiceId)).ToList();

                        foreach (var r in rules)
                        {
                            var type = parameterTypes.FirstOrDefault(p => p.Id == r.NeededTypeId);
                            string typeName = type?.Name ?? $"TypeId: {r.NeededTypeId}";
                            listBoxRules.Items.Add($"{typeName} {r.ComparisonOperator} {r.Value}");
                        }

                        listBoxRules.Tag = rules;
                        btnAddRule.Enabled = true;
                    }
                }

                async Task RefreshServices()
                {
                    //listBoxServices.SelectedIndexChanged -= ListBoxServices_SelectedIndexChanged;

                    listBoxServices.Items.Clear();
                    services = (await _adminController.GetAllServicesAsync()).ToList();

                    listBoxServices.Items.AddRange(services
                        .Select(s => s.DeactivationDate.HasValue
                            ? $"{s.Name} (деактивировано {s.DeactivationDate.Value:dd.MM.yyyy})"
                            : s.Name)
                        .ToArray());

                    //listBoxServices.SelectedIndexChanged += ListBoxServices_SelectedIndexChanged;

                    //listBoxServices.SelectedIndex = 0;
                }


                /*void UpdateServiceButtons()
                {
                    btnUpdate.Enabled = !string.IsNullOrWhiteSpace(textBoxName.Text)
                                     && !string.IsNullOrWhiteSpace(textBoxDesc.Text)
                                     && listBoxServices.SelectedIndex >= 0;
                }*/

                listBoxServices.SelectedIndexChanged += async (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        if (services[idx].DeactivationDate != null && services[idx].DeactivationDate > DateOnly.MinValue)
                        {
                            btnAddRule.Enabled = false;
                            btnUpdate.Enabled = false;
                            btnDeactService.Enabled = false;
                        }
                        else 
                        {
                            var selected = services[idx];
                            btnUpdate.Enabled = true;
                            textBoxName.Text = selected.Name;
                            textBoxDesc.Text = selected.Description;
                            btnCancelService.Enabled = true;
                            btnDeactService.Enabled = true;
                            btnAddService.Enabled = false;
                            await RefreshRulesForSelectedService();
                        }
                    }
                    else
                    {
                        listBoxRules.Items.Clear();
                        textBoxName.Clear();
                        textBoxDesc.Clear();
                        btnAddService.Enabled = true;
                        btnAddRule.Enabled = false;
                        btnUpdate.Enabled = false;
                        btnCancelService.Enabled = false;
                        btnDeactService.Enabled = false;
                    }

                    //UpdateServiceButtons();
                    //await RefreshRulesForSelectedService();
                };

                listBoxRules.SelectedIndexChanged += (s, ev) =>
                {
                    btnEditRule.Enabled = btnDeleteRule.Enabled = listBoxRules.SelectedIndex >= 0;
                };


                //textBoxName.TextChanged += (s, ev) => UpdateServiceButtons();
                //textBoxDesc.TextChanged += (s, ev) => UpdateServiceButtons();

                btnCancelService.Click += async (s, e) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        listBoxServices.SelectedIndex = -1;
                        await RefreshRulesForSelectedService();
                    }
                };

                btnAddService.Click += async (s, ev) =>
                {
                    string name = textBoxName.Text.Trim();
                    string description = textBoxDesc.Text.Trim();

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                    {
                        MessageBox.Show("Пожалуйста, заполните поля Название и Описание.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var newService = new ServiceDTO
                    {
                        Id = 0,
                        Name = name,
                        Description = description,
                        ActivationDate = DateOnly.FromDateTime(DateTime.Now)
                    };

                    services.Add(newService);

                    listBoxServices.Items.Add(newService.Name);

                    await _adminController.ReplaceAllServicesAsync(services); // если нужно — сохраняем услуги

                    await RefreshServices(); // обновляем локальный список и UI

                    int newIndex = services.FindIndex(s => s.Name == newService.Name && s.Description == newService.Description);
                    if (newIndex >= 0)
                        listBoxServices.SelectedIndex = newIndex;

                    await RefreshRulesForSelectedService();
                };


                btnUpdate.Click += async (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        if (services[idx].DeactivationDate != null && services[idx].DeactivationDate > DateOnly.MinValue)
                        {
                            MessageBox.Show("Эта услуга деактивирована. Ее нельзя изменить!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        int serviceId = services[idx].Id; // Сохраняем ID

                        services[idx].Name = textBoxName.Text.Trim();
                        services[idx].Description = textBoxDesc.Text.Trim();

                        await _adminController.ReplaceAllServicesAsync(services); // если нужно — сохраняем услуги

                        await RefreshServices(); // обновляем локальный список и UI

                        int newIndex = services.FindIndex(s => s.Name == services[idx].Name && s.Description == services[idx].Description);
                        if (newIndex >= 0)
                            listBoxServices.SelectedIndex = newIndex;
                    }
                };



                btnDeactService.Click += async (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;

                    // Найдем нужную услугу по Id
                    if (idx >= 0)
                    {
                        if (services[idx].DeactivationDate != null && services[idx].DeactivationDate > DateOnly.MinValue)
                        {
                            MessageBox.Show("Эта услуга уже деактивирована.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        services[idx].DeactivationDate = DateOnly.FromDateTime(DateTime.Today);
                        textBoxName.Text += $" деактивирована: {services[idx].ToString()}";
                        MessageBox.Show("Услуга деактивирована");

                        await _adminController.ReplaceAllServicesAsync(services);
                        await RefreshServices();

                        //сбрасываем самостоятельно
                        listBoxRules.Items.Clear();
                        textBoxName.Clear();
                        textBoxDesc.Clear();
                        btnAddService.Enabled = true;
                        btnAddRule.Enabled = false;
                        btnUpdate.Enabled = false;
                        btnCancelService.Enabled = false;
                        btnDeactService.Enabled = false;
                    }
                };

                // Добавление нового правила
                btnAddRule.Click += (s, ev) =>
                {
                    int idx = listBoxServices.SelectedIndex;
                    if (idx >= 0)
                    {
                        labelRuleValue.Visible = textBoxRuleValue.Visible =
                        labelOperator.Visible = comboBoxOperator.Visible =
                        labelRuleType.Visible = comboBoxParameterTypes.Visible =
                        btnSaveRule.Visible = btnCancelRule.Visible =
                        labelDeadlineDays.Visible = textBoxDeadlineDays.Visible = true;

                        textBoxRuleValue.Text = "";
                        comboBoxOperator.SelectedIndex = 0;
                        comboBoxParameterTypes.SelectedValue = 0;
                        textBoxDeadlineDays.Text = "";
                    }
                };

                // Редактирование правила
                btnEditRule.Click += (s, ev) =>
                {
                    int idx = listBoxRules.SelectedIndex;
                    int serviceIdx = listBoxServices.SelectedIndex;
                    if (idx >= 0 && serviceIdx >= 0)
                    {
                        var rules = (List<RuleDTO>)listBoxRules.Tag;
                        var ruleToEdit = rules[idx];

                        // Показываем панель редактирования с текущими значениями
                        labelRuleValue.Visible = textBoxRuleValue.Visible =
                        labelOperator.Visible = comboBoxOperator.Visible =
                        labelRuleType.Visible = comboBoxParameterTypes.Visible =
                        btnSaveRule.Visible = btnCancelRule.Visible =
                        labelDeadlineDays.Visible = textBoxDeadlineDays.Visible = true;

                        textBoxRuleValue.Text = ruleToEdit.Value;
                        comboBoxOperator.SelectedItem = ruleToEdit.ComparisonOperator;
                        comboBoxParameterTypes.SelectedValue = ruleToEdit.NeededTypeId;

                        // Заполняем срок
                        textBoxDeadlineDays.Text = ruleToEdit.DeadlineDays?.ToString() ?? "";

                        // Временно сохраняем редактируемое правило в Tag
                        btnSaveRule.Tag = ruleToEdit;
                    }
                };


                btnDeleteRule.Click += async (s, ev) =>
                {
                    int idx = listBoxRules.SelectedIndex;
                    if (idx >= 0)
                    {
                        var rules = (List<RuleDTO>)listBoxRules.Tag;
                        var ruleToDelete = rules[idx];

                        var confirm = MessageBox.Show("Удалить выбранное правило?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirm == DialogResult.Yes)
                        {
                            await _adminController.DeleteRuleAsync(ruleToDelete.Id);
                            await RefreshRulesForSelectedService();
                        }
                    }
                };

                // Сохранение правила
                btnSaveRule.Click += async (s, ev) =>
                {
                    int serviceIdx = listBoxServices.SelectedIndex;
                    if (serviceIdx < 0) return;

                    if (string.IsNullOrWhiteSpace(textBoxRuleValue.Text) || comboBoxOperator.SelectedIndex < 0)
                    {
                        MessageBox.Show("Введите значение и выберите оператор!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int? deadlineDays = null;
                    if (int.TryParse(textBoxDeadlineDays.Text.Trim(), out int parsedDeadline))
                    {
                        deadlineDays = parsedDeadline;
                    }

                    var ruleDto = new RuleDTO
                    {
                        ServiceId = services[serviceIdx].Id,
                        Value = textBoxRuleValue.Text.Trim(),
                        ComparisonOperator = comboBoxOperator.SelectedItem.ToString(),
                        NeededTypeId = (int)comboBoxParameterTypes.SelectedValue,
                        DeadlineDays = deadlineDays
                    };

                    var existingRule = btnSaveRule.Tag as RuleDTO;
                    if (existingRule != null)
                    {
                        ruleDto.Id = existingRule.Id;
                        await _adminController.UpdateRuleAsync(ruleDto);
                    }
                    else
                    {
                        await _adminController.AddRuleAsync(ruleDto);
                    }

                    // Сброс формы
                    textBoxRuleValue.Clear();
                    comboBoxOperator.SelectedIndex = -1;
                    comboBoxParameterTypes.SelectedIndex = -1;
                    textBoxDeadlineDays.Clear();

                    labelRuleValue.Visible = textBoxRuleValue.Visible =
                    labelOperator.Visible = comboBoxOperator.Visible =
                    labelRuleType.Visible = comboBoxParameterTypes.Visible =
                    btnSaveRule.Visible = btnCancelRule.Visible =
                    labelDeadlineDays.Visible = textBoxDeadlineDays.Visible = false;

                    btnSaveRule.Tag = null;

                    await RefreshRulesForSelectedService();
                };

                btnCancelRule.Click += (s, ev) =>
                {
                    // Скрываем элементы формы
                    labelRuleValue.Visible = textBoxRuleValue.Visible =
                    labelOperator.Visible = comboBoxOperator.Visible =
                    labelRuleType.Visible = comboBoxParameterTypes.Visible =
                    btnSaveRule.Visible =
                    labelDeadlineDays.Visible = textBoxDeadlineDays.Visible = false;

                    // Очищаем поля
                    textBoxRuleValue.Clear();
                    comboBoxOperator.SelectedIndex = -1;
                    comboBoxParameterTypes.SelectedIndex = -1;

                    // (Опционально) Скрываем кнопку отмены
                    btnCancelRule.Visible = false;
                };


                btnOk.Click += async (s, ev) =>
                {
                    await _adminController.ReplaceAllServicesAsync(services); // если нужно — сохраняем услуги
                };

                editForm.ShowDialog(this);
            }
        }
    }
}


