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
            label1 = new Label();
            buttonChangeData = new Button();
            buttonAddParameters = new Button();
            buttonEditData = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(150, 20);
            label1.TabIndex = 0;
            label1.Text = "Добро пожаловать, ";
            // 
            // buttonChangeData
            // 
            buttonChangeData.Location = new Point(12, 409);
            buttonChangeData.Name = "buttonChangeData";
            buttonChangeData.Size = new Size(158, 29);
            buttonChangeData.TabIndex = 1;
            buttonChangeData.Text = "Изменить Данные";
            buttonChangeData.UseVisualStyleBackColor = true;
            buttonChangeData.Click += BtnEditPersonalData_Click;
            // 
            // buttonAddParameters
            // 
            buttonAddParameters.Location = new Point(603, 12);
            buttonAddParameters.Name = "buttonAddParameters";
            buttonAddParameters.Size = new Size(185, 29);
            buttonAddParameters.TabIndex = 2;
            buttonAddParameters.Text = "Добавить параметры";
            buttonAddParameters.UseVisualStyleBackColor = true;
            buttonAddParameters.Click += buttonAddParameters_Click;
            // 
            // buttonEditData
            // 
            buttonEditData.Location = new Point(603, 47);
            buttonEditData.Name = "buttonEditData";
            buttonEditData.Size = new Size(185, 29);
            buttonEditData.TabIndex = 3;
            buttonEditData.Text = "Изменить параметры";
            buttonEditData.UseVisualStyleBackColor = true;
            buttonEditData.Click += buttonEditData_Click;
            // 
            // CitizenForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonEditData);
            Controls.Add(buttonAddParameters);
            Controls.Add(buttonChangeData);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "CitizenForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CitizenForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button buttonChangeData;
        private Button buttonAddParameters;
        private Button buttonEditData;
    }
}