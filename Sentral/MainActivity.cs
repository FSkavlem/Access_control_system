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

        private static void StartServer(object o)
        {
            // Server setup config
            Socket ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            ListenSocket.Bind(serverEP);
            ListenSocket.Listen(10);

            Type? d = Type.GetType("ClassLibrary.CardComms");

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
            Socket ComSocket = arg as Socket;
            bool error = false;
            bool complete = false;

            string Confirmation = "Server_ACK";
            SharedMethod.SendString(ComSocket, Confirmation, out error);

            while (!error)
            {
                // receive data from connected socket.
                string mottattData = SharedMethod.ReceiveString(ComSocket, out error);
                CardComms? data = JsonSerializer.Deserialize<CardComms>(mottattData);

                if (data != null)  //nullcheck, handle cardComms
                {
                    if (data.Alarm_bool)
                    {
                        //ALARM EVENT


                    }
                    if (data.Need_validation && !data.Alarm_bool)
                    {
                        //CHECK CARD NUMBER TO PIN SQL QUERY
                        bool Validation = CheckUserPin(ref data);
                        ReturnCardComms queryReturn = new ReturnCardComms(Validation);
                        string jsonString = JsonSerializer.Serialize(queryReturn);
                        complete = SharedMethod.SendString(ComSocket,jsonString,out error);
                        data.Need_validation = false;
                    }

                }
                if (complete) break;
            }
            // Lukker kommunikasjonssokkel og viser info
            Debug.WriteLine("Comm Diconnected: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);
            ComSocket.Close();
        }

        private static bool CheckUserPin(ref CardComms data)
        {
            bool access;
            Bruker SQLqueryBruker = SQLrequestUser(data.CardID);
            if (SQLqueryBruker.Pin == data.Pin)
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

        private static Bruker SQLrequestUser(int cardID)
        {
            //Get user from SQL DB in accordance with cardID
            DateTime fuuuu = new DateTime(2025, 12, 25, 10, 30, 50);
            Bruker SQLbruker = new Bruker("fredrik", "skavlem",1234,fuuuu, "1111");
            return SQLbruker;
        }
    }
}