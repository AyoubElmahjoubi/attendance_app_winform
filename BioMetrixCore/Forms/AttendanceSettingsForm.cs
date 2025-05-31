using System;
using System.Windows.Forms;

namespace BioMetrixCore
{
    public partial class AttendanceSettingsForm : Form
    {
        private AttendanceSettings settings;

        public AttendanceSettingsForm()
        {
            InitializeComponent();
            settings = AttendanceSettings.Instance;
            LoadSettings();
            
            // Show descriptions for each tab
            lblTabDescription.Text = "Configure time limits that will trigger alerts (highlighted in red) when exceeded.";
        }

        private void LoadSettings()
        {
            // Load time limits
            dtpCheckInLimit.Value = DateTime.Today.Add(settings.CheckInLimit);
            dtpCheckOutLimit.Value = DateTime.Today.Add(settings.CheckOutLimit);
            dtpMaxPauseTime.Value = DateTime.Today.Add(settings.MaxPauseDuration);
            
            // Load default pause time
            dtpDefaultPauseTime.Value = DateTime.Today.Add(settings.DefaultPauseTime);
            chkUseDefaultPause.Checked = settings.UseDefaultPauseTime;
            
            // Load classification time ranges
            dtpCheckInStart.Value = DateTime.Today.Add(settings.CheckInStartTime);
            dtpCheckInEnd.Value = DateTime.Today.Add(settings.CheckInEndTime);
            
            dtpPauseStart.Value = DateTime.Today.Add(settings.PauseStartTime);
            dtpPauseEnd.Value = DateTime.Today.Add(settings.PauseEndTime);
            
            dtpCheckOutStart.Value = DateTime.Today.Add(settings.CheckOutStartTime);
            dtpCheckOutEnd.Value = DateTime.Today.Add(settings.CheckOutEndTime);
        }

        private void SaveSettings()
        {
            // Save time limits
            settings.CheckInLimit = dtpCheckInLimit.Value.TimeOfDay;
            settings.CheckOutLimit = dtpCheckOutLimit.Value.TimeOfDay;
            settings.MaxPauseDuration = dtpMaxPauseTime.Value.TimeOfDay;
            
            // Save default pause time
            settings.DefaultPauseTime = dtpDefaultPauseTime.Value.TimeOfDay;
            settings.UseDefaultPauseTime = chkUseDefaultPause.Checked;
            
            // Save classification time ranges
            settings.CheckInStartTime = dtpCheckInStart.Value.TimeOfDay;
            settings.CheckInEndTime = dtpCheckInEnd.Value.TimeOfDay;
            
            settings.PauseStartTime = dtpPauseStart.Value.TimeOfDay;
            settings.PauseEndTime = dtpPauseEnd.Value.TimeOfDay;
            
            settings.CheckOutStartTime = dtpCheckOutStart.Value.TimeOfDay;
            settings.CheckOutEndTime = dtpCheckOutEnd.Value.TimeOfDay;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset all settings to defaults?", "Confirm Reset", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                settings.ResetToDefaults();
                LoadSettings();
            }
        }

        private void chkUseDefaultPause_CheckedChanged(object sender, EventArgs e)
        {
            dtpDefaultPauseTime.Enabled = chkUseDefaultPause.Checked;
        }
        
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the description label based on the selected tab
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Limits tab
                    lblTabDescription.Text = "Configure time limits that will trigger alerts (highlighted in red) when exceeded.";
                    break;
                case 1: // Time Ranges tab
                    lblTabDescription.Text = "Configure the time ranges used to classify fingerprint records as check-in, pause, or check-out.";
                    break;
                default:
                    lblTabDescription.Text = "";
                    break;
            }
        }
    }
}
