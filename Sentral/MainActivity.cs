using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using ClassLibrary;
using System.Text.Json;
using System.Runtime.CompilerServices;
using System.Dynamic;
using Npgsql;
using System;

namespace Sentral
{
    //delegates to get handle for mainform
    public delegate void UpdateView(List<object> data, ListView x);
    public delegate void UpdateFormAccesList(AccessEntryTry x);
    public delegate void UpdateFormAlarmList(AlarmLogEntry x);
    public partial class MainActivity : Form
    {
        public static MainActivity mainform;
        public static generate_accesslogs genlogsform;

        public MainActivity()
        {
            InitializeComponent();
            if (mainform == null)   //singleton
            {   
                mainform = this;
            }
            //TCP listner
            TCPConnectionListner connectionListner = new TCPConnectionListner(this);

            //event subscribers
            TCPConnectionHandler.newAccessEntryTry += TCPConnectionHandler_newAccessEntryTry;
            TCPConnectionHandler.AlarmRaised += TCPConnectionHandler_AlarmRaised;

            //secondary window for form
            genlogsform = new generate_accesslogs();
            genlogsform.Hide();
            //start syncing proccess, first pull data from SQL DB then, pass to mainform
            Task.Run(() => SyncAccessLogView());
            Task.Run(() => SyncAlarmLogView());
        }
        /*********************************************************DelegateHandlers*****************************************************************/
        private void TCPConnectionHandler_AlarmRaised(AlarmLogEntry x) => mainform.Invoke(new UpdateFormAlarmList(mainform.UpdateAlarmLogs), x);
        private void TCPConnectionHandler_newAccessEntryTry(AccessEntryTry x) => mainform.Invoke(new UpdateFormAccesList(mainform.UpdateAccessLogs), x);

        /***********************************************************Functions**********************************************************************/
        private async void SyncAccessLogView()
        {  
            string c = "SELECT accesslog.cardid," + //SQL query string  
                "usertable.fornavn, usertable.etternavn,accesslog.tid,accesslog.doornr,accesslog.accessgranted " +      
                "FROM accesslog " +
                "INNER JOIN usertable ON accesslog.cardid = usertable.personid " +
                "ORDER BY tid DESC;";
            Task<List<object>> task = SQL_Query.Query(c); //metod that returns a list of object based on the SQL query string
            task.Wait();                                  //waits for async sql query to finish
            mainform.Invoke(new UpdateView(mainform.PopulateListView), task.Result, mainform.listview_access_log); //passes the list of object to mainform to populatelist
        }
        private async void SyncAlarmLogView()
        {   //identical built to SyncAccessLogView
            string c = "SELECT CONCAT(usertable.etternavn,' ',usertable.fornavn), alarmlog.tid,alarmlog.doornr,alarmlog.alarmtype" +
                " FROM alarmlog" +
                " INNER JOIN usertable ON alarmlog.lastuser = usertable.personid" +
                " ORDER BY alarmlog.tid DESC;";
            Task<List<object>> task = SQL_Query.Query(c);
            task.Wait();
            mainform.Invoke(new UpdateView(mainform.PopulateListView), task.Result, mainform.listview_alarm_log);
        }
        private void PopulateListView(List<object> a, ListView x)
        {   //populates listview x from list a
            foreach (var item in a)
            {
                List<object> b = item as List<object>;  //in order to work with item as list we need to cast item to new list
                string[] row = new string[b.Count];     //makes an array based on the item length
                for (int i = 0; i < b.Count; i++)       //loops through the item list
                {
                    row[i] = b[i].ToString();           //inserts the data from item into stringarray
                }
                ListViewItem listViewItem = new ListViewItem(row);
                x.Items.Add(listViewItem);              //adds the string array to listview
            }
        }
        private void UpdateAlarmLogs(AlarmLogEntry x)
        {   //UpdateAlarmLogs
            Task.Run(() => SQL_insertion.InsertIntoAlarmLog(x));//inserts the AlarmLogEntry into the Alarmlog DB
            string lastuser = string.Format("{0} {1}", x.LastUser.Fornavn, x.LastUser.Etternavn);
            string[] row = {lastuser,                           //creates a string array based on AlarmLogEntry
                x.TimeStamp.ToString(),
                x.DoorNumber.ToString(),
                x.AlarmType };
            ListViewItem listViewItem = new ListViewItem(row);
            listview_alarm_log.Items.Insert(0,listViewItem);   //the stringarray is inserted in listview at start, so we dont need to sync from SQL DB agian.
        }
        private void UpdateAccessLogs(AccessEntryTry x)
        {
            Task.Run(() => SQL_insertion.InsertIntoAccesslog(x));//inserts the AccessEntryTry into the Accesslog DB

            string a = string.Empty;
            if (x.AccessGranted) a = "True";
            else a = "False";
            string[] row = {x._User.CardID.ToString(),         //creates a string array based on AccessEntryTry
                            x._User.Fornavn,
                            x._User.Etternavn,
                            x.TimeStamp.ToString(),
                            x.DoorNr.ToString(),
                            a};
            ListViewItem listViewItem = new ListViewItem(row);
            listview_access_log.Items.Insert(0,listViewItem); //the stringarray is inserted in listview at start, so we dont need to sync from SQL DB agian.
        }
        private bool MessageBox(string message, string title)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /***********************************************************BUTTONS**********************************************************************/
        private void generate_accesslogs_Click(object sender, EventArgs e)
        {
            genlogsform.ShowDialog(); //shows the form 
        }
        private void open_folder_Click(object sender, EventArgs e)
        {   //starts a new windows explorer at filename path
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() 
            {
                FileName = Application.StartupPath, 
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox("Are you sure you want to exit application?", "Warning"))
            {
                this.Close();   //closes mainform
                this.Dispose(); //released the resources thats held by mainform
            }
        }



    }

}
