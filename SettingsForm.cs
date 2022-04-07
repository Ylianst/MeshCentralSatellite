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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeshCentralSatellite
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        public string host { get { return hostTextBox.Text; } set { hostTextBox.Text = value; updateInfo(); } }
        public string user { get { return userTextBox.Text; } set { userTextBox.Text = value; updateInfo(); } }
        public string pass { get { return passTextBox.Text; } set { passTextBox.Text = value; updateInfo(); } }
        public string ca { get { return caTextBox.Text; } set { caTextBox.Text = value; updateInfo(); } }
        public Boolean ignoreCert { get { return skipTlsCheckBox.Checked; } set { skipTlsCheckBox.Checked = value; updateInfo(); } }
        public Boolean log { get { return logCheckBox.Checked; } set { logCheckBox.Checked = value; updateInfo(); } }
        public Boolean debug { get { return debugCheckBox.Checked; } set { debugCheckBox.Checked = value; updateInfo(); } }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            updateInfo();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void updateInfo()
        {
            bool ok = true;
            if (hostTextBox.Text.Length == 0) { ok = false; }
            if (userTextBox.Text.Length == 0) { ok = false; }
            if (passTextBox.Text.Length == 0) { ok = false; }
            if (caTextBox.Text.Length != 0)
            {
                string[] splitStr = caTextBox.Text.Split('\\');
                if ((splitStr.Length != 2) || (splitStr[0].Length == 0) || (splitStr[1].Length == 0)) { ok = false; }
            }
            okButton.Enabled = ok;
        }

        private void hostTextBox_TextChanged(object sender, EventArgs e)
        {
            updateInfo();
        }

        private void hostTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            updateInfo();
        }
    }
}
