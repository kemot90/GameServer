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

        //utworzenie obiektu zawierającego ustawienia i połączenie z bazą danych
        public GlobalMySql dataBase;

        //obiekt ustawień aplikacji
        private Properties.Settings settings = Properties.Settings.Default;

        //delegat funkcji przyjmującej jako argument stringa
        private delegate void SetString(string str);

        //obiekt przechowujący obiekty ubiegające się o dostęp do wątku
        private Object Sync;

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

            dataBase = new GlobalMySql();
        }

        //dodanie tekstu do okna logów synchronicznie
        private void AddLog(string log)
        {
            logs.AppendText(log + "\n");
        }

        //dodanie tekstu do okna logów asynchronicznie
        private void AddLogAsynch(string str)
        {
            Invoke
            (
                new SetString(AddLog),
                new Object[] { str }
            );
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

                AddLog("[" + GetSrverDateTime() + "][Serwer]: Serwer zakończył nasłuchiwanie");
            }
            else //jeżeli serwer nie jest w trakcie działania
            {
                //to podczas uruchamiania ustaw przycisk do wyłączania
                switchSvr.Text = "Wyłącz";

                //ustaw, że serwer jest aktywny
                isRunning = true;

                //ustawienie i uruchomienie nasłuchiwania Listen() w nowym wątku
                listenerTh = new Thread(Listen);
                listenerTh.Priority = ThreadPriority.BelowNormal;
                listenerTh.IsBackground = true;
                listenerTh.Start();
            }
        }

        //funkcja logowanie do serwera
        //gdy dane logowanie są poprawne, to zwraca identyfikator gracza
        //w przeciwnym wypadku zwraca 0
        private ulong Login(string login, string md5pass)
        {
            //próba otworzenia połączenia
            try
            {
                if (dataBase.Connection.State != ConnectionState.Open)
                    dataBase.Connection.Open();
            }
            catch
            {
                //MessageBox.Show("Nie można połączyć się z bazą danych! Błąd: \n" + ex.Message.ToString(), "Błąd bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return 0;
            }
            //zdefiniowanie zmiennej polecenia w obrębie obiektu połączenia connection
            MySqlCommand polecenie = dataBase.Connection.CreateCommand();
            //utworzenie zapytania
            polecenie.CommandText = "SELECT id FROM `player` WHERE `player`.`Login`='" + login + "' AND password='" + md5pass + "'";
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
                        //MessageBox.Show("Podano błędny Login lub hasło. Spróbuj jeszcze raz podając poprawne dane lub skorzystaj z opcji przypomnienia hasła.", "Nieudane logowanie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
            dataBase.Connection.Close();
            return 0;
        }

        //pobieranie danych gracza
        private string[] GetPlayerData(ulong player_id)
        {
            string[] data;
            //jeżeli połącznie ma status różny od otwartego
            if (dataBase.Connection.State != ConnectionState.Open)
            {
                //to spróbuj je otworzyć
                try
                {
                    dataBase.Connection.Open();
                }
                catch
                {
                    return null;
                }
            }
            //zdefiniowanie zmiennej polecenia w obrębie obiektu połączenia connection
            MySqlCommand polecenie = dataBase.Connection.CreateCommand();
            //utworzenie zapytania
            polecenie.CommandText = "SELECT * FROM `player` WHERE `player`.`id`='" + player_id + "'";

            try
            {
                //próba wykonanie polecenia i zapisanie jego wyniku do reader
                using (MySqlDataReader reader = polecenie.ExecuteReader())
                {
                    if (reader.HasRows) //jeżeli wybrało wiersze z bazy
                    {
                        while (reader.Read())
                        {
                            data = new string[reader.FieldCount - 1];
                            data[0] = reader.GetString("login");
                            data[1] = reader.GetString("password");
                            data[2] = reader.GetString("access");
                            data[3] = reader.GetString("email");
                            return data;
                        }
                    }
                    else
                    {
                        //MessageBox.Show("Podano błędny Login lub hasło. Spróbuj jeszcze raz podając poprawne dane lub skorzystaj z opcji przypomnienia hasła.", "Nieudane logowanie", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null;
                    }
                }
            }
            catch
            {
                //MessageBox.Show("Nie udało się pobrać danych z bazy danych.", "Błąd pobierania danych!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            return null;
        }

        //wysyłanie danych gracza
        private bool SendPlayerData(ulong playerId, Command response, Socket socket)
        {
            string[] dane = new string[3];

            dane = GetPlayerData(playerId);

            //utworzenie odpowiedzi
            response.Request(ServerCmd.PLAYER_DATA);
            response.Add(dane);
            response.Apply(socket);

            AddLogAsynch("[" + GetSrverDateTime() + "][ " + dane[0] + " ]: Pobrał dane gracza.");
            return true;
        }

        //nasłuchiwanie i dodawanie klientów
        private void Listen()
        {
            AddLogAsynch("[" + GetSrverDateTime() + "][Serwer]: Serwer rozpoczął nasłuchiwanie");
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
                        Thread klientTh = new Thread(ClientService);
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
                UpdateQuery = "UPDATE `" + dataBase.MySqlBase + "`.`" + args[2] + "` SET ";
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
            //utworzenie obiektu połączenia
            MySqlConnection connection = dataBase.Connection;
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
        private void ClientService(object s)
        {
            //gniazdo klienta, którego obsługujemy
            Socket socket = s as Socket;

            //dekoder UTF-8
            code = new UTF8Encoding();

            //zdefiniowanie nazwy klienta - potem zamieniana na nazwę użytkownika
            string clientName = socket.RemoteEndPoint.ToString();

            //informacja o tym czy klientowi udało się zalogować
            bool successLog = false;
                
            //bufor do pobierania danych od klienta
            byte[] buf;
             
            //inicjalizacja wielkości paczki z żądaniem
            int packageSize = 0;

            /* -------------- INICJALIZACJA OBIEKTÓW DLA GRACZA -------------- */

            //obiekt postaci
            Character character;

            /* --------------------------------------------------------------- */
            
            while (socket.Connected && isRunning && IsConnected(socket))
            {
                if (socket.Available > 0)
                {
                    //jeżeli ostatnia paczka została odczytana
                    if (packageSize == 0)
                    {
                        //to ustaw bufor na 4 bajty = int32
                        buf = new byte[4];

                        //i odczytaj wielkość nowej paczki
                        socket.Receive(buf);
                        packageSize = BitConverter.ToInt32(buf, 0);
                    }
                    else
                    {
                        //jeżeli wielkość paczki jest ustalona
                        //to ustaw bufor na wielkość jej odpowiadającą
                        buf = new byte[packageSize];
                        
                        //komenda wczytana z bufora
                        string cmd = code.GetString(buf, 0, socket.Receive(buf));

                        //po wczytaniu ustaw wielkość paczki na 0 zgłaszając tym samym gotowość do przyjęcia następnej
                        packageSize = 0;
                        
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
                                ulong userID = Login(args[1], args[2]);
                                if (userID == 0)
                                {
                                    AddLogAsynch("[" + GetSrverDateTime() + "][Klient]: Nieudana próba logowania (Login: " + args[1] + ")");
                                }
                                else
                                {
                                    AddLogAsynch("[" + GetSrverDateTime() + "][ " + args[1] + " ]: Udane logowanie. ID = " + userID);
                                    clientName = args[1];
                                    successLog = true;

                                    //utworzenie obiektu postaci dla zalogowanego gracza
                                    character = new Character(userID, dataBase);

                                    //uaktualnienie w bazie danych daty ostatniego logowania
                                    ExecuteQuery("UPDATE `" + dataBase.MySqlBase + "`.`player` SET `lastlogin` = '" + GetSrverDateTime() + "' WHERE `player`.`id` =" + userID + ";");
                                }
                                //utworzenie odpowiedzi
                                response.Request(ClientCmd.LOGIN);
                                response.Add(userID.ToString());
                                response.Apply(socket);
                                break;

                            /*
                             * WYSYŁANIE DANYCH GRACZA
                             * kolejność danych: komenda, Login, hasło, dostęp, email
                             */
                            case ClientCmd.GET_PLAYER_DATA:
                                Thread sendPlayerDataTh = new Thread(unused => SendPlayerData(ulong.Parse(args[1]), response, socket));
                                sendPlayerDataTh.Priority = ThreadPriority.BelowNormal;
                                sendPlayerDataTh.IsBackground = true;
                                sendPlayerDataTh.Start();
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
                            default:
                                AddLogAsynch("[" + GetSrverDateTime() + "][Klient]: Odebrano nieznaną komendę!");
                                break;
                        }
                        response.Clear();
                    }
                }
            }
            if (successLog)
            {
                AddLogAsynch("[" + GetSrverDateTime() + "][ " + clientName + " ]: Gracz rozłączył się z serwerem.");
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

        //pobranie czasu serwera w formacie akceptowanym przez bazę danych MySql
        private string GetSrverDateTime()
        {
            //utworzenie obiektu DateTime dla ustalenia czasu serwera
            //zostanie on ustawiony jako czas ostatniego logowania
            //będzie dodawany do logów na serwerze
            DateTime date = DateTime.Now;

            return String.Format("{0: yyyy'-'MM'-'dd HH:mm:ss}", date);
        }
    }
}
