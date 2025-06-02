using System;
using System.IO;
using System.Collections.Generic;

namespace BioMetrixCore
{
    public class AttendanceSettings
    {
        // Settings file path in the same directory as the executable
        private static readonly string SettingsFilePath = Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            "AttendanceSettings.txt");
            
        // Singleton instance
        private static AttendanceSettings _instance;
        
        // Time limits for alerts
        public TimeSpan CheckInLimit { get; set; } = new TimeSpan(9, 0, 0); // Default: 9:00 AM
        public TimeSpan CheckOutLimit { get; set; } = new TimeSpan(17, 0, 0); // Default: 5:00 PM
        public TimeSpan MaxPauseDuration { get; set; } = new TimeSpan(1, 0, 0); // Default: 1 hour
        
        // Default pause time for missing pause records
        public TimeSpan DefaultPauseTime { get; set; } = new TimeSpan(1, 0, 0); // Default: 1 hour
        
        // Flag to use default pause time when pause records are missing
        public bool UseDefaultPauseTime { get; set; } = false;
        
        // Default check-in and check-out times
        public bool UseDefaultCheckInTime { get; set; } = false;
        public bool UseDefaultCheckOutTime { get; set; } = false;
        public TimeSpan DefaultCheckInTime { get; set; } = new TimeSpan(9, 0, 0); // Default: 9:00 AM
        public TimeSpan DefaultCheckOutTime { get; set; } = new TimeSpan(17, 0, 0); // Default: 5:00 PM
        
        // Classification time ranges
        public TimeSpan CheckInStartTime { get; set; } = new TimeSpan(8, 30, 0); // Default: 8:30 AM
        public TimeSpan CheckInEndTime { get; set; } = new TimeSpan(10, 30, 0); // Default: 10:30 AM
        
        public TimeSpan PauseStartTime { get; set; } = new TimeSpan(13, 0, 0); // Default: 1:00 PM
        public TimeSpan PauseEndTime { get; set; } = new TimeSpan(14, 30, 0); // Default: 2:30 PM
        
        public TimeSpan CheckOutStartTime { get; set; } = new TimeSpan(17, 0, 0); // Default: 5:00 PM
        public TimeSpan CheckOutEndTime { get; set; } = new TimeSpan(20, 0, 0); // Default: 8:00 PM
        
        // Private constructor
        private AttendanceSettings() { }
        
        // Get the singleton instance
        public static AttendanceSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AttendanceSettings();
                    _instance.LoadSettings();
                }
                return _instance;
            }
        }
        
        // Save current settings to file
        public void SaveToFile()
        {
            try
            {
                // Create a list of key-value pairs to save
                var lines = new List<string>
                {
                    // Time limits
                    $"CheckInLimit={CheckInLimit.Ticks}",
                    $"CheckOutLimit={CheckOutLimit.Ticks}",
                    $"MaxPauseDuration={MaxPauseDuration.Ticks}",
                    
                    // Default pause time
                    $"DefaultPauseTime={DefaultPauseTime.Ticks}",
                    $"UseDefaultPauseTime={UseDefaultPauseTime}",
                    
                    // Default check-in and check-out times
                    $"UseDefaultCheckInTime={UseDefaultCheckInTime}",
                    $"UseDefaultCheckOutTime={UseDefaultCheckOutTime}",
                    $"DefaultCheckInTime={DefaultCheckInTime.Ticks}",
                    $"DefaultCheckOutTime={DefaultCheckOutTime.Ticks}",
                    
                    // Classification time ranges
                    $"CheckInStartTime={CheckInStartTime.Ticks}",
                    $"CheckInEndTime={CheckInEndTime.Ticks}",
                    
                    $"PauseStartTime={PauseStartTime.Ticks}",
                    $"PauseEndTime={PauseEndTime.Ticks}",
                    
                    $"CheckOutStartTime={CheckOutStartTime.Ticks}",
                    $"CheckOutEndTime={CheckOutEndTime.Ticks}"
                };
                
                // Write all lines to the file
                File.WriteAllLines(SettingsFilePath, lines);
                
                System.Windows.Forms.MessageBox.Show($"Settings saved to {SettingsFilePath}", "Settings Saved", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error saving settings: {ex.Message}", "Error", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
        
        // Load settings from file
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    // Read all lines from the file
                    string[] lines = File.ReadAllLines(SettingsFilePath);
                    
                    // Create a dictionary to store the settings
                    var settings = new Dictionary<string, string>();
                    
                    // Parse each line into key-value pairs
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            settings[parts[0]] = parts[1];
                        }
                    }
                    
                    // Apply the settings
                    
                    // Time limits
                    if (settings.ContainsKey("CheckInLimit"))
                        CheckInLimit = new TimeSpan(long.Parse(settings["CheckInLimit"]));
                    if (settings.ContainsKey("CheckOutLimit"))
                        CheckOutLimit = new TimeSpan(long.Parse(settings["CheckOutLimit"]));
                    if (settings.ContainsKey("MaxPauseDuration"))
                        MaxPauseDuration = new TimeSpan(long.Parse(settings["MaxPauseDuration"]));
                    
                    // Default pause time
                    if (settings.ContainsKey("DefaultPauseTime"))
                        DefaultPauseTime = new TimeSpan(long.Parse(settings["DefaultPauseTime"]));
                    if (settings.ContainsKey("UseDefaultPauseTime"))
                        UseDefaultPauseTime = bool.Parse(settings["UseDefaultPauseTime"]);
                    
                    // Default check-in and check-out times
                    if (settings.ContainsKey("UseDefaultCheckInTime"))
                        UseDefaultCheckInTime = bool.Parse(settings["UseDefaultCheckInTime"]);
                    if (settings.ContainsKey("UseDefaultCheckOutTime"))
                        UseDefaultCheckOutTime = bool.Parse(settings["UseDefaultCheckOutTime"]);
                    if (settings.ContainsKey("DefaultCheckInTime"))
                        DefaultCheckInTime = new TimeSpan(long.Parse(settings["DefaultCheckInTime"]));
                    if (settings.ContainsKey("DefaultCheckOutTime"))
                        DefaultCheckOutTime = new TimeSpan(long.Parse(settings["DefaultCheckOutTime"]));
                    
                    // Classification time ranges
                    if (settings.ContainsKey("CheckInStartTime"))
                        CheckInStartTime = new TimeSpan(long.Parse(settings["CheckInStartTime"]));
                    if (settings.ContainsKey("CheckInEndTime"))
                        CheckInEndTime = new TimeSpan(long.Parse(settings["CheckInEndTime"]));
                    
                    if (settings.ContainsKey("PauseStartTime"))
                        PauseStartTime = new TimeSpan(long.Parse(settings["PauseStartTime"]));
                    if (settings.ContainsKey("PauseEndTime"))
                        PauseEndTime = new TimeSpan(long.Parse(settings["PauseEndTime"]));
                    
                    if (settings.ContainsKey("CheckOutStartTime"))
                        CheckOutStartTime = new TimeSpan(long.Parse(settings["CheckOutStartTime"]));
                    if (settings.ContainsKey("CheckOutEndTime"))
                        CheckOutEndTime = new TimeSpan(long.Parse(settings["CheckOutEndTime"]));
                }
            }
            catch (Exception)
            {
                // If there's an error loading settings, use defaults
                ResetToDefaults();
            }
        }
        
        // Reset settings to defaults
        public void ResetToDefaults()
        {
            CheckInLimit = new TimeSpan(9, 0, 0);
            CheckOutLimit = new TimeSpan(17, 0, 0);
            MaxPauseDuration = new TimeSpan(1, 0, 0);
            
            DefaultPauseTime = new TimeSpan(1, 0, 0);
            UseDefaultPauseTime = false;
            
            UseDefaultCheckInTime = false;
            UseDefaultCheckOutTime = false;
            DefaultCheckInTime = new TimeSpan(9, 0, 0);
            DefaultCheckOutTime = new TimeSpan(17, 0, 0);
            
            CheckInStartTime = new TimeSpan(8, 30, 0);
            CheckInEndTime = new TimeSpan(10, 30, 0);
            
            PauseStartTime = new TimeSpan(13, 0, 0);
            PauseEndTime = new TimeSpan(14, 30, 0);
            
            CheckOutStartTime = new TimeSpan(17, 0, 0);
            CheckOutEndTime = new TimeSpan(20, 0, 0);
            
            // Save the default settings to file
            SaveToFile();
        }
    }
}
