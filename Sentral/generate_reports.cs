using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;

namespace Sentral
{
    public partial class generate_reports : Form
    {
        public generate_reports()
        {
            InitializeComponent();
        }

        /*********************************************************functions*****************************************************************/
        /*
         * All makeXreport functions are identical built, only difference is what they query the DB and what messages they send after completion.
         */
        public async Task MakeAccessLogReport(DateTime a, DateTime b)
        {
            /*
             * this function makes a access log report based on selected start time and selected endtime in UI
             */
            string c = $"SELECT cardid, tid, doornr, accessgranted FROM accesslog WHERE (tid < '{b}') and (tid > '{a}');";              //this is a SQL query string
            Task<List<object>> task = SQL_Query.Query(c);                                                                               //sendsQuery to DB 
            task.Wait();
            SQL_Query.SQLQuerylist2TXT(task.Result, "AccessLog", "cardid, tid, doornr, accessgranted");                                 //this method converts list of object to TXT reports
            System.Windows.Forms.MessageBox.Show("AccessLog report Created!", "Complete",MessageBoxButtons.OK, MessageBoxIcon.Warning);    //reports that report have been made by messagebox
        }
        public async Task MakeNoAccessLogReport(string a)
        {
            /*
             * this function makes a no access report based on all no access atempts on a door(string a)
             */ 
            string c = $"SELECT cardid, tid, doornr, accessgranted FROM accesslog WHERE (accessgranted = false) and (doornr = {a});";
            Task<List<object>> task = SQL_Query.Query(c);
            task.Wait();
            SQL_Query.SQLQuerylist2TXT(task.Result, "NoAccessLog", "cardid, tid, doornr, accessgranted");                               
            System.Windows.Forms.MessageBox.Show("NoAccessLog report Created!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning); //reports that report have been made by messag
        }
        public async Task MakeAlarmReport(DateTime a, DateTime b)
        {
            /*
             * this function makes a alarm report from selected start time and selected endtime in UI
             */
            string c = $"SELECT lastuser,tid, doornr,alarmtype FROM alarmlog WHERE (tid < '{b}') and (tid > '{a}');";
            Task<List<object>> task = SQL_Query.Query(c);
            task.Wait();
            SQL_Query.SQLQuerylist2TXT(task.Result, "Alarm Report", "lastuser ,tid ,doornr ,alarmtype");
            System.Windows.Forms.MessageBox.Show("Alarm Report Created!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public async Task MakeUserDataReport()
        {
            /*
             * this function makes a user data report containing all information on the users 
             */
            string c = $"SELECT etternavn,fornavn, enddato, pin, personid FROM usertable ORDER BY etternavn ASC";
            Task<List<object>> task = SQL_Query.Query(c);
            task.Wait();
            SQL_Query.SQLQuerylist2TXT(task.Result, "User Report", "etternavn,fornavn,enddato,pin,kortid");
            System.Windows.Forms.MessageBox.Show("User Report Created!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        public async Task MakeFirstLastReport(string a)
        {
            /*
             * this function makes a report on when the first and last entry point in a room have been based on doornr(string a)
             */
            string c = $"SELECT MIN(accesslog.tid),MAX(accesslog.tid) FROM accesslog WHERE doornr = {a}";
            Task<List<object>> task = SQL_Query.Query(c);
            task.Wait();
            SQL_Query.SQLQuerylist2TXT(task.Result, $"First Last Report for Door {a}", "first_time,last_time");
            System.Windows.Forms.MessageBox.Show("First Last Report Created!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private bool MessageBox(string message, string title)
        {   //generates a messagebox with message as text and title and returns true/false based on user input
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
        /*********************************************************buttons*****************************************************************/
        private void listnoaccess_Click(object sender, EventArgs e)
        {
            var a = GetFromTxtbox();                   //Gets doornr from textbox  
            if (MessageBox($"Are you sure you want to generate no access logs for room {a}?", "Generation"))
            {
                Task.Run(() => MakeNoAccessLogReport(a)); //Starts async operation to generate report
            }
        }
        private void listaccess_Click(object sender, EventArgs e)
        {
            var a = dateTimePicker1.Value;
            var b = dateTimePicker2.Value;
            if (MessageBox($"Are you sure you want to generate access logs from date {a} to {b} ?", "Generation"))
            {
                Task.Run(() => MakeAccessLogReport(a, b));//Starts async operation to generate report
            }
        }
        private void alarmreport_Click(object sender, EventArgs e)
        {
            var a = dateTimePicker1.Value;
            var b = dateTimePicker2.Value;
            if (MessageBox($"Are you sure you want to generate Alarm report from date {a} to {b} ?", "Generation"))
            {
                Task.Run(() => MakeAlarmReport(a,b));//Starts async operation to generate report
            }
        }
        private void Userdata_report_Click(object sender, EventArgs e)
        {
            if (MessageBox($"Are you sure you want to generate Userdata report?", "Generation"))
            {
                Task.Run(() => MakeUserDataReport());//Starts async operation to generate report
            }
        }
        private void firstlast_Click(object sender, EventArgs e)
        {
            var a = GetFromTxtbox();                   //Gets doornr from textbox  
            if (MessageBox($"Are you sure you want to generate First last report for room {a}?", "Generation"))
            {
                Task.Run(() => MakeFirstLastReport(a));//Starts async operation to generate report
            }
        }
        private void openfolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Application.StartupPath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
        private void exit_Click(object sender, EventArgs e)
        {
            this.Hide(); 
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {   //ensures nothing else than digits is entered in text box
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.')) e.Handled = true;
        }
        private string GetFromTxtbox()
        {
            string a = textBox1.Text;
            if (a == "") return "0";
            else return a;
           
        }
    }
}
