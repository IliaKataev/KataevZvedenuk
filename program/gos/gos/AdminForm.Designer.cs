namespace gos
{
    partial class AdminForm
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
            buttonAddUser = new Button();
            buttonAddPT = new Button();
            buttonRule = new Button();
            buttonService = new Button();
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
            // buttonAddUser
            // 
            buttonAddUser.Location = new Point(12, 385);
            buttonAddUser.Name = "buttonAddUser";
            buttonAddUser.Size = new Size(226, 53);
            buttonAddUser.TabIndex = 1;
            buttonAddUser.Text = "Добавить пользователя";
            buttonAddUser.UseVisualStyleBackColor = true;
            buttonAddUser.Click += buttonAddUser_Click;
            // 
            // buttonAddPT
            // 
            buttonAddPT.Location = new Point(568, 9);
            buttonAddPT.Name = "buttonAddPT";
            buttonAddPT.Size = new Size(220, 53);
            buttonAddPT.TabIndex = 2;
            buttonAddPT.Text = "Тип параметра";
            buttonAddPT.UseVisualStyleBackColor = true;
            buttonAddPT.Click += buttonAddPT_Click;
            // 
            // buttonRule
            // 
            buttonRule.Location = new Point(568, 130);
            buttonRule.Margin = new Padding(2, 2, 2, 2);
            buttonRule.Name = "buttonRule";
            buttonRule.Size = new Size(222, 54);
            buttonRule.TabIndex = 3;
            buttonRule.Text = "Правила";
            buttonRule.UseVisualStyleBackColor = true;
            buttonRule.Click += buttonRule_Click;
            // 
            // buttonService
            // 
            buttonService.Location = new Point(568, 67);
            buttonService.Margin = new Padding(2, 2, 2, 2);
            buttonService.Name = "buttonService";
            buttonService.Size = new Size(220, 58);
            buttonService.TabIndex = 4;
            buttonService.Text = "Услуги";
            buttonService.UseVisualStyleBackColor = true;
            buttonService.Click += buttonService_Click;
            // 
            // AdminForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonService);
            Controls.Add(buttonRule);
            Controls.Add(buttonAddPT);
            Controls.Add(buttonAddUser);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "AdminForm";
            Text = "AdminForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button buttonAddUser;
        private Button buttonAddPT;
        private Button buttonRule;
        private Button buttonService;
    }
}