using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Timers;
using System.Xml.Serialization;
using System.Diagnostics.Eventing.Reader;
using System;

namespace CardReaderForm
{
    /******************************************************Handles for thread communication*************************************************************/
    public delegate int GetCardID();
    public delegate int GetDoorNr();
    public delegate bool GetResetAlarm();
    public delegate Door GetPublicDoor();
    public delegate void SetCardSwipeForm(bool x);
    public delegate void SetPublicDoor(Door x);
    public delegate Tuple<bool, int> GetAlarm();
    public delegate void SetAlarm(int y);
    public delegate void RestetAccessEntryForm(bool x);
    public delegate void FourDigitsEntered(object sender, EventHandler e);
    public delegate void NewCardSwiped(object sender, EventHandler e);
    public delegate void NewAccessRequest(object sender, NewAccessRequestEventArgs e);
    public delegate void PublishAlarmEvent(PublishAlarmEventArgs e);
    public delegate void OpenDoorEvent(bool x);
    public delegate void SetDoorLabelStatus(bool x);
    public delegate void SetSeverLabelStatus(bool x);

    public partial class CardReaderForm : Form
    {
        /******************************************************global vars*************************************************************/
        public event EventHandler FourDigitsEntered;
        public event NewAccessRequest NewAccesRequest;
        public event PublishAlarmEvent PublishAlarm;
        public event OpenDoorEvent OpenDoorEvent;
        /*********************************************external class vars used by other threads****************************************/
        public bool ResetAlarm;
        public Door PublicDoor;
        public bool Alarm;
        public int AlarmType;
        private string keypadstring;

        public bool DEBUG = true;                                                                      //to activate swipcard and doornr without serial connection

        /*********************************************internal class vars used by cardreade rform****************************************/
        private CardReaderForm instance;
        private CardInfo sent_cardinfo;
        private TCPclient tcpClient = new TCPclient();
        private SerialClient serialClient = new SerialClient();
        private bool accessentry;

        public CardReaderForm()
        {
            InitializeComponent();
            StartUIupdater();                                                                           //starts a timer that updates door open/closed checkbox
            SetStartStateOfPublicVariables();                                                           //sets initial public variables
            SetAvailableSerialPortsToDropdown();                                                        //gets available comports and put the in combobox

            this.MinimumSize = new Size(581, 390);                                                      //size of window is hardcoded so its not changeable
            this.MaximumSize = new Size(581, 390);                                                     

            instance = this;                                                                            //singleton
                                                                                                        //subscribe to events
            instance.FourDigitsEntered += FourdigitsEntered;                                            //event when 4 digits is entered on keypad
            tcpClient._PinAnswerFromDB += TcpClient_PinAnswerFromDB;                                    //event when validation of pin to card is received
            tcpClient.ServerConnectStatus += TcpClient_ServerConnectStatus;                             //event when changes in server, cardreader connection
            serialClient.NewAlarmEvent += SerialClient_DoorAlarm;                                       //event when alarm from door is raised
            serialClient.DoorConnectStatus += SerialClient_DoorConnectStatus;                           //event when connection to door is changed 

            Thread.CurrentThread.Name = "CardReaderForm";

            tcpClient.RunCLient(this,GetDoorNr());                                                      //starts tcpclient to cardreaderform
            if (DEBUG) debug();                                                                         //sets debugmode
        }
        private void debug()                                            
        {
            ToogleSwipeCardAndPin(true);
            textBox_doornr.Enabled = true;
        }

        /******************************************************Event Handlers*************************************************************/
        #region EventHandlers
        private void TcpClient_ServerConnectStatus(object? sender, EventArgs e)                 
        {
            instance.Invoke(new SetSeverLabelStatus(instance.SetServerLabalStatus), (bool)sender);      //When Server connection status changes set indicator in UI. Marshal to UI thread
        }
        private void SerialClient_DoorConnectStatus(object? sender, EventArgs e)
        {
            instance.Invoke(new SetDoorLabelStatus(instance.SetDoorLabalStatus), (bool)sender);         //When Door connection status changes set indicator in UI. Marshal to UI thread
        }
        private void SerialClient_DoorAlarm(object sender, DoorAlarmEventArgs e)                        
        {
            /*When alarm is received from the door, this is on the serial thread
            *to be able to execute changed on UI we must marshal up to parent
            *that hold the UI
            */
            instance.Invoke(new SetAlarm(instance.PublishAlarmEvent), e.alarmtypes);                    
        }
        private void FourdigitsEntered(object? sender, EventArgs e)
        {
            CardInfo x = new CardInfo { CardID = getCardIDfromForm(), DoorNr = PublicDoor.nodeNum, PinEntered = keypadstring, Time = DateTime.Now };
            sent_cardinfo = x;                                                                          //assembles the class to hold the accessrequest
            NewAccessRequest handler = NewAccesRequest;                                                 
            handler?.Invoke(this, new NewAccessRequestEventArgs(x));                                    //flaggs the event NewAccessRequest with the accessrequest
        }                                                                                               //wrapped in the EventArgs
        private void TcpClient_PinAnswerFromDB(object sender, PinAnswerFromDBEventArgs e)
        {       
            //tcpClient thread                                                                          //event that is published by TCP client when we 
            ReturnCardComms x = e.returnCardComms;                                                      //have received ansveR if the pin entered was correct
            instance.Invoke(new RestetAccessEntryForm(instance.ResetAccessEntry), x.Validation);        //lifts the validation to parent(cardreaderform)
        }
        private void PublishAlarmEvent(int x)
        {                                                                                               //this is the elevated alarm in the cardreadform.
            AlarmEvent z = new AlarmEvent { Alarm_type = x, DoorNumber = PublicDoor.nodeNum, LastUser = sent_cardinfo, Time = DateTime.Now };
            PublishAlarm?.Invoke(new PublishAlarmEventArgs(z));                                         //this event is subscribed by TCP connection handler to send alarm to sentral
        }

        private void FireFourDigitsEnteredEvent() => FourDigitsEntered?.Invoke(this, EventArgs.Empty); //flags the fourdigitsentered event
        private void FireOpenDoorEvent(bool x) => OpenDoorEvent?.Invoke(x);                            //flags the opendoorevent subscribed by serialCLient
        private void Timer_Elapsed(object sender, EventArgs e) => UpdateCheckBoxes();                  //updates the door status on the UI when timer has elapsed
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            /* 
             * this event ensures that the inserted keys in cardid is only digits. still possible to copy paste characters in.
             */
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* 
             * this event starts the serialclient when a COMPORT is selected.
             */
            cbComPort.Enabled = false;
            serialClient.RunCLient(this, cbComPort.SelectedItem.ToString());
            ToogleSwipeCardAndPin(true);
        }
        #endregion
        /*************************************************delegate handlers**********************************************************/
        public void SetPublicDoor(Door x) => PublicDoor = x;            //sets the public door held in Cardreaderform
        public int getCardIDfromForm() => int.Parse(textBox1.Text);     //gets the doornr from UI
        /******************************************************Functions*************************************************************/
        private void ResetAccessEntry(bool x)                                                           
        {
            /* 
             * this method resets the UI to be able to swipe a new card. makes a entry in the listview regardless
             * if the access was approved or not. Flags the event to open door based on the bool x
             */
            WipePinEntered();                                           //removed the entered pin from UI
            UpdateListView(x);                                          //updates the list in the cardreader to display the user and card
            ToogleSwipeCardAndPin(true);                                //enables the buttons so new card can be swiped
            accessentry = false;                                                                    
            if (x) FireOpenDoorEvent(x);                                //flags the event opendoor, subscribed by serialclient and unlocks and opendoor based on x
        }
        public void SetKeyPadChar(string x)
        {
            /* 
             * ensures that no more than 4 digitis is entered in the pin entered from keypad.
             * when 4 digits has been entered, flags the Fourdigitsentered event that
             * starts the new acces request chain of events.
             */
            if ((keypadstring.Length < 4) & accessentry)               //if new accesstry is available and less than 4 digits has been entered
            {
                keypadstring += x;                                     //adds keypad digit to a string.
                textBox_pinentered.Text = keypadstring;                //displays the entered string in UI
            }
            if (keypadstring.Length == 4)                              //when the keypadstring reaches 4 digits
            {
                FireFourDigitsEnteredEvent();                          //flag event fourdigits entered that starts new accessrequest try
            }
        }
        public int GetDoorNr()
        {
            /* 
             * reutns the hardcoded doornr
             */
            string doornr = textBox_doornr.Text;                     //gets string from textbox in UI
            int placeholder = 0;                                     //creates a int placeholdt for out in tryparse
            bool success = int.TryParse(doornr, out placeholder);    //uses inbuilt function in int to get integer from string
            if (success) return placeholder;                         //if this is successful return the integer
            else return 0;                                           //if its not successful return 0
        }
        private void StartUIupdater()
        {
            /* 
             * this method starts a timer when elsapsed updates the UI 
             */
            System.Windows.Forms.Timer UIupdateTimer;               //create a timer object
            UIupdateTimer = new System.Windows.Forms.Timer();       //create a new timer object
            UIupdateTimer.Interval = 50;                            //sets the interval of timer to 50 ms
            UIupdateTimer.Tick += new EventHandler(Timer_Elapsed);  //assigns the method to flag when tiemer is elapsed
            UIupdateTimer.Enabled = true;                           //starts the timer
        }
        private void ToogleSwipeCardAndPin(bool x)
        {
            /* 
             * toogles the swipecard button and enables the card id to be writen
             */
            button_cardreader.Enabled = x;                           //enables/disables controll of the swipecard button
            textBox1.Enabled = x;                                    //enables the cardid textbox to be writen in.
        }
        private void SetStartStateOfPublicVariables()
        {
            /* 
             * this method is used to set initalvalues of global variables.
             */
            textBox_doornr.Text = "1";
            keypadstring = "";
            sent_cardinfo = new CardInfo();
            Alarm = false;
            AlarmType = 0;
            ResetAlarm = false;
            PublicDoor = new Door();
        }

        private void WipePinEntered()
        {
            /* 
             * this method remoeved the entered pin from UI
             */ 
            textBox_pinentered.Text = "";           //resets pinentered textbox where the digits from keypad is displayed
            keypadstring = "";                      //sets the keypadstring to nothing
        }
        private void UpdateCheckBoxes()
        {
            /* 
             * this method updates the checkboxes in UI based on the public door
             */
            checkBox_cardonreader.Checked = PublicDoor.AccessTry_e4;    
            checkBox_doorLock.Checked = PublicDoor.DoorLocked_e5;
            checkBox_dooropen.Checked = PublicDoor.DoorOpen_e6;
        }
        private void UpdateListView(bool x)
        {
            /* 
             * this method updates the listbox that hold if the access was for particual 
             * card was approved or not by server. 
             */
            string a;                   
            if (x) a = "Granted";
            else a = "Denied";
            string[] row = { sent_cardinfo.CardID.ToString(), sent_cardinfo.PinEntered, sent_cardinfo.Time.ToString(), a };
            ListViewItem listViewItem = new ListViewItem(row);
            listview_access_log.Items.Clear();
            listview_access_log.Items.Add(listViewItem);
        }
        private void SetAvailableSerialPortsToDropdown()
        {
            /* 
             * this method gets available COMPORTS on the system and display them in a dropdown box
             */
            string[] portNames = SerialPort.GetPortNames();
            foreach (string name in portNames)
            {
                cbComPort.Items.Add(name);
            }
        }

        private void SetDoorLabalStatus(bool x)
        {
            /* 
             * this method sets the doorstatus in UI and color it 
             * green if connected and red if its not connected
             */
            if (x)
            {
                label_connectedSerial.Text = "Connected to door";
                label_connectedSerial.BackColor = Color.Green;
            }
            else
            {
                label_connectedSerial.Text = "Not connected to door";
                label_connectedSerial.BackColor = Color.Red;
            }
        }
        private void SetServerLabalStatus(bool x)
        {
           /* 
            * this method sets the serverstatus in UI and color it 
            * green if connected and red if its not connected
            */
            if (x)
            {
                label_connectedTcp.Text = "Connected to server";
                label_connectedTcp.BackColor = Color.Green;
            }
            else
            {
                label_connectedTcp.Text = "Not connected to server";
                label_connectedTcp.BackColor = Color.Red;
            }
        }
        /******************************************************BUTTONS*************************************************************/
        private void keypad_1_Click(object sender, EventArgs e) => SetKeyPadChar("1");
        private void keypad_2_Click(object sender, EventArgs e) => SetKeyPadChar("2");
        private void keypad_3_Click(object sender, EventArgs e) => SetKeyPadChar("3");
        private void keypad_4_Click(object sender, EventArgs e) => SetKeyPadChar("4");
        private void keypad_5_Click(object sender, EventArgs e) => SetKeyPadChar("5");
        private void keypad_6_Click(object sender, EventArgs e) => SetKeyPadChar("6");
        private void keypad_7_Click(object sender, EventArgs e) => SetKeyPadChar("7");
        private void keypad_8_Click(object sender, EventArgs e) => SetKeyPadChar("8");
        private void keypad_9_Click(object sender, EventArgs e) => SetKeyPadChar("9");
        private void keypad_0_Click(object sender, EventArgs e) => SetKeyPadChar("0");
        private void button_cardreader_Click(object sender, EventArgs e)
        {
            accessentry = true;
            ToogleSwipeCardAndPin(false);
        }
    }

}
  