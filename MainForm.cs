using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using Commands;

namespace GameServer
{
    public partial class MainForm : Form
    {
        //flaga informująca czy serwer działa czy jest wyłączony
        private bool isRunning;

        //listener nasłuchujący żądań klientów
        private TcpListener server;

        //ustawienie kodera/dekodera
        private UTF8Encoding code;

        //wątek obsługujący listenera przyłączającego klientów
        private Thread listenerTh;

        //obiekt ustawień aplikacji
        private Properties.Settings settings = Properties.Settings.Default;

        //obiekt przechowujący próby dostępu do obsługi 

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

        //delegat funkcji przyjmującej jako argument stringa
        private delegate void SetString(string str);

        //obiekt przechowujący obiekty ubiegające się o dostęp do wątku
        private Object Sync;

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

        public MainForm()
        {
            InitializeComponent();

            Sync = new Object();
            //domyślnie serwer jest wyłączony
            isRunning = false;

            //utworzenie gniazda serwera
            try
            {
                server = new TcpListener(IPAddress.Any, 8001);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie udało się utworzyć gniazda serwera. Dalsze korzystanie z aplikacji może generować błędy! Uruchom aplikację jeszcze raz.\nDebuger message:\n" + ex.ToString(), "Błąd tworzenia gniazda serwera!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //wczytanie ustawien konfiguracyjnych
            mysqlLogin = settings.mysqlLogin;
            mysqlPass = settings.mysqlPass;
            mysqlBase = settings.mysqlBase;
            mysqlHost = settings.mysqlHost;

        }

        //dodanie tekstu do okna logów synchronicznie
        private void addLog(string log)
        {
            logs.AppendText(log + "\n");
        }

        //dodanie tekstu do okna logów asynchronicznie
        private void addLogAsynch(string str)
        {
            Invoke
            (
                new SetString(addLog),
                new Object[] { str }
            );
        }

        //funkcja tworząca connection string
        public string conStr(string host, string user, string password, string dataBase)
        {
            string connectionString = "server=" + host + ";user id=" + user + "; pwd=" + password + ";database=" + dataBase + ";";
            return connectionString;
        }

        //zamiana stringa cmd na akcję i ciąg argumentów
        private string[] cmdToArgs(string command)
        {
            string[] args = command.Split(';');
            return args;
        }

        //metoda włączająca/wyłączająca serwer
        private void switchSvr_Click(object sender, EventArgs e)
        {
            if (isRunning) //jeżeli działa
            {
                //to przy wyłączaniu ustaw przycisk do włączania
                switchSvr.Text = "Włącz";

                //ustaw, że serwer jest nieaktywny
                isRunning = false;

                //wstrzymanie wątku głównego do czasu zakończenia listenerTh i włączenie go do głównego
                listenerTh.Join();

                addLog("[Serwer]: Serwer zakończył nasłuchiwanie");
            }
            else //jeżeli serwer nie jest w trakcie działania
            {
                //to podczas uruchamiania ustaw przycisk do wyłączania
                switchSvr.Text = "Wyłącz";

                //ustaw, że serwer jest aktywny
                isRunning = true;

                //ustawienie i uruchomienie nasłuchiwania listen() w nowym wątku
                listenerTh = new Thread(listen);
                listenerTh.Priority = ThreadPriority.BelowNormal;
                listenerTh.IsBackground = true;
                listenerTh.Start();
            }
        }

        //funkcja logowanie do serwera
        //gdy dane logowanie są poprawne, to zwraca identyfikator gracza
        //w przeciwnym wypadku zwraca 0
        private ulong login(string login, string md5pass)
        {
            //wprowadzenie danych do logowania
            String conData = conStr(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
            //utworzenie obiektu połączenia
            MySqlConnection connection = new MySqlConnection(conData);
            //próba otworzenia połączenia
            try
            {
                connection.Open();
            }
            catch
            {
                //MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            //zdefiniowanie zmiennej polecenia w obrębie obiektu połączenia connection
            MySqlCommand polecenie = connection.CreateCommand();
            //utworzenie zapytania
            polecenie.CommandText = "SELECT id FROM `player` WHERE `player`.`login`='" + login + "' AND password='" + md5pass + "'";
            //StringBuilder builder = new StringBuilder();
            try
            {
                using (MySqlDataReader reader = polecenie.ExecuteReader())
                {
                    if (reader.HasRows) //jeżeli wybrało wiersze z bazy
                    {
                        while (reader.Read())
                        {
                            //MessageBox.Show("Identyfikator gracza: " + reader.GetString(0) + " Login: " + reader.GetString(2) + " Pole nr 1: " + reader.GetString(1));
                            //connectionInfo.AppendText("\n");
                            //utworzenie nowego wątku, uruchamiającego nową aplikację
                            //new Interface(reader.GetInt32(0)).Show();
                            return reader.GetUInt64("id");
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Podano błędny login lub hasło. Spróbuj jeszcze raz podając poprawne dane lub skorzystaj z opcji przypomnienia hasła.", "Nieudane logowanie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
            connection.Close();
            return 0;
        }

        private bool getPlayerData(ulong player_id, ref string[] data)
        {
            //wprowadzenie danych do logowania
            String conData = conStr(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
            //utworzenie obiektu połączenia
            MySqlConnection connection = new MySqlConnection(conData);
            //próba otworzenia połączenia
            try
            {
                connection.Open();
            }
            catch
            {
                //MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            //zdefiniowanie zmiennej polecenia w obrębie obiektu połączenia connection
            MySqlCommand polecenie = connection.CreateCommand();
            //utworzenie zapytania
            polecenie.CommandText = "SELECT * FROM `player` WHERE `player`.`id`='" + player_id + "'";
            //StringBuilder builder = new StringBuilder();
            try
            {
                using (MySqlDataReader reader = polecenie.ExecuteReader())
                {
                    if (reader.HasRows) //jeżeli wybrało wiersze z bazy
                    {
                        while (reader.Read())
                        {
                            //MessageBox.Show("Identyfikator gracza: " + reader.GetString(0) + " Login: " + reader.GetString(2) + " Pole nr 1: " + reader.GetString(1));
                            //connectionInfo.AppendText("\n");
                            //utworzenie nowego wątku, uruchamiającego nową aplikację
                            //new Interface(reader.GetInt32(0)).Show();
                            //return reader.GetUInt64(0);
                            data = new string[reader.FieldCount - 1];
                            data[0] = reader.GetString("login");
                            data[1] = reader.GetString("password");
                            data[2] = reader.GetString("access");
                            data[3] = reader.GetString("email");
                            return true;
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Podano błędny login lub hasło. Spróbuj jeszcze raz podając poprawne dane lub skorzystaj z opcji przypomnienia hasła.", "Nieudane logowanie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
            connection.Close();
            return false;
        }

        //nasłuchiwanie i dodawanie klientów
        private void listen()
        {
            addLogAsynch("[Serwer]: Serwer rozpoczął nasłuchiwanie");
            //dopóki zmienna sterująca stanem działania serwera jest ustawiona na true
            //dopóki serwer nasłuchuje
            while (isRunning)
            {
                try
                {
                    //wystartuj serwer aby rozpocząć nasłuchiwanie
                    server.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Błąd startu serwera!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                try
                {
                    //sprawdzenie czy ktoś oczekuje na obsłużenie
                    if (server.Pending())
                    {
                        //utworzenie nowego wątku klienta
                        Thread klientTh = new Thread(service);
                        klientTh.Priority = ThreadPriority.BelowNormal;
                        
                        //wątek kończy się wraz z zakończeniem wątku, który go wywołał
                        klientTh.IsBackground = true;
                        //uruchomienie wątku - jako argument, gniazdo do komunikacji z klientem
                        klientTh.Start(server.AcceptSocket());
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Błąd tworzenia wątku klienta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            Thread.Sleep(1); //potrzebne aby oszczędzić czasu CPU
            server.Stop();
        }

        private string CreateMySqlUpdateQuery(string[] args)
        {
            /*
             * UAKTUALNIANIE PÓL BAZY DANYCH
             * kolejność danych: komenda, identyfikator, tabela, pola, wartości
             */
            string UpdateQuery = "";
            int fieldsCount = args.Length - 3;
            if ((fieldsCount % 2) == 0)
            {
                UpdateQuery = "UPDATE `" + mysqlBase + "`.`" + args[2] + "` SET ";
                for (int i = 0; i < fieldsCount / 2; i++)
                {
                    UpdateQuery += "`" + args[i + 3] + "` = '" + args[i + 3 + fieldsCount / 2] + "', ";
                }
                UpdateQuery = UpdateQuery.Remove(UpdateQuery.Length - 2, 2);
                UpdateQuery += " WHERE `" + args[2] + "`.`id` = " + args[1] + "";
            }
            return UpdateQuery;
        }

        private void ExecuteQuery(object query)
        {
            //wprowadzenie danych do logowania
            String conData = conStr(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
            //utworzenie obiektu połączenia
            MySqlConnection connection = new MySqlConnection(conData);
            //próba otworzenia połączenia
            try
            {
                connection.Open();

                //zdefiniowanie zmiennej polecenia w obrębie obiektu połączenia connection
                MySqlCommand polecenie = connection.CreateCommand();
                //utworzenie zapytania
                polecenie.CommandText = (string)query;

                polecenie.ExecuteNonQuery();
            }
            catch
            {
                //MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        //właściwa obsługa klienta
        private void service(object s)
        {
            Monitor.Enter(Sync);
            try
            {
                Socket socket = s as Socket;
                code = new UTF8Encoding();
                string clientName = socket.RemoteEndPoint.ToString();
                bool successLog = false;
                byte[] buf = new byte[4096];

                //addLogAsynch("[Klient]: Klient " + clientName + " połączył się z serwerem.");

                while (socket.Connected && isRunning && IsConnected(socket))
                {
                    if (socket.Available > 0)
                    {
                        //komenda wczytana z bufora
                        string cmd = code.GetString(buf, 0, socket.Receive(buf));

                        //utworzenie obiektu komendy
                        Command response = new Command();

                        //zamiana lini komendy na nazwę akcji args[0] i argumenty - reszta tablicy
                        string[] args = cmdToArgs(cmd);

                        switch (args[0])
                        {
                            /*
                             * LOGOWANIE
                             */
                            case ClientCmd.LOGIN:
                                ulong userID = login(args[1], args[2]);
                                if (userID == 0)
                                {
                                    addLogAsynch("[Klient]: Nieudana próba logowania (login: " + args[1] + ")");
                                }
                                else
                                {
                                    addLogAsynch("[ " + args[1] + " ]: Udane logowanie. ID = " + userID);
                                    clientName = args[1];
                                    successLog = true;
                                }
                                //utworzenie odpowiedzi
                                response.Request(ClientCmd.LOGIN);
                                response.Add(userID.ToString());
                                response.Apply(socket);

                                //wysłanie odpowiedzi
                                //socket.Send(response.Byte);
                                break;

                            /*
                             * WYSYŁANIE DANYCH GRACZA
                             * kolejność danych: komenda, login, hasło, dostęp, email
                             */
                            case ClientCmd.GET_PLAYER_DATA:
                                string[] dane = new string[3];
                                while (!getPlayerData(ulong.Parse(args[1]), ref dane))
                                {
                                    Thread.Sleep(1);
                                }
                                //utworzenie odpowiedzi
                                response.Request(ServerCmd.PLAYER_DATA);
                                response.Add(dane);
                                response.Apply();

                                socket.Send(response.Byte);
                                addLogAsynch("[ " + dane[0] + " ]: Pobrał dane gracza.");
                                break;

                            /*
                             * UAKTUALNIANIE PÓL BAZY DANYCH
                             * kolejność danych: komenda, identyfikator, tabela, pola, wartości
                             */
                            case ClientCmd.UPDATE_DATA_BASE:
                                string UpdateQuery = CreateMySqlUpdateQuery(args);
                                ExecuteQuery(UpdateQuery);
                                //utworzenie odpowiedzi
                                response.Request(ServerCmd.DATA_BASE_UPDATED);
                                response.Apply(socket);
                                break;

                            /*case ClientCmd.GET_DUPA:
                                string connectionString = conStr(mysqlHost, mysqlLogin, mysqlPass, mysqlBase);
                                MySqlConnection connection = new MySqlConnection(connectionString);
                                connection.Open();

                                MySqlCommand query = connection.CreateCommand();
                                query.CommandText = "SELECT wartosc FROM `dupy` WHERE `id` = '" + args[1] + "'";
                                MySqlDataReader reader = query.ExecuteReader();
                                while (reader.Read())
                                {
                                    addLogAsynch("Wartość dupy: " + reader.GetString("wartosc"));
                                }
                                break;*/
                            default:
                                addLogAsynch("[!][Klient]: Odebrano nieznaną komendę!");
                                break;
                        }
                        response.Clear();
                    }
                }
                if (successLog)
                {
                    addLogAsynch("[ " + clientName + " ]: Gracz rozłączył się z serwerem.");
                }
            }
            finally
            {
                Monitor.Exit(Sync);
            }
        }

        //sprawdzenie czy na gnieździe nasłuchuje jeszcze klient
        private bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        private void ustawieniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm ustawienia = new SettingsForm(this);
            ustawienia.ShowDialog(this);
        }
    }
}
