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
    public delegate int GetCardID();
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
        public event EventHandler FourDigitsEntered;
        public event NewAccessRequest NewAccesRequest;
        public event PublishAlarmEvent PublishAlarm;
        public event OpenDoorEvent OpenDoorEvent;

        //locker 
        object locker = new object();
        //external vars        
        public bool ResetAlarm;

        public Door PublicDoor;
        //public bool OpenDoor;
        public bool Alarm;
        public int AlarmType;
        private string keypadstring;

        public bool DEBUG = true;

        //internal vars
        private CardReaderForm instance;
        private CardInfo sent_cardinfo;
        private TCPclient tcpClient = new TCPclient();
        private SerialClient serialClient = new SerialClient();
        private List<AccessLogCardForm> accessLog = new List<AccessLogCardForm>();
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

            tcpClient.RunCLient(this);                                                                  //starts tcpclient to cardreader
            if (DEBUG) debug();
        }
        private void debug()
        {
            ToogleSwipeCardAndPin(true);
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
            instance.Invoke(new SetAlarm(instance.PublishAlarmEvent), e.alarmtypes);                    /*When alarm is received from the door, this is on the serial thread
                                                                                                         *to be able to execute changed on UI we must marshal up to parent
                                                                                                         *that hold the UI
                                                                                                         */
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

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbComPort.Enabled = false;
            serialClient.RunCLient(this, cbComPort.SelectedItem.ToString());
            ToogleSwipeCardAndPin(true);
        }
        #endregion
        /******************************************************Functions*************************************************************/
        private void ResetAccessEntry(bool x)                                                           //alsoHandlesOpenDoor
        {
            WipePinEntered();                                                                           //removed the entered pin from UI
            UpdateListView(x);                                                                          //updates the list in the cardreader to display the user and card
            ToogleSwipeCardAndPin(true);                                                                //enables the buttons so new card can be swiped
            accessentry = false;
            if (x) FireOpenDoorEvent(x);
        }
        private void ToogleSwipeCardAndPin(bool x)
        {
            button_cardreader.Enabled = x;
            textBox1.Enabled = x;
        }
        private void SetStartStateOfPublicVariables()
        {
            keypadstring = "";
            sent_cardinfo = new CardInfo();
            Alarm = false;
            AlarmType = 0;
            ResetAlarm = false;
            PublicDoor = new Door();
        }
        private void StartUIupdater()
        {
            System.Windows.Forms.Timer UIupdateTimer;
            UIupdateTimer = new System.Windows.Forms.Timer();
            UIupdateTimer.Interval = 50;
            UIupdateTimer.Tick += new EventHandler(Timer_Elapsed);
            UIupdateTimer.Enabled = true;
        }
        public void SetKeyPadChar(string x)
        {
            if ((keypadstring.Length <= 4) & accessentry)
            {
                keypadstring += x;
                textBox_pinentered.Text = keypadstring;
            }
            if (keypadstring.Length == 4)
            {
                FireFourDigitsEnteredEvent();
            }
        }
        public void SetPublicDoor(Door x) => PublicDoor = x;
        public int getCardIDfromForm() => int.Parse(textBox1.Text);
        private void WipePinEntered()
        {
            textBox_pinentered.Text = "";
            keypadstring = "";
        }
        private void UpdateCheckBoxes()
        {
            checkBox_cardonreader.Checked = PublicDoor.AccessTry_e4;
            checkBox_doorLock.Checked = PublicDoor.DoorLocked_e5;
            checkBox_dooropen.Checked = PublicDoor.DoorOpen_e6;
        }
        private void UpdateListView(bool x)
        {
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
            string[] portNames = SerialPort.GetPortNames();
            foreach (string name in portNames)
            {
                cbComPort.Items.Add(name);
            }
        }

        private void SetDoorLabalStatus(bool x)
        {
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
  