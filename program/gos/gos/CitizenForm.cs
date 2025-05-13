using gos.controllers;
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
    public partial class CitizenForm : Form
    {
        private readonly CitizenController _citizenController;

        public CitizenForm(CitizenController citizenController)
        {
            InitializeComponent();
            _citizenController = citizenController;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Гражданин";

            // Кнопки основного меню
            var btnEditPersonalData = new Button { Text = "Изменить персональные данные", Width = 250, Top = 20, Left = 20 };
            btnEditPersonalData.Click += BtnEditPersonalData_Click;
            Controls.Add(btnEditPersonalData);

            // Поля редактирования
            lblFullName = new Label { Text = "ФИО:", Left = 20, Top = 70, Visible = false };
            txtFullName = new TextBox { Left = 120, Top = 70, Width = 200, Visible = false };

            lblPassword = new Label { Text = "Пароль:", Left = 20, Top = 100, Visible = false };
            txtPassword = new TextBox { Left = 120, Top = 100, Width = 200, Visible = false, PasswordChar = '*' };

            // Кнопки управления
            btnSave = new Button { Text = "Сохранить", Left = 120, Top = 140, Visible = false };
            btnSave.Click += BtnSave_Click;

            btnBack = new Button { Text = "Назад", Left = 220, Top = 140, Visible = false };
            btnBack.Click += BtnBack_Click;

            Controls.AddRange(new Control[] { lblFullName, txtFullName, lblPassword, txtPassword, btnSave, btnBack });
        }

        private void BtnEditPersonalData_Click(object sender, EventArgs e)
        {
            ToggleEditMode(true);

        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Поле ФИО не может быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                await _citizenController.UpdatePersonalData(fullName, password);
                MessageBox.Show("Данные успешно обновлены.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ToggleEditMode(false);
            }
            catch (Exception ex)
            {
                // Выводим подробности о внутреннем исключении
                string errorMessage = $"Ошибка при обновлении: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nВнутреннее исключение: {ex.InnerException.Message}";
                }

                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            ToggleEditMode(false);
        }

        private void ToggleEditMode(bool enabled)
        {
            lblFullName.Visible = enabled;
            txtFullName.Visible = enabled;

            lblPassword.Visible = enabled;
            txtPassword.Visible = enabled;

            btnSave.Visible = enabled;
            btnBack.Visible = enabled;

            // Скрываем кнопки основного меню
            foreach (Control control in Controls)
            {
                if (control is Button btn && btn.Text == "Изменить персональные данные")
                {
                    control.Visible = !enabled;
                }
            }
        }

        // Поля формы
        private Label lblFullName;
        private TextBox txtFullName;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnSave;
        private Button btnBack;
    }
}
