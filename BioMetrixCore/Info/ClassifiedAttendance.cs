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

                // Calculate total pause time
                TimeSpan pauseTime = TimeSpan.Zero;
                int pauseCount = Math.Min(PauseStartTimes.Count, PauseEndTimes.Count);
                
                for (int i = 0; i < pauseCount; i++)
                {
                    pauseTime += PauseEndTimes[i] - PauseStartTimes[i];
                }

                // Calculate work time (check-out minus check-in minus pause time)
                return (lastCheckOut - firstCheckIn) - pauseTime;
            }
        }
    }
}
