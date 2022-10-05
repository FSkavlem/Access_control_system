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
            this.keypad_4 = new System.Windows.Forms.Button();
            this.keypad_5 = new System.Windows.Forms.Button();
            this.keypad_1 = new System.Windows.Forms.Button();
            this.keypad_2 = new System.Windows.Forms.Button();
            this.keypad_7 = new System.Windows.Forms.Button();
            this.keypad_8 = new System.Windows.Forms.Button();
            this.keypad_3 = new System.Windows.Forms.Button();
            this.keypad_6 = new System.Windows.Forms.Button();
            this.keypad_9 = new System.Windows.Forms.Button();
            this.keypad_0 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_pinentered = new System.Windows.Forms.TextBox();
            this.button_cardreader = new System.Windows.Forms.Button();
            this.checkBox_cardonreader = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
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
            this.listview_access_log.Location = new System.Drawing.Point(0, 302);
            this.listview_access_log.Name = "listview_access_log";
            this.listview_access_log.Size = new System.Drawing.Size(564, 47);
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
            this.textBox1.Location = new System.Drawing.Point(53, 108);
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
            this.label3.Location = new System.Drawing.Point(4, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "CardID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(12, 278);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(139, 21);
            this.label4.TabIndex = 6;
            this.label4.Text = "Last Access Entry";
            // 
            // keypad_4
            // 
            this.keypad_4.Location = new System.Drawing.Point(286, 132);
            this.keypad_4.Name = "keypad_4";
            this.keypad_4.Size = new System.Drawing.Size(42, 37);
            this.keypad_4.TabIndex = 7;
            this.keypad_4.Text = "4";
            this.keypad_4.UseVisualStyleBackColor = true;
            this.keypad_4.Click += new System.EventHandler(this.keypad_4_Click);
            // 
            // keypad_5
            // 
            this.keypad_5.Location = new System.Drawing.Point(334, 132);
            this.keypad_5.Name = "keypad_5";
            this.keypad_5.Size = new System.Drawing.Size(42, 37);
            this.keypad_5.TabIndex = 8;
            this.keypad_5.Text = "5";
            this.keypad_5.UseVisualStyleBackColor = true;
            this.keypad_5.Click += new System.EventHandler(this.keypad_5_Click);
            // 
            // keypad_1
            // 
            this.keypad_1.Location = new System.Drawing.Point(286, 89);
            this.keypad_1.Name = "keypad_1";
            this.keypad_1.Size = new System.Drawing.Size(42, 37);
            this.keypad_1.TabIndex = 9;
            this.keypad_1.Text = "1";
            this.keypad_1.UseVisualStyleBackColor = true;
            this.keypad_1.Click += new System.EventHandler(this.keypad_1_Click);
            // 
            // keypad_2
            // 
            this.keypad_2.Location = new System.Drawing.Point(334, 89);
            this.keypad_2.Name = "keypad_2";
            this.keypad_2.Size = new System.Drawing.Size(42, 37);
            this.keypad_2.TabIndex = 10;
            this.keypad_2.Text = "2";
            this.keypad_2.UseVisualStyleBackColor = true;
            this.keypad_2.Click += new System.EventHandler(this.keypad_2_Click);
            // 
            // keypad_7
            // 
            this.keypad_7.Location = new System.Drawing.Point(286, 175);
            this.keypad_7.Name = "keypad_7";
            this.keypad_7.Size = new System.Drawing.Size(42, 37);
            this.keypad_7.TabIndex = 11;
            this.keypad_7.Text = "7";
            this.keypad_7.UseVisualStyleBackColor = true;
            this.keypad_7.Click += new System.EventHandler(this.keypad_7_Click);
            // 
            // keypad_8
            // 
            this.keypad_8.Location = new System.Drawing.Point(334, 175);
            this.keypad_8.Name = "keypad_8";
            this.keypad_8.Size = new System.Drawing.Size(42, 37);
            this.keypad_8.TabIndex = 12;
            this.keypad_8.Text = "8";
            this.keypad_8.UseVisualStyleBackColor = true;
            this.keypad_8.Click += new System.EventHandler(this.keypad_8_Click);
            // 
            // keypad_3
            // 
            this.keypad_3.Location = new System.Drawing.Point(382, 90);
            this.keypad_3.Name = "keypad_3";
            this.keypad_3.Size = new System.Drawing.Size(42, 37);
            this.keypad_3.TabIndex = 13;
            this.keypad_3.Text = "3";
            this.keypad_3.UseVisualStyleBackColor = true;
            this.keypad_3.Click += new System.EventHandler(this.keypad_3_Click);
            // 
            // keypad_6
            // 
            this.keypad_6.Location = new System.Drawing.Point(382, 132);
            this.keypad_6.Name = "keypad_6";
            this.keypad_6.Size = new System.Drawing.Size(42, 37);
            this.keypad_6.TabIndex = 14;
            this.keypad_6.Text = "6";
            this.keypad_6.UseVisualStyleBackColor = true;
            this.keypad_6.Click += new System.EventHandler(this.keypad_6_Click);
            // 
            // keypad_9
            // 
            this.keypad_9.Location = new System.Drawing.Point(382, 175);
            this.keypad_9.Name = "keypad_9";
            this.keypad_9.Size = new System.Drawing.Size(42, 37);
            this.keypad_9.TabIndex = 15;
            this.keypad_9.Text = "9";
            this.keypad_9.UseVisualStyleBackColor = true;
            this.keypad_9.Click += new System.EventHandler(this.keypad_9_Click);
            // 
            // keypad_0
            // 
            this.keypad_0.Location = new System.Drawing.Point(334, 218);
            this.keypad_0.Name = "keypad_0";
            this.keypad_0.Size = new System.Drawing.Size(42, 37);
            this.keypad_0.TabIndex = 16;
            this.keypad_0.Text = "0";
            this.keypad_0.UseVisualStyleBackColor = true;
            this.keypad_0.Click += new System.EventHandler(this.keypad_0_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(245, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "PinEntered";
            // 
            // textBox_pinentered
            // 
            this.textBox_pinentered.Enabled = false;
            this.textBox_pinentered.Location = new System.Drawing.Point(315, 49);
            this.textBox_pinentered.MaxLength = 4;
            this.textBox_pinentered.Name = "textBox_pinentered";
            this.textBox_pinentered.Size = new System.Drawing.Size(100, 23);
            this.textBox_pinentered.TabIndex = 18;
            this.textBox_pinentered.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_cardreader
            // 
            this.button_cardreader.Location = new System.Drawing.Point(19, 175);
            this.button_cardreader.Name = "button_cardreader";
            this.button_cardreader.Size = new System.Drawing.Size(123, 43);
            this.button_cardreader.TabIndex = 19;
            this.button_cardreader.Text = "Swipe Card";
            this.button_cardreader.UseVisualStyleBackColor = true;
            this.button_cardreader.Click += new System.EventHandler(this.button_cardreader_Click);
            // 
            // checkBox_cardonreader
            // 
            this.checkBox_cardonreader.AutoSize = true;
            this.checkBox_cardonreader.Enabled = false;
            this.checkBox_cardonreader.Location = new System.Drawing.Point(147, 144);
            this.checkBox_cardonreader.Name = "checkBox_cardonreader";
            this.checkBox_cardonreader.Size = new System.Drawing.Size(15, 14);
            this.checkBox_cardonreader.TabIndex = 20;
            this.checkBox_cardonreader.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 15);
            this.label6.TabIndex = 21;
            this.label6.Text = "Card Activated on reader";
            // 
            // CardReaderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(564, 349);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.checkBox_cardonreader);
            this.Controls.Add(this.button_cardreader);
            this.Controls.Add(this.textBox_pinentered);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.keypad_0);
            this.Controls.Add(this.keypad_9);
            this.Controls.Add(this.keypad_6);
            this.Controls.Add(this.keypad_3);
            this.Controls.Add(this.keypad_8);
            this.Controls.Add(this.keypad_7);
            this.Controls.Add(this.keypad_2);
            this.Controls.Add(this.keypad_1);
            this.Controls.Add(this.keypad_5);
            this.Controls.Add(this.keypad_4);
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
        private Button keypad_4;
        private Button keypad_5;
        private Button keypad_1;
        private Button keypad_2;
        private Button keypad_7;
        private Button keypad_8;
        private Button keypad_3;
        private Button keypad_6;
        private Button keypad_9;
        private Button keypad_0;
        private Label label5;
        private TextBox textBox_pinentered;
        private Button button_cardreader;
        private CheckBox checkBox_cardonreader;
        private Label label6;
    }
}