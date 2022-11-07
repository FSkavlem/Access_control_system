using ClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Sentral
{
    public delegate void UpdateViewUserform(List<object> data, ListView x);
    public partial class UserManagement : Form
    {
        public UserManagement userManagement;
        public MainActivity mainform;
        public List<int> primarykeylist = new List<int>();
        public UserManagement(MainActivity main)
        {
            InitializeComponent();
            userManagement = this; //singleton
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            /* 
             * in order to run task async and invoke form, we need to ensure that the handle is created.
             * easiest way to do this is to overrid the OnHandleCreated function.
             */
            base.OnHandleCreated(e);
            Task.Run(() => SyncUserlist());
        }

        private async void SyncUserlist()
        {
            /* 
             * this function sync the listview in the usermanagement window to 
             * match the usertable table from SQL database 
             */
            string c = "SELECT usertable.personid, usertable.etternavn, usertable.fornavn,usertable.enddato,usertable.pin " +
                       "FROM usertable " +
                       "ORDER BY usertable.personid ASC";
            Task<List<object>> task = SQL_Query.Query(c);       //metod that returns a list of object based on the SQL query string
            task.Wait();                                        //waits for async sql query to finish
            userManagement.Invoke(new UpdateViewUserform(userManagement.PopulateListView), task.Result, userManagement.listView1); //passes the list of object to mainform to populatelist
        }
        public async Task UpdateUser(string[] a)
        {
            /*
             * this updates a user in the SQL database
             */
            string c = $"UPDATE usertable SET " +              //SQL string
                $"etternavn = '{a[1]}', " +
                $"fornavn = '{a[2]}', " +
                $"enddato = '{a[3]}', " +
                $"pin = {a[4]} " +
                $"WHERE personid = {a[0]} " ;

            Task<List<object>> task = SQL_Query.Query(c);     //sends SQL string to DB
            task.Wait();                                      //waits for task to complete
            Thread.Sleep(500);                                //waits 500ms for new DB connection
            Task.Run(() => SyncUserlist());                   //syncs the listview from SQL DB again
            System.Windows.Forms.MessageBox.Show("User updated!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);//informs user of progress
        }
        public async Task AddUser(string[] a)
        {
            /*
             * this function adds a user to the SQL usertable. From user textboxes in usermanagement
             */
            string c = $"INSERT INTO usertable (personid,fornavn,etternavn,enddato,pin) " +   //SQL string
                $"VALUES ({a[0]},'{a[1]}','{a[2]}','{a[3]}',{a[4]})";
            Task.Run(() => SQL_insertion.injection(c));                                       //sends SQL string to DB
            Thread.Sleep(500);                                                                //waits 500ms for new DB connection
            Task.Run(() => SyncUserlist());                                                   //syncs the listview from SQL DB again
            System.Windows.Forms.MessageBox.Show("User Created!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);//informs user of progress
        }
        public async Task DeleteUser(string[] a)
        {
            /*
             * this function deletes a user from the SQL database usertable
             */
            string c = $"DELETE from usertable " +          //SQL string
                $"WHERE personid = {a[0]}";                 
            Task.Run(() => SQL_insertion.injection(c));     //sends SQL string to DB
            Thread.Sleep(500);                              //waits 500ms for new DB connection
            Task.Run(() => SyncUserlist());                 //syncs the listview from SQL DB again
            System.Windows.Forms.MessageBox.Show("User Deleted!", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);//informs user of progress
        }
        private void PopulateListView(List<object> a, ListView x)
        {
            /* 
             * this function is a generic function that populates listview x from list a
             */
            listView1.Items.Clear();
            primarykeylist.Clear();
            foreach (var item in a)
            {
                List<object> b = item as List<object>;          //in order to work with item as list we need to cast item to new list
                string[] row = new string[b.Count];             //makes an array based on the item length
                for (int i = 0; i < b.Count; i++)               //loops through the item list
                {
                    row[i] = b[i].ToString();                   //inserts the data from item into stringarray
                }
                ListViewItem listViewItem = new ListViewItem(row);
                    
                int temp = 0;                                   //makes a temperary int holder
                int.TryParse(row[0], out temp);                 //takes out the primarykey as integer 
                primarykeylist.Add(temp);                       //adds it to the primarykey list 

                x.Items.Add(listViewItem);                      //adds the string array to listview
            }
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* 
             * this function happends when an item in the listview is selected. Updates the textboxes accordingly
             */
            if (listView1.SelectedItems.Count > 0)
            {
                txtbox_kortid.Enabled = false;
                ListViewItem item = this.listView1.SelectedItems[0];
                txtbox_kortid.Text = item.SubItems[0].Text;
                txtbox_etternavn.Text = item.SubItems[1].Text;
                txtbox_fornavn.Text= item.SubItems[2].Text;
                txtbox_selectedenddate.Text = item.SubItems[3].Text;
                txtbox_pin.Text = item.SubItems[4].Text;
            }
        }
        private string[] GetFromUserForm()
        {
            /* 
             * this function pulls the txtbox information into a string array.
             */
            string[] strings = new string[5];
            strings[0] = txtbox_kortid.Text;
            strings[1] = txtbox_fornavn.Text;
            strings[2] = txtbox_etternavn.Text;
            strings[3] = txtbox_selectedenddate.Text;
            strings[4] = txtbox_pin.Text;
            return strings;
        }
        private bool CheckForEmpty(string[] a)
        {
            /* 
             * this function checks if there is any empty fields in textboxes, returns false if no empty. True of they are
             */
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == "")
                {
                    var b = System.Windows.Forms.MessageBox.Show("Ikke alle felt er fylt ut!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }
            return false;
        }
        private void dateTimePicker1_CloseUp(object sender, EventArgs e)
        {
            /* 
            * this function add the date to date txtbox when date is selected
            */
            txtbox_selectedenddate.Text = dateTimePicker1.Value.ToString(); 
        }

        private void bt_update_user_Click(object sender, EventArgs e)
        {
            /* 
            * this function starts the add user events, checks if the fields are empty and 
            * if adding a new user have started(nothing in kortid field)
            */
            var a = GetFromUserForm();
            if (MessageBox($"Do you want to update user {a[1]}, {a[2]}","Warning"))
            {
 
                if (CheckForEmpty(a)) return;       //check if the form is not empty

                if (txtbox_kortid.Enabled)
                {
                    var b = System.Windows.Forms.MessageBox.Show("Selekter er bruker først!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {

                    Task.Run(() => UpdateUser(a));
                }
                
            }
        }

        private void bt_add_user_Click(object sender, EventArgs e)
        {

            string[] a = GetFromUserForm();
            if (MessageBox($"Do you want to add user {a[1]}, {a[2]}", "Warning"))
            {
                if (CheckForEmpty(a)) return;       //check if the form is not empty

                foreach (var item in primarykeylist) //primarykey check
                {
                    if (a[0]==item.ToString())
                    {
                        var b = System.Windows.Forms.MessageBox.Show("Samme primarykey eksisterer bytt Kortid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                Task.Run(() => AddUser(a));
            }
        }
        

        private void bt_clear_user_Click(object sender, EventArgs e)
        {

            if (MessageBox("Do you want to clear the user form? no action will be taken on database","Warning"))
            {
                txtbox_kortid.Enabled = true;
                ClearUserForm();
            }
        }

        private void bt_delete_user_Click(object sender, EventArgs e)
        {
            var a = GetFromUserForm();
            if (MessageBox($"Do you want to delete user {a[1]}, {a[2]}", "Warning"))
            {
                var c = CheckForEmpty(a);
                if (CheckForEmpty(a))  return;    //check if the form is not empty
                if (txtbox_kortid.Enabled)
                {
                    var b = System.Windows.Forms.MessageBox.Show("Selekter er bruker først!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {

                    Task.Run(() => DeleteUser(a));
                }
            }
        }
        private void ClearUserForm()
        {
            txtbox_kortid.Text = "";
            txtbox_etternavn.Text = "";
            txtbox_fornavn.Text = "";
            txtbox_selectedenddate.Text = "";
            txtbox_pin.Text = "";
        }
        private bool MessageBox(string message, string title)
        {
            /* 
            * this function is created a generic messages messagebox based on 
            * message and title passed to function
            */
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
    }
}
