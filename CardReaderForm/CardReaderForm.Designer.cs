namespace CardReaderForm
{
    partial class CardReaderForm
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
            this.listview_access_log = new System.Windows.Forms.ListView();
            this.Card_number = new System.Windows.Forms.ColumnHeader();
            this.pin_entered = new System.Windows.Forms.ColumnHeader();
            this.access_time = new System.Windows.Forms.ColumnHeader();
            this.access_granted = new System.Windows.Forms.ColumnHeader();
            this.checkBox_dooropen = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox_doorLock = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.reset_alarm_button = new System.Windows.Forms.Button();
            this.checkBox_Alarm = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listview_access_log
            // 
            this.listview_access_log.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Card_number,
            this.pin_entered,
            this.access_time,
            this.access_granted});
            this.listview_access_log.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listview_access_log.FullRowSelect = true;
            this.listview_access_log.GridLines = true;
            this.listview_access_log.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listview_access_log.Location = new System.Drawing.Point(0, 112);
            this.listview_access_log.Name = "listview_access_log";
            this.listview_access_log.Size = new System.Drawing.Size(565, 47);
            this.listview_access_log.TabIndex = 1;
            this.listview_access_log.UseCompatibleStateImageBehavior = false;
            this.listview_access_log.View = System.Windows.Forms.View.Details;
            // 
            // Card_number
            // 
            this.Card_number.Text = "Card number";
            this.Card_number.Width = 120;
            // 
            // pin_entered
            // 
            this.pin_entered.Text = "Pin Entered";
            this.pin_entered.Width = 120;
            // 
            // access_time
            // 
            this.access_time.Text = "Time";
            this.access_time.Width = 200;
            // 
            // access_granted
            // 
            this.access_granted.Text = "Access Granted";
            this.access_granted.Width = 120;
            // 
            // checkBox_dooropen
            // 
            this.checkBox_dooropen.AutoSize = true;
            this.checkBox_dooropen.Enabled = false;
            this.checkBox_dooropen.Location = new System.Drawing.Point(97, 29);
            this.checkBox_dooropen.Name = "checkBox_dooropen";
            this.checkBox_dooropen.Size = new System.Drawing.Size(15, 14);
            this.checkBox_dooropen.TabIndex = 2;
            this.checkBox_dooropen.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Door Open";
            // 
            // checkBox_doorLock
            // 
            this.checkBox_doorLock.AutoSize = true;
            this.checkBox_doorLock.Enabled = false;
            this.checkBox_doorLock.Location = new System.Drawing.Point(97, 49);
            this.checkBox_doorLock.Name = "checkBox_doorLock";
            this.checkBox_doorLock.Size = new System.Drawing.Size(15, 14);
            this.checkBox_doorLock.TabIndex = 2;
            this.checkBox_doorLock.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Door Lock";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(199, 45);
            this.textBox1.MaxLength = 4;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 23);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "1234";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "CardID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(0, 88);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Last Access Entry";
            // 
            // reset_alarm_button
            // 
            this.reset_alarm_button.Enabled = false;
            this.reset_alarm_button.Location = new System.Drawing.Point(424, 63);
            this.reset_alarm_button.Name = "reset_alarm_button";
            this.reset_alarm_button.Size = new System.Drawing.Size(129, 43);
            this.reset_alarm_button.TabIndex = 7;
            this.reset_alarm_button.Text = "Reset Alarm";
            this.reset_alarm_button.UseVisualStyleBackColor = true;
            this.reset_alarm_button.Click += new System.EventHandler(this.reset_alarm_button_Click);
            // 
            // checkBox_Alarm
            // 
            this.checkBox_Alarm.AutoSize = true;
            this.checkBox_Alarm.Enabled = false;
            this.checkBox_Alarm.Location = new System.Drawing.Point(469, 43);
            this.checkBox_Alarm.Name = "checkBox_Alarm";
            this.checkBox_Alarm.Size = new System.Drawing.Size(15, 14);
            this.checkBox_Alarm.TabIndex = 8;
            this.checkBox_Alarm.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(424, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Alarm";
            // 
            // CardReaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 159);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox_Alarm);
            this.Controls.Add(this.reset_alarm_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox_doorLock);
            this.Controls.Add(this.checkBox_dooropen);
            this.Controls.Add(this.listview_access_log);
            this.Name = "CardReaderForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListView listview_access_log;
        private ColumnHeader pin_entered;
        private ColumnHeader Card_number;
        private ColumnHeader access_time;
        private ColumnHeader access_granted;
        private CheckBox checkBox_dooropen;
        private Label label1;
        private CheckBox checkBox_doorLock;
        private Label label2;
        private TextBox textBox1;
        private Label label3;
        private Label label4;
        private Button reset_alarm_button;
        private CheckBox checkBox_Alarm;
        private Label label5;
    }
}