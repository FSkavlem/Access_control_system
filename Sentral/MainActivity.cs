using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using ClassLibrary;
using System.Text.Json;

namespace Sentral
{
    public partial class MainActivity : Form
    {
        object SQL_LOCK = new object();
        public MainActivity()
        {
            InitializeComponent();
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
                ThreadPool.QueueUserWorkItem(KommunikasjonMedEnKlient, ComSocket);
            }
        }
        private static void KommunikasjonMedEnKlient(object arg)
        {
            Socket ComSocket = (Socket)arg;
            bool error = false;
            bool complete = false;
            ClassLibrary.Message.SendString(ComSocket, PackageIdentifier.ServerACK, out error);

            while (!error)
            {
                // receive data from connected socket.
                string receivedString = ClassLibrary.Message.ReceiveString(ComSocket, out error);
                string packageID = ClassLibrary.Message.GetPackageIdentifier(ref receivedString,out receivedString);

                switch (packageID)
                {
                    case PackageIdentifier.AlarmEvent:
                        AlarmEvent alarmEvent = JsonSerializer.Deserialize<AlarmEvent>(receivedString);
                        break;
                    case PackageIdentifier.CardInfo:

                        break;
                    case PackageIdentifier.PinValidation:
                        CardInfo cardInfo = JsonSerializer.Deserialize<CardInfo>(receivedString);
                        bool Validation = CheckUserPin(cardInfo);

                        ReturnCardComms queryReturn = new ReturnCardComms(Validation);
                        string jsonString = JsonSerializer.Serialize(queryReturn);
                        jsonString = ClassLibrary.Message.AddPackageIdentifier(PackageIdentifier.PinValidation, jsonString);
                        complete = ClassLibrary.Message.SendString(ComSocket, jsonString, out error);
                        break;
                }

                if (complete) break;
            }
            // Lukker kommunikasjonssokkel og viser info
            Debug.WriteLine("Comm Diconnected: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);
            ComSocket.Close();
        }

        private static bool CheckUserPin(CardInfo data)
        {
            bool access;
            User SQLqueryBruker = SQLrequestUser(data.CardID);
            if (SQLqueryBruker.Pin == data.PinEntered)
            {
                access = true;
            }
            else
            {
                access = false;
            }

            SQLlogAccessEntry(new AccessLogEntry(SQLqueryBruker,data.Time,access));
            return access;
        }


        private static void SQLlogAlarm(AlarmLogEntry x)
        {
            //something something log alarm SQL, ID set bt SQL DB

        }

        private static void SQLlogAccessEntry(AccessLogEntry x)
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