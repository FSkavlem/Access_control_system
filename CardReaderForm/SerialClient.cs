using ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardReaderForm
{
    class SerialClient
    {
        public static bool AccessTry;
        public static bool ResetProccess;
        public static CardReaderForm mainform;
        public void RunCLient(CardReaderForm f)
        {
            mainform = f;
            ThreadStart ts = new ThreadStart(StartSerialClient);
            Thread SerialThread = new Thread(ts);
            SerialThread.Name = "SerialclientThread";
            SerialThread.IsBackground = true;
            SerialThread.Start();
        }

        private static void StartSerialClient()
        {
            Door door = new Door();
            AccessTry = false;
            ResetProccess = false;
            System.Timers.Timer? DoorTimer;
            SerialPort? serialPort = new SerialPort();
            DoorTimer = new System.Timers.Timer();
            DoorTimer.Interval = 45000;
            DoorTimer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);

            SetPublicDoor setPublicDoor = new SetPublicDoor(mainform.SetPublicDoor);
            SetUpdateDoorBool setUpdateDoorBool = new SetUpdateDoorBool(mainform.SetUpdateDoorBool);
          
            while (true)
            {
                if (!serialPort.IsOpen) connect2Serial(ref serialPort); //connects to serial if its not open

                if (ResetProccess)
                {
                    door.AccessTry_e4 = false;
                    ToggleCheckBoxesInSimSim(4, false, ref serialPort);
                    ResetProccess = false;
                }
                if (door.enteredPin.Length >= 4) DoorTimer.Interval = 1; //after 4 buttons is pressed. Stops e4

                if (serialPort.BytesToRead > 0)
                {
                    string str = serial_data_received(ref serialPort);//will only take 65 byte at a time so only one send at the time will be handled

                    updateDoorVars(str, ref door);                  //update current door status
                    mainform.Invoke(setPublicDoor, door);           //pass current door status to main thread
                    mainform.Invoke(setUpdateDoorBool, true);       //sets bool for new update

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
            string parseString = string.Format("$O{0}{1}", n, temp);
            serialPort.Write(parseString);
        }
        private static void Timer_Elapsed(object sender, EventArgs e)
        {
            var a = (System.Timers.Timer)sender; //get timer handle
            AccessTry = false;
            ResetProccess = true;

            SetAccessTryEvent setAccessTry = new SetAccessTryEvent(mainform.SetNewAccessTry);
            mainform.Invoke(setAccessTry, true); //sets accesstry bool to true on mainthread

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

    }
}
