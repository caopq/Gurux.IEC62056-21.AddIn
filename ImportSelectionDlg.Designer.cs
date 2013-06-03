namespace Gurux.IEC62056_21.AddIn
{
	partial class ImportSelectionDlg
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
            this.SerialNumberLbl = new System.Windows.Forms.Label();
            this.SerialNumberTB = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // SerialNumberLbl
            // 
            this.SerialNumberLbl.AutoSize = true;
            this.SerialNumberLbl.Location = new System.Drawing.Point(12, 9);
            this.SerialNumberLbl.Name = "SerialNumberLbl";
            this.SerialNumberLbl.Size = new System.Drawing.Size(154, 13);
            this.SerialNumberLbl.TabIndex = 5;
            this.SerialNumberLbl.Text = "Meter Serial Number (Optional):";
            // 
            // SerialNumberTB
            // 
            this.SerialNumberTB.Location = new System.Drawing.Point(15, 26);
            this.SerialNumberTB.Name = "SerialNumberTB";
            this.SerialNumberTB.Size = new System.Drawing.Size(231, 20);
            this.SerialNumberTB.TabIndex = 0;
            // 
            // ImportSelectionDlg
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(296, 289);
            this.ControlBox = false;
            this.Controls.Add(this.SerialNumberTB);
            this.Controls.Add(this.SerialNumberLbl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ImportSelectionDlg";
            this.ShowInTaskbar = false;
            this.Text = "ImportSelectionDlg";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label SerialNumberLbl;
        private System.Windows.Forms.TextBox SerialNumberTB;

	}
}