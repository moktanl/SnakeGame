namespace SnakeGame
{
    partial class Form1
    {
        
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Form1
            // 
            AccessibleName = "";
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(584, 561);
            Cursor = Cursors.Cross;
            KeyPreview = true;
            Name = "Form1";
            Text = "Snake Game";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion
    }
}
