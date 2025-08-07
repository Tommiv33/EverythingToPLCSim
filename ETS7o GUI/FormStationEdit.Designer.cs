namespace ETS7oGUI
{
    partial class FormStationEdit
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
            groupBox1 = new GroupBox();
            label6 = new Label();
            label5 = new Label();
            cbSlotNr = new ComboBox();
            cbRackNr = new ComboBox();
            label4 = new Label();
            btnChosePlcsimIp = new Button();
            tbName = new TextBox();
            label3 = new Label();
            tbPlcsimIpAddress = new TextBox();
            label2 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(320, 214);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(87, 27);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(225, 214);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(87, 27);
            btnOK.TabIndex = 7;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(cbSlotNr);
            groupBox1.Controls.Add(cbRackNr);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(btnChosePlcsimIp);
            groupBox1.Controls.Add(tbName);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(tbPlcsimIpAddress);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(14, 14);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(393, 189);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Station Data";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(160, 121);
            label6.Name = "label6";
            label6.Size = new Size(174, 60);
            label6.TabIndex = 49;
            label6.Text = "Position of CPU\r\n- S7-300: Always 0/2\r\n- S7-400: 0/2 or from HWKonfig\r\n- S7-1200/1500: Always 0/1\r\n";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(219, 90);
            label5.Name = "label5";
            label5.Size = new Size(18, 15);
            label5.TabIndex = 48;
            label5.Text = " / ";
            // 
            // cbSlotNr
            // 
            cbSlotNr.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSlotNr.FormattingEnabled = true;
            cbSlotNr.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18" });
            cbSlotNr.Location = new Point(247, 87);
            cbSlotNr.MaxDropDownItems = 3;
            cbSlotNr.Name = "cbSlotNr";
            cbSlotNr.Size = new Size(51, 23);
            cbSlotNr.TabIndex = 7;
            // 
            // cbRackNr
            // 
            cbRackNr.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRackNr.FormattingEnabled = true;
            cbRackNr.Items.AddRange(new object[] { "0", "1", "2", "3" });
            cbRackNr.Location = new Point(163, 87);
            cbRackNr.MaxDropDownItems = 3;
            cbRackNr.Name = "cbRackNr";
            cbRackNr.Size = new Size(51, 23);
            cbRackNr.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(17, 90);
            label4.Name = "label4";
            label4.Size = new Size(101, 15);
            label4.TabIndex = 47;
            label4.Text = "Plcsim Rack / Slot";
            // 
            // btnChosePlcsimIp
            // 
            btnChosePlcsimIp.Location = new Point(309, 51);
            btnChosePlcsimIp.Name = "btnChosePlcsimIp";
            btnChosePlcsimIp.Size = new Size(35, 26);
            btnChosePlcsimIp.TabIndex = 5;
            btnChosePlcsimIp.Text = "...";
            btnChosePlcsimIp.UseVisualStyleBackColor = true;
            btnChosePlcsimIp.Click += btnChosePlcsimIp_Click;
            // 
            // tbName
            // 
            tbName.Location = new Point(163, 22);
            tbName.MaxLength = 20;
            tbName.Name = "tbName";
            tbName.Size = new Size(138, 23);
            tbName.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 25);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 46;
            label3.Text = "Name";
            // 
            // tbPlcsimIpAddress
            // 
            tbPlcsimIpAddress.Location = new Point(163, 55);
            tbPlcsimIpAddress.MaxLength = 20;
            tbPlcsimIpAddress.Name = "tbPlcsimIpAddress";
            tbPlcsimIpAddress.Size = new Size(138, 23);
            tbPlcsimIpAddress.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 58);
            label2.Name = "label2";
            label2.Size = new Size(100, 15);
            label2.TabIndex = 43;
            label2.Text = "Plcsim IP Address";
            // 
            // FormStationEdit
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(421, 250);
            Controls.Add(groupBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormStationEdit";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Station";
            Load += FormStationEdit_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnChosePlcsimIp;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPlcsimIpAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSlotNr;
        private System.Windows.Forms.ComboBox cbRackNr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}