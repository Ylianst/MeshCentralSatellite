namespace MeshCentralSatellite
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel5 = new System.Windows.Forms.Panel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.consoleTabPage = new System.Windows.Forms.TabPage();
            this.mainTextBox = new System.Windows.Forms.TextBox();
            this.eventsTabPage = new System.Windows.Forms.TabPage();
            this.eventsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.eventImageList = new System.Windows.Forms.ImageList(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.mainToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.clearConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.installToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uninstallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTestComputerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeTestComputerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.devicesContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRelayMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.remoteDesktopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.askConsentBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.askConsentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.privacyBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.httpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.httpsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rdpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.panel5.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.consoleTabPage.SuspendLayout();
            this.eventsTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.mainStatusStrip.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.devicesContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.mainPanel);
            this.panel5.Controls.Add(this.pictureBox1);
            this.panel5.Controls.Add(this.mainStatusStrip);
            this.panel5.Controls.Add(this.mainMenuStrip);
            resources.ApplyResources(this.panel5, "panel5");
            this.panel5.Name = "panel5";
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.mainTabControl);
            resources.ApplyResources(this.mainPanel, "mainPanel");
            this.mainPanel.Name = "mainPanel";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.consoleTabPage);
            this.mainTabControl.Controls.Add(this.eventsTabPage);
            resources.ApplyResources(this.mainTabControl, "mainTabControl");
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            // 
            // consoleTabPage
            // 
            this.consoleTabPage.Controls.Add(this.mainTextBox);
            resources.ApplyResources(this.consoleTabPage, "consoleTabPage");
            this.consoleTabPage.Name = "consoleTabPage";
            this.consoleTabPage.UseVisualStyleBackColor = true;
            // 
            // mainTextBox
            // 
            resources.ApplyResources(this.mainTextBox, "mainTextBox");
            this.mainTextBox.Name = "mainTextBox";
            this.mainTextBox.ReadOnly = true;
            // 
            // eventsTabPage
            // 
            this.eventsTabPage.Controls.Add(this.eventsListView);
            resources.ApplyResources(this.eventsTabPage, "eventsTabPage");
            this.eventsTabPage.Name = "eventsTabPage";
            this.eventsTabPage.UseVisualStyleBackColor = true;
            // 
            // eventsListView
            // 
            this.eventsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            resources.ApplyResources(this.eventsListView, "eventsListView");
            this.eventsListView.FullRowSelect = true;
            this.eventsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.eventsListView.Name = "eventsListView";
            this.eventsListView.SmallImageList = this.eventImageList;
            this.eventsListView.UseCompatibleStateImageBehavior = false;
            this.eventsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // eventImageList
            // 
            this.eventImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("eventImageList.ImageStream")));
            this.eventImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.eventImageList.Images.SetKeyName(0, "MeshIcon50.png");
            this.eventImageList.Images.SetKeyName(1, "icons01.png");
            this.eventImageList.Images.SetKeyName(2, "icons02.png");
            this.eventImageList.Images.SetKeyName(3, "icons03.png");
            this.eventImageList.Images.SetKeyName(4, "icons04.png");
            this.eventImageList.Images.SetKeyName(5, "icons05.png");
            this.eventImageList.Images.SetKeyName(6, "icons06.png");
            this.eventImageList.Images.SetKeyName(7, "icons07.png");
            this.eventImageList.Images.SetKeyName(8, "icons08.png");
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(110)))), ((int)(((byte)(188)))));
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::MeshCentralSatellite.Properties.Resources.MC2Banner;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripStatusLabel,
            this.toolStripStatusLabel1});
            resources.ApplyResources(this.mainStatusStrip, "mainStatusStrip");
            this.mainStatusStrip.Name = "mainStatusStrip";
            // 
            // mainToolStripStatusLabel
            // 
            this.mainToolStripStatusLabel.Name = "mainToolStripStatusLabel";
            resources.ApplyResources(this.mainToolStripStatusLabel, "mainToolStripStatusLabel");
            this.mainToolStripStatusLabel.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.serviceToolStripMenuItem,
            this.testingToolStripMenuItem});
            resources.ApplyResources(this.mainMenuStrip, "mainMenuStrip");
            this.mainMenuStrip.Name = "mainMenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem5,
            this.clearConsoleToolStripMenuItem,
            this.toolStripMenuItem4,
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.toolStripMenuItem6,
            this.exitToolStripMenuItem1});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // clearConsoleToolStripMenuItem
            // 
            this.clearConsoleToolStripMenuItem.Name = "clearConsoleToolStripMenuItem";
            resources.ApplyResources(this.clearConsoleToolStripMenuItem, "clearConsoleToolStripMenuItem");
            this.clearConsoleToolStripMenuItem.Click += new System.EventHandler(this.clearConsoleToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            resources.ApplyResources(this.connectToolStripMenuItem, "connectToolStripMenuItem");
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            resources.ApplyResources(this.disconnectToolStripMenuItem, "disconnectToolStripMenuItem");
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            resources.ApplyResources(this.exitToolStripMenuItem1, "exitToolStripMenuItem1");
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.exitToolStripMenuItem1_Click);
            // 
            // serviceToolStripMenuItem
            // 
            this.serviceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem1,
            this.installToolStripMenuItem,
            this.uninstallToolStripMenuItem});
            this.serviceToolStripMenuItem.Name = "serviceToolStripMenuItem";
            resources.ApplyResources(this.serviceToolStripMenuItem, "serviceToolStripMenuItem");
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // installToolStripMenuItem
            // 
            this.installToolStripMenuItem.Name = "installToolStripMenuItem";
            resources.ApplyResources(this.installToolStripMenuItem, "installToolStripMenuItem");
            this.installToolStripMenuItem.Click += new System.EventHandler(this.installToolStripMenuItem_Click);
            // 
            // uninstallToolStripMenuItem
            // 
            this.uninstallToolStripMenuItem.Name = "uninstallToolStripMenuItem";
            resources.ApplyResources(this.uninstallToolStripMenuItem, "uninstallToolStripMenuItem");
            this.uninstallToolStripMenuItem.Click += new System.EventHandler(this.uninstallToolStripMenuItem_Click);
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createTestComputerToolStripMenuItem,
            this.removeTestComputerToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            resources.ApplyResources(this.testingToolStripMenuItem, "testingToolStripMenuItem");
            // 
            // createTestComputerToolStripMenuItem
            // 
            this.createTestComputerToolStripMenuItem.Name = "createTestComputerToolStripMenuItem";
            resources.ApplyResources(this.createTestComputerToolStripMenuItem, "createTestComputerToolStripMenuItem");
            this.createTestComputerToolStripMenuItem.Click += new System.EventHandler(this.createTestComputerToolStripMenuItem_Click);
            // 
            // removeTestComputerToolStripMenuItem
            // 
            this.removeTestComputerToolStripMenuItem.Name = "removeTestComputerToolStripMenuItem";
            resources.ApplyResources(this.removeTestComputerToolStripMenuItem, "removeTestComputerToolStripMenuItem");
            this.removeTestComputerToolStripMenuItem.Click += new System.EventHandler(this.removeTestComputerToolStripMenuItem_Click);
            // 
            // devicesContextMenuStrip
            // 
            this.devicesContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.devicesContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMapToolStripMenuItem,
            this.addRelayMapToolStripMenuItem,
            this.toolStripMenuItem3,
            this.remoteDesktopToolStripMenuItem,
            this.remoteFilesToolStripMenuItem,
            this.httpToolStripMenuItem,
            this.httpsToolStripMenuItem,
            this.rdpToolStripMenuItem,
            this.sshToolStripMenuItem,
            this.scpToolStripMenuItem});
            this.devicesContextMenuStrip.Name = "devicesContextMenuStrip";
            resources.ApplyResources(this.devicesContextMenuStrip, "devicesContextMenuStrip");
            // 
            // addMapToolStripMenuItem
            // 
            resources.ApplyResources(this.addMapToolStripMenuItem, "addMapToolStripMenuItem");
            this.addMapToolStripMenuItem.Name = "addMapToolStripMenuItem";
            // 
            // addRelayMapToolStripMenuItem
            // 
            this.addRelayMapToolStripMenuItem.Name = "addRelayMapToolStripMenuItem";
            resources.ApplyResources(this.addRelayMapToolStripMenuItem, "addRelayMapToolStripMenuItem");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // remoteDesktopToolStripMenuItem
            // 
            this.remoteDesktopToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.askConsentBarToolStripMenuItem,
            this.askConsentToolStripMenuItem,
            this.privacyBarToolStripMenuItem});
            this.remoteDesktopToolStripMenuItem.Name = "remoteDesktopToolStripMenuItem";
            resources.ApplyResources(this.remoteDesktopToolStripMenuItem, "remoteDesktopToolStripMenuItem");
            // 
            // askConsentBarToolStripMenuItem
            // 
            this.askConsentBarToolStripMenuItem.Name = "askConsentBarToolStripMenuItem";
            resources.ApplyResources(this.askConsentBarToolStripMenuItem, "askConsentBarToolStripMenuItem");
            // 
            // askConsentToolStripMenuItem
            // 
            this.askConsentToolStripMenuItem.Name = "askConsentToolStripMenuItem";
            resources.ApplyResources(this.askConsentToolStripMenuItem, "askConsentToolStripMenuItem");
            // 
            // privacyBarToolStripMenuItem
            // 
            this.privacyBarToolStripMenuItem.Name = "privacyBarToolStripMenuItem";
            resources.ApplyResources(this.privacyBarToolStripMenuItem, "privacyBarToolStripMenuItem");
            // 
            // remoteFilesToolStripMenuItem
            // 
            this.remoteFilesToolStripMenuItem.Name = "remoteFilesToolStripMenuItem";
            resources.ApplyResources(this.remoteFilesToolStripMenuItem, "remoteFilesToolStripMenuItem");
            // 
            // httpToolStripMenuItem
            // 
            this.httpToolStripMenuItem.Name = "httpToolStripMenuItem";
            resources.ApplyResources(this.httpToolStripMenuItem, "httpToolStripMenuItem");
            // 
            // httpsToolStripMenuItem
            // 
            this.httpsToolStripMenuItem.Name = "httpsToolStripMenuItem";
            resources.ApplyResources(this.httpsToolStripMenuItem, "httpsToolStripMenuItem");
            // 
            // rdpToolStripMenuItem
            // 
            this.rdpToolStripMenuItem.Name = "rdpToolStripMenuItem";
            resources.ApplyResources(this.rdpToolStripMenuItem, "rdpToolStripMenuItem");
            // 
            // sshToolStripMenuItem
            // 
            this.sshToolStripMenuItem.Name = "sshToolStripMenuItem";
            resources.ApplyResources(this.sshToolStripMenuItem, "sshToolStripMenuItem");
            // 
            // scpToolStripMenuItem
            // 
            this.scpToolStripMenuItem.Name = "scpToolStripMenuItem";
            resources.ApplyResources(this.scpToolStripMenuItem, "scpToolStripMenuItem");
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 2000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel5);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.mainMenuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.consoleTabPage.ResumeLayout(false);
            this.consoleTabPage.PerformLayout();
            this.eventsTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.devicesContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.ContextMenuStrip devicesContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem httpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem httpsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rdpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRelayMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem remoteDesktopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem remoteFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem askConsentBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem askConsentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem privacyBarToolStripMenuItem;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.TextBox mainTextBox;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel mainToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem serviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem installToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uninstallToolStripMenuItem;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage consoleTabPage;
        private System.Windows.Forms.TabPage eventsTabPage;
        private System.Windows.Forms.ListView eventsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem testingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTestComputerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeTestComputerToolStripMenuItem;
        private System.Windows.Forms.ImageList eventImageList;
        private System.Windows.Forms.ToolStripMenuItem clearConsoleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
    }
}

