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

namespace CardReaderForm
{
    public delegate bool GetPinValidation();
    public delegate bool GetAccessTry();
    public delegate Tuple<bool,int> GetAlarm();
    public delegate Door GetPublicDoor();
    public delegate int GetCardID();
    public delegate bool GetResetAlarm();
    public delegate Tuple<bool, string> GetKeyPadChar();
    public delegate bool GetCardSwipeForm();

    public delegate void SetPinValidation(bool x);
    public delegate void SetAccessTryEvent(bool x);
    public delegate void SetPublicDoor(Door x);
    public delegate void SetCardInfo(CardInfo x);
    public delegate void SetAlarm(bool x, int y);
    public delegate void SetResetAlarm(bool x);
    public delegate void SetCardSwipeForm(bool x);

    public partial class CardReaderForm : Form
    {
        private Tuple<bool, string> keypadchar;
        //external vars
        public bool PinValidation;  //can only get this value Once.
        public bool newAccessTry;
        public bool Alarm;
        public int AlarmType;
        public Door PublicDoor;
        public CardInfo CardinfoSent;
        public bool ResetAlarm;
        public bool FormCardSwipe;
        public Tuple<bool, string> KeyPadChar
        {
            get
            {
                Tuple<bool, string> a = keypadchar;
                keypadchar = Tuple.Create(false, "");
                return a;
            }
            set { keypadchar = value; }
        }

        //internal vars
        private TCPclient tcpClient = new TCPclient();
        private SerialClient serialClient = new SerialClient();
        private List<AccessLogCardForm> accessLog = new List<AccessLogCardForm>();

        public CardReaderForm()
        {
            InitializeComponent();
            StartUIupdater();
            SetStartStateOfPublicVariables();

            this.MinimumSize = new Size(581, 390);
            this.MaximumSize = new Size(581, 390);


            tcpClient.RunCLient(this);
            serialClient.RunCLient(this);
        }

        private void SetStartStateOfPublicVariables()
        {
            FormCardSwipe = false;
            keypadchar = Tuple.Create(false, "");
            newAccessTry = false;
            PinValidation = false;
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
        private void Timer_Elapsed(object sender, EventArgs e)
        {
            UpdateCheckBoxes();
            UpdatePinEntered();
            UpdateIfCardReaderActivated();
        }
        public void SetPinValidation(bool x)//Gets set from TCP client when pin validation is received
        {
            PinValidation = x;
            WipePinEntered();
            UpdateListView(x);
        }
        private void UpdateIfCardReaderActivated()
        {
            if (PublicDoor.AccessTry_e4)
            {
                textBox1.Enabled = false;
                button_cardreader.Enabled = false;
            }

            else
            {
                textBox1.Enabled = true;
                button_cardreader.Enabled = true;
            }  
        }
        private void UpdatePinEntered()
        {
            textBox_pinentered.Text = PublicDoor.enteredPin;
        }
        public bool GetCardSwipeForm() => FormCardSwipe;
        public void SetCardSwipeForm(bool x) => FormCardSwipe = x; 
        public Tuple<bool, string> GetKeyPadChar() => KeyPadChar;
        public void SetKeyPadChar(string x)
        {
            if (keypadchar.Item1 != true)
            {
                Tuple<bool, string> a = Tuple.Create(true, x);
                KeyPadChar = a;
            }
        }
        public void SetResetAlarm(bool x) => ResetAlarm = x;
        public bool GetResetAlarm() => ResetAlarm;
        public void SetCardInfo(CardInfo x) => CardinfoSent = x;
        public void SetPublicDoor(Door x) => PublicDoor = x;
        public Door GetPublicDoor() => PublicDoor;
        public int GetTextID() => int.Parse(textBox1.Text);
        public bool GetNewAccessTry() => newAccessTry;
        public void SetNewAccessTry(bool x) => newAccessTry=x;
        public int getCardIDfromForm() => int.Parse(textBox1.Text);
        public void SetAlarm(bool x, int y)
        {
            Alarm = x;
            AlarmType = y;
        }
        public Tuple<bool, int> GetAlarm()
        {
            return Tuple.Create(Alarm, AlarmType);
        }
        public bool GetPinValidation()
        {
            var x = PinValidation;
            PinValidation = false;
            return x;
        }
        private void WipePinEntered()
        {
            textBox_pinentered.Text = "";
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
            string[] row = { CardinfoSent.CardID.ToString(), CardinfoSent.PinEntered, CardinfoSent.Time.ToString(),a};
            ListViewItem listViewItem = new ListViewItem(row);
            
            listview_access_log.Items.Clear();
            listview_access_log.Items.Add(listViewItem);
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;

        }
        private void reset_alarm_button_Click(object sender, EventArgs e)
        {
            ResetAlarm = true;
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
            FormCardSwipe = true;
        }
    }

}