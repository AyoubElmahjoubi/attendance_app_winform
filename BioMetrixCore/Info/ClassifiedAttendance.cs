using System;
using System.Collections.Generic;

namespace BioMetrixCore
{
    public class ClassifiedAttendance
    {
        public DateTime Date { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public List<DateTime> CheckInTimes { get; set; } = new List<DateTime>();
        public List<DateTime> PauseStartTimes { get; set; } = new List<DateTime>();
        public List<DateTime> PauseEndTimes { get; set; } = new List<DateTime>();
        public List<DateTime> CheckOutTimes { get; set; } = new List<DateTime>();
        
        // Track if pause times were added automatically
        public bool HasDefaultPauseStart { get; set; } = false;
        public bool HasDefaultPauseEnd { get; set; } = false;
        
        // Controls whether to display pause times
        public bool HidePauseTimes { get; set; } = false;

        public TimeSpan? TotalPauseTime
        {
            get
            {
                // If no pause data, return zero
                if (PauseStartTimes.Count == 0 || PauseEndTimes.Count == 0)
                    return TimeSpan.Zero;

                // Calculate total pause time by summing the time between each start and end
                TimeSpan pauseTime = TimeSpan.Zero;
                int pauseCount = Math.Min(PauseStartTimes.Count, PauseEndTimes.Count);
                
                // Sort both lists to ensure proper pairing
                var sortedStarts = new List<DateTime>(PauseStartTimes);
                var sortedEnds = new List<DateTime>(PauseEndTimes);
                sortedStarts.Sort();
                sortedEnds.Sort();
                
                for (int i = 0; i < pauseCount; i++)
                {
                    pauseTime += sortedEnds[i] - sortedStarts[i];
                }

                return pauseTime;
            }
        }

        public TimeSpan? TotalWorkTime
        {
            get
            {
                if (CheckInTimes.Count == 0 || CheckOutTimes.Count == 0)
                    return null;

                // Get earliest check-in and latest check-out
                DateTime firstCheckIn = CheckInTimes[0];
                foreach (var time in CheckInTimes)
                {
                    if (time < firstCheckIn)
                        firstCheckIn = time;
                }

                DateTime lastCheckOut = CheckOutTimes[0];
                foreach (var time in CheckOutTimes)
                {
                    if (time > lastCheckOut)
                        lastCheckOut = time;
                }

                // Get the total pause time
                var pauseTime = TotalPauseTime ?? TimeSpan.Zero;

                // Calculate work time (check-out minus check-in minus pause time)
                return (lastCheckOut - firstCheckIn) - pauseTime;
            }
        }
    }
}
