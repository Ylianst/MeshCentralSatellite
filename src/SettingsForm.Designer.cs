namespace MeshCentralSatellite
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.devNameComboBox = new System.Windows.Forms.ComboBox();
            this.skipTlsCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.passTextBox = new System.Windows.Forms.TextBox();
            this.userTextBox = new System.Windows.Forms.TextBox();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.caTextBox = new System.Windows.Forms.TextBox();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.debugCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.templateComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkCaButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.certCommonNameComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.uuidCheckBox = new System.Windows.Forms.CheckBox();
            this.samCheckBox = new System.Windows.Forms.CheckBox();
            this.userCheckBox = new System.Windows.Forms.CheckBox();
            this.hostCheckBox = new System.Windows.Forms.CheckBox();
            this.dnsCheckBox = new System.Windows.Forms.CheckBox();
            this.dnCheckBox = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.securityGroupsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.devNameComboBox);
            this.groupBox1.Controls.Add(this.skipTlsCheckBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.passTextBox);
            this.groupBox1.Controls.Add(this.userTextBox);
            this.groupBox1.Controls.Add(this.hostTextBox);
            this.groupBox1.Location = new System.Drawing.Point(15, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MeshCentral Login";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 101);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "Device Name";
            // 
            // devNameComboBox
            // 
            this.devNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.devNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.devNameComboBox.FormattingEnabled = true;
            this.devNameComboBox.Items.AddRange(new object[] {
            "Operating System Name",
            "Node Identifier"});
            this.devNameComboBox.Location = new System.Drawing.Point(116, 98);
            this.devNameComboBox.Name = "devNameComboBox";
            this.devNameComboBox.Size = new System.Drawing.Size(228, 21);
            this.devNameComboBox.TabIndex = 14;
            // 
            // skipTlsCheckBox
            // 
            this.skipTlsCheckBox.AutoSize = true;
            this.skipTlsCheckBox.Location = new System.Drawing.Point(116, 125);
            this.skipTlsCheckBox.Name = "skipTlsCheckBox";
            this.skipTlsCheckBox.Size = new System.Drawing.Size(152, 17);
            this.skipTlsCheckBox.TabIndex = 6;
            this.skipTlsCheckBox.Text = "Skip TLS certificate check";
            this.skipTlsCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Login Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Login Username";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Server Hostname";
            // 
            // passTextBox
            // 
            this.passTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.passTextBox.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.passTextBox.Location = new System.Drawing.Point(116, 72);
            this.passTextBox.Name = "passTextBox";
            this.passTextBox.PasswordChar = '';
            this.passTextBox.Size = new System.Drawing.Size(228, 20);
            this.passTextBox.TabIndex = 2;
            this.passTextBox.TextChanged += new System.EventHandler(this.hostTextBox_TextChanged);
            this.passTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hostTextBox_KeyUp);
            // 
            // userTextBox
            // 
            this.userTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userTextBox.Location = new System.Drawing.Point(116, 46);
            this.userTextBox.Name = "userTextBox";
            this.userTextBox.Size = new System.Drawing.Size(228, 20);
            this.userTextBox.TabIndex = 1;
            this.userTextBox.TextChanged += new System.EventHandler(this.hostTextBox_TextChanged);
            this.userTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hostTextBox_KeyUp);
            // 
            // hostTextBox
            // 
            this.hostTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hostTextBox.Location = new System.Drawing.Point(116, 20);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(228, 20);
            this.hostTextBox.TabIndex = 0;
            this.hostTextBox.TextChanged += new System.EventHandler(this.hostTextBox_TextChanged);
            this.hostTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hostTextBox_KeyUp);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Authority Name";
            // 
            // caTextBox
            // 
            this.caTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.caTextBox.Location = new System.Drawing.Point(117, 24);
            this.caTextBox.Name = "caTextBox";
            this.caTextBox.Size = new System.Drawing.Size(196, 20);
            this.caTextBox.TabIndex = 9;
            this.caTextBox.TextChanged += new System.EventHandler(this.hostTextBox_TextChanged);
            this.caTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hostTextBox_KeyUp);
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Location = new System.Drawing.Point(10, 28);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(82, 17);
            this.logCheckBox.TabIndex = 7;
            this.logCheckBox.Text = "Write log.txt";
            this.logCheckBox.UseVisualStyleBackColor = true;
            // 
            // debugCheckBox
            // 
            this.debugCheckBox.AutoSize = true;
            this.debugCheckBox.Location = new System.Drawing.Point(184, 28);
            this.debugCheckBox.Name = "debugCheckBox";
            this.debugCheckBox.Size = new System.Drawing.Size(98, 17);
            this.debugCheckBox.TabIndex = 8;
            this.debugCheckBox.Text = "Write debug.txt";
            this.debugCheckBox.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(651, 341);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(570, 341);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Template";
            // 
            // templateComboBox
            // 
            this.templateComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.templateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.templateComboBox.FormattingEnabled = true;
            this.templateComboBox.Location = new System.Drawing.Point(117, 49);
            this.templateComboBox.Name = "templateComboBox";
            this.templateComboBox.Size = new System.Drawing.Size(226, 21);
            this.templateComboBox.TabIndex = 13;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.checkCaButton);
            this.groupBox2.Controls.Add(this.templateComboBox);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.caTextBox);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(349, 82);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Certificate Authority";
            // 
            // checkCaButton
            // 
            this.checkCaButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkCaButton.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.checkCaButton.Location = new System.Drawing.Point(317, 24);
            this.checkCaButton.Name = "checkCaButton";
            this.checkCaButton.Size = new System.Drawing.Size(26, 20);
            this.checkCaButton.TabIndex = 7;
            this.checkCaButton.Text = "";
            this.checkCaButton.UseVisualStyleBackColor = true;
            this.checkCaButton.Click += new System.EventHandler(this.checkCaButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.logCheckBox);
            this.groupBox3.Controls.Add(this.debugCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(3, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(349, 55);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Logging";
            // 
            // certCommonNameComboBox
            // 
            this.certCommonNameComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.certCommonNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.certCommonNameComboBox.FormattingEnabled = true;
            this.certCommonNameComboBox.Items.AddRange(new object[] {
            "Distinguished Name",
            "DNS FQDN",
            "Hostname",
            "User Principal Name",
            "SAM Account Name",
            "UUID"});
            this.certCommonNameComboBox.Location = new System.Drawing.Point(118, 19);
            this.certCommonNameComboBox.Name = "certCommonNameComboBox";
            this.certCommonNameComboBox.Size = new System.Drawing.Size(225, 21);
            this.certCommonNameComboBox.TabIndex = 13;
            this.certCommonNameComboBox.SelectedIndexChanged += new System.EventHandler(this.hostTextBox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Common Name";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.certCommonNameComboBox);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.uuidCheckBox);
            this.groupBox4.Controls.Add(this.samCheckBox);
            this.groupBox4.Controls.Add(this.userCheckBox);
            this.groupBox4.Controls.Add(this.hostCheckBox);
            this.groupBox4.Controls.Add(this.dnsCheckBox);
            this.groupBox4.Controls.Add(this.dnCheckBox);
            this.groupBox4.Location = new System.Drawing.Point(3, 91);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(349, 147);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Certificate Configuration";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(93, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Alternative Names";
            // 
            // uuidCheckBox
            // 
            this.uuidCheckBox.AutoSize = true;
            this.uuidCheckBox.Checked = true;
            this.uuidCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uuidCheckBox.Location = new System.Drawing.Point(183, 121);
            this.uuidCheckBox.Name = "uuidCheckBox";
            this.uuidCheckBox.Size = new System.Drawing.Size(53, 17);
            this.uuidCheckBox.TabIndex = 7;
            this.uuidCheckBox.Text = "UUID";
            this.uuidCheckBox.UseVisualStyleBackColor = true;
            // 
            // samCheckBox
            // 
            this.samCheckBox.AutoSize = true;
            this.samCheckBox.Checked = true;
            this.samCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.samCheckBox.Location = new System.Drawing.Point(183, 98);
            this.samCheckBox.Name = "samCheckBox";
            this.samCheckBox.Size = new System.Drawing.Size(123, 17);
            this.samCheckBox.TabIndex = 6;
            this.samCheckBox.Text = "SAM Account Name";
            this.samCheckBox.UseVisualStyleBackColor = true;
            // 
            // userCheckBox
            // 
            this.userCheckBox.AutoSize = true;
            this.userCheckBox.Checked = true;
            this.userCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.userCheckBox.Location = new System.Drawing.Point(183, 75);
            this.userCheckBox.Name = "userCheckBox";
            this.userCheckBox.Size = new System.Drawing.Size(122, 17);
            this.userCheckBox.TabIndex = 5;
            this.userCheckBox.Text = "User Principal Name";
            this.userCheckBox.UseVisualStyleBackColor = true;
            // 
            // hostCheckBox
            // 
            this.hostCheckBox.AutoSize = true;
            this.hostCheckBox.Checked = true;
            this.hostCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.hostCheckBox.Location = new System.Drawing.Point(9, 121);
            this.hostCheckBox.Name = "hostCheckBox";
            this.hostCheckBox.Size = new System.Drawing.Size(74, 17);
            this.hostCheckBox.TabIndex = 4;
            this.hostCheckBox.Text = "Hostname";
            this.hostCheckBox.UseVisualStyleBackColor = true;
            // 
            // dnsCheckBox
            // 
            this.dnsCheckBox.AutoSize = true;
            this.dnsCheckBox.Checked = true;
            this.dnsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dnsCheckBox.Location = new System.Drawing.Point(9, 98);
            this.dnsCheckBox.Name = "dnsCheckBox";
            this.dnsCheckBox.Size = new System.Drawing.Size(82, 17);
            this.dnsCheckBox.TabIndex = 3;
            this.dnsCheckBox.Text = "DNS FQDN";
            this.dnsCheckBox.UseVisualStyleBackColor = true;
            // 
            // dnCheckBox
            // 
            this.dnCheckBox.AutoSize = true;
            this.dnCheckBox.Checked = true;
            this.dnCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.dnCheckBox.Location = new System.Drawing.Point(9, 75);
            this.dnCheckBox.Name = "dnCheckBox";
            this.dnCheckBox.Size = new System.Drawing.Size(120, 17);
            this.dnCheckBox.TabIndex = 2;
            this.dnCheckBox.Text = "Distinguished Name";
            this.dnCheckBox.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Size = new System.Drawing.Size(714, 323);
            this.splitContainer1.SplitterDistance = 355;
            this.splitContainer1.TabIndex = 16;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.securityGroupsCheckedListBox);
            this.groupBox5.Location = new System.Drawing.Point(3, 156);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(349, 164);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Security Groups";
            // 
            // securityGroupsCheckedListBox
            // 
            this.securityGroupsCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.securityGroupsCheckedListBox.FormattingEnabled = true;
            this.securityGroupsCheckedListBox.Location = new System.Drawing.Point(9, 19);
            this.securityGroupsCheckedListBox.Name = "securityGroupsCheckedListBox";
            this.securityGroupsCheckedListBox.Size = new System.Drawing.Size(334, 139);
            this.securityGroupsCheckedListBox.Sorted = true;
            this.securityGroupsCheckedListBox.TabIndex = 0;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(738, 376);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox debugCheckBox;
        private System.Windows.Forms.CheckBox skipTlsCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox passTextBox;
        private System.Windows.Forms.TextBox userTextBox;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox caTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox templateComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox certCommonNameComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox uuidCheckBox;
        private System.Windows.Forms.CheckBox samCheckBox;
        private System.Windows.Forms.CheckBox userCheckBox;
        private System.Windows.Forms.CheckBox hostCheckBox;
        private System.Windows.Forms.CheckBox dnsCheckBox;
        private System.Windows.Forms.CheckBox dnCheckBox;
        private System.Windows.Forms.Button checkCaButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox devNameComboBox;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckedListBox securityGroupsCheckedListBox;
    }
}