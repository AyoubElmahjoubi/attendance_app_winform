///
///    Experimented By : Ozesh Thapa
///    Email: dablackscarlet@gmail.com
///
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BioMetrixCore
{
    public partial class Master : Form
    {
        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;

        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    ShowStatusBar("The device is connected !!", true);
                    btnConnect.Text = "Disconnect";
                    ToggleControls(true);
                }
                else
                {
                    ShowStatusBar("The device is diconnected !!", true);
                    objZkeeper.Disconnect();
                    btnConnect.Text = "Connect";
                    ToggleControls(false);
                }
            }
        }


        private void ToggleControls(bool value)
        {
            btnBeep.Enabled = value;
            btnDownloadFingerPrint.Enabled = value;
            btnPullData.Enabled = value;
            btnPowerOff.Enabled = value;
            btnRestartDevice.Enabled = value;
            btnGetDeviceTime.Enabled = value;
            btnEnableDevice.Enabled = value;
            btnDisableDevice.Enabled = value;
            btnGetAllUserID.Enabled = value;
            btnUploadUserInfo.Enabled = value;
            tbxMachineNumber.Enabled = !value;
            tbxPort.Enabled = !value;
            tbxDeviceIP.Enabled = !value;
            tbxPassword.Enabled = !value;

        }

        public Master()
        {
            InitializeComponent();
            ToggleControls(false);
            ShowStatusBar(string.Empty, true);
            DisplayEmpty();
        }


        /// <summary>
        /// Your Events will reach here if implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="actionType"></param>
        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar("The device is switched off", true);
                        DisplayEmpty();
                        btnConnect.Text = "Connect";
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                string ipAddress = tbxDeviceIP.Text.Trim();
                string port = tbxPort.Text.Trim();
                string password = tbxPassword.Text.Trim();
                int portNumber = 4370;
                int commPassword = 0;

                if (ipAddress == string.Empty || port == string.Empty)
                {
                    ShowStatusBar("IP Address and Port is mandatory !!", false);
                    return;
                }

                // Convert the string port to integer
                try
                {
                    portNumber = Convert.ToInt32(port);
                }
                catch (Exception)
                {
                    ShowStatusBar("Port Number seems to be wrong format !!", false);
                    return;
                }

                // Convert the password to integer if it's not empty
                if (!string.IsNullOrEmpty(password))
                {
                    // First, trim any whitespace that might cause parsing issues
                    password = password.Trim();
                    
                    // Try direct parsing first
                    if (int.TryParse(password, out commPassword))
                    {
                        // Successfully parsed
                    }
                    else
                    {
                        ShowStatusBar("Password must be a valid number.", false);
                        this.Cursor = Cursors.Default;
                        return;
                    }
                }
                
                this.Cursor = Cursors.WaitCursor;
                ShowStatusBar(string.Empty, true);

                // Ping the device first
                bool isDeviceConnected = manipulator.PingDevice(ipAddress);
                if (!isDeviceConnected)
                {
                    ShowStatusBar("The device at " + ipAddress + ":" + port + " did not respond!! Check if the device is powered on and connected to the network.", false);
                    this.Cursor = Cursors.Default;
                    return;
                }

                ShowStatusBar("Device responded to ping. Attempting to connect...", true);
                objZkeeper = new ZkemClient(RaiseDeviceEvent);   
                
                // Use the password if provided
                if (commPassword > 0)
                {
                    IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber, commPassword);
                }
                else
                {
                    IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);
                }

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));
                    lblDeviceInfo.Text = deviceInfo;
                }
                else
                {
                    ShowStatusBar("Failed to connect to the device. Please check device settings and try again.", false);
                }
            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;
        }


        public void ShowStatusBar(string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.FromArgb(230, 112, 134);
        }


        private void btnPingDevice_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                string ipAddress = tbxDeviceIP.Text.Trim();

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                {
                    ShowStatusBar("The Device IP is invalid !!", false);
                    return;
                }

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (isValidIpA)
                    ShowStatusBar("The device is active", true);
                else
                    ShowStatusBar("Could not read any response", false);
            }
            catch (Exception ex)
            {
                ShowStatusBar("Error pinging device: " + ex.Message, false);
            }
        }

        private void btnGetAllUserID_Click(object sender, EventArgs e)
        {
            try
            {
                ICollection<UserIDInfo> lstUserIDInfo = manipulator.GetAllUserID(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));

                if (lstUserIDInfo != null && lstUserIDInfo.Count > 0)
                {
                    BindToGridView(lstUserIDInfo);
                    ShowStatusBar(lstUserIDInfo.Count + " records found !!", true);
                }
                else
                {
                    DisplayEmpty();
                    DisplayListOutput("No records found");
                }

            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private void btnBeep_Click(object sender, EventArgs e)
        {
            objZkeeper.Beep(100);
        }

        private void btnDownloadFingerPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                ICollection<UserInfo> lstFingerPrintTemplates = manipulator.GetAllUserInfo(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));
                if (lstFingerPrintTemplates != null && lstFingerPrintTemplates.Count > 0)
                {
                    BindToGridView(lstFingerPrintTemplates);
                    ShowStatusBar(lstFingerPrintTemplates.Count + " records found !!", true);
                }
                else
                    DisplayListOutput("No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }


        private void btnPullData_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    BindToGridView(lstMachineInfo);
                    ShowStatusBar(lstMachineInfo.Count + " records found !!", true);
                }
                else
                    DisplayListOutput("No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }

        }

        private void btnClassifyAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                ShowStatusBar(string.Empty, true);

                // Check if device is connected
                if (!IsDeviceConnected)
                {
                    ShowStatusBar("Please connect to the device first!", false);
                    return;
                }

                // Open the AttendanceClassifierForm
                AttendanceClassifierForm classifierForm = new AttendanceClassifierForm();
                classifierForm.Show();
                
                ShowStatusBar("Attendance classifier opened successfully", true);
            }
            catch (Exception ex)
            {
                ShowStatusBar($"Error opening attendance classifier: {ex.Message}", false);
            }
        }

        private void btnAttendanceSettings_Click(object sender, EventArgs e)
        {
            using (var frm = new AttendanceSettingsForm())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ShowStatusBar("Attendance settings updated successfully", true);
                }
            }
        }


        private void ClearGrid()
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }


            dgvRecords.DataSource = null;
            dgvRecords.Controls.Clear();
            dgvRecords.Rows.Clear();
            dgvRecords.Columns.Clear();
        }
        private void BindToGridView(object list)
        {
            ClearGrid();
            dgvRecords.DataSource = list;
            dgvRecords.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            UniversalStatic.ChangeGridProperties(dgvRecords);
        }



        private void DisplayListOutput(string message)
        {
            if (dgvRecords.Controls.Count > 2)
            { dgvRecords.Controls.RemoveAt(2); }

            ShowStatusBar(message, false);
        }

        private void DisplayEmpty()
        {
            ClearGrid();
            dgvRecords.Controls.Add(new DataEmpty());
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        { UniversalStatic.DrawLineInFooter(pnlHeader, Color.FromArgb(204, 204, 204), 2); }



        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            var resultDia = DialogResult.None;
            resultDia = MessageBox.Show("Do you wish to Power Off the Device ??", "Power Off Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resultDia == DialogResult.Yes)
            {
                bool deviceOff = objZkeeper.PowerOffDevice(int.Parse(tbxMachineNumber.Text.Trim()));

            }

            this.Cursor = Cursors.Default;
        }

        private void btnRestartDevice_Click(object sender, EventArgs e)
        {

            DialogResult rslt = MessageBox.Show("Do you wish to restart the device now ??", "Restart Device", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rslt == DialogResult.Yes)
            {
                if (objZkeeper.RestartDevice(int.Parse(tbxMachineNumber.Text.Trim())))
                    ShowStatusBar("The device is being restarted, Please wait...", true);
                else
                    ShowStatusBar("Operation failed,please try again", false);
            }

        }

        private void btnGetDeviceTime_Click(object sender, EventArgs e)
        {
            int machineNumber = int.Parse(tbxMachineNumber.Text.Trim());
            int dwYear = 0;
            int dwMonth = 0;
            int dwDay = 0;
            int dwHour = 0;
            int dwMinute = 0;
            int dwSecond = 0;

            bool result = objZkeeper.GetDeviceTime(machineNumber, ref dwYear, ref dwMonth, ref dwDay, ref dwHour, ref dwMinute, ref dwSecond);

            string deviceTime = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond).ToString();
            List<DeviceTimeInfo> lstDeviceInfo = new List<DeviceTimeInfo>();
            lstDeviceInfo.Add(new DeviceTimeInfo() { DeviceTime = deviceTime });
            BindToGridView(lstDeviceInfo);
        }


        private void btnEnableDevice_Click(object sender, EventArgs e)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceEnabled = objZkeeper.EnableDevice(int.Parse(tbxMachineNumber.Text.Trim()), true);

        }



        private void btnDisableDevice_Click(object sender, EventArgs e)
        {
            // This is of no use since i implemented zkemKeeper the other way
            bool deviceDisabled = objZkeeper.DisableDeviceWithTimeOut(int.Parse(tbxMachineNumber.Text.Trim()), 3000);
        }

        private void tbxPort_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxPort); }

        private void tbxMachineNumber_TextChanged(object sender, EventArgs e)
        { UniversalStatic.ValidateInteger(tbxMachineNumber); }

        private void btnUploadUserInfo_Click(object sender, EventArgs e)
        {
            // Add you new UserInfo Here and  uncomment the below code
            //List<UserInfo> lstUserInfo = new List<UserInfo>();
            //manipulator.UploadFTPTemplate(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()), lstUserInfo);
        }
    }
}
