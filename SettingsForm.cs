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
            SvrMainForm.MySqlLogin = settings.mysqlLogin;
            SvrMainForm.MySqlPassword = settings.mysqlPass;
            SvrMainForm.MySqlBase = settings.mysqlBase;
            SvrMainForm.MySqlHost = settings.mysqlHost;
            SvrMainForm.MySqlPort = settings.mysqlPort;
            if (mysqlPort.Text != "")
            {
                SvrMainForm.ConnectionString = conStr(mysqlHost.Text, mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text, mysqlPort.Text);
            }
            else
            {
                SvrMainForm.ConnectionString = conStr(mysqlHost.Text, mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text);
            }
            this.Close();
        }

        private void testConnection_Click(object sender, EventArgs e)
        {
            //wprowadzenie danych do logowania
            String conData;
            if (mysqlPort.Text != "")
            {
                conData = conStr(mysqlHost.Text, mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text, mysqlPort.Text);
            }
            else
            {
                conData = conStr(mysqlHost.Text, mysqlLogin.Text, mysqlPass.Text, mysqlBase.Text);
            }
            
            //utworzenie obiektu połączenia
            MySqlConnection connection = new MySqlConnection(conData);
            //próba otworzenia połączenia
            try
            {
                connection.Open();
                MessageBox.Show("Udało połączyć się z hostem", "Połączenie udane!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd połączenia z bazą danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //funkcja tworząca connection string
        public string conStr(string host, string user, string password, string dataBase)
        {
            string connectionString = "server=" + host + ";port=42789;user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }
        //funkcja tworząca connection string
        public string conStr(string host, string user, string password, string dataBase, string port)
        {
            string connectionString = "server=" + host + ";port="+port+";user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }
    }
}
