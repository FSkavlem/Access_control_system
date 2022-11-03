using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ClassLibrary;

namespace CardReaderForm
{
   public delegate void DoorAlarms(object sender, DoorAlarmEventArgs e);
   public delegate void DoorConnectStatus(object sender, bool x);
    class SerialClient
    {
        public static CardReaderForm mainform;
        public static SerialClient serialClient;
        public static SerialPort serialPort = new SerialPort();
        public Door doorStatus;
        public event DoorAlarms NewAlarmEvent;
        public event EventHandler DoorTimerExpired;
        public event EventHandler DoorConnectStatus;
        //must close SerialClient
        public bool alarmOn { get; set; }
        bool Opendoor{ get; set; }
        public static bool LostConnection { get; set; }
        System.Timers.Timer AlarmTimer;
        public void RunCLient(CardReaderForm f, string ComPortName)
        {
            mainform = f;
            Thread SerialThread = new Thread(StartSerialClient);
            SerialThread.Name = "SerialclientThread";
            SerialThread.IsBackground = true;
            SerialThread.Start(ComPortName);
        }
        private void StartSerialClient(object arg)
        {
            string comportname = (string)arg;
            System.Timers.Timer PollingTimer = new System.Timers.Timer();
            System.Timers.Timer DoorTimer = new System.Timers.Timer();
            AlarmTimer = new System.Timers.Timer();

            SetupTimers(ref PollingTimer, 3000, true, true);
            SetupTimers(ref DoorTimer, 60000, false, false);
            SetupTimers(ref AlarmTimer, 6000, false, false);
            PollingTimer.Elapsed += new ElapsedEventHandler(PollingTimer_Timer_Elapsed);
            DoorTimer.Elapsed += new ElapsedEventHandler(Doortimer_Timer_Elapsed);
            AlarmTimer.Elapsed += new ElapsedEventHandler(AlarmTimer_Timer_Elapsed);

            mainform.OpenDoorEvent += Mainform_OpenDoorEvent;
            serialPort.ErrorReceived += SerialPort_ErrorReceived;

            doorStatus = new Door();
            bool serialportHasbeenOpened = false;
            Opendoor = false;
            
            alarmOn = false;
            LostConnection = false;
            serialClient = this;
            serialPort = new SerialPort();
        
            while (true)
            {
                if (!serialPort.IsOpen & !serialportHasbeenOpened) Statemachine(States.SerialPortNotOpen);
                if (!serialPort.IsOpen & serialportHasbeenOpened) Statemachine(States.LostConnection);
                if (serialPort.IsOpen)
                {
                    if (serialPort.BytesToRead > 0) Statemachine(States.TrafficOnSerialPort);
                }
                //---------Alarms-------
                if (doorStatus.Alarm_e7) Statemachine(States.GenericAlarmFromDoor);
                if (doorStatus.DoorForce > 500) Statemachine(States.ForceAlarmFromDoor);
                if (doorStatus.DoorOpen_e6 & doorStatus.DoorLocked_e5) Statemachine(States.ForceAlarmFromDoor);
                //---------Alarms-------
                if (Opendoor) Statemachine(States.OpenDoor);
                if (!doorStatus.DoorOpen_e6 & !doorStatus.DoorLocked_e5) Statemachine(States.DoorIsClosedButNotLocked);
                if (doorStatus.DoorOpen_e6) Statemachine(States.StartDoorTimer);
                if (!doorStatus.DoorOpen_e6) Statemachine(States.StopDoorTimer);

            }

            void Statemachine(int states)
            {
                switch (states)
                {
                    case States.SerialPortNotOpen:
                        connect2Serial(comportname,19200);
                        serialportHasbeenOpened = true;
                        break;
                    case States.LostConnection:
                        serialportHasbeenOpened = false;
                        LostConnection = false;
                        DoorConnectStatus?.Invoke(false, new EventArgs());
                        break;
                    case States.TrafficOnSerialPort:
                        DoorConnectStatus?.Invoke(true, new EventArgs());
                        string str = serial_data_received();  //will only take 65 byte at a time so only one send at the time will be handled   
                        doorStatus = DecompileSerialString(str);//update current door status
                        updateGlobalDoor(doorStatus);
                        break;
                    case States.GenericAlarmFromDoor:
                        SetGlobalAlarm(AlarmTypes.GenericAlarm);
                        break;
                    case States.ForceAlarmFromDoor:
                        SetGlobalAlarm(AlarmTypes.ForceDoor);
                        break;
                    case States.OpenDoor:
                        UnlockDoor();
                        OpenDoor();
                        doorStatus.DoorLocked_e5 = false;
                        doorStatus.DoorOpen_e6 = true; 
                        Opendoor = false;
                        //updateGlobalDoor(doorStatus);
                        break;
                    case States.DoorIsClosedButNotLocked:
                        LockDoor();
                        doorStatus.DoorLocked_e5 = true;
                        updateGlobalDoor(doorStatus);
                        break;
                    case States.StartDoorTimer:
                        DoorTimer.Enabled = true;
                        break;
                    case States.StopDoorTimer:
                        DoorTimer.Enabled = false;
                        break;

                    default:
                        break;
                }

            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            var a = 1;
        }

        private void OpenDoor() => ToggleCheckBoxesInSimSim(6, true);

        private void Mainform_OpenDoorEvent(bool x) => Opendoor = x;
 
        private void SetGlobalAlarm(int x)
        {
            if (!alarmOn)
            {
                AlarmTimer.Start();
                TurnOffAlarmSIMSIM();
                doorStatus.Alarm_e7 = false;
                DoorAlarms doorAlarms = NewAlarmEvent;
                doorAlarms?.Invoke(this, new DoorAlarmEventArgs(x)); //fireEvent
                var a = doorStatus;
                alarmOn = true;
            }
        }
        private void SetAlarmBool(bool x) => alarmOn = x;
        private static void updateGlobalDoor(Door doorStatus)
        {
            try
            {
                SetPublicDoor setPublicDoor = new SetPublicDoor(mainform.SetPublicDoor);
                mainform.Invoke(setPublicDoor, doorStatus);
            }
            catch (Exception)
            {
            }
        }
        private static void LockDoor() => ToggleCheckBoxesInSimSim(5, true);
        private static void UnlockDoor() => ToggleCheckBoxesInSimSim(5, false);
        private static void TurnOffAlarmSIMSIM() => ToggleCheckBoxesInSimSim(7, false);
        private static void ToggleCheckBoxesInSimSim(int n, bool x)
        {
            string temp = "";
            if (x) temp = "1";
            else temp = "0";
            string parseString = string.Format("$O{0}{1}", n, temp);
            serialPort.Write(parseString);
        }
        private static Door DecompileSerialString(string str)
        {
            Door door = new Door();
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
            door.DoorOpen_e6 = arr[6]; //checkbox true door open, checkbox false door closed
            door.Alarm_e7 = arr[7];
            door.DoorForce = int.Parse(result[6]);
            return door;
        
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
        private static string serial_data_received()
        {
            byte[] buffer = new byte[65];
            serialPort.Read(buffer, 0, 65);
            string str = Encoding.Default.GetString(buffer);
            return str;
        }
        private static void connect2Serial(string com, int baud)
        {
            serialPort = new SerialPort(com, baud);
            serialPort.NewLine = "\n\r";
            serialPort.Encoding = Encoding.UTF8;
            //adds an event when data is received from serialPort, stupid dont do it
            //serialPort.DataReceived += new SerialDataReceivedEventHandler(serial_data_received);
            serialPort.ReadTimeout = 1000;
            serialPort.WriteTimeout = 2000;
            serialPort.Open();

        }
        private static void SetupTimers(ref System.Timers.Timer x,int interval, bool autoreset, bool start)
        {
            x.Interval = interval;
            x.AutoReset = autoreset;
            x.Enabled = start;
        }
        private static void PollingTimer_Timer_Elapsed(object? sender, ElapsedEventArgs e) => PollDoor();
        private static void Doortimer_Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var a = (System.Timers.Timer)sender; //get timer handle
            a.Enabled = false;
            serialClient.SetGlobalAlarm(AlarmTypes.DoorOpenTooLong);
        }   
        private static void AlarmTimer_Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            //UNSAFE
            var a = (System.Timers.Timer)sender;
            a.Stop();
            serialClient.alarmOn = false;
        }
        private static void PollDoor()
        {
            try
            {
                serialPort.Write("$R");
            }
            catch (Exception)
            {
                if (serialPort.IsOpen)
                {
                    LostConnection = true;
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }

        }
    }

}

