using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace GameServer
{
    public class GlobalMySql
    {
        /* -----------------USTAWIENIA DLA POŁĄCZENIA Z BAZĄ MYSQL----------------- */
        //obiekt ustawień aplikacji
        private Properties.Settings settings = Properties.Settings.Default;

        /*
         * Dane logowania do bazy danych wczytane z pliku konfiguracyjnego Settings.settings
         * 1. Login
         * 2. Hasło
         * 3. Baza danych
         * 4. Host
         */
        private string mysqlLogin;
        private string mysqlPass;
        private string mysqlBase;
        private string mysqlHost;
        private string mysqlPort;

        private string connectionString;

        //utworzenie obiektu połączenia
        private MySqlConnection connection;

        /* ------------------------------------------------------------------------ */

        //akcesory do obecnych ustawień bazy danych formy
        public string MySqlLogin
        {
            set
            {
                mysqlLogin = value;
            }
        }
        public string MySqlPassword
        {
            set
            {
                mysqlPass = value;
            }
        }
        public string MySqlBase
        {
            get
            {
                return mysqlBase;
            }
            set
            {
                mysqlBase = value;
            }
        }
        public string MySqlHost
        {
            set
            {
                mysqlHost = value;
            }
        }
        public string MySqlPort
        {
            set
            {
                mysqlPort = value;
            }
        }

        public GlobalMySql()
        {
            //wczytanie ustawień i utworzenie odpowiedniego connection stringa
            ConnectionStringFromSettings();

            //inicjalizacja pola połączenia
            connection = new MySqlConnection(connectionString);
        }
        public GlobalMySql(string login, string password, string database, string host)
        {
            //inicjalizacja pola połączenia
            connection = new MySqlConnection(CreateConnectionString(host, login, password, database));
        }
        public GlobalMySql(string login, string password, string database, string host, string port)
        {
            //inicjalizacja pola połączenia
            connection = new MySqlConnection(CreateConnectionString(host, login, password, database, port));
        }

        ~GlobalMySql()
        {
            connection.Close();
        }

        public MySqlConnection Connection
        {
            get
            {
                return connection;
            }
        }

        //funkcja tworząca connection string
        public string CreateConnectionString(string host, string user, string password, string dataBase)
        {
            string connectionString = "server=" + host + ";user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }
        //funkcja tworząca connection string
        public string CreateConnectionString(string host, string user, string password, string dataBase, string port)
        {
            string connectionString = "server=" + host + ";port=" + port + ";user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }

        //funkcja tworząca odpowiedni connection string na podstawie aktualnych ustawień
        private void ConnectionStringFromSettings()
        {
            //wczytanie ustawien konfiguracyjnych
            mysqlLogin = settings.mysqlLogin;
            mysqlPass = settings.mysqlPass;
            mysqlBase = settings.mysqlBase;
            mysqlHost = settings.mysqlHost;
            mysqlPort = settings.mysqlPort;

            if (mysqlPort != "")
            {
                connectionString = CreateConnectionString(mysqlHost, mysqlLogin, mysqlPass, mysqlBase, mysqlPort);
            }
            else
            {
                connectionString = CreateConnectionString(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
            }
        }

        //odświeżenie connection string przez ponowne utworzenie
        public void RefreshConnection()
        {
            //wczytanie ustawień i utworzenie odpowiedniego connection stringa
            ConnectionStringFromSettings();

            //inicjalizacja pola połączenia
            connection = new MySqlConnection(connectionString);
        }
    }
}
