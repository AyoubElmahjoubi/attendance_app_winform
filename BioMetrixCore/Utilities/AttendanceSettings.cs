using System;

namespace BioMetrixCore
{
    public class AttendanceSettings
    {
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
        
        // Private constructor to prevent direct instantiation
        private AttendanceSettings() { }
        
        // Get the singleton instance
        public static AttendanceSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AttendanceSettings();
                }
                return _instance;
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
        }
    }
}
