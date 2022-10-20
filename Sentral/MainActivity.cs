using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using ClassLibrary;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Dynamic;
using Npgsql;

namespace Sentral
{
    public delegate void UpdateFormAccesList(AccessEntryTry x);
    public delegate void UpdateFormAlarmList(AlarmLogEntry x);
    public partial class MainActivity : Form
    {
        public int AccessIDnumber;
        public int AlarmIDnumber;
        public static MainActivity mainform;
      
        //must add 
        //UpdateAccessLogs(AccessEntryTry x) method must insert into accesstry into SQL DB
        //UpdateAlarmLogs method must insert into alarm SQL DB

        public MainActivity()
        {
            InitializeComponent();
            AccessIDnumber = 0;
            AlarmIDnumber = 0;
            mainform = this;
            TCPConnectionListner connectionListner = new TCPConnectionListner(this);

            //event subscribers
            TCPConnectionHandler.newAccessEntryTry += TCPConnectionHandler_newAccessEntryTry;
            TCPConnectionHandler.AlarmRaised += TCPConnectionHandler_AlarmRaised;
        }

        private void TCPConnectionHandler_AlarmRaised(AlarmLogEntry x) => mainform.Invoke(new UpdateFormAlarmList(mainform.UpdateAlarmLogs), x);

        private void TCPConnectionHandler_newAccessEntryTry(AccessEntryTry x) => mainform.Invoke(new UpdateFormAccesList(mainform.UpdateAccessLogs), x);

        private void UpdateAlarmLogs(AlarmLogEntry x)
        {
            //INSERT INTO SQL DB
            AlarmIDnumber++;
            string lastuser = string.Format("{0},{1}", x.LastUser.Etternavn, x.LastUser.Fornavn);
            string[] row = {AlarmIDnumber.ToString(),
                            x.AlarmType,
                            lastuser,
                            x.TimeStamp.ToString()};
            ListViewItem listViewItem = new ListViewItem(row);
            listview_alarm_log.Items.Add(listViewItem);
        }
        private void UpdateAccessLogs(AccessEntryTry x)
        {   
            //AND INSERT INTO DB
            string a = string.Empty;
            AccessIDnumber++;
            if (x.AccessGranted) a = "Granted";
            else a = "Denied";
            string[] row = {AccessIDnumber.ToString(),
                            x.User.CardID.ToString(),
                            x.User.Etternavn,
                            x.User.Fornavn,
                            x.DoorNr.ToString(),
                            x.TimeStamp.ToString(),
                            a};
            ListViewItem listViewItem = new ListViewItem(row);
            listview_access_log.Items.Add(listViewItem);
        }

        private static void SQLlogAlarm(AlarmLogEntry x)
        {
            //something something log alarm SQL, ID set bt SQL DB

        }
        private static void SQLlogAccessEntry(AccessEntryTry x)
        {
            //something something log access try from data to SQL, ID set bt SQL DB
            var connString = "Host=server;Username=user;Password=mypass;Database=dbName";

            using var con = new NpgsqlConnection(connString);
            con.Open();

            // Insert some data
            using (var cmd = new NpgsqlCommand("INSERT INTO logAccess (user_id, door_nr, access_granted, date_time) " +
                "VALUES (@UserId,@DoorNr,@AccessGranted, @DateTime)", con))
            {
                cmd.Parameters.AddWithValue("UserId", x.User.CardID);
                cmd.Parameters.AddWithValue("DoorNr", x.DoorNr);
                cmd.Parameters.AddWithValue("AccessGranted", x.AccessGranted);
                cmd.Parameters.AddWithValue("DateTime", x.TimeStamp);
                cmd.ExecuteNonQuery();
            }

            con.Close();
        }
    }

}
