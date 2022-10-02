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
        public static object DoorLock = new object();
        public static object BoolLock = new object();
        public static bool updateDoor;
        public static bool newAccessTry;
        public static Door PublicDoor;

        //-----------------------------------FOR DEBUGGING------------------
        public static CardInfo lastCardUsed = new CardInfo { CardID = 1222, PinEntered = "1333", Number = 1, Time = DateTime.Now };
        public static CardComms comms = new CardComms { Time = DateTime.Now, Number = 1, CardID = 1234, Pin = "1111", Alarm_bool = false, Alarm_type = 0, Lastuser = 4432, Need_validation = true };
        public static CardInfo cardInfo = new CardInfo { CardID = 1234, Number = 1, PinEntered = "1111" };
        public static AlarmEvent alarmEvent = new AlarmEvent { Alarm_bool = true, Alarm_type = 2, LastCardUsed = lastCardUsed };
        //-----------------------------------FOR DEBUGGING------------------
        public CardReaderForm()
        {
            InitializeComponent();
            updateDoor = false;
            newAccessTry = false;
            ThreadPool.QueueUserWorkItem(StartTCPClient);
            ThreadPool.QueueUserWorkItem(StartSerialClient);
        }

        private static void StartTCPClient(object? state)
        {
            Door door = new Door();
            bool error = false;
            Socket comSocket;
            while (true)
            {
                comSocket = Connect2Server(); //no need for 
                while (comSocket.Connected)
                {
                    if (updateDoor){
                        lock (DoorLock) door = PublicDoor;
                        lock (BoolLock) updateDoor = false;
                        var a = door;
                        //FORTSETT HER, updateDoor or newAccessTry til TCP server!
                    }

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

        public static bool AccessTry;
        public static bool ResetProccess;
        private static void StartSerialClient(object? state)
        {
            Door door = new Door();
            AccessTry = false;
            ResetProccess = false;
            System.Timers.Timer? DoorTimer;
            SerialPort? serialPort = new SerialPort();
            DoorTimer = new System.Timers.Timer();
            DoorTimer.Interval = 45000;
            DoorTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

            while (true)
            {
                if (!serialPort.IsOpen) connect2Serial(ref serialPort); //connects to serial if its not open

                if (ResetProccess){
                    door.AccessTry_e4 = false;
                    ToggleCheckBoxesInSimSim(4, false, ref serialPort);
                    ResetProccess=false;
                }
                if (door.enteredPin.Length >= 4) DoorTimer.Interval = 1; //after 4 buttons is pressed. Stops e4
               
                if (serialPort.BytesToRead > 0)
                {
                    string str = serial_data_received(ref serialPort);//will only take 65 byte at a time so only one send at the time will be handled
                    updateDoorVars(str, ref door);
                    lock (DoorLock) PublicDoor = door;
                    lock (BoolLock) updateDoor = true;
                    if (door.AccessTry_e4 & !AccessTry)
                    {
                        DoorTimer.Start();
                        door.enteredPin = string.Empty;
                        AccessTry = true;
                    }
                    if (AccessTry) KeyPadStorage(ref door);
                    WipeKeyPadSIMSIM(ref serialPort, ref door);
                    var a = door;
                }
            }
        }

        private static void WipeKeyPadSIMSIM(ref SerialPort serialPort, ref Door door)
        {
            for (int i = 0; i < 4; i++)
            {
                if (door.KeyPad[i])
                {
                    ToggleCheckBoxesInSimSim(i, false, ref serialPort);
                    door.KeyPad[i] = false;
                }
            }
        }
        private static void ToggleCheckBoxesInSimSim(int n, bool x, ref SerialPort serialPort)
        {
            string temp = "";
            if (x) temp = "1";
            else temp = "0";
            string parseString = string.Format("$O{0}{1}",n, temp);
            serialPort.Write(parseString);
        }
        private static void Timer_Elapsed(object sender, EventArgs e)
        {
            var a = (System.Timers.Timer)sender; //get timer handle
            AccessTry = false;
            ResetProccess = true;
            lock (BoolLock) newAccessTry = true;

            a.Stop(); // stops timer.

        }
        private static void connect2Serial(ref SerialPort serialPort)
        {
            serialPort = new SerialPort("COM20", 152000);
            serialPort.NewLine = "\n\r";
            serialPort.Encoding = Encoding.UTF8;
            //adds an event when data is received from serialPort
            //serialPort.DataReceived += new SerialDataReceivedEventHandler(serial_data_received);
            serialPort.ReadTimeout = 10000;
            serialPort.Open(); 
        }
        private static string serial_data_received(ref SerialPort temp)
        {
            byte[] buffer = new byte[65];
            temp.Read(buffer, 0, 65);
            string str = Encoding.Default.GetString(buffer);
            return str;
        }
        private static bool[] GetBoolArrayFromString(string v)
        {
            bool[] result = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                if (v[i] == '1') result[i] = true;
                else result[i] = false;
            }
            return result;
        }
        private static DateTime GetDatetimeFromStrings(string v)
        {
            DateTime parsedDate;
            DateTime.TryParseExact(v, "yyyyMMddhhmmss", null,
                               System.Globalization.DateTimeStyles.AllowWhiteSpaces |
                               System.Globalization.DateTimeStyles.AdjustToUniversal,
                               out parsedDate);
            return parsedDate;
        }
        private static void updateDoorVars(string str, ref Door door)
        {
            //{\n\r$A001B20221002C120157D01010000E00000000F0500G0500H0500I020J020#}
            //A = nodeNum B="yyyyMMdd" C="hhmmss" D=8inputs E=8outputs F=termistor G=Potm1 H=Potm2 I=TempSens1 J=TempSens2 
            str = str.Remove(str.Length - 1);
            string azpattern = "[A-J]+";
            string[] result = System.Text.RegularExpressions.Regex.Split(str, azpattern); //returns a string array splitt on capital chars
            door.nodeNum = int.Parse(result[1]);
            door.time = GetDatetimeFromStrings(result[2] + result[3]);
            bool[] arr = GetBoolArrayFromString(result[5]);
            door.KeyPad = new bool[] { arr[0], arr[1], arr[2], arr[3] };
            door.AccessTry_e4 = arr[4];
            door.DoorLocked_e5 = arr[5];
            door.DoorOpen_e6 = arr[6];
            door.Alarm_e7 = arr[7];
            if (int.Parse(result[6]) > 500) door.DoorForcedOpen = true;
            else door.DoorForcedOpen = false;
        }
        private static void KeyPadStorage(ref Door door)
        {
            if (door.KeyPad[0]) door.enteredPin += "0";
            if (door.KeyPad[1]) door.enteredPin += "1";
            if (door.KeyPad[2]) door.enteredPin += "2";
            if (door.KeyPad[3]) door.enteredPin += "3";
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