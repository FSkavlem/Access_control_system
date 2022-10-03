using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;

namespace CardReaderForm
{
    public delegate bool GetAccessTryEvent();
    public delegate bool GetUpdateDoorBool();
    public delegate Door GetPublicDoor();
    public delegate int GetCardID();

    public delegate void SetAccessTryEvent(bool x);
    public delegate void SetPublicDoor(Door x);
    public delegate void SetUpdateDoorBool(bool x);
    

    public partial class CardReaderForm : Form
    {
        delegate int GetCardID();
        public static object DoorLock = new object();
        public static object BoolLock = new object();
        public static bool updateDoor;
        public static bool newAccessTry;
        public static Door PublicDoor;
        private TCPclient tcpClient = new TCPclient();
        private SerialClient serialClient = new SerialClient();

        //-----------------------------------FOR DEBUGGING------------------
        public static CardInfo lastCardUsed = new CardInfo { CardID = 1222, PinEntered = "1333", Number = 1, Time = DateTime.Now };
        public static CardInfo cardInfo = new CardInfo { CardID = 1234, Number = 1, PinEntered = "1111" };
        public static AlarmEvent alarmEvent = new AlarmEvent { Alarm_bool = true, Alarm_type = 2, LastCardUsed = lastCardUsed };
        //-----------------------------------FOR DEBUGGING------------------
        public CardReaderForm()
        {
            InitializeComponent();
            updateDoor = false;
            newAccessTry = false;

            tcpClient.RunCLient(this);
            serialClient.RunCLient(this);
        }

        public void SetPublicDoor(Door x) => PublicDoor = x;
        public Door GetPublicDoor() => PublicDoor;
        public void SetUpdateDoorBool(bool x) => updateDoor = x;
        public bool GetUpdateDoorBool() => updateDoor;
        public int GetTextID() => int.Parse(textBox1.Text);
        public bool GetNewAccessTry() => newAccessTry;
        public void SetNewAccessTry(bool x) => newAccessTry=x;
        public int getCardIDfromForm() => int.Parse(textBox1.Text);

  

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;

        }
    }

}