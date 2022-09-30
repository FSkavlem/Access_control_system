using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClassLibrary
{
    
    public delegate void UserValidation(object source, UserValidationArgs args);
    public class UserValidationArgs : EventArgs
    {
        public CardComms cardComms { get; }
        public UserValidationArgs(CardComms cc) => cardComms = cc;
    }

    public class SharedMethod
    {
        public static string ReceiveString(Socket comSocket, out bool error)
        {
            string answer = "";
            error = false;
            try
            {
                byte[] data = new byte[1024];
                int antallBytesMottatt = comSocket.Receive(data);

                if (antallBytesMottatt > 0) answer = Encoding.ASCII.GetString(data, 0, antallBytesMottatt);
                else error = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Feil: " + e.Message);
                error = true;
            }
            return answer;
        }

        public static bool SendString(Socket comSocket, string data, out bool error)
        {
            error = false;
            try
            {
                byte[] data1 = Encoding.ASCII.GetBytes(data);
                comSocket.Send(data1, data1.Length, SocketFlags.None);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                error = true;
                return false;
            }
            
        }
    }



    public class Bruker
    {
        private string? fornavn;
        private string? etternavn;
        private string? pin;        //string? means it can be null
        private int kortID;
        private DateTime endDato;
        

        public Bruker(string fornavn, string etternavn, int kortID, DateTime endDato, string pin)
        {
            this.Fornavn = fornavn;
            this.Etternavn = etternavn;
            this.KortID = kortID;
            this.EndDato = endDato;
            this.Pin = pin;
        }

        public string? Fornavn { get => fornavn; set => fornavn = value; }
        public string? Etternavn { get => etternavn; set => etternavn = value; }
        public string? Pin { get => pin; set => pin = value; }
        public int KortID { get => kortID; set => kortID = value; }
        public DateTime EndDato { get => endDato; set => endDato = value; }

    }
    public class CardComms
    {
        private int cardID;
        private string? pin;
        private int number;
        private DateTime? time;
        private bool need_validation;
        private bool alarm_bool;
        private int alarm_type;
        private int lastuser;

        public CardComms()  //contructor for communication from card reader to Central
        {
            this.cardID = 0;
            this.pin = "";
            this.time = null;
            this.number =0;
            this.alarm_bool = false;
            this.need_validation = false;
            this.alarm_type = 0;
            this.lastuser = 0;
        }
        public int CardID { get => cardID; set => cardID = value; }
        public string? Pin { get => pin; set => pin = value; }
        public DateTime? Time { get => time; set => time = value; }
        public int Number { get => number; set => number = value; }
        public int Lastuser { get => lastuser; set => lastuser = value; }
        public bool Alarm_bool { get => alarm_bool; set => alarm_bool = value; }
        public int Alarm_type { get => alarm_type; set => alarm_type = value; }
        public bool Need_validation { get => need_validation; set => need_validation = value; }
    }
    public class ReturnCardComms   //contructor for communication from Central to cardReader
    {
        private bool validation;
        public ReturnCardComms(bool x = false)  //bool x = false makes contructor optional and sets deafult to false
        {
            validation = x;
        }
        public bool Validation { get => validation; set => validation = value; }
    }
 
}