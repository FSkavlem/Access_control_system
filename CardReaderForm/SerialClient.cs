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
    /*********************************************handles for other threads(delegates)***************************************/
   public delegate void DoorAlarms(object sender, DoorAlarmEventArgs e);
   public delegate void DoorConnectStatus(object sender, bool x);
    class SerialClient
    {
    /*******************************************************Public vars******************************************************/
        public static CardReaderForm cardreaderform;
        public static SerialClient serialClient;
        public static SerialPort serialPort = new SerialPort();
        public Door doorStatus;
        public event DoorAlarms NewAlarmEvent;
        public event EventHandler DoorTimerExpired;
        public event EventHandler DoorConnectStatus;

        public bool alarmOn { get; set; }
        bool Opendoor{ get; set; }
        public static bool LostConnection { get; set; }
        System.Timers.Timer AlarmTimer;
        public void RunCLient(CardReaderForm f, string ComPortName)
        {
            cardreaderform = f;
            Thread SerialThread = new Thread(StartSerialClient);                               //starts the serial client on separate thread
            SerialThread.Name = "SerialclientThread";                                          //sets name on thread
            SerialThread.IsBackground = true;                                                   
            SerialThread.Start(ComPortName);                                                   //starts the thread
        }
        private void StartSerialClient(object arg)
        {
            string comportname = (string)arg;
            System.Timers.Timer PollingTimer = new System.Timers.Timer();   
            System.Timers.Timer DoorTimer = new System.Timers.Timer();
            AlarmTimer = new System.Timers.Timer();

            SetupTimers(ref PollingTimer, 3000, true, true);                                   //starts a timer that polls the door every 3 second
            SetupTimers(ref DoorTimer, 60000, false, false);                                   //sets the door open timer to 60sek, will publish alarms after
            SetupTimers(ref AlarmTimer, 6000, false, false);                                   //publishes alarms in 6 seconds interval
            PollingTimer.Elapsed += new ElapsedEventHandler(PollingTimer_Timer_Elapsed);       //sets the method when polling timer has elapsed
            DoorTimer.Elapsed += new ElapsedEventHandler(Doortimer_Timer_Elapsed);             //sets the method when open door timer has elapsed   
            AlarmTimer.Elapsed += new ElapsedEventHandler(AlarmTimer_Timer_Elapsed);           //sets the method when 6 secons has elapsed when sending alarms

            cardreaderform.OpenDoorEvent += Mainform_OpenDoorEvent;                            //event that cardreaderform publishes when door is to be opened
            serialPort.ErrorReceived += SerialPort_ErrorReceived;                              //event when serialport gets error, is not currently working as intended
    /****************************************************inital start values*************************************************/
            doorStatus = new Door();
            bool serialportHasbeenOpened = false;
            Opendoor = false;
            alarmOn = false;
            LostConnection = false;
            serialClient = this;
            serialPort = new SerialPort();
    /****************************************************State selector******************************************************/
            /*
             * the design idea behind the serial communicator is a contantly updating statemachine. 
             * this is done by having a never ending while loop with if statements that listens to changes in the program.
             * This approach was choosen, since it the method the school provided during classes.
             * Ironically this statemachine mimics an event driven programming architecture
             * The serialcommunicator program could use a new refractor where the whole program is rewritten. 
             * This is due to the many alterations done to the program during the development of the project. 
             */
            while (true)
            {
                if (!serialPort.IsOpen & !serialportHasbeenOpened) Statemachine(States.SerialPortNotOpen);                 //state: serial port is not open 
                if (!serialPort.IsOpen & serialportHasbeenOpened) Statemachine(States.LostConnection);                     //state: serialcommuncation port lost, currently not working
                if (serialPort.IsOpen) { if (serialPort.BytesToRead > 0) Statemachine(States.TrafficOnSerialPort); }       //state: when traffic is received on the serialport
                if (doorStatus.Alarm_e7) Statemachine(States.GenericAlarmFromDoor);                                        //state: generic alarm is triggered on door                
                if (doorStatus.DoorForce > 500) Statemachine(States.ForceAlarmFromDoor);                                   //state: the door has been forced open
                if (doorStatus.DoorOpen_e6 & doorStatus.DoorLocked_e5) Statemachine(States.ForceAlarmFromDoor);            //state: the door has been opened when it was locked
                if (Opendoor) Statemachine(States.OpenDoor);                                                               //state: unlocks and open the door
                if (!doorStatus.DoorOpen_e6 & !doorStatus.DoorLocked_e5) Statemachine(States.DoorIsClosedButNotLocked);    //state: the door is closed but not locked, locks door
                if (doorStatus.DoorOpen_e6) Statemachine(States.StartDoorTimer);                                           //state: starts the opendoortimer when door is opened
                if (!doorStatus.DoorOpen_e6) Statemachine(States.StopDoorTimer);                                           //state: stops the opendoortimer when the door is closed

            }
    /****************************************************Statemachine********************************************************/
            void Statemachine(int states)
            {
                switch (states)
                {
                    case States.SerialPortNotOpen:                             //state: serial port is not open       
                        connect2Serial(comportname,19200);                     //this method connects to serialport passed from cardreaderform at 19200 baud
                        serialportHasbeenOpened = true;                        //bool used by statemachineselector to choose state
                        break;
                    case States.LostConnection:                                //state: serialcommuncation port lost, currently not working
                        serialportHasbeenOpened = false;                       //bool used by statemachineselector to choose state
                        LostConnection = false;                                 
                        DoorConnectStatus?.Invoke(false, new EventArgs());     //flags event subscribed by cardreaderform to display lost connection in UI 
                        break;
                    case States.TrafficOnSerialPort:                           //state: when traffic is received on the serialport
                        DoorConnectStatus?.Invoke(true, new EventArgs());      //flags event subscribed by cardreaderform to display established connection in UI 
                        string str = serial_data_received();                   //will only take 65 byte which is the string size, only receives one send package at the time
                        doorStatus = DecompileSerialString(str);               //takes the recived string from door and decompiles it into a Door class
                        updateGlobalDoor(doorStatus);                          //updates the Global Door variables shared between Cardreaderform, serialclient and tcpclient
                        break;
                    case States.GenericAlarmFromDoor:                          //state: generic alarm is triggered on door  
                        SetGlobalAlarm(AlarmTypes.GenericAlarm);               //this method flags the alarm event with a Generic alarm that contains a timestamp
                        break;
                    case States.ForceAlarmFromDoor:                            //state: the door has been forced open
                        SetGlobalAlarm(AlarmTypes.ForceDoor);                  //this method flags the alarm event with a forced door alarm that contains a timestamp
                        break;
                    case States.OpenDoor:                                      //state: unlocks and open the door           
                        UnlockDoor();                                          //method that unlocks the door
                        OpenDoor();                                            //method that opens the door
                        doorStatus.DoorLocked_e5 = false;                      //changes the global variable that the door has been unlocked
                        doorStatus.DoorOpen_e6 = true;                         //changes the global variable that the door has been opened
                        Opendoor = false;                                      //sets the statemachine bool variable to false since door has been opened
                        break;
                    case States.DoorIsClosedButNotLocked:                      //state: the door is closed but not locked, locks door
                        LockDoor();                                            //this method locks the door
                        doorStatus.DoorLocked_e5 = true;                       //changes the global variable that the door has been locked        
                        break;
                    case States.StartDoorTimer:                                //state: starts the opendoortimer when door is opened
                        DoorTimer.Enabled = true;                              //starts the Doortimer 
                        break;
                    case States.StopDoorTimer:                                 //state: stops the opendoortimer when the door is closed
                        DoorTimer.Enabled = false;                             //stops the Doortimer 
                        break;
                    default:                                                   //default state is NIL, so nothing happends if no state is choosen
                        break;
                }

            }
        }
    /****************************************************EventHandlers********************************************************/
        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {   
            /* We tried for the project to implement an error function when the serial connection is lost.
             * we were not successful in this and, when the nature of disconnect is hard to detect.
             * one idea is to have a timer that starts when the door is polled, and if it does not responds 
             * asume lost connection from the door
             */                                                         
        }
        private void OpenDoor() => ToggleCheckBoxesInSimSim(6, true);                                       //this method sets the check box 6 true in SIMSIM                              
        private void Mainform_OpenDoorEvent(bool x) => Opendoor = x;                                        //sets statemachine bool when event is flagged
        private static void PollingTimer_Timer_Elapsed(object? sender, ElapsedEventArgs e) => PollDoor();   //moves from timerthread to polldoor from serialclient thread
        private static void Doortimer_Timer_Elapsed(object? sender, ElapsedEventArgs e)                     //executed when doortimer ends
        {
            var a = (System.Timers.Timer)sender;                                                            //get timer handle
            a.Enabled = false;                                                                              //stops timer
            serialClient.SetGlobalAlarm(AlarmTypes.DoorOpenTooLong);                                        //this method flags the alarm event doooropentolong subscribed by cardreaderform 
        }
        private static void AlarmTimer_Timer_Elapsed(object? sender, ElapsedEventArgs e)                    //executed when alarmtimer has elapsen
        {   
            var a = (System.Timers.Timer)sender;                                                            //get timer handle
            a.Stop();                                                                                       //stops timer
            serialClient.alarmOn = false;                                                                   //sets bool for statemachine selector 
        }
    /****************************************************Functions********************************************************/
        private void SetGlobalAlarm(int x)              
        {
            /* 
             * this function takes a alarmtype(as int) and flags the event doorAlarms, this is subscribed on by
             * cardreaderform, so the alarms is passed to TCP client and sent to sentral
             */
            if (!alarmOn){                                          //ensures that alarms are only sent each 6 seconds
                AlarmTimer.Start();                                 //starts the alarmtimer that count down 6 seconds to hinder spam of alarm
                TurnOffAlarmSIMSIM();                               //turns of any generic alarm from SIMSIM if present
                doorStatus.Alarm_e7 = false;                        //sets the local door alarm to false
                DoorAlarms doorAlarms = NewAlarmEvent;              //makes the class doorAlarms
                doorAlarms?.Invoke(this, new DoorAlarmEventArgs(x));//flags the event doorAlarms subscriber cardReaderForm
                alarmOn = true;                                     //sets alarmon bool that is set false by alarmtimer 
            }
        }
        private static void updateGlobalDoor(Door doorStatus)
        {
            /* 
             * this function updates the shared door between the threads. Its wrapped in a try catch, due to when exiting application
             * parent thread cardreaderform. Parent thread is ended before child for unknown reasons.
             */
            try
            {
                SetPublicDoor setPublicDoor = new SetPublicDoor(cardreaderform.SetPublicDoor); //makes a delegate to pass to marshal
                cardreaderform.Invoke(setPublicDoor, doorStatus);                              //invokes the marshal and passed to serialclient
            }
            catch (Exception)
            {
            }
        }
        private static void LockDoor() => ToggleCheckBoxesInSimSim(5, true);                   //refractor to lock door by toggling checkbox 5 in SIMSIM
        private static void UnlockDoor() => ToggleCheckBoxesInSimSim(5, false);                //refractor to unlock door by toggling checkbox 6 in SIMSIM
        private static void TurnOffAlarmSIMSIM() => ToggleCheckBoxesInSimSim(7, false);        //refractor to unlock door by toggling checkbox 7 in SIMSIM
        private static void ToggleCheckBoxesInSimSim(int n, bool x)
        {
            /* 
             * this function is a generic function that updates the checkboxes in SIMSIM
             */
            string temp = "";
            if (x) temp = "1";
            else temp = "0";
            string parseString = string.Format("$O{0}{1}", n, temp);                           //builds string
            serialPort.Write(parseString);                                                     //writes string to serialport.
        }
        private static Door DecompileSerialString(string str)
        {
            /* 
            *  this function decompiles the received serialstring from the door(SIMSIM) by using REGEX function and
            *  puts the variables in a Class called Door and updates the parameters of this object IAW.
            *  received string. The received string looks like
            *  \n\r$A001B20221002C120157D01010000E00000000F0500G0500H0500I020J020# where the different letter are
            *  A = nodeNum B="yyyyMMdd" C="hhmmss" D=8inputs E=8outputs F=termistor G=Potm1 H=Potm2 I=TempSens1 J=TempSens2 
            */
            Door door = new Door();                                                       //creates the door object
            str = str.Remove(str.Length - 1);                                             //removed end hashtag
            string azpattern = "[A-J]+";                                                  //sets the REGEX character range
            string[] result = System.Text.RegularExpressions.Regex.Split(str, azpattern); //returns a string array split on Chararacterrange fomr azpattern
            door.nodeNum = int.Parse(result[1]);                                          //doornumber
            door.time = GetDatetimeFromStrings(result[2] + result[3]);                    //this method combines B and C into a datetime object
            bool[] arr = GetBoolArrayFromString(result[5]);                               //deserialize the inputs checkboxes and makes a bool array
            door.KeyPad = new bool[] { arr[0], arr[1], arr[2], arr[3] };                  //legacy when SIMSIM was used as keypad
            door.AccessTry_e4 = arr[4];                                                   //new accesstry from Simsim
            door.DoorLocked_e5 = arr[5];                                                  //door locked/unlocked from SIMSIM
            door.DoorOpen_e6 = arr[6];                                                    //checkbox true door open, checkbox false door closed
            door.Alarm_e7 = arr[7];                                                       //Generic alarm from door(SIMSIM) 
            door.DoorForce = int.Parse(result[6]);                                        //force potmeter from door
            return door;                                                                  //returns the door
        
        }
        private static bool[] GetBoolArrayFromString(string v)
        {
            /* 
            * this function converts a string of binary values to bool array and returns it
            */                          
            bool[] result = new bool[8];                                                 //creates a empty bool array
            for (int i = 0; i < 8; i++)                                                  //loops over the binary string
            {
                if (v[i] == '1') result[i] = true;                                       //if the string place is 1 set array position to true
                else result[i] = false;                                                  //if its 0 sett array position to false
            }
            return result;                                                               //returns the boolarray
        }
        private static DateTime GetDatetimeFromStrings(string v)
        {
           /* 
            * this function gets takes a string and converts it to datetime object and returns it
            */
            DateTime parsedDate;                                                         //makes an empty datetime object
            DateTime.TryParseExact(v, "yyyyMMddhhmmss", null,                            //uses built in function tryparse from string to datetime
                               System.Globalization.DateTimeStyles.AllowWhiteSpaces |    //allows for spaces in the string if garble has happend
                               System.Globalization.DateTimeStyles.AdjustToUniversal,    //uses time style format from computer
                               out parsedDate);                                          //get the datetime object from tryparse function
            return parsedDate;                                                           //returns the datetime object
        }
        private static string serial_data_received()
        {
            /* 
             * this function reads 65 byte from serial port
             */
            byte[] buffer = new byte[65];                                               //makes a byte array of 65 bytes
            serialPort.Read(buffer, 0, 65);                                             //reads 65 bytes from the serialp port
            string str = Encoding.Default.GetString(buffer);                            //converts the byte array to string 
            return str;                                                                 //returns string
        }
        private static void connect2Serial(string com, int baud)
        {
           /* 
            * this method opens the selected COM port selcted in cardreadform
            */  
            serialPort = new SerialPort(com, baud);                                     //creates a new serialport object
            serialPort.NewLine = "\n\r";                                                //staes start of message 
            serialPort.Encoding = Encoding.UTF8;                                        //represent characters as ASCII 
            serialPort.ReadTimeout = 1000;                                              //sets timeout to 1 second for read 
            serialPort.WriteTimeout = 2000;                                             //sets timeout to 2 second to write to port
            serialPort.Open();                                                          //opens the COMport

        }
        private static void SetupTimers(ref System.Timers.Timer x,int interval, bool autoreset, bool start)
        {
           /* 
            * generic method to setups timers
            */
            x.Interval = interval;                                                      //sets how long the timer will run til it flags elsapsed event
            x.AutoReset = autoreset;                                                    //sets if the timer should start timer agian when timer has elapsed
            x.Enabled = start;                                                          //starts the given timer
        }

        private static void PollDoor()
        {  /* 
            * This method polls the door to get the door to publish its string on comport
            */
            try
            {
                serialPort.Write("$R");                                                 //sends $R to SIMSIM which gets SIMSIM to publish its current state
            }
            catch (Exception)
            {
                if (serialPort.IsOpen)                                                  //tries to apply error if connection is lost to simsim
                {           
                    LostConnection = true;
                    serialPort.Close();
                    serialPort.Dispose();
                }
            }

        }
    }

}

