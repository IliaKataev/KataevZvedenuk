namespace gos
{
    partial class CitizenForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            labelWelcome = new Label();
            buttonChangeData = new Button();
            buttonAddParameters = new Button();
            buttonAddApplication = new Button();
            dataGridViewApplications = new DataGridView();
            buttonCancelApplication = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewApplications).BeginInit();
            SuspendLayout();
            // 
            // labelWelcome
            // 
            labelWelcome.AutoSize = true;
            labelWelcome.Location = new Point(12, 9);
            labelWelcome.Name = "labelWelcome";
            labelWelcome.Size = new Size(150, 20);
            labelWelcome.TabIndex = 0;
            labelWelcome.Text = "Добро пожаловать, ";
            // 
            // buttonChangeData
            // 
            buttonChangeData.Location = new Point(564, 374);
            buttonChangeData.Name = "buttonChangeData";
            buttonChangeData.Size = new Size(224, 29);
            buttonChangeData.TabIndex = 1;
            buttonChangeData.Text = "Изменить имя и пароль";
            buttonChangeData.UseVisualStyleBackColor = true;
            buttonChangeData.Click += BtnEditPersonalData_Click;
            // 
            // buttonAddParameters
            // 
            buttonAddParameters.Location = new Point(564, 409);
            buttonAddParameters.Name = "buttonAddParameters";
            buttonAddParameters.Size = new Size(224, 29);
            buttonAddParameters.TabIndex = 2;
            buttonAddParameters.Text = "Персональные данные";
            buttonAddParameters.UseVisualStyleBackColor = true;
            buttonAddParameters.Click += buttonAddParameters_Click;
            // 
            // buttonAddApplication
            // 
            buttonAddApplication.Location = new Point(12, 301);
            buttonAddApplication.Name = "buttonAddApplication";
            buttonAddApplication.Size = new Size(175, 29);
            buttonAddApplication.TabIndex = 3;
            buttonAddApplication.Text = "Новое заявление";
            buttonAddApplication.UseVisualStyleBackColor = true;
            buttonAddApplication.Click += buttonAddApplication_Click;
            // 
            // dataGridViewApplications
            // 
            dataGridViewApplications.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewApplications.Location = new Point(12, 32);
            dataGridViewApplications.Name = "dataGridViewApplications";
            dataGridViewApplications.RowHeadersWidth = 51;
            dataGridViewApplications.Size = new Size(776, 263);
            dataGridViewApplications.TabIndex = 4;
            // 
            // buttonCancelApplication
            // 
            buttonCancelApplication.Location = new Point(193, 301);
            buttonCancelApplication.Name = "buttonCancelApplication";
            buttonCancelApplication.Size = new Size(175, 29);
            buttonCancelApplication.TabIndex = 5;
            buttonCancelApplication.Text = "Отменить заявление";
            buttonCancelApplication.UseVisualStyleBackColor = true;
            buttonCancelApplication.Click += buttonCancelApplication_Click;
            // 
            // CitizenForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonCancelApplication);
            Controls.Add(dataGridViewApplications);
            Controls.Add(buttonAddApplication);
            Controls.Add(buttonAddParameters);
            Controls.Add(buttonChangeData);
            Controls.Add(labelWelcome);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "CitizenForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CitizenForm";
            ((System.ComponentModel.ISupportInitialize)dataGridViewApplications).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelWelcome;
        private Button buttonChangeData;
        private Button buttonAddParameters;
        private Button buttonAddApplication;
        private DataGridView dataGridViewApplications;
        private Button buttonCancelApplication;
    }
}