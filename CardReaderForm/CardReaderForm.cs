using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace CardReaderForm
{
    public delegate bool GetPinValidation();
    public delegate bool GetAccessTry();

    public delegate Door GetPublicDoor();
    public delegate int GetCardID();

    public delegate void SetPinValidation(bool x);
    public delegate void SetAccessTryEvent(bool x);
    public delegate void SetPublicDoor(Door x);
    public delegate void SetUpdateUIBool(bool x);


    public partial class CardReaderForm : Form
    {

        public bool pinvalidation;  //can only get this value Once.
        public bool UpdateUI{ set { updateCheckBoxes(); }} //fuck this make timer insted, make update UI method with timer.
        public bool newAccessTry;
        public bool newAlarmEvent;

        public Door PublicDoor;
        private TCPclient tcpClient = new TCPclient();
        private SerialClient serialClient = new SerialClient();

        public CardReaderForm()
        {
            InitializeComponent();
            newAccessTry = false;
            pinvalidation = false;
            this.MinimumSize = new Size(581, 200);
            this.MaximumSize = new Size(581, 200);
            PublicDoor = new Door();

            tcpClient.RunCLient(this);
            serialClient.RunCLient(this);
        }

        public void SetPinValidation(bool x) => pinvalidation = x; 
        public void SetPublicDoor(Door x) => PublicDoor = x;
        public Door GetPublicDoor() => PublicDoor;
        public void SetUpdateUIBool(bool x) => UpdateUI= x;
        public int GetTextID() => int.Parse(textBox1.Text);
        public bool GetNewAccessTry() => newAccessTry;
        public void SetNewAccessTry(bool x) => newAccessTry=x;
        public int getCardIDfromForm() => int.Parse(textBox1.Text);
        public bool GetPinValidation()
        {
            var x = pinvalidation;
            pinvalidation = false;
            return x;
        }
        private void updateCheckBoxes()
        {
            checkBox_doorLock.Checked = PublicDoor.DoorLocked_e5;
            checkBox_dooropen.Checked = PublicDoor.DoorOpen_e6;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;

        }
    }

}