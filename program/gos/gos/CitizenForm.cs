using gos.controllers;
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
        private DataGridView dgvParameters;

        public CitizenForm(CitizenController citizenController)
        {
            InitializeComponent();
            _citizenController = citizenController;
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
                Button btnOk = new Button() { Text = "Добавить", Left = 150, Top = 110, DialogResult = DialogResult.OK };
                Button btnCancel = new Button() { Text = "Отмена", Left = 250, Top = 110, DialogResult = DialogResult.Cancel };

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

        private void buttonEditData_Click(object sender, EventArgs e)
        {

        }
    }
}
