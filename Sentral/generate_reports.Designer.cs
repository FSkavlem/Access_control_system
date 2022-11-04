namespace Sentral
{
    partial class generate_reports
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.listaccess = new System.Windows.Forms.Button();
            this.listnoaccess = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.openfolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.alarmreport = new System.Windows.Forms.Button();
            this.Userdata_report = new System.Windows.Forms.Button();
            this.firstlast = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(145, 34);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 23);
            this.dateTimePicker1.TabIndex = 0;
            this.dateTimePicker1.Value = new System.DateTime(2022, 8, 1, 0, 0, 0, 0);
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(145, 80);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(200, 23);
            this.dateTimePicker2.TabIndex = 1;
            // 
            // listaccess
            // 
            this.listaccess.Location = new System.Drawing.Point(12, 12);
            this.listaccess.Name = "listaccess";
            this.listaccess.Size = new System.Drawing.Size(99, 45);
            this.listaccess.TabIndex = 2;
            this.listaccess.Text = "Access report";
            this.listaccess.UseVisualStyleBackColor = true;
            this.listaccess.Click += new System.EventHandler(this.listaccess_Click);
            // 
            // listnoaccess
            // 
            this.listnoaccess.Location = new System.Drawing.Point(12, 63);
            this.listnoaccess.Name = "listnoaccess";
            this.listnoaccess.Size = new System.Drawing.Size(99, 66);
            this.listnoaccess.TabIndex = 2;
            this.listnoaccess.Text = "No Access Report";
            this.listnoaccess.UseVisualStyleBackColor = true;
            this.listnoaccess.Click += new System.EventHandler(this.listnoaccess_Click);
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(12, 351);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(99, 27);
            this.exit.TabIndex = 2;
            this.exit.Text = "Exit";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.exit_Click);
            // 
            // openfolder
            // 
            this.openfolder.Location = new System.Drawing.Point(12, 318);
            this.openfolder.Name = "openfolder";
            this.openfolder.Size = new System.Drawing.Size(99, 27);
            this.openfolder.TabIndex = 2;
            this.openfolder.Text = "Open folder";
            this.openfolder.UseVisualStyleBackColor = true;
            this.openfolder.Click += new System.EventHandler(this.openfolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(145, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Til";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(145, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Fra ";
            // 
            // alarmreport
            // 
            this.alarmreport.Location = new System.Drawing.Point(12, 135);
            this.alarmreport.Name = "alarmreport";
            this.alarmreport.Size = new System.Drawing.Size(99, 66);
            this.alarmreport.TabIndex = 2;
            this.alarmreport.Text = "Alarm report";
            this.alarmreport.UseVisualStyleBackColor = true;
            this.alarmreport.Click += new System.EventHandler(this.alarmreport_Click);
            // 
            // Userdata_report
            // 
            this.Userdata_report.Location = new System.Drawing.Point(12, 207);
            this.Userdata_report.Name = "Userdata_report";
            this.Userdata_report.Size = new System.Drawing.Size(99, 66);
            this.Userdata_report.TabIndex = 2;
            this.Userdata_report.Text = "Userdata report";
            this.Userdata_report.UseVisualStyleBackColor = true;
            this.Userdata_report.Click += new System.EventHandler(this.Userdata_report_Click);
            // 
            // firstlast
            // 
            this.firstlast.Location = new System.Drawing.Point(117, 207);
            this.firstlast.Name = "firstlast";
            this.firstlast.Size = new System.Drawing.Size(99, 66);
            this.firstlast.TabIndex = 2;
            this.firstlast.Text = "First and last for room";
            this.firstlast.UseVisualStyleBackColor = true;
            this.firstlast.Click += new System.EventHandler(this.firstlast_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(238, 241);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 23);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "1";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(238, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Door number";
            // 
            // generate_reports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 390);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.openfolder);
            this.Controls.Add(this.firstlast);
            this.Controls.Add(this.Userdata_report);
            this.Controls.Add(this.alarmreport);
            this.Controls.Add(this.listnoaccess);
            this.Controls.Add(this.listaccess);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "generate_reports";
            this.Text = "generate_accesslogs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private Button listaccess;
        private Button listnoaccess;
        private Button exit;
        private Button openfolder;
        private Label label1;
        private Label label2;
        private Button alarmreport;
        private Button Userdata_report;
        private Button firstlast;
        private TextBox textBox1;
        private Label label3;
    }
}