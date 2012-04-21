using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

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
            mysqlPort.Text = settings.mysqlPort;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings.mysqlLogin = mysqlLogin.Text;
            settings.mysqlPass = mysqlPass.Text;
            settings.mysqlBase = mysqlBase.Text;
            settings.mysqlHost = mysqlHost.Text;
            settings.mysqlPort = mysqlPort.Text;
            settings.Save();
            SvrMainForm.dataBase.MySqlLogin = settings.mysqlLogin;
            SvrMainForm.dataBase.MySqlPassword = settings.mysqlPass;
            SvrMainForm.dataBase.MySqlBase = settings.mysqlBase;
            SvrMainForm.dataBase.MySqlHost = settings.mysqlHost;
            SvrMainForm.dataBase.MySqlPort = settings.mysqlPort;
            SvrMainForm.dataBase.RefreshConnection();
            this.Close();
        }

        private void testConnection_Click(object sender, EventArgs e)
        {
            GlobalMySql testConnection;
            if (mysqlPort.Text == "")
            {
                testConnection = new GlobalMySql(mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text, mysqlHost.Text);
            }
            else
            {
                testConnection = new GlobalMySql(mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text, mysqlHost.Text, mysqlPort.Text);
            }
            
            //próba otworzenia połączenia
            try
            {
                testConnection.Connection.Open();
                MessageBox.Show("Udało połączyć się z hostem", "Połączenie udane!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd połączenia z bazą danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
