using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;

namespace CardReaderForm
{
    public partial class CardReaderForm : Form
    {
        public static SerialPort serialPort;
        public static string SerialString;
        //-----------------------------------FOR DEBUGGING------------------
        public static CardInfo lastCardUsed = new CardInfo { CardID = 1222, PinEntered = "1333", Number = 1, Time = DateTime.Now };
        public static CardComms comms = new CardComms { Time = DateTime.Now, Number = 1, CardID = 1234, Pin = "1111", Alarm_bool = false, Alarm_type = 0, Lastuser = 4432, Need_validation = true };
        public static CardInfo cardInfo = new CardInfo { CardID = 1234, Number = 1, PinEntered = "1111" };
        public static AlarmEvent alarmEvent = new AlarmEvent { Alarm_bool = true, Alarm_type = 2, LastCardUsed = lastCardUsed };
        //-----------------------------------FOR DEBUGGING------------------
        public CardReaderForm()
        {
            InitializeComponent();
            ThreadPool.QueueUserWorkItem(StartClient);

        }

        private static void StartClient(object? state)
        {
            bool error = false;
            Socket comSocket;
            SerialPort doorPort = new SerialPort();

            while (true)
            {
                comSocket = Connect2Server(); //no need for 
                if (!doorPort.IsOpen) doorPort = Connect2Door();

                while (comSocket.Connected)
                {
                    if (comSocket.Available > 0)
                    {
                        string? receivedString = Central.ReceiveString(comSocket, out error);
                        string packageID = Central.GetPackageIdentifier(ref receivedString, out receivedString);
                        switch (packageID)
                        {
                            case PackageIdentifier.ServerACK:
                                Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                break;
                            case PackageIdentifier.PinValidation:
                                ReturnCardComms returnCardComms = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                                //FORSTEETT HER
                                    Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                break;
                            case PackageIdentifier.RequestNumber:
                            default:
                                break;
                        }
                    }
                }
                Thread.Sleep(1000); //Wait 1000MS before trying to reconnect
            }
        }
        private static SerialPort Connect2Door()
        {
            //char 10 \n linefeed, char 13 \r carrige return
            //{\n\r$A001B20221002C120157D01010000E00000000F0500G0500H0500I020J020#}
            serialPort = new SerialPort("COM20", 152000);
            serialPort.Close();
            serialPort.NewLine = "\n\r";
            serialPort.Encoding = Encoding.UTF8;
            //adds an event when data is received from serialPort
            serialPort.DataReceived += new SerialDataReceivedEventHandler(serial_data_received);
            serialPort.Open();
            serialPort.ReadTimeout = 10000;
            return serialPort;

        }

        private static void serial_data_received(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[255];
            SerialPort temp = (SerialPort)sender;
            temp.Read(buffer, 0, buffer.Length);
            var str = System.Text.Encoding.Default.GetString(buffer);
            var someDoorEvent = true;
            if (someDoorEvent = true)
            {

            }

        }

        private static Socket Connect2Server()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            try
            {
                clientSocket.Connect(serverEP);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Link failed to establish");
            }
            return clientSocket;
        }
    }

}