namespace Sentral
{
    partial class UserManagement
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView1 = new System.Windows.Forms.ListView();
            this.kortid = new System.Windows.Forms.ColumnHeader();
            this.etternavn = new System.Windows.Forms.ColumnHeader();
            this.fornavn = new System.Windows.Forms.ColumnHeader();
            this.enddato = new System.Windows.Forms.ColumnHeader();
            this.pin = new System.Windows.Forms.ColumnHeader();
            this.txtbox_kortid = new System.Windows.Forms.TextBox();
            this.lbl1 = new System.Windows.Forms.Label();
            this.lbl2 = new System.Windows.Forms.Label();
            this.txtbox_fornavn = new System.Windows.Forms.TextBox();
            this.lbl3 = new System.Windows.Forms.Label();
            this.txtbox_etternavn = new System.Windows.Forms.TextBox();
            this.lbl4 = new System.Windows.Forms.Label();
            this.txtbox_pin = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtbox_selectedenddate = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.bt_update_user = new System.Windows.Forms.Button();
            this.bt_add_user = new System.Windows.Forms.Button();
            this.bt_delete_user = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.bt_clear_user = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.kortid,
            this.etternavn,
            this.fornavn,
            this.enddato,
            this.pin});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(507, 540);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // kortid
            // 
            this.kortid.Text = "Kort ID";
            // 
            // etternavn
            // 
            this.etternavn.Text = "Etternavn";
            this.etternavn.Width = 120;
            // 
            // fornavn
            // 
            this.fornavn.Text = "Fornavn";
            this.fornavn.Width = 120;
            // 
            // enddato
            // 
            this.enddato.Text = "End date";
            this.enddato.Width = 120;
            // 
            // pin
            // 
            this.pin.Text = "Pin";
            this.pin.Width = 90;
            // 
            // txtbox_kortid
            // 
            this.txtbox_kortid.Enabled = false;
            this.txtbox_kortid.Location = new System.Drawing.Point(587, 62);
            this.txtbox_kortid.Name = "txtbox_kortid";
            this.txtbox_kortid.Size = new System.Drawing.Size(207, 23);
            this.txtbox_kortid.TabIndex = 1;
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Location = new System.Drawing.Point(524, 65);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(39, 15);
            this.lbl1.TabIndex = 2;
            this.lbl1.Text = "kortID";
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Location = new System.Drawing.Point(524, 94);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(50, 15);
            this.lbl2.TabIndex = 4;
            this.lbl2.Text = "Fornavn";
            // 
            // txtbox_fornavn
            // 
            this.txtbox_fornavn.Location = new System.Drawing.Point(587, 91);
            this.txtbox_fornavn.Name = "txtbox_fornavn";
            this.txtbox_fornavn.Size = new System.Drawing.Size(207, 23);
            this.txtbox_fornavn.TabIndex = 3;
            // 
            // lbl3
            // 
            this.lbl3.AutoSize = true;
            this.lbl3.Location = new System.Drawing.Point(524, 123);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(57, 15);
            this.lbl3.TabIndex = 6;
            this.lbl3.Text = "Etternavn";
            // 
            // txtbox_etternavn
            // 
            this.txtbox_etternavn.Location = new System.Drawing.Point(587, 120);
            this.txtbox_etternavn.Name = "txtbox_etternavn";
            this.txtbox_etternavn.Size = new System.Drawing.Size(207, 23);
            this.txtbox_etternavn.TabIndex = 5;
            // 
            // lbl4
            // 
            this.lbl4.AutoSize = true;
            this.lbl4.Location = new System.Drawing.Point(524, 152);
            this.lbl4.Name = "lbl4";
            this.lbl4.Size = new System.Drawing.Size(24, 15);
            this.lbl4.TabIndex = 8;
            this.lbl4.Text = "Pin";
            // 
            // txtbox_pin
            // 
            this.txtbox_pin.Location = new System.Drawing.Point(587, 149);
            this.txtbox_pin.Name = "txtbox_pin";
            this.txtbox_pin.Size = new System.Drawing.Size(207, 23);
            this.txtbox_pin.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(524, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "End date";
            // 
            // txtbox_selectedenddate
            // 
            this.txtbox_selectedenddate.Location = new System.Drawing.Point(587, 198);
            this.txtbox_selectedenddate.Name = "txtbox_selectedenddate";
            this.txtbox_selectedenddate.Size = new System.Drawing.Size(207, 23);
            this.txtbox_selectedenddate.TabIndex = 9;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(587, 227);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(207, 23);
            this.dateTimePicker1.TabIndex = 11;
            this.dateTimePicker1.CloseUp += new System.EventHandler(this.dateTimePicker1_CloseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(524, 233);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Date";
            // 
            // bt_update_user
            // 
            this.bt_update_user.Location = new System.Drawing.Point(525, 337);
            this.bt_update_user.Name = "bt_update_user";
            this.bt_update_user.Size = new System.Drawing.Size(212, 42);
            this.bt_update_user.TabIndex = 12;
            this.bt_update_user.Text = "Update user";
            this.bt_update_user.UseVisualStyleBackColor = true;
            this.bt_update_user.Click += new System.EventHandler(this.bt_update_user_Click);
            // 
            // bt_add_user
            // 
            this.bt_add_user.Location = new System.Drawing.Point(524, 385);
            this.bt_add_user.Name = "bt_add_user";
            this.bt_add_user.Size = new System.Drawing.Size(212, 42);
            this.bt_add_user.TabIndex = 12;
            this.bt_add_user.Text = "Add user";
            this.bt_add_user.UseVisualStyleBackColor = true;
            this.bt_add_user.Click += new System.EventHandler(this.bt_add_user_Click);
            // 
            // bt_delete_user
            // 
            this.bt_delete_user.Location = new System.Drawing.Point(524, 433);
            this.bt_delete_user.Name = "bt_delete_user";
            this.bt_delete_user.Size = new System.Drawing.Size(212, 42);
            this.bt_delete_user.TabIndex = 12;
            this.bt_delete_user.Text = "Delete user";
            this.bt_delete_user.UseVisualStyleBackColor = true;
            this.bt_delete_user.Click += new System.EventHandler(this.bt_delete_user_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(525, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 13;
            this.label2.Text = "User Administration";
            // 
            // bt_clear_user
            // 
            this.bt_clear_user.Location = new System.Drawing.Point(525, 289);
            this.bt_clear_user.Name = "bt_clear_user";
            this.bt_clear_user.Size = new System.Drawing.Size(212, 42);
            this.bt_clear_user.TabIndex = 12;
            this.bt_clear_user.Text = "Clear";
            this.bt_clear_user.UseVisualStyleBackColor = true;
            this.bt_clear_user.Click += new System.EventHandler(this.bt_clear_user_Click);
            // 
            // UserManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 564);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.bt_clear_user);
            this.Controls.Add(this.bt_delete_user);
            this.Controls.Add(this.bt_add_user);
            this.Controls.Add(this.bt_update_user);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtbox_selectedenddate);
            this.Controls.Add(this.lbl4);
            this.Controls.Add(this.txtbox_pin);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.txtbox_etternavn);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.txtbox_fornavn);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.txtbox_kortid);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "UserManagement";
            this.Text = "UserManagement";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListView listView1;
        private TextBox txtbox_kortid;
        private Label lbl1;
        private Label lbl2;
        private TextBox txtbox_fornavn;
        private Label lbl3;
        private TextBox txtbox_etternavn;
        private Label lbl4;
        private TextBox txtbox_pin;
        private Label label5;
        private TextBox txtbox_selectedenddate;
        private ColumnHeader kortid;
        private ColumnHeader etternavn;
        private ColumnHeader fornavn;
        private ColumnHeader enddato;
        private ColumnHeader pin;
        private DateTimePicker dateTimePicker1;
        private Label label1;
        private Button bt_update_user;
        private Button bt_add_user;
        private Button bt_delete_user;
        private Label label2;
        private Button bt_clear_user;
    }
}