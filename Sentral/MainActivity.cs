using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using ClassLibrary;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Dynamic;

namespace Sentral
{
    public delegate void UpdateFormAccesList(AccessLogEntrySQL x);
    public delegate void UpdateFormAlarmList(AlarmLogSQLEntry x);
    public partial class MainActivity : Form
    {
        public int AccessIDnumber;
        public int AlarmIDnumber;
        public static MainActivity mainform;

        public MainActivity()
        {
            InitializeComponent();
            StartUIupdater();
            AccessIDnumber = 0;
            AlarmIDnumber = 0;
            mainform = this;
            ThreadPool.QueueUserWorkItem(StartServer);
        }

        private static void StartServer(object? o)
        {
            // Server setup config
            Socket ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            ListenSocket.Bind(serverEP);
            ListenSocket.Listen(10);

            while (true)
            {
                Socket ComSocket = ListenSocket.Accept(); // blokkerende metode
                // Write established comms to Output Window
                Debug.WriteLine("Comm Established: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);
                // passes socket to other thread
                ThreadPool.QueueUserWorkItem(TCPclient, ComSocket);
            }
        }
        private void StartUIupdater()
        {
            System.Windows.Forms.Timer UIupdateTimer;
            UIupdateTimer = new System.Windows.Forms.Timer();
            UIupdateTimer.Interval = 100;
            UIupdateTimer.Tick += new EventHandler(Timer_Elapsed);
            UIupdateTimer.Enabled = true;
        }
        private void Timer_Elapsed(object? sender, EventArgs e)
        {
            
        }
        private void UpdateAlarmListView(AlarmLogSQLEntry x)
        {
            AlarmIDnumber++;
            string lastuser = string.Format("{0},{1}", x.LastUser.Etternavn, x.LastUser.Fornavn);
            string[] row = {AlarmIDnumber.ToString(),
                            x.AlarmType,
                            lastuser,
                            x.TimeStamp.ToString()};
            ListViewItem listViewItem = new ListViewItem(row);
            listview_alarm_log.Items.Add(listViewItem);
        }
        private void UpdateAccessListView(AccessLogEntrySQL x)
        {   //should be from DB 
            string a = string.Empty;
            AccessIDnumber++;
            if (x.AccessGranted) a = "Granted";
            else a = "Denied";
            string[] row = {AccessIDnumber.ToString(),
                            x.User.CardID.ToString(), 
                            x.User.Etternavn,
                            x.User.Fornavn,
                            x.DoorNr.ToString(),
                            x.TimeStamp.ToString(),
                            a};
            ListViewItem listViewItem = new ListViewItem(row);
            listview_access_log.Items.Add(listViewItem);
        }
        //TCP_CLIENT
        private static void TCPclient(object arg)
        {
            Socket ComSocket = (Socket)arg;
            bool error = false;
            bool complete = false;
            ClassLibrary.Message.SendString(ComSocket, PackageIdentifier.ServerACK, out error);

            while (!error)
            {
                
                if(ComSocket.Available > 0) // receive data from connected socket if available
                {
                    string receivedString = ClassLibrary.Message.ReceiveString(ComSocket, out error);
                    string packageID = ClassLibrary.Message.GetPackageIdentifier(ref receivedString, out receivedString);

                    switch (packageID)
                    {
                        case PackageIdentifier.AlarmEvent:
                            AlarmEvent alarmEvent = JsonSerializer.Deserialize<AlarmEvent>(receivedString);
                            AlarmLogger(alarmEvent);
                            break;
                        case PackageIdentifier.CardInfo:

                            break;
                        case PackageIdentifier.PinValidation:

                            CardInfo cardInfo = JsonSerializer.Deserialize<CardInfo>(receivedString);
                            User user = SQLrequestUser(cardInfo.CardID);
                            bool validation = CheckUserPin(cardInfo,user);
                            
                            AccessLogEntrySQL accesslogEntry = new AccessLogEntrySQL(user, cardInfo.Time, validation,cardInfo.DoorNr);
                            AccessLogger(accesslogEntry);

                            ReturnCardComms queryReturn = new ReturnCardComms {Validation=validation};

                            string jsonString = JsonSerializer.Serialize(queryReturn);
                            jsonString = ClassLibrary.Message.AddPackageIdentifier(PackageIdentifier.PinValidation, jsonString);
                            complete = ClassLibrary.Message.SendString(ComSocket, jsonString, out error);
                            break;
                    }
                }
            }
            // Lukker kommunikasjonssokkel og viser info
            Debug.WriteLine("Comm Diconnected: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);
            ComSocket.Close();
        }
        private static void AlarmLogger(AlarmEvent alarmEvent)
        {
            string alarmtype = AlarmTypes.toString(alarmEvent.Alarm_type);
            User user = SQLrequestUser(alarmEvent.LastUser.CardID);
            AlarmLogSQLEntry alarmEntry = new AlarmLogSQLEntry { AlarmType = alarmtype, DoorNumber = alarmEvent.DoorNumber, LastUser = user, TimeStamp = alarmEvent.Time };
            UpdateFormAlarmList x = new UpdateFormAlarmList(mainform.UpdateAlarmListView);

            mainform.Invoke(x, alarmEntry);
            SQLlogAlarm(alarmEntry);
        }
        private static void AccessLogger(AccessLogEntrySQL accesslogEntry)
        {
            UpdateFormAccesList x = new UpdateFormAccesList(mainform.UpdateAccessListView);
            mainform.Invoke(x, accesslogEntry);
            SQLlogAccessEntry(accesslogEntry);
        }
        private static bool CheckUserPin(CardInfo data, User user)
        {
            bool access;
            if (user.Pin == data.PinEntered)
            {
                access = true;
            }
            else
            {
                access = false;
            }
            return access;
        }
        private static void SQLlogAlarm(AlarmLogSQLEntry x)
        {
            //something something log alarm SQL, ID set bt SQL DB

        }
        private static void SQLlogAccessEntry(AccessLogEntrySQL x)
        {
            //something something log access try from data to SQL, ID set bt SQL DB
        }

        private static User SQLrequestUser(int cardID)
        {
            //Get user from SQL DB in accordance with cardID
            DateTime fuuuu = new DateTime(2025, 12, 25, 10, 30, 50);
            User SQLbruker = new User("fredrik", "skavlem",1234,fuuuu, "1111");
            return SQLbruker;
        }
    }
}