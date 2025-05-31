using gos.controllers;
using gos.models;
using gos.models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

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
            FormClosed += CivilServantForm_Closed;
        }
        private async void CivilServantForm_Load(object sender, EventArgs e)
        {
            labelWelcome.Text = $"Добро пожаловать, {_currentUser.FullName}!";
            await LoadApplications();
        }

        private void CivilServantForm_Closed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }


        private async Task LoadApplications()
        {
            try
            {
                var applications = await _controller.LoadApplications();

                foreach (var app in applications)
                {
                    Console.WriteLine($"AppId: {app.ApplicationId}, ClosureDate: {app.ClosureDate}");
                }


                var services = await _controller.LoadAvailableServices();
                var serviceMap = services.ToDictionary(s => s.Id, s => s.Name);

                // исключаем Canceled
                var filteredApplications = applications
                    .Where(app => app.Status != ApplicationStatus.CANCELED)
                    .ToList();

                var displayList = filteredApplications.Select(app => new
                {
                    app.ApplicationId, 
                    Услуга = serviceMap.ContainsKey(app.ServiceId) ? serviceMap[app.ServiceId] : "Неизвестно",
                    Создано = app.CreationDate.ToString("dd.MM.yyyy"),
                    Дедлайн = app.Deadline.HasValue ? app.Deadline.Value.ToString("dd.MM.yyyy") : "—",
                    Статус = app.Status.ToString(),
                    Результат = app.Result ?? "—",
                    Закрыто = app.ClosureDate.HasValue
                            ? app.ClosureDate.Value.ToString("dd.MM.yyyy")
                            : "—"
                    
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
            var services = await _controller.LoadAvailableServices();
            var serviceMap = services.ToDictionary(s => s.Id, s => s.Name);

            var application = applications.FirstOrDefault(a => a.ApplicationId == applicationId);
            if (application == null)
            {
                MessageBox.Show("Заявление не найдено.");
                return;
            }

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
                           $"Дедлайн: {(application.Deadline.HasValue ? application.Deadline.Value.ToString("dd.MM.yyyy") : "—")}\n" +
                           $"Статус: {application.Status}\n" +
                           $"Результат: {application.Result ?? "—"}\n" +
                           $"Закрыто: {(application.ClosureDate.HasValue ? application.ClosureDate.Value.ToString("dd.MM.yyyy") : "—")}",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                var comboBoxStatus = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(20, 410),
                    Width = 320
                };

                // Поле ввода результата
                var textBoxResult = new TextBox()
                {
                    Multiline = true,
                    Width = 320,
                    Height = 60,
                    Location = new Point(20, 300),
                    Text = application.Result ?? ""
                };

                var buttonProcess = new Button()
                {
                    Text = "Обработать",
                    Location = new Point(20, 365),
                    Width = 150,
                    Height = 40
                };

                comboBoxStatus.SelectedIndexChanged += async (s, e) =>
                {
                    var selectedStatus = (ApplicationStatus)comboBoxStatus.SelectedValue;
                    if (selectedStatus == ApplicationStatus.REJECTED)
                    {
                        application.ClosureDate = DateTime.Now;
                        // Обновляем на форме отображение даты закрытия
                        labelInfo.Text = $"ID: {application.ApplicationId}\n" +
                                         $"Услуга ID: {application.ServiceId}\n" +
                                         $"Создано: {application.CreationDate:dd.MM.yyyy}\n" +
                                         $"Дедлайн: {(application.Deadline.HasValue ? application.Deadline.Value.ToString("dd.MM.yyyy") : "—")}\n" +
                                         $"Статус: {application.Status}\n" +
                                         $"Результат: {application.Result ?? "—"}\n" +
                                         $"Закрыто: {application.ClosureDate.Value.ToString("dd.MM.yyyy")}";
                    }
                    applications = await RefreshApplications(serviceMap);
                };

                buttonProcess.Click += async (_, __) =>
                {
                    try
                    {
                        var updatedApplication = await _controller.ProcessApplication(application);
                        //MessageBox.Show(updatedApplication.Deadline.ToString());
                        application.Status = updatedApplication.Status;
                        application.Result = updatedApplication.Result;
                        application.ClosureDate = updatedApplication.ClosureDate;
                        application.Deadline = updatedApplication.Deadline;

                        MessageBox.Show("Заявление обработано:\n\n" +
                                        $"Статус: {application.Status}\n" +
                                        $"Результат: {application.Result}\n" +
                                        $"Дедлайн: {(application.Deadline.HasValue ? application.Deadline.Value.ToString("dd.MM.yyyy") : "—")}\n" +
                                        $"Закрыто: {(application.ClosureDate.HasValue ? application.ClosureDate.Value.ToString("dd.MM.yyyy") : "—")}");


                        comboBoxStatus.SelectedValue = application.Status;
                        textBoxResult.Text = application.Result ?? "";

                        labelInfo.Text = $"ID: {application.ApplicationId}\n" +
                                         $"Услуга ID: {application.ServiceId}\n" +
                                         $"Создано: {application.CreationDate:dd.MM.yyyy}\n" +
                                         $"Дедлайн: {(application.Deadline.HasValue ? application.Deadline.Value.ToString("dd.MM.yyyy") : "—")}\n" +
                                         $"Статус: {application.Status}\n" +
                                         $"Результат: {application.Result ?? "—"}\n" +
                                         $"Закрыто: {(application.ClosureDate.HasValue ? application.ClosureDate.Value.ToString("dd.MM.yyyy") : "—")}";
                        MessageBox.Show(application.ClosureDate.Value.ToString("dd.MM.yyyy"));
                        applications = await RefreshApplications(serviceMap);

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
                    Location = new Point(220, 365),
                    Width = 120,
                    Height = 40,
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

                        // обновляем список
                        applications = await RefreshApplications(serviceMap);

                        processForm.DialogResult = DialogResult.OK;
                        processForm.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                    }

                    if (application.Status == ApplicationStatus.REJECTED && !application.ClosureDate.HasValue)
                    {
                        application.ClosureDate = DateTime.Now;
                    }

                };

                if (processForm.ShowDialog() == DialogResult.OK)
                {
                    await LoadApplications(); // обновить таблицу после сохранения
                }
            }
        }

        private async Task<List<ApplicationDTO>> RefreshApplications(Dictionary<int, string> serviceMap)
        {
            var applications = (await _controller.LoadApplications()).ToList();

            var filteredApps = applications
                .Where(app => app.Status != ApplicationStatus.CANCELED)
                .ToList();


            var displayList = filteredApps.Select(app => new
            {
                ApplicationId = app.ApplicationId,
                Услуга = serviceMap.ContainsKey(app.ServiceId) ? serviceMap[app.ServiceId] : "Неизвестно",
                Создано = app.CreationDate.ToString("dd.MM.yyyy") ?? "—",
                Дедлайн = app.Deadline.HasValue ? app.Deadline.Value.ToString("dd.MM.yyyy") : "—",
                Статус = app.Status.ToString(),
                Результат = app.Result ?? "—",
                Закрыто = app.ClosureDate.HasValue ? app.ClosureDate.Value.ToString("dd.MM.yyyy") : "—"
            }).ToList();


            dataGridViewApplications.DataSource = null;
            dataGridViewApplications.DataSource = displayList;

            if (dataGridViewApplications.Columns["ApplicationId"] != null)
            {
                dataGridViewApplications.Columns["ApplicationId"].Visible = false;
            }

            return applications;
        }
    }
}
