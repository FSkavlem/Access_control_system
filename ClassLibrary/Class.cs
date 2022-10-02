using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ClassLibrary
{
    
    public class Central
    {
        public static string AddPackageIdentifier(string identifier, string aString) => identifier + aString;
        public static string GetPackageIdentifier(ref string? stringIn,out string stringOut)
        {
            string temp= stringIn.Substring(0, PackageIdentifier.Length);
            stringOut = stringIn.Remove(0, PackageIdentifier.Length);    
            return temp;
        }
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

    public class Door
    {
        public int nodeNum { get; set; }
        public DateTime time { get; set; }
        public bool[] KeyPad { get; set; }
        public bool AccessTry_e4 { get; set; }
        public bool DoorLocked_e5 { get; set; }
        public bool DoorOpen_e6 { get; set; }
        public bool Alarm_e7 { get; set; }
        public bool DoorForcedOpen { get; set; }
        public string enteredPin { get; set; }

        public Door()
        {
            enteredPin = "";
            KeyPad = new bool[4];
        }
    }
    public class User
    {
        public string? Fornavn { get; set; }
        public string? Etternavn { get; set; }
        public string? Pin { get; set; }     //string? means it can be null
        public int CardID { get; set; }
        public DateTime? EndDato { get; set; }

        public User(string fornavn, string etternavn, int kortID, DateTime endDato, string pin)
        {
            Fornavn = fornavn;
            Etternavn = etternavn;
            CardID = kortID;
            EndDato = endDato;
            Pin = pin;
        }
    }
    public class CardInfo
    {   //generated default contructor
        public int CardID { get; set; }
        public string? PinEntered { get; set; }
        public int Number { get; set; }
        public DateTime? Time { get; set; }
    }
    public class AlarmEvent
    {
        public bool Alarm_bool { get; set; }
        public int Alarm_type { get; set; }
        public CardInfo LastCardUsed { get; set; }
        public DateTime? Time { get; set; }
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
        public User? LastUser { get; set; }
        public int AlarmType { get; set; }
        public DateTime? TimeStamp { get; set; }
        public AlarmLogEntry(User? lastUser, int alarmType, DateTime? timeStamp)
        {
            LastUser = lastUser;
            AlarmType = alarmType;
            TimeStamp = timeStamp;
        }
    }
    public static class PackageIdentifier
    {
        public const int Length = 6;
        public const string ServerACK = "100001";
        public const string AlarmEvent = "200001";
        public const string CardInfo = "300001";
        public const string RequestNumber = "400001";
        public const string PinValidation = "500001";
    }


    public class AccessLogEntry
    {
        public User? User { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool AccessGranted { get; set; }
        public AccessLogEntry(User? user, DateTime? timeStamp, bool accessGranted)
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