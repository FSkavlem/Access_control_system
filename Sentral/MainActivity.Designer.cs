namespace Sentral
{
    partial class MainActivity
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.user_admin = new System.Windows.Forms.Button();
            this.open_folder = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.generate_accesslogs = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Access_Panel = new System.Windows.Forms.Panel();
            this.Alarm_window = new System.Windows.Forms.Panel();
            this.listview_alarm_log = new System.Windows.Forms.ListView();
            this.Lastuser = new System.Windows.Forms.ColumnHeader();
            this.time = new System.Windows.Forms.ColumnHeader();
            this.doornr = new System.Windows.Forms.ColumnHeader();
            this.AlarmType = new System.Windows.Forms.ColumnHeader();
            this.Access_window = new System.Windows.Forms.Panel();
            this.listview_access_log = new System.Windows.Forms.ListView();
            this.Card_number = new System.Windows.Forms.ColumnHeader();
            this.fornavn = new System.Windows.Forms.ColumnHeader();
            this.etternavn = new System.Windows.Forms.ColumnHeader();
            this.access_time = new System.Windows.Forms.ColumnHeader();
            this.Door = new System.Windows.Forms.ColumnHeader();
            this.access_granted = new System.Windows.Forms.ColumnHeader();
            this.ButtonPanel.SuspendLayout();
            this.Alarm_window.SuspendLayout();
            this.Access_window.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.user_admin);
            this.ButtonPanel.Controls.Add(this.open_folder);
            this.ButtonPanel.Controls.Add(this.button5);
            this.ButtonPanel.Controls.Add(this.panel3);
            this.ButtonPanel.Controls.Add(this.generate_accesslogs);
            this.ButtonPanel.Controls.Add(this.panel1);
            this.ButtonPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(200, 686);
            this.ButtonPanel.TabIndex = 0;
            // 
            // user_admin
            // 
            this.user_admin.Dock = System.Windows.Forms.DockStyle.Top;
            this.user_admin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.user_admin.Location = new System.Drawing.Point(0, 245);
            this.user_admin.Name = "user_admin";
            this.user_admin.Size = new System.Drawing.Size(200, 44);
            this.user_admin.TabIndex = 10;
            this.user_admin.Text = "User admin";
            this.user_admin.UseVisualStyleBackColor = true;
            this.user_admin.Click += new System.EventHandler(this.user_admin_Click);
            // 
            // open_folder
            // 
            this.open_folder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.open_folder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_folder.Location = new System.Drawing.Point(0, 598);
            this.open_folder.Name = "open_folder";
            this.open_folder.Size = new System.Drawing.Size(200, 44);
            this.open_folder.TabIndex = 9;
            this.open_folder.Text = "Open Report Folder";
            this.open_folder.UseVisualStyleBackColor = true;
            this.open_folder.Click += new System.EventHandler(this.open_folder_Click);
            // 
            // button5
            // 
            this.button5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(0, 642);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(200, 44);
            this.button5.TabIndex = 8;
            this.button5.Text = "Exit";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 245);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 441);
            this.panel3.TabIndex = 5;
            // 
            // generate_accesslogs
            // 
            this.generate_accesslogs.Dock = System.Windows.Forms.DockStyle.Top;
            this.generate_accesslogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.generate_accesslogs.Location = new System.Drawing.Point(0, 201);
            this.generate_accesslogs.Name = "generate_accesslogs";
            this.generate_accesslogs.Size = new System.Drawing.Size(200, 44);
            this.generate_accesslogs.TabIndex = 1;
            this.generate_accesslogs.Text = "Reports";
            this.generate_accesslogs.UseVisualStyleBackColor = true;
            this.generate_accesslogs.Click += new System.EventHandler(this.generate_accesslogs_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 201);
            this.panel1.TabIndex = 0;
            // 
            // Access_Panel
            // 
            this.Access_Panel.Dock = System.Windows.Forms.DockStyle.Right;
            this.Access_Panel.Location = new System.Drawing.Point(1049, 0);
            this.Access_Panel.Name = "Access_Panel";
            this.Access_Panel.Size = new System.Drawing.Size(122, 686);
            this.Access_Panel.TabIndex = 1;
            // 
            // Alarm_window
            // 
            this.Alarm_window.Controls.Add(this.listview_alarm_log);
            this.Alarm_window.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Alarm_window.Location = new System.Drawing.Point(200, 458);
            this.Alarm_window.Name = "Alarm_window";
            this.Alarm_window.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.Alarm_window.Size = new System.Drawing.Size(849, 228);
            this.Alarm_window.TabIndex = 2;
            // 
            // listview_alarm_log
            // 
            this.listview_alarm_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Lastuser,
            this.time,
            this.doornr,
            this.AlarmType});
            this.listview_alarm_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listview_alarm_log.FullRowSelect = true;
            this.listview_alarm_log.GridLines = true;
            this.listview_alarm_log.Location = new System.Drawing.Point(10, 0);
            this.listview_alarm_log.Name = "listview_alarm_log";
            this.listview_alarm_log.Size = new System.Drawing.Size(829, 218);
            this.listview_alarm_log.TabIndex = 0;
            this.listview_alarm_log.UseCompatibleStateImageBehavior = false;
            this.listview_alarm_log.View = System.Windows.Forms.View.Details;
            // 
            // Lastuser
            // 
            this.Lastuser.Text = "ID";
            this.Lastuser.Width = 200;
            // 
            // time
            // 
            this.time.Text = "Time";
            this.time.Width = 300;
            // 
            // doornr
            // 
            this.doornr.Text = "Door nr";
            this.doornr.Width = 80;
            // 
            // AlarmType
            // 
            this.AlarmType.Text = "Alarm type";
            this.AlarmType.Width = 140;
            // 
            // Access_window
            // 
            this.Access_window.Controls.Add(this.listview_access_log);
            this.Access_window.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Access_window.Location = new System.Drawing.Point(200, 0);
            this.Access_window.Name = "Access_window";
            this.Access_window.Padding = new System.Windows.Forms.Padding(10);
            this.Access_window.Size = new System.Drawing.Size(849, 458);
            this.Access_window.TabIndex = 3;
            // 
            // listview_access_log
            // 
            this.listview_access_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Card_number,
            this.fornavn,
            this.etternavn,
            this.access_time,
            this.Door,
            this.access_granted});
            this.listview_access_log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listview_access_log.FullRowSelect = true;
            this.listview_access_log.GridLines = true;
            this.listview_access_log.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listview_access_log.Location = new System.Drawing.Point(10, 10);
            this.listview_access_log.Name = "listview_access_log";
            this.listview_access_log.Size = new System.Drawing.Size(829, 438);
            this.listview_access_log.TabIndex = 0;
            this.listview_access_log.UseCompatibleStateImageBehavior = false;
            this.listview_access_log.View = System.Windows.Forms.View.Details;
            // 
            // Card_number
            // 
            this.Card_number.Text = "Card number";
            this.Card_number.Width = 100;
            // 
            // fornavn
            // 
            this.fornavn.Text = "Fornavn";
            this.fornavn.Width = 120;
            // 
            // etternavn
            // 
            this.etternavn.Text = "Etternavn";
            this.etternavn.Width = 120;
            // 
            // access_time
            // 
            this.access_time.Text = "Access Time";
            this.access_time.Width = 150;
            // 
            // Door
            // 
            this.Door.Text = "Door";
            // 
            // access_granted
            // 
            this.access_granted.Text = "Access Granted";
            this.access_granted.Width = 100;
            // 
            // MainActivity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 686);
            this.Controls.Add(this.Access_window);
            this.Controls.Add(this.Alarm_window);
            this.Controls.Add(this.Access_Panel);
            this.Controls.Add(this.ButtonPanel);
            this.Name = "MainActivity";
            this.Text = "Form1";
            this.ButtonPanel.ResumeLayout(false);
            this.Alarm_window.ResumeLayout(false);
            this.Access_window.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel ButtonPanel;
        private Button open_folder;
        private Button button5;
        private Panel panel3;
        private Button generate_accesslogs;
        private Panel panel1;
        private Panel Access_Panel;
        private Panel Alarm_window;
        private Panel Access_window;
        private ListView listview_access_log;
        private ColumnHeader Card_number;
        private ColumnHeader access_time;
        private ColumnHeader access_granted;
        private ListView listview_alarm_log;
        private ColumnHeader Lastuser;
        private ColumnHeader AlarmType;
        private ColumnHeader doornr;
        private ColumnHeader time;
        private ColumnHeader Door;
        private ColumnHeader fornavn;
        private ColumnHeader etternavn;
        private Button user_admin;
    }
}