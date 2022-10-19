using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;

namespace ClassLibrary
{
    public class SQLUser_Query
    {
        public static async Task<User> GetUser(CardInfo a)
        {
            int cardID = a.CardID;
            User Bruker = new User();

            var cs = "";
            using var con = new NpgsqlConnection(cs);

            await using var conn = new NpgsqlConnection(cs);
            await con.OpenAsync();

            using var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = $"select * from usertable where personid = {cardID};";

            await using NpgsqlDataReader rdr = cmd.ExecuteReader();
            {
                while (await rdr.ReadAsync())
                {
                    Bruker = new User(rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(0), rdr.GetDateTime(3), Convert.ToString(rdr.GetInt32(4)));
                }
            }
            con.Close();
            return Bruker;
        }

    }
    public class Messages
    {
        public static string AddPackageIdentifier(string identifier, string aString) => identifier + aString;
        public static string GetPackageIdentifier(ref string? stringIn,out string stringOut)
        {
            if (stringIn != string.Empty)
            {
                string temp = stringIn.Substring(0, PackageIdentifier.Length);
                stringOut = stringIn.Remove(0, PackageIdentifier.Length);
                return temp;
            }
            else
            {
                stringOut = string.Empty;
                return string.Empty;
            }


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
        public bool AccessTry_e4 { get; set; } = false;
        public bool DoorLocked_e5 { get; set; } = true;
        public bool DoorOpen_e6 { get; set; } = false;
        public bool Alarm_e7 { get; set; } = false;
        public string enteredPin { get; set; }
        public int DoorForce { get; set; }

        public Door()
        {
            enteredPin = "";
            KeyPad = new bool[4];
        }
    }
    public class User
    {
        public string Fornavn { get; set; }
        public string Etternavn { get; set; }
        public string Pin { get; set; }     //string? means it can be null
        public int CardID { get; set; }
        public DateTime? EndDato { get; set; }
        public User()
        {
            Fornavn = "";
            Etternavn = "";
            Pin = "";
            CardID = 0;
        }
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
    {   //generates default contructor
        public int CardID { get; set; }
        public string PinEntered { get; set; }
        public int DoorNr { get; set; }
        public DateTime Time { get; set; }
    }
    public static class States
    {
        public const int SerialPortNotOpen = 1;
        public const int DoorIsClosedButNotLocked = 5;
        public const int TrafficOnSerialPort = 8;
        public const int GenericAlarmFromDoor = 17;
        public const int ForceAlarmFromDoor = 18;
        public const int LockDoor = 13;
        public const int OpenDoor = 14;
        public const int StartDoorTimer = 12;
        public const int StopDoorTimer = 19;
        public const int LostConnection = 20;

        public const int ResetAlarm = 2;
        public const int AlarmRaised = 3;
        public const int PinVerified = 4;
        
        public const int ResetAccessProcess = 6;
        public const int FourDigitsEntered = 7;
        
        public const int AccessTry = 9;
        public const int SendAlarmEvent = 10;
        public const int TrafficOnTCPsocket = 11;
        
        public const int DoorClosed = 14;
        public const int KeyPadPressed = 15;
        public const int FormCardSwipe = 16;

    }
    public static class AlarmTypes
    {
        public const int NoAlarm = 0;
        public const int ForceDoor = 1;
        public const int DoorOpenTooLong = 2;
        public const int GenericAlarm = 3;
        public static string toString(this int x)
        {
            string a = string.Empty;
            switch (x)
            {

                case 1:
                    a = "Door forced";
                    break;
                case 2:
                    a = "Door open to long";
                    break;
                case 3:
                    a = "Generic Alarm";
                    break;
                default:
                    break;
            }
            return a;
        }
    }

    public class AlarmEvent
    {
        public int Alarm_type { get; set; }
        public DateTime Time { get; set; }
        public CardInfo LastUser { get; set; }
        public int DoorNumber { get; set; }
    }
    public class AlarmLogEntry
    {
        public User LastUser { get; set; }
        public string AlarmType { get; set; }
        public DateTime TimeStamp { get; set; }
        public int DoorNumber { get; set; }
    }
    public static class PackageIdentifier
    {
        public const int Length = 6;
        public const string ServerACK = "100001";
        public const string ClosingDown = "100002";
        public const string AlarmEvent = "200001";
        public const string ResetAlarm = "200002";
        public const string CardInfo = "300001";
        public const string RequestNumber = "400001";
        public const string PinValidation = "500001";


    }

    public class AccessEntryTry
    {
        public int DoorNr { get; set; }
        public User User { get; set; }
        public DateTime? TimeStamp { get; set; }
        public bool AccessGranted { get; set; }
        public AccessEntryTry(User? user, DateTime? timeStamp, bool accessGranted, int doorNr)
        {
            User = user;
            TimeStamp = timeStamp;
            AccessGranted = accessGranted;
            DoorNr = doorNr;
        }
    }
    public class AccessLogCardForm
    {
        public int CardID { get; set; }
        public string PinEntered { get; set; }
        public DateTime Time { get; set; }
        public bool AccessGranted { get; set; }
        public AccessLogCardForm(int cardID, string pinEntered, DateTime time, bool accessGranted)
        {
            CardID = cardID;
            PinEntered = pinEntered;
            Time = time;
            AccessGranted = accessGranted;
        }

    }
    public class ReturnCardComms   //contructor for communication from Central to cardReader
    {
        public bool Validation { get; set; }

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
    public class DoorAlarmEventArgs : EventArgs
    {
        public DoorAlarmEventArgs(int alarmtypes)
        {
            this.alarmtypes = alarmtypes;
        }

        public int alarmtypes { get; set; }

    }
    public class AccessEntryTryArgs : EventArgs
    {
        public AccessEntryTryArgs(User x)
        {
            this.data = x;
        }

        public User data { get; set; }

    }
    public class PinAnswerFromDBEventArgs : EventArgs
    {
        public PinAnswerFromDBEventArgs(ReturnCardComms returnCardComms)
        {
            this.returnCardComms = returnCardComms;
        }

        public ReturnCardComms returnCardComms { get; set; }

    }

}