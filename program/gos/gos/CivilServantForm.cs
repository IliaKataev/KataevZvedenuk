using gos.controllers;
using gos.models;
using gos.models.DTO;
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
    public partial class CivilServantForm : Form
    {
        private readonly CivilServantController _controller;
        private readonly UserDTO _currentUser;
        public CivilServantForm(CivilServantController controller, UserDTO currentUser)
        {


            InitializeComponent();
            _currentUser = currentUser;
            _controller = controller;
            Load += CivilServantForm_Load;
        }
        private async void CivilServantForm_Load(object sender, EventArgs e)
        {
            labelWelcome.Text = $"Добро пожаловать, {_currentUser.FullName}!";
            await LoadApplicationsAsync();
        }

        private async Task LoadApplicationsAsync()
        {
            try
            {
                var applications = await _controller.LoadApplications();
                var services = await _controller.LoadAvailableServices();
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

        private async void buttonProcessApplication_Click(object sender, EventArgs e)
        {
            if (dataGridViewApplications.CurrentRow == null)
            {
                MessageBox.Show("Выберите заявление для обработки.");
                return;
            }

            // Получаем ID выбранного заявления
            var applicationId = (int)dataGridViewApplications.CurrentRow.Cells["ApplicationId"].Value;

            // Загружаем полные данные по заявлению
            var applications = await _controller.LoadApplications();
            var application = applications.FirstOrDefault(a => a.ApplicationId == applicationId);
            if (application == null)
            {
                MessageBox.Show("Заявление не найдено.");
                return;
            }

            // Создание модального окна
            using (Form processForm = new Form())
            {
                processForm.Text = "Обработка заявления";
                processForm.ClientSize = new Size(600, 600);
                processForm.StartPosition = FormStartPosition.CenterParent;
                processForm.MaximizeBox = false;
                processForm.MinimizeBox = false;
                processForm.ShowInTaskbar = false;

                // Информация о заявлении
                var labelInfo = new Label()
                {
                    Text = $"ID: {application.ApplicationId}\n" +
                           $"Услуга ID: {application.ServiceId}\n" +
                           $"Создано: {application.CreationDate:dd.MM.yyyy}\n" +
                           $"Дедлайн: {application.Deadline:dd.MM.yyyy}\n" +
                           $"Статус: {application.Status}\n" +
                           $"Результат: {application.Result ?? "—"}\n" +
                           $"Закрыто: {(application.ClosureDate.HasValue ? application.ClosureDate.Value.ToString("dd.MM.yyyy") : "—")}",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                var comboBoxStatus = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(20, 340),
                    Width = 300
                };

                // Поле ввода результата
                var textBoxResult = new TextBox()
                {
                    Multiline = true,
                    Width = 320,
                    Height = 60,
                    Location = new Point(20, 180),
                    Text = application.Result ?? ""
                };

                // Затычка кнопки "Обработать"
                var buttonProcess = new Button()
                {
                    Text = "Обработать (затычка)",
                    Location = new Point(20, 260),
                    Width = 150
                };
                buttonProcess.Click += async (_, __) =>
                {
                    try
                    {
                        var updatedApplication = await _controller.ProcessApplication(application);
                        application.Status = updatedApplication.Status;
                        application.Result = updatedApplication.Result;
                        application.ClosureDate = updatedApplication.ClosureDate;

                        MessageBox.Show("Заявление обработано:\n\n" +
                                        $"Статус: {application.Status}\n" +
                                        $"Результат: {application.Result}\n" +
                                        $"Закрыто: {application.ClosureDate?.ToString("dd.MM.yyyy") ?? "—"}");

                        comboBoxStatus.SelectedValue = application.Status;
                        textBoxResult.Text = application.Result ?? "";

                        // Обновление текста в labelInfo (если нужно сразу показать результат)
                        labelInfo.Text = $"ID: {application.ApplicationId}\n" +
                                         $"Услуга ID: {application.ServiceId}\n" +
                                         $"Создано: {application.CreationDate:dd.MM.yyyy}\n" +
                                         $"Дедлайн: {application.Deadline:dd.MM.yyyy}\n" +
                                         $"Статус: {application.Status}\n" +
                                         $"Результат: {application.Result ?? "—"}\n" +
                                         $"Закрыто: {(application.ClosureDate.HasValue ? application.ClosureDate.Value.ToString("dd.MM.yyyy") : "—")}";



                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обработке: {ex.Message}");
                    }
                };


                // Кнопка "Сохранить"
                var buttonSave = new Button()
                {
                    Text = "Сохранить",
                    Location = new Point(200, 260),
                    Width = 120,
                    DialogResult = DialogResult.OK
                };

                comboBoxStatus.DataSource = Enum.GetValues(typeof(ApplicationStatus))
                          .Cast<ApplicationStatus>()
                          .Select(status => new
                          {
                              Value = status,
                              Text = status.ToString()
                          })
                .ToList();

                comboBoxStatus.DisplayMember = "Text";
                comboBoxStatus.ValueMember = "Value";

                comboBoxStatus.SelectedValue = application.Status;

                processForm.Controls.AddRange(new Control[] { labelInfo, textBoxResult, buttonProcess, buttonSave, comboBoxStatus });

                // После добавления всех контролов
                processForm.Shown += (_, __) =>
                {
                    comboBoxStatus.SelectedValue = application.Status;
                };


                buttonSave.Click += async (_, __) =>
                {
                    application.Result = textBoxResult.Text;
                    application.Status = (ApplicationStatus)comboBoxStatus.SelectedValue;
                    try
                    {
                        await _controller.UpdateApplicationResult(application);
                        processForm.DialogResult = DialogResult.OK;
                        processForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                    }
                };



                // Показываем модально
                if (processForm.ShowDialog() == DialogResult.OK)
                {
                    LoadApplicationsAsync(); // обновить таблицу после сохранения
                }
            }
        }

    }
}
