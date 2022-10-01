using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClassLibrary
{
    
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
        public string? Fornavn { get; set; }
        public string? Etternavn { get; set; }
        public string? Pin { get; set; }     //string? means it can be null
        public int KortID { get; set; }
        public DateTime? EndDato { get; set; }

        public Bruker(string fornavn, string etternavn, int kortID, DateTime endDato, string pin)
        {
            Fornavn = fornavn;
            Etternavn = etternavn;
            KortID = kortID;
            EndDato = endDato;
            Pin = pin;
        }
    }
    public class CardComms
    {

        public int CardID { get; set; }
        public string? Pin { get; set; }
        public int Number { get; set; }
        public DateTime? Time { get; set; }
        public bool Need_validation { get; set; }
        public bool Alarm_bool { get; set; }
        public int Alarm_type { get; set; }
        public int Lastuser { get; set; }

        public CardComms()  //contructor for communication from card reader to Central
        {
            CardID = 0;
            Pin = "";
            Time = null;
            Number =0;
            Alarm_bool = false;
            Need_validation = false;
            Alarm_type = 0;
            Lastuser = 0;
        }
    }

    public class AlarmLogEntry
    {
        public Bruker? LastUser { get; set; }
        public int AlarmType { get; set; }
        public DateTime? TimeStamp { get; set; }
        public AlarmLogEntry(Bruker? lastUser, int alarmType, DateTime? timeStamp)
        {
            LastUser = lastUser;
            AlarmType = alarmType;
            TimeStamp = timeStamp;
        }
    }

    public class AccessLogEntry
    {
        public Bruker? User { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool AccessGranted { get; set; }
        public AccessLogEntry(Bruker? user, DateTime? timeStamp, bool accessGranted)
        {
            User = user;
            TimeStamp = timeStamp;
            AccessGranted = accessGranted;
        }
    }
    public class ReturnCardComms   //contructor for communication from Central to cardReader
    {
        public bool Validation { get; set; }
        public ReturnCardComms(bool x = false)  //bool x = false makes contructor optional and sets deafult to false
        {
            Validation = x;
        }
    }
 
}