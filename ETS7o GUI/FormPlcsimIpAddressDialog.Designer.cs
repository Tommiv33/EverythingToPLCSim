namespace ETS7oGUI
{
    partial class FormPlcsimIpAddressDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            btnCancel = new Button();
            btnOK = new Button();
            listBoxPlcsimIpAddresses = new ListBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(561, 725);
            btnCancel.Margin = new Padding(8, 9, 8, 9);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(212, 73);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Enabled = false;
            btnOK.Location = new Point(332, 725);
            btnOK.Margin = new Padding(8, 9, 8, 9);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(212, 73);
            btnOK.TabIndex = 5;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // listBoxPlcsimIpAddresses
            // 
            listBoxPlcsimIpAddresses.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBoxPlcsimIpAddresses.FormattingEnabled = true;
            listBoxPlcsimIpAddresses.ItemHeight = 41;
            listBoxPlcsimIpAddresses.Location = new Point(34, 79);
            listBoxPlcsimIpAddresses.Margin = new Padding(8, 9, 8, 9);
            listBoxPlcsimIpAddresses.Name = "listBoxPlcsimIpAddresses";
            listBoxPlcsimIpAddresses.Size = new Size(732, 619);
            listBoxPlcsimIpAddresses.TabIndex = 7;
            listBoxPlcsimIpAddresses.SelectedIndexChanged += listBoxPlcsimIpAddresses_SelectedIndexChanged;
            listBoxPlcsimIpAddresses.MouseDoubleClick += listBoxPlcsimIpAddresses_MouseDoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 28);
            label1.Margin = new Padding(8, 0, 8, 0);
            label1.Name = "label1";
            label1.Size = new Size(429, 41);
            label1.TabIndex = 8;
            label1.Text = "Network reachable Plcsim PLCs";
            // 
            // FormPlcsimIpAddressDialog
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(808, 823);
            Controls.Add(label1);
            Controls.Add(listBoxPlcsimIpAddresses);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(8, 9, 8, 9);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPlcsimIpAddressDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Network reachable PLC";
            Load += FormPlcsimIpAddressDialog_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ListBox listBoxPlcsimIpAddresses;
        private System.Windows.Forms.Label label1;
    }
}