using ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CardReaderForm
{
    class SerialClient
    {
        public static bool AccessTry;
        public static bool Alarm;
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
            bool pinValidationEvent = false ;
            bool resetAlarm = false; ;
            bool doorTimerStarted = false;

            AccessTry = false;
            ResetProccess = false;

            System.Timers.Timer EnterPinTimer = new System.Timers.Timer(); 
            System.Timers.Timer DoorTimer = new System.Timers.Timer();

            SetupTimers(ref DoorTimer, ref EnterPinTimer);

            SerialPort? serialPort = new SerialPort();
            while (true) //MAIN LOOP
            {
                //get public vars
                pinValidationEvent = GetPublicPinValidation();
                resetAlarm = GetPublicResetAlarm();


                if (!serialPort.IsOpen) Statemachine(States.SerialPortNotOpen);                                 //Serial port is not open
                if (door.DoorLocked_e5 & door.DoorOpen_e6) Statemachine(States.DoorLocked);                     //door is locked, but opens from app
                if (resetAlarm) Statemachine(States.ResetAlarm);                                                //reset alarm
                if (pinValidationEvent) Statemachine(States.PinVerified);                                       //pin is verified
                if (ResetProccess) Statemachine(States.ResetAccessProcess);                                     //Card is recorded with and 4 pin entered
                if (door.DoorOpen_e6 & !doorTimerStarted) Statemachine(States.StartDoorTimer);                  //door is open, timer starts
                if (!door.DoorOpen_e6) Statemachine(States.DoorClosed);                                         //door is closed
                if (!door.DoorOpen_e6 & !door.DoorLocked_e5) Statemachine(States.DoorIsClosedButNotLocked);     //door is closed but not locked
                if (door.enteredPin.Length >= 4 & AccessTry) Statemachine(States.FourDigitsEntered);            //four digits of pin is entered
                if (door.AccessTry_e4 & !AccessTry) Statemachine(States.AccessTry);                             //a card has been recorded, starts logging of pin entry
                if (serialPort.BytesToRead > 0) Statemachine(States.TrafficOnSerialPort);                       //traffic is available on serialCom


                void Statemachine(int currentstate)
                {
                    switch (currentstate)
                    {
                        case States.SerialPortNotOpen:
                            connect2Serial(ref serialPort);
                            break;

                        case States.ResetAlarm:
                            RaiseAlarmEvent(false, ClassLibrary.AlarmTypes.NoAlarm);
                            WipeAlarm(ref serialPort);
                            break;

                        case States.PinVerified:
                            UnlockDoor(ref serialPort);
                            break;

                        case States.DoorIsClosedButNotLocked:
                            LockDoor(ref serialPort);
                            door.DoorLocked_e5 = true;
                            updateGlobalDoor(door);
                            break;

                        case States.ResetAccessProcess:
                            EnterPinTimer.Interval = 45000;
                            door.AccessTry_e4 = false;
                            updateGlobalDoor(door);
                            ToggleCheckBoxesInSimSim(4, false, ref serialPort);
                            ResetProccess = false;
                            break;

                        case States.FourDigitsEntered:
                            EnterPinTimer.Interval = 10;
                            break;

                        case States.TrafficOnSerialPort:
                            string str = serial_data_received(ref serialPort);  //will only take 65 byte at a time so only one send at the time will be handled
                            updateVarsFromSerialString(str, ref door);          //update current door status
                            updateGlobalDoor(door);
                            if (AccessTry) KeyPadStorage(ref door);
                            WipeKeyPadSIMSIM(ref serialPort, ref door);
                            break;

                        case States.AccessTry:
                            EnterPinTimer.Start();
                            door.enteredPin = string.Empty;
                            AccessTry = true;
                            break;
                        case States.StartDoorTimer:
                            DoorTimer.Start();
                            doorTimerStarted = true;
                            break;
                        case States.DoorClosed:
                            DoorTimer.Stop();
                            doorTimerStarted = false;
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(20);
            }
        }
        private static void SetupTimers(ref System.Timers.Timer doorTimer,ref System.Timers.Timer enterPinTimer)
        {
            doorTimer.Interval = 60000;
            doorTimer.AutoReset = false;
            doorTimer.Elapsed += new ElapsedEventHandler(Doortimer_Timer_Elapsed);

            enterPinTimer.Interval = 45000;
            enterPinTimer.AutoReset = false;
            enterPinTimer.Elapsed += new ElapsedEventHandler(EnterPin_Timer_Elapsed);
        }
        private static void WipeAlarm(ref SerialPort serialPort)
        {
            SetResetAlarm setResetAlarm = new SetResetAlarm(mainform.SetResetAlarm);
            ToggleCheckBoxesInSimSim(7, false, ref serialPort);
            mainform.Invoke(setResetAlarm, false);
            Alarm = false;
        }
        private static void updateGlobalDoor(Door door)
        {  //pass current door status to main thread
            SetPublicDoor setPublicDoor = new SetPublicDoor(mainform.SetPublicDoor);
            mainform.Invoke(setPublicDoor, door); 
        }
        private static void LockDoor(ref SerialPort serialPort)
        {
            ToggleCheckBoxesInSimSim(5, true, ref serialPort);
        }
        private static void UnlockDoor(ref SerialPort serialPort)
        {
            ToggleCheckBoxesInSimSim(5, false, ref serialPort);
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
        private static void Doortimer_Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            var a = (System.Timers.Timer)sender; //get timer handle
            RaiseAlarmEvent(true, AlarmTypes.DoorOpenTooLong);
            a.Stop();
        }
        private static void EnterPin_Timer_Elapsed(object? sender, ElapsedEventArgs e)
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
        private static void updateVarsFromSerialString(string str, ref Door door)
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
            if (arr[7]) RaiseAlarmEvent(true, ClassLibrary.AlarmTypes.GenericAlarm);
            if (int.Parse(result[6]) > 500) RaiseAlarmEvent(true, AlarmTypes.ForceDoor);
        }
        private static void RaiseAlarmEvent(bool x, int y)
        {
            SetAlarm setAlarm = new SetAlarm(mainform.SetAlarm);
            mainform.Invoke(setAlarm, x, y);
            Alarm = true;
        }
        private static void KeyPadStorage(ref Door door)
        {
            if (door.KeyPad[0]) door.enteredPin += "0";
            if (door.KeyPad[1]) door.enteredPin += "1";
            if (door.KeyPad[2]) door.enteredPin += "2";
            if (door.KeyPad[3]) door.enteredPin += "3";
        }
        private static bool GetPublicPinValidation() => (bool)mainform.Invoke(new GetPinValidation(mainform.GetPinValidation));
        private static bool GetPublicResetAlarm() => (bool)mainform.Invoke(new GetResetAlarm(mainform.GetResetAlarm));
    }
}
