using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ClassLibrary;

namespace CardReaderForm
{
    public delegate void DoorAlarms(object sender, DoorAlarmEventArgs e);
    public delegate void setAlarmBool(bool x);
    class SerialClient
    {
        public static CardReaderForm mainform;
        public static SerialClient serialClient;
        public static SerialPort serialPort;
        public Door doorStatus;
        public event DoorAlarms NewAlarmEvent;
        public bool alarmOn { get; set; }

        public void RunCLient(CardReaderForm f)
        {
            mainform = f;
            ThreadStart ts = new ThreadStart(StartSerialClient);
            Thread SerialThread = new Thread(ts);
            SerialThread.Name = "SerialclientThread";
            SerialThread.IsBackground = true;
            SerialThread.Start();
        }
        private void StartSerialClient()
        {
            System.Timers.Timer PollingTimer = new System.Timers.Timer();
            System.Timers.Timer DoorTimer = new System.Timers.Timer();
            System.Timers.Timer AlarmTimer = new System.Timers.Timer();

            SetupTimers(ref PollingTimer, 3000, true, true);
            SetupTimers(ref DoorTimer, 30000, false, false);
            SetupTimers(ref AlarmTimer, 6000, false, false);
            PollingTimer.Elapsed += new ElapsedEventHandler(PollingTimer_Timer_Elapsed);
            DoorTimer.Elapsed += new ElapsedEventHandler(Doortimer_Timer_Elapsed);
            AlarmTimer.Elapsed += new ElapsedEventHandler(AlarmTimer_Timer_Elapsed);

            doorStatus = new Door();
            alarmOn = false;
            serialClient = this;
            serialPort = new SerialPort();

            //SetupTimers(ref DoorTimer, 200, ref PollingTimer, 3000);   //will poll door every 3s.
            

            bool Opendoor = false;


            while (true)
            {
                UpdateGlobalVars();
                if (!serialPort.IsOpen) Statemachine(States.SerialPortNotOpen);
                if (serialPort.BytesToRead > 0) Statemachine(States.TrafficOnSerialPort);
                //---------Alarms-------
                if (doorStatus.Alarm_e7) Statemachine(States.GenericAlarmFromDoor);
                if (doorStatus.DoorForce > 500) Statemachine(States.ForceAlarmFromDoor);
                if (doorStatus.DoorOpen_e6 & doorStatus.DoorLocked_e5) Statemachine(States.ForceAlarmFromDoor);
                //---------Alarms-------
                if (Opendoor) Statemachine(States.LockDoor);
                if (doorStatus.DoorOpen_e6) Statemachine(States.StartDoorTimer);
                if (!doorStatus.DoorOpen_e6) Statemachine(States.StopDoorTimer);
                if (!doorStatus.DoorOpen_e6 & !doorStatus.DoorLocked_e5) Statemachine(States.DoorIsClosedButNotLocked);
                
            }
            
            void UpdateGlobalVars()
            {
                Opendoor = GetOpenDoorBool();
            }
            void Statemachine(int states)
            {
                switch (states)
                {
                    case States.SerialPortNotOpen:
                        connect2Serial("COM20",19200);
                        break;
                    case States.TrafficOnSerialPort:
                        string str = serial_data_received();  //will only take 65 byte at a time so only one send at the time will be handled   
                        doorStatus = DecompileSerialString(str);//update current door status
                        updateGlobalDoor(doorStatus);
                        break;
                    case States.GenericAlarmFromDoor:
                        SetGlobalAlarm(ref AlarmTimer, AlarmTypes.GenericAlarm);
                        break;
                    case States.ForceAlarmFromDoor:
                        SetGlobalAlarm(ref AlarmTimer, AlarmTypes.ForceDoor);
                        break;
                    case States.OpenDoor:
                        UnlockDoor();
                        doorStatus.DoorLocked_e5 = false;
                        updateGlobalDoor(doorStatus);
                        break;
                    case States.DoorIsClosedButNotLocked:
                        LockDoor();
                        doorStatus.DoorLocked_e5 = true;
                        updateGlobalDoor(doorStatus);
                        break;
                    case States.StartDoorTimer:
                        DoorTimer.Start();
                        break;
                    case States.StopDoorTimer:
                        DoorTimer.Stop();
                        break;

                    default:
                        break;
                }

            }
        }

        private void SetGlobalAlarm(ref System.Timers.Timer alarmTimer, int x)
        {
            if (!alarmOn)
            {
                alarmTimer.Start();
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
            door.DoorOpen_e6 = arr[6];
            door.Alarm_e7 = arr[7];
            door.DoorForce = int.Parse(result[6]);

            return door;
        
        }
        private static bool GetOpenDoorBool()
        {
            try
            {
                bool a = (bool)mainform.Invoke(new GetOpenDoorBool(mainform.GetOpenDoorBool));
                return a;
            }
            catch (Exception)
            {
                return false;
            }
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
            //SetGlobalAlarm(true, AlarmTypes.DoorOpenTooLong);
            a.Stop();
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
            serialPort.Write("$R");
        }
    }
    public class DoorAlarmEventArgs : EventArgs
    {
        public DoorAlarmEventArgs(int alarmtypes)
        {
            this.alarmtypes = alarmtypes;
        }

        public int alarmtypes { get; set; }

    }
}

