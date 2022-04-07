/*
Copyright 2022 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;

namespace MeshCentralSatellite
{
    public partial class LoginForm : Form
    {
        private MainForm parent;
        public int currentPanel = 0;
        public X509Certificate2 lastBadConnectCert = null;

        public LoginForm(MainForm parent)
        {
            this.parent = parent;
            InitializeComponent();
            this.Controls.Add(panel1);
            this.Controls.Add(panel2);
            this.Controls.Add(panel3);
            serverNameComboBox.Text = Settings.GetRegValue("ServerName", "");
            userNameTextBox.Text = Settings.GetRegValue("UserName", "");
            if (parent.argServerName != null) { serverNameComboBox.Text = parent.argServerName; }
            if (parent.argUserName != null) { userNameTextBox.Text = parent.argUserName; }
            if (parent.argPassword != null) { passwordTextBox.Text = parent.argPassword; }
            setPanel(1);
            updateInfo();
        }

        private void setPanel(int newPanel)
        {
            if (currentPanel == newPanel) return;
            panel1.Visible = (newPanel == 1);
            panel2.Visible = (newPanel == 2);
            panel3.Visible = (newPanel == 3);
            currentPanel = newPanel;

            // Setup stuff
            if (newPanel == 1) { tokenRememberCheckBox.Checked = false; }
            nextButton2.Enabled = (tokenTextBox.Text.Replace(" ", "") != "");
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if ((serverNameComboBox.Text != "") && (userNameTextBox.Text != "")) { passwordTextBox.Focus(); }
            else if (serverNameComboBox.Text != "") { userNameTextBox.Focus(); }
            else { serverNameComboBox.Focus(); }
            if ((parent.argServerName != null) && (parent.argUserName != null) && (parent.argPassword != null) && (parent.autoLogin)) { nextButton1_Click(this, null); }
            parent.autoLogin = false;
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.loginForm = null;
        }

        private void nextButton1_Click(object sender, EventArgs e)
        {
            // Attempt to login
            parent.meshcentral = new MeshCentralServer();
            parent.meshcentral.debug = parent.debug;
            parent.meshcentral.tlsdump = parent.tlsdump;
            parent.meshcentral.ignoreCert = parent.ignoreCert;
            if (parent.acceptableCertHash != null) { parent.meshcentral.okCertHash2 = parent.acceptableCertHash; }
            parent.meshcentral.onStateChanged += Meshcentral_onStateChanged;
            if (parent.lastBadConnectCert != null)
            {
                parent.meshcentral.okCertHash = parent.lastBadConnectCert.GetCertHashString();
            }
            else
            {
                string ignoreCert = Settings.GetRegValue("IgnoreCert", null);
                if (ignoreCert != null) { parent.meshcentral.okCertHash = ignoreCert; }
            }

            // Load two factor cookie if present
            string twoFactorCookie = Settings.GetRegValue("TwoFactorCookie", null);
            if ((twoFactorCookie != null) && (twoFactorCookie != "")) { twoFactorCookie = "cookie=" + twoFactorCookie; } else { twoFactorCookie = null; }

            Uri serverurl = null;
            int keyIndex = serverNameComboBox.Text.IndexOf("?key=");
            if (keyIndex >= 0)
            {
                string hostname = serverNameComboBox.Text.Substring(0, keyIndex);
                string loginkey = serverNameComboBox.Text.Substring(keyIndex + 5);
                try { serverurl = new Uri("wss://" + hostname + "/control.ashx?key=" + loginkey); } catch (Exception) { }
                parent.meshcentral.connect(serverurl, userNameTextBox.Text, passwordTextBox.Text, twoFactorCookie);
            }
            else
            {
                try { serverurl = new Uri("wss://" + serverNameComboBox.Text + "/control.ashx"); } catch (Exception) { }
                parent.meshcentral.connect(serverurl, userNameTextBox.Text, passwordTextBox.Text, twoFactorCookie);
            }
        }

        private void updateInfo()
        {
            nextButton1.Enabled = (serverNameComboBox.Text != "") && (userNameTextBox.Text != "") && (passwordTextBox.Text != "") && (parent.meshcentral == null);
            if (parent.meshcentral == null)
            {
                nextButton1.Enabled = true;
                serverNameComboBox.Enabled = true;
                userNameTextBox.Enabled = true;
                passwordTextBox.Enabled = true;
            }
        }

        private void Meshcentral_onStateChanged(int state)
        {
            if (parent.meshcentral == null) return;
            if (this.InvokeRequired) { this.Invoke(new MeshCentralServer.onStateChangedHandler(Meshcentral_onStateChanged), state); return; }
            updateInfo();
            parent.updateInfo();

            if (state == 0)
            {
                if (parent.meshcentral.disconnectMsg == "tokenrequired")
                {
                    parent.Log("2FA token required");
                    emailTokenButton.Visible = (parent.meshcentral.disconnectEmail2FA == true) && (parent.meshcentral.disconnectEmail2FASent == false);
                    tokenEmailSentLabel.Visible = (parent.meshcentral.disconnectEmail2FASent == true) || (parent.meshcentral.disconnectSms2FASent == true);
                    smsTokenButton.Visible = ((parent.meshcentral.disconnectSms2FA == true) && (parent.meshcentral.disconnectSms2FASent == false));
                    if (parent.meshcentral.disconnectEmail2FASent) { tokenEmailSentLabel.Text = Translate.T(Properties.Resources.EmailSent); }
                    if (parent.meshcentral.disconnectSms2FASent) { tokenEmailSentLabel.Text = Translate.T(Properties.Resources.SmsSent); }
                    if ((parent.meshcentral.disconnectEmail2FA == true) && (parent.meshcentral.disconnectEmail2FASent == false))
                    {
                        smsTokenButton.Left = emailTokenButton.Left + emailTokenButton.Width + 5;
                    }
                    else
                    {
                        smsTokenButton.Left = emailTokenButton.Left;
                    }
                    tokenTextBox.Text = "";
                    if (parent.meshcentral.twoFactorCookieDays > 0)
                    {
                        tokenRememberCheckBox.Visible = true;
                        tokenRememberCheckBox.Text = string.Format(Translate.T(Properties.Resources.DontAskXDays), parent.meshcentral.twoFactorCookieDays);
                    }
                    else
                    {
                        tokenRememberCheckBox.Visible = false;
                    }

                    setPanel(2);
                    tokenTextBox.Focus();
                }
                else { setPanel(1); }

                if ((parent.meshcentral.disconnectMsg != null) && parent.meshcentral.disconnectMsg.StartsWith("noauth"))
                {
                    stateLabel.Text = Translate.T(Properties.Resources.InvalidUsernameOrPassword);
                    stateLabel.Visible = true;
                    stateClearTimer.Enabled = false;
                    stateClearTimer.Enabled = true;
                    serverNameComboBox.Focus();
                }
                else if ((parent.meshcentral.disconnectMsg != null) && parent.meshcentral.disconnectMsg.StartsWith("notools"))
                {
                    stateLabel.Text = Translate.T(Properties.Resources.NoToolsAllowed);
                    stateLabel.Visible = true;
                    stateClearTimer.Enabled = false;
                    stateClearTimer.Enabled = true;
                    serverNameComboBox.Focus();
                }
                else if ((parent.meshcentral.disconnectMsg != null) && parent.meshcentral.disconnectMsg.StartsWith("emailvalidationrequired"))
                {
                    stateLabel.Text = Translate.T(Properties.Resources.EmailVerificationRequired);
                    stateLabel.Visible = true;
                    stateClearTimer.Enabled = false;
                    stateClearTimer.Enabled = true;
                    serverNameComboBox.Focus();
                }
                else if (parent.meshcentral.disconnectMsg == "cert")
                {
                    lastBadConnectCert = parent.meshcentral.disconnectCert;
                    certDetailsTextBox.Text = "---Issuer---\r\n" + lastBadConnectCert.Issuer.Replace(", ", "\r\n") + "\r\n\r\n---Subject---\r\n" + lastBadConnectCert.Subject.Replace(", ", "\r\n");
                    setPanel(3);
                    certDetailsButton.Focus();
                }
                else if (parent.meshcentral.disconnectMsg == null) {
                    stateLabel.Text = Translate.T(Properties.Resources.UnableToConnect);
                    stateLabel.Visible = true;
                    stateClearTimer.Enabled = false;
                    stateClearTimer.Enabled = true;
                    serverNameComboBox.Focus();
                }

                // Clean up the server
                parent.meshcentral.onStateChanged -= Meshcentral_onStateChanged;
                parent.meshcentral = null;
                updateInfo();
            }
            else if (state == 1)
            {
                parent.Log("Connecting to " + serverNameComboBox.Text);
                stateLabel.Visible = false;
                nextButton1.Enabled = false;
                serverNameComboBox.Enabled = false;
                userNameTextBox.Enabled = false;
                passwordTextBox.Enabled = false;
            }
            else if (state == 2)
            {
                parent.Log("Connected to " + serverNameComboBox.Text);

                stateLabel.Visible = false;
                Settings.SetRegValue("ServerName", serverNameComboBox.Text);
                Settings.SetRegValue("UserName", userNameTextBox.Text);
                if (parent.meshcentral.username != null)
                {
                    parent.Text = parent.title + " - " + parent.meshcentral.username;
                }
                else
                {
                    parent.Text = parent.title + " - " + userNameTextBox.Text;
                }

                // If we need to remember the 2nd factor, ask for a cookie now.
                if (tokenRememberCheckBox.Checked) { parent.meshcentral.sendCommand("{\"action\":\"twoFactorCookie\"}"); }

                parent.meshcentral.onStateChanged -= Meshcentral_onStateChanged;
                parent.ConnectedToServer();

                Close();
            }
        }

        private void userNameTextBox_TextChanged(object sender, EventArgs e)
        {
            updateInfo();
        }

        private void serverNameComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { e.Handled = true; userNameTextBox.Focus(); }
        }

        private void userNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { e.Handled = true; passwordTextBox.Focus(); }
        }

        private void passwordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { e.Handled = true; if (nextButton1.Enabled) { nextButton1_Click(this, null); } }
        }

        private void tokenTextBox_TextChanged(object sender, EventArgs e)
        {
            nextButton2.Enabled = (tokenTextBox.Text.Replace(" ", "") != "");
        }

        private void tokenTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            nextButton2.Enabled = (tokenTextBox.Text.Replace(" ", "") != "");
        }

        private void tokenTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) { nextButton2_Click(this, null); e.Handled = true; }
        }

        private void emailTokenButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Translate.T(Properties.Resources.SendTokenEmail), Translate.T(Properties.Resources.TwoFactorAuthentication), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                parent.sendEmailToken = true;
                parent.sendSMSToken = false;
                nextButton2_Click(this, null);
            }
        }

        private void smsTokenButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, Translate.T(Properties.Resources.SendTokenSMS), Translate.T(Properties.Resources.TwoFactorAuthentication), MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                parent.sendEmailToken = false;
                parent.sendSMSToken = true;
                nextButton2_Click(this, null);
            }
        }

        private void nextButton2_Click(object sender, EventArgs e)
        {
            if ((tokenTextBox.Text.Replace(" ", "") == "") && (parent.sendEmailToken == false) && (parent.sendSMSToken == false)) return;

            Uri serverurl = null;
            int keyIndex = serverNameComboBox.Text.IndexOf("?key=");
            if (keyIndex >= 0)
            {
                string hostname = serverNameComboBox.Text.Substring(0, keyIndex);
                string loginkey = serverNameComboBox.Text.Substring(keyIndex + 5);
                serverurl = new Uri("wss://" + hostname + "/control.ashx?key=" + loginkey);
            }
            else
            {
                serverurl = new Uri("wss://" + serverNameComboBox.Text + "/control.ashx");
            }

            parent.meshcentral = new MeshCentralServer();
            parent.meshcentral.debug = parent.debug;
            parent.meshcentral.tlsdump = parent.tlsdump;
            parent.meshcentral.ignoreCert = parent.ignoreCert;
            if (parent.lastBadConnectCert != null)
            {
                parent.meshcentral.okCertHash = parent.lastBadConnectCert.GetCertHashString();
            }
            else
            {
                string ignoreCert = Settings.GetRegValue("IgnoreCert", null);
                if (ignoreCert != null) { parent.meshcentral.okCertHash = ignoreCert; }
            }
            parent.meshcentral.onStateChanged += Meshcentral_onStateChanged;
            if (parent.sendEmailToken == true)
            {
                parent.sendEmailToken = false;
                parent.meshcentral.connect(serverurl, userNameTextBox.Text, passwordTextBox.Text, "**email**");
            }
            else if (parent.sendSMSToken == true)
            {
                parent.sendSMSToken = false;
                parent.meshcentral.connect(serverurl, userNameTextBox.Text, passwordTextBox.Text, "**sms**");
            }
            else
            {
                parent.meshcentral.connect(serverurl, userNameTextBox.Text, passwordTextBox.Text, tokenTextBox.Text.Replace(" ", ""));
            }
        }

        private void stateClearTimer_Tick(object sender, EventArgs e)
        {
            stateLabel.Visible = false;
            stateClearTimer.Enabled = false;
        }

        private void backButton2_Click(object sender, EventArgs e)
        {
            updateInfo();
            setPanel(1);
        }

        private void backButton3_Click(object sender, EventArgs e)
        {
            lastBadConnectCert = null;
            if (parent.meshcentral != null)
            {
                // Clean up the server
                parent.meshcentral.onStateChanged -= Meshcentral_onStateChanged;
                parent.meshcentral = null;
                try { parent.meshcentral.disconnect(); } catch (Exception) { }
            }
            stateLabel.Visible = false;
            updateInfo();
            setPanel(1);
        }

        private void certDetailsButton_Click(object sender, EventArgs e)
        {
            X509Certificate2UI.DisplayCertificate(lastBadConnectCert);
        }

        private void nextButton3_Click(object sender, EventArgs e)
        {
            parent.lastBadConnectCert = lastBadConnectCert;

            // If we need to remember this certificate
            if (rememberCertCheckBox.Checked) { Settings.SetRegValue("IgnoreCert", parent.lastBadConnectCert.GetCertHashString()); }

            nextButton1_Click(this, null);
        }
    }
}
