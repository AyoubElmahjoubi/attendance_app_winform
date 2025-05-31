namespace BioMetrixCore
{
    partial class AttendanceSettingsForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLimits = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpDefaultPauseTime = new System.Windows.Forms.DateTimePicker();
            this.chkUseDefaultPause = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpMaxPauseTime = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpCheckOutLimit = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpCheckInLimit = new System.Windows.Forms.DateTimePicker();
            this.tabTimeRanges = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dtpCheckOutEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpCheckOutStart = new System.Windows.Forms.DateTimePicker();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpPauseEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpPauseStart = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpCheckInEnd = new System.Windows.Forms.DateTimePicker();
            this.dtpCheckInStart = new System.Windows.Forms.DateTimePicker();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblTabDescription = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabLimits.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabTimeRanges.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLimits);
            this.tabControl1.Controls.Add(this.tabTimeRanges);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(460, 350);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabLimits
            // 
            this.tabLimits.Controls.Add(this.groupBox2);
            this.tabLimits.Controls.Add(this.groupBox1);
            this.tabLimits.Location = new System.Drawing.Point(4, 22);
            this.tabLimits.Name = "tabLimits";
            this.tabLimits.Padding = new System.Windows.Forms.Padding(3);
            this.tabLimits.Size = new System.Drawing.Size(452, 324);
            this.tabLimits.TabIndex = 0;
            this.tabLimits.Text = "Alert Limits";
            this.tabLimits.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.dtpDefaultPauseTime);
            this.groupBox2.Controls.Add(this.chkUseDefaultPause);
            this.groupBox2.Location = new System.Drawing.Point(16, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(420, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Default Pause Settings";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(146, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "Default Pause Time Duration:";
            // 
            // dtpDefaultPauseTime
            // 
            this.dtpDefaultPauseTime.CustomFormat = "HH:mm";
            this.dtpDefaultPauseTime.Enabled = false;
            this.dtpDefaultPauseTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDefaultPauseTime.Location = new System.Drawing.Point(171, 73);
            this.dtpDefaultPauseTime.Name = "dtpDefaultPauseTime";
            this.dtpDefaultPauseTime.ShowUpDown = true;
            this.dtpDefaultPauseTime.Size = new System.Drawing.Size(100, 20);
            this.dtpDefaultPauseTime.TabIndex = 2;
            // 
            // chkUseDefaultPause
            // 
            this.chkUseDefaultPause.AutoSize = true;
            this.chkUseDefaultPause.Location = new System.Drawing.Point(22, 30);
            this.chkUseDefaultPause.Name = "chkUseDefaultPause";
            this.chkUseDefaultPause.Size = new System.Drawing.Size(303, 17);
            this.chkUseDefaultPause.TabIndex = 0;
            this.chkUseDefaultPause.Text = "Use default pause time when employee forgets to register it";
            this.chkUseDefaultPause.UseVisualStyleBackColor = true;
            this.chkUseDefaultPause.CheckedChanged += new System.EventHandler(this.chkUseDefaultPause_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.dtpMaxPauseTime);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpCheckOutLimit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.dtpCheckInLimit);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(420, 145);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time Limits for Alerts";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Maximum Pause Time Limit:";
            // 
            // dtpMaxPauseTime
            // 
            this.dtpMaxPauseTime.CustomFormat = "HH:mm";
            this.dtpMaxPauseTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpMaxPauseTime.Location = new System.Drawing.Point(171, 101);
            this.dtpMaxPauseTime.Name = "dtpMaxPauseTime";
            this.dtpMaxPauseTime.ShowUpDown = true;
            this.dtpMaxPauseTime.Size = new System.Drawing.Size(100, 20);
            this.dtpMaxPauseTime.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Check-Out Time Limit (min):";
            // 
            // dtpCheckOutLimit
            // 
            this.dtpCheckOutLimit.CustomFormat = "HH:mm";
            this.dtpCheckOutLimit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckOutLimit.Location = new System.Drawing.Point(171, 63);
            this.dtpCheckOutLimit.Name = "dtpCheckOutLimit";
            this.dtpCheckOutLimit.ShowUpDown = true;
            this.dtpCheckOutLimit.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckOutLimit.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Check-In Time Limit (max):";
            // 
            // dtpCheckInLimit
            // 
            this.dtpCheckInLimit.CustomFormat = "HH:mm";
            this.dtpCheckInLimit.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckInLimit.Location = new System.Drawing.Point(171, 25);
            this.dtpCheckInLimit.Name = "dtpCheckInLimit";
            this.dtpCheckInLimit.ShowUpDown = true;
            this.dtpCheckInLimit.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckInLimit.TabIndex = 0;
            // 
            // tabTimeRanges
            // 
            this.tabTimeRanges.Controls.Add(this.groupBox5);
            this.tabTimeRanges.Controls.Add(this.groupBox4);
            this.tabTimeRanges.Controls.Add(this.groupBox3);
            this.tabTimeRanges.Location = new System.Drawing.Point(4, 22);
            this.tabTimeRanges.Name = "tabTimeRanges";
            this.tabTimeRanges.Padding = new System.Windows.Forms.Padding(3);
            this.tabTimeRanges.Size = new System.Drawing.Size(452, 324);
            this.tabTimeRanges.TabIndex = 1;
            this.tabTimeRanges.Text = "Classification Ranges";
            this.tabTimeRanges.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.dtpCheckOutEnd);
            this.groupBox5.Controls.Add(this.dtpCheckOutStart);
            this.groupBox5.Location = new System.Drawing.Point(16, 217);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(420, 91);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Check-Out Time Range";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(229, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "End Time:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(19, 32);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Start Time:";
            // 
            // dtpCheckOutEnd
            // 
            this.dtpCheckOutEnd.CustomFormat = "HH:mm";
            this.dtpCheckOutEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckOutEnd.Location = new System.Drawing.Point(293, 28);
            this.dtpCheckOutEnd.Name = "dtpCheckOutEnd";
            this.dtpCheckOutEnd.ShowUpDown = true;
            this.dtpCheckOutEnd.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckOutEnd.TabIndex = 3;
            // 
            // dtpCheckOutStart
            // 
            this.dtpCheckOutStart.CustomFormat = "HH:mm";
            this.dtpCheckOutStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckOutStart.Location = new System.Drawing.Point(86, 28);
            this.dtpCheckOutStart.Name = "dtpCheckOutStart";
            this.dtpCheckOutStart.ShowUpDown = true;
            this.dtpCheckOutStart.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckOutStart.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.dtpPauseEnd);
            this.groupBox4.Controls.Add(this.dtpPauseStart);
            this.groupBox4.Location = new System.Drawing.Point(16, 120);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(420, 91);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pause Time Range";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(229, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "End Time:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Start Time:";
            // 
            // dtpPauseEnd
            // 
            this.dtpPauseEnd.CustomFormat = "HH:mm";
            this.dtpPauseEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPauseEnd.Location = new System.Drawing.Point(293, 28);
            this.dtpPauseEnd.Name = "dtpPauseEnd";
            this.dtpPauseEnd.ShowUpDown = true;
            this.dtpPauseEnd.Size = new System.Drawing.Size(100, 20);
            this.dtpPauseEnd.TabIndex = 3;
            // 
            // dtpPauseStart
            // 
            this.dtpPauseStart.CustomFormat = "HH:mm";
            this.dtpPauseStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPauseStart.Location = new System.Drawing.Point(86, 28);
            this.dtpPauseStart.Name = "dtpPauseStart";
            this.dtpPauseStart.ShowUpDown = true;
            this.dtpPauseStart.Size = new System.Drawing.Size(100, 20);
            this.dtpPauseStart.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.dtpCheckInEnd);
            this.groupBox3.Controls.Add(this.dtpCheckInStart);
            this.groupBox3.Location = new System.Drawing.Point(16, 23);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(420, 91);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Check-In Time Range";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(229, 32);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "End Time:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(19, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Start Time:";
            // 
            // dtpCheckInEnd
            // 
            this.dtpCheckInEnd.CustomFormat = "HH:mm";
            this.dtpCheckInEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckInEnd.Location = new System.Drawing.Point(293, 28);
            this.dtpCheckInEnd.Name = "dtpCheckInEnd";
            this.dtpCheckInEnd.ShowUpDown = true;
            this.dtpCheckInEnd.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckInEnd.TabIndex = 1;
            // 
            // dtpCheckInStart
            // 
            this.dtpCheckInStart.CustomFormat = "HH:mm";
            this.dtpCheckInStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCheckInStart.Location = new System.Drawing.Point(86, 28);
            this.dtpCheckInStart.Name = "dtpCheckInStart";
            this.dtpCheckInStart.ShowUpDown = true;
            this.dtpCheckInStart.Size = new System.Drawing.Size(100, 20);
            this.dtpCheckInStart.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(316, 376);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(397, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(12, 368);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lblTabDescription
            // 
            this.lblTabDescription.AutoSize = true;
            this.lblTabDescription.Location = new System.Drawing.Point(15, 365);
            this.lblTabDescription.Name = "lblTabDescription";
            this.lblTabDescription.Size = new System.Drawing.Size(311, 13);
            this.lblTabDescription.TabIndex = 4;
            this.lblTabDescription.Text = "Configure time limits that will trigger alerts when exceeded.";
            // 
            // AttendanceSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 411);
            this.Controls.Add(this.lblTabDescription);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AttendanceSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Attendance Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabLimits.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabTimeRanges.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLimits;
        private System.Windows.Forms.TabPage tabTimeRanges;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpCheckInLimit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpMaxPauseTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpCheckOutLimit;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkUseDefaultPause;
        private System.Windows.Forms.DateTimePicker dtpDefaultPauseTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DateTimePicker dtpCheckInEnd;
        private System.Windows.Forms.DateTimePicker dtpCheckInStart;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dtpCheckOutEnd;
        private System.Windows.Forms.DateTimePicker dtpCheckOutStart;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DateTimePicker dtpPauseEnd;
        private System.Windows.Forms.DateTimePicker dtpPauseStart;
        private System.Windows.Forms.Label lblTabDescription;
    }
}
