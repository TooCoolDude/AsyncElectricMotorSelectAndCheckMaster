namespace MotorSelectAndCheck
{
    partial class MotorCalculationForm
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
            labelPras = new Label();
            labelResult = new Label();
            textBoxResult = new RichTextBox();
            selectMotorBox = new ComboBox();
            CheckMotorButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(249, 32);
            label1.TabIndex = 0;
            label1.Text = "Расчетная мощность:";
            // 
            // labelPras
            // 
            labelPras.AutoSize = true;
            labelPras.Location = new Point(267, 9);
            labelPras.Name = "labelPras";
            labelPras.Size = new Size(57, 32);
            labelPras.TabIndex = 1;
            labelPras.Text = "Pras";
            // 
            // labelResult
            // 
            labelResult.AutoSize = true;
            labelResult.Location = new Point(12, 497);
            labelResult.Name = "labelResult";
            labelResult.Size = new Size(78, 32);
            labelResult.TabIndex = 2;
            labelResult.Text = "Result";
            // 
            // textBoxResult
            // 
            textBoxResult.Location = new Point(12, 548);
            textBoxResult.Name = "textBoxResult";
            textBoxResult.Size = new Size(196, 85);
            textBoxResult.TabIndex = 3;
            textBoxResult.Text = "";
            // 
            // selectMotorBox
            // 
            selectMotorBox.FormattingEnabled = true;
            selectMotorBox.Location = new Point(19, 126);
            selectMotorBox.Name = "selectMotorBox";
            selectMotorBox.Size = new Size(339, 40);
            selectMotorBox.TabIndex = 4;
            // 
            // CheckMotorButton
            // 
            CheckMotorButton.Location = new Point(395, 120);
            CheckMotorButton.Name = "CheckMotorButton";
            CheckMotorButton.Size = new Size(150, 46);
            CheckMotorButton.TabIndex = 5;
            CheckMotorButton.Text = "Рассчитать";
            CheckMotorButton.UseVisualStyleBackColor = true;
            CheckMotorButton.Click += CheckMotorButton_Click;
            // 
            // MotorCalculationForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(974, 929);
            Controls.Add(CheckMotorButton);
            Controls.Add(selectMotorBox);
            Controls.Add(textBoxResult);
            Controls.Add(labelResult);
            Controls.Add(labelPras);
            Controls.Add(label1);
            Name = "MotorCalculationForm";
            Text = "Выбор и проверка электродвигателя";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        public Label labelPras;
        private Label labelResult;
        public RichTextBox textBoxResult;
        public ComboBox selectMotorBox;
        public Button CheckMotorButton;
    }
}