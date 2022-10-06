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
    public delegate bool GetOpenDoorBool();


    public delegate void RestetAccessEntryForm(bool x);
    public delegate void FourDigitsEntered(object sender, EventHandler e);
    public delegate void NewCardSwiped(object sender, EventHandler e);
    public delegate void NewAccessRequest(object sender, NewAccessRequestEventArgs e);
    public delegate void PublishAlarmEvent(PublishAlarmEventArgs e);

    public partial class CardReaderForm : Form
    {
        public event EventHandler FourDigitsEntered;
        public event EventHandler NewCardSwiped;
        public event NewAccessRequest NewAccesRequest;
        public event PublishAlarmEvent PublishAlarm;

        //locker 
        object locker = new object();
        //external vars        
        public bool ResetAlarm;

        public Door PublicDoor;
        public bool OpenDoor;
        public bool Alarm;
        public int AlarmType;
        private string keypadstring;
        //private bool FourDigitsEntered;

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
            StartUIupdater();
            SetStartStateOfPublicVariables();

            this.MinimumSize = new Size(581, 390);
            this.MaximumSize = new Size(581, 390);
            //singleton
            instance = this;
            //subscribe to events
            instance.FourDigitsEntered += FourdigitsEntered;
            instance.NewCardSwiped += CardReaderForm_NewCardSwiped;
            tcpClient._PinAnswerFromDB += TcpClient_PinAnswerFromDB;
            serialClient.NewAlarmEvent += SerialClient_DoorAlarm;
            Thread.CurrentThread.Name = "CardReaderForm";

            tcpClient.RunCLient(this);
            serialClient.RunCLient(this);
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
        #region EventHandlers
        private void SerialClient_DoorAlarm(object sender, DoorAlarmEventArgs e)
        {
            //serial_thread
            var a = 1;
            instance.Invoke(new SetAlarm(instance.PublishAlarmEvent), e.alarmtypes);
        }
        private void FourdigitsEntered(object? sender, EventArgs e)
        {
            //intra thread
            CardInfo x = new CardInfo { CardID = getCardIDfromForm(), DoorNr = PublicDoor.nodeNum, PinEntered = keypadstring, Time = PublicDoor.time };
            sent_cardinfo = x;
            NewAccessRequest handler = NewAccesRequest;
            handler?.Invoke(this, new NewAccessRequestEventArgs(x)); //fire event
        }
        private void TcpClient_PinAnswerFromDB(object sender, PinAnswerFromDBEventArgs e)
        {
            //tcpClient thread
            ReturnCardComms x = e.returnCardComms;
            var a = Thread.CurrentThread;
            instance.Invoke(new RestetAccessEntryForm(instance.ResetAccessEntry), x.Validation);
        }

        private void CardReaderForm_NewCardSwiped(object? sender, EventArgs e)
        {
            accessentry = true;
            ToogleSwipeCardAndPin(false);

        }
        private void ResetAccessEntry(bool x)
        {
            WipePinEntered();
            UpdateListView(x);
            WipePinEntered();
            ToogleSwipeCardAndPin(true);
            accessentry = false;
        }
        private void ToogleSwipeCardAndPin(bool x)
        {
            button_cardreader.Enabled = x;
            textBox1.Enabled = x;
        }
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
        }
        #endregion

        private void PublishAlarmEvent(int x)
        {
            AlarmEvent z = new AlarmEvent {Alarm_type=x, DoorNumber=PublicDoor.nodeNum,LastUser=sent_cardinfo, Time=DateTime.Now};
            PublishAlarmEvent y = PublishAlarm;
            y?.Invoke(new PublishAlarmEventArgs(z));

        }
        public bool GetOpenDoorBool() => OpenDoor;
        protected virtual void SetKeyPadChar(string x)
        {
            if ((keypadstring.Length <= 4) & accessentry)
            {
                keypadstring += x;
                textBox_pinentered.Text = keypadstring;
            }
            if (keypadstring.Length == 4)
            {
                EventHandler handler = FourDigitsEntered;
                handler?.Invoke(this, EventArgs.Empty); //fire event
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
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;

        }
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
            EventHandler handler = NewCardSwiped;
            handler?.Invoke(this, EventArgs.Empty); //fire event
        }
    }
    public class NewAccessRequestEventArgs : EventArgs
    {
        public NewAccessRequestEventArgs(CardInfo carddata)
        {
            this.carddata = carddata;
        }

        public CardInfo carddata { get; set; }

    }
    public class PublishAlarmEventArgs : EventArgs
    {
        public PublishAlarmEventArgs(AlarmEvent x)
        {
            this.alarmevent = x;
        }

        public AlarmEvent alarmevent { get; set; }

    }
}
  