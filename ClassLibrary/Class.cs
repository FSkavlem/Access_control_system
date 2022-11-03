using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;

namespace ClassLibrary
{   

    /******************************************************Function Classes*************************************************************/
    public class SQLUser_Query
    {  
        public static async Task<User> GetUser(CardInfo a)
        {  /* 
            * this function is a legacy function that exists from prior iterations of the program
            * it takes a card id and gets a user from usertable in SQL database. Program is not altered to
            * return users with generic SQL_query.Query function
            */
            int cardID = a.CardID;                                         //grabs the card id from CardInfo class 
            User Bruker = new User();                                      //makes a temporary user holder
            using var con = new NpgsqlConnection(DBINFO.connectionString); //makes a postgre connection object based on the server connection string
            await con.OpenAsync();                                         //opens a connection to SQL DB asynchronously
            using var cmd = new NpgsqlCommand();                           //makes a postgre command object
            cmd.Connection = con;                                          //passes established connection to Database to command object
            cmd.CommandText = $"select * from usertable " +                //sets the SQL query string
                $"where personid = {cardID};";
            await using NpgsqlDataReader rdr = cmd.ExecuteReader();        //starts read the query from database
            {
                while (await rdr.ReadAsync())                              //row of query
                {                                                          //assigns the different values from query of DB to temporary user holder
                    Bruker = new User(rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(0), rdr.GetDateTime(3), Convert.ToString(rdr.GetInt32(4)));
                }
            }
            con.Close();                                                   //cloeses the connection to SQL DB
            return Bruker;                                                 //returns the temporary user.
        }

    }
    public class SQL_Query
    {   
        public static async Task<List<object>> Query(string querystring)        
        {   
            /* 
             * this function takes a SQL string with no limits and returns a List of objects where each entry in return list contains
             * the complete row of the query as a list of object
            */
            using var con = new NpgsqlConnection(DBINFO.connectionString); //makes a connection object based on connection string DBINFO
            await con.OpenAsync();                                         //establishes connection async and waits for it              
            using var cmd = new NpgsqlCommand();                           //makes postgre command object
            cmd.Connection = con;                                          //sets the connection point of the command to established connection
            cmd.CommandText = querystring;                                 //sets the sql command from passed string
            List<object> list = new List<object>();                        //temporary holder for list of objects
            await using NpgsqlDataReader rdr = cmd.ExecuteReader();        //starts read the query from database
            {
                while (await rdr.ReadAsync())                              //get first row of query, then second, untill no rows left
                {
                    List<object> listinner = new List<object>();           
                    for (int i = 0; i < rdr.FieldCount; i++)               //loops through the coloums of the query
                    {
                        listinner.Add(rdr.GetValue(i));                    //passes the objects in coloums of row into list inner
                    }
                    list.Add(listinner);                                   //puts innerlist of object that contains the complete row into a list
                }
            }
            con.Close();
            return list;                                                   //returns the list of rows from sql query
        }
        public static void SQLQuerylist2TXT(List<object> y, string filename, string firstLine)
        {
            /* 
             * this function takes a list of objects that contains list of objects and unwraps the lists 
             * and makes a txt document containing each row
             */
            string[] pastearr = new string[y.Count + 1];                   //defining size of array based on length of List of objects, +1 to add own line
            pastearr[0] = firstLine;                                       //firstline contains costume string, used for short info of table
            string pastestring = string.Empty;                             
            var counter = 1;                                               //counter to use in foreach

            foreach (var item in y)
            {
                var x = item as List<object>;                              //must cast item to be able to use as list
                pastestring = string.Empty;         
                for (int i = 0; i < x.Count; i++)                          //loops through items objects
                {
                    if (i != 0) pastestring += ",";                        //dont add , before data in row
                    pastestring += x[i].ToString();                        //makes a string of the row
                }       
                pastearr[counter] = pastestring;                           //assigns that string to array
                counter++;                                                  
            }
            string z = filename + " " + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";  //makes a filename based on time 
            File.WriteAllLines(z, pastearr);                               //converts array of strings into txt file
        }
    }
    public class SQL_insertion
    {
        public static async void InsertIntoAlarmLog(AlarmLogEntry x)
        {                                                                   //Inserts string into DB alarmlog asynchronously
            string insertstring = $"INSERT INTO alarmlog (lastuser,tid,doornr,alarmtype) VALUES ({x.LastUser.CardID}, '{x.TimeStamp.ToString()}',{x.DoorNumber},'{x.AlarmType}');";
            Task.Run(() => injection(insertstring));                        //starts the SQL injection async
        }
        public static async void InsertIntoAccesslog(AccessEntryTry x)
        {                                                                   //Inserts string into DB alarmlog asynchronously
            string b = string.Empty;
            if (x.AccessGranted) { b = "TRUE"; }
            else { b = "FALSE"; }
            string insertstring = $"INSERT INTO accesslog (cardid,tid,doornr,accessgranted) VALUES ({x._User.CardID}, '{x.TimeStamp.ToString()}',{x.DoorNr},{b});";
            Task.Run(() => injection(insertstring));                        //starts the SQL injection async
        }
        private async static void injection(string x)
        {
            /* 
             * this function takes a string and tries to insert it into the database based
             * in the string
             */
            using var conn = new NpgsqlConnection(DBINFO.connectionString); //makes a postgre 
            await conn.OpenAsync();                                         //opens connection to database
            using var cmd = new NpgsqlCommand(x, conn);                     /*makes postgre command object based on established
                                                                              connection and string thats inserted into method*/
            try
            {
                await cmd.ExecuteNonQueryAsync();                           //tries to insert into database based on cmd.
            }
            catch (Exception)
            {
            }
            conn.Close();                                                   //closes the connection to DB
        }
    }

    public class Messages
    {
        public static string AddPackageIdentifier(string identifier, string aString) => identifier + aString; //adds a string to another string
        public static string GetPackageIdentifier(ref string? stringIn,out string stringOut)
        {    /* 
             * this function takes a string as inpuit slices of the first 6 letters
             * these are saved and returned as the packageidentifer
             */
            if (stringIn != string.Empty)                                       //if the input string is not empty
            {
                string temp = stringIn.Substring(0, PackageIdentifier.Length);  //makes a new string based on the first 6 letters in the in string, this is the packageidentifer
                stringOut = stringIn.Remove(0, PackageIdentifier.Length);       //removed the first 6 letters in the string in
                return temp;                                                    //returns the packageidentifer
            }
            else
            {
                stringOut = string.Empty;
                return string.Empty;
            }


        }
        public static string ReceiveString(Socket comSocket, out bool error)
        {
            /* 
             * this function takes return a string from available data on comSocket
             */
            string answer = "";
            error = false;
            try
            {
                byte[] data = new byte[1024];                                  //makes a 1024 byte buffer to hold data from comsocket
                int antallBytesMottatt = comSocket.Receive(data);              //gets how many bytes is received
                if (antallBytesMottatt > 0) answer = Encoding.ASCII.GetString(data, 0, antallBytesMottatt); //gets the string received
                else error = true;
            }
            catch (Exception e)
            {
                error = true;
            }
            return answer;                                                     //returns the string received
        }
        public static bool SendString(Socket comSocket, string data, out bool error)
        {   /* 
             * this function sends a string
             */
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
    /******************************************************Static Data holders*************************************************************/
    public static class DBINFO
    {
        public static string connectionString = "Host=20.56.240.122;Username=h577783;Password=g7Np2wVa;Database=h577783";
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

    /*********************************************************Constructors*****************************************************************/
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


    public class AccessEntryTry
    {
        public int DoorNr { get; set; }
        public User _User { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool AccessGranted { get; set; }
        public AccessEntryTry(User? user, DateTime timeStamp, bool accessGranted, int doorNr)
        {
            _User = user;
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
    /*********************************************************EventArgs*****************************************************************/
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