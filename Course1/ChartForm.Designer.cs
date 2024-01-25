namespace MotorSelectAndCheck
{
    partial class ChartForm
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
            plotView1 = new OxyPlot.WindowsForms.PlotView();
            SuspendLayout();
            // 
            // plotView1
            // 
            plotView1.Location = new Point(27, 33);
            plotView1.Name = "plotView1";
            plotView1.PanCursor = Cursors.Hand;
            plotView1.Size = new Size(1497, 884);
            plotView1.TabIndex = 0;
            plotView1.Text = "plotView1";
            plotView1.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView1.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView1.ZoomVerticalCursor = Cursors.SizeNS;
            plotView1.Click += plotView1_Click;
            // 
            // ChartForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 936);
            Controls.Add(plotView1);
            Name = "ChartForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ChartForm";
            ResumeLayout(false);
        }

        #endregion

        public OxyPlot.WindowsForms.PlotView plotView1;
    }
}