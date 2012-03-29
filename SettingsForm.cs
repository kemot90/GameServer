using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameServer
{
    public partial class SettingsForm : Form
    {
        private Properties.Settings settings = Properties.Settings.Default;
        private MainForm SvrMainForm;
        public SettingsForm(MainForm ServerMainForm)
        {
            InitializeComponent();
            SvrMainForm = ServerMainForm;
            mysqlLogin.Text = settings.mysqlLogin;
            mysqlPass.Text = settings.mysqlPass;
            mysqlBase.Text = settings.mysqlBase;
            mysqlHost.Text = settings.mysqlHost;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings.mysqlLogin = mysqlLogin.Text;
            settings.mysqlPass = mysqlPass.Text;
            settings.mysqlBase = mysqlBase.Text;
            settings.mysqlHost = mysqlHost.Text;
            settings.Save();
            SvrMainForm.MySqlLogin = settings.mysqlLogin;
            SvrMainForm.MySqlPassword = settings.mysqlPass;
            SvrMainForm.MySqlBase = settings.mysqlBase;
            SvrMainForm.MySqlHost = settings.mysqlHost;
            this.Close();
        }
    }
}
