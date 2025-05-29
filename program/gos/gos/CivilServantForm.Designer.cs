namespace gos
{
    partial class CivilServantForm
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
            dataGridViewApplications = new DataGridView();
            buttonProcessApplication = new Button();
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
            // dataGridViewApplications
            // 
            dataGridViewApplications.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewApplications.Location = new Point(12, 50);
            dataGridViewApplications.Name = "dataGridViewApplications";
            dataGridViewApplications.RowHeadersWidth = 51;
            dataGridViewApplications.Size = new Size(776, 265);
            dataGridViewApplications.TabIndex = 1;
            // 
            // buttonProcessApplication
            // 
            buttonProcessApplication.Location = new Point(309, 321);
            buttonProcessApplication.Name = "buttonProcessApplication";
            buttonProcessApplication.Size = new Size(183, 29);
            buttonProcessApplication.TabIndex = 2;
            buttonProcessApplication.Text = "Обработать заявление";
            buttonProcessApplication.UseVisualStyleBackColor = true;
            buttonProcessApplication.Click += buttonProcessApplication_Click;
            // 
            // CivilServantForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonProcessApplication);
            Controls.Add(dataGridViewApplications);
            Controls.Add(labelWelcome);
            Name = "CivilServantForm";
            Text = "CivilServantForm";
            ((System.ComponentModel.ISupportInitialize)dataGridViewApplications).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelWelcome;
        private DataGridView dataGridViewApplications;
        private Button buttonProcessApplication;
    }
}