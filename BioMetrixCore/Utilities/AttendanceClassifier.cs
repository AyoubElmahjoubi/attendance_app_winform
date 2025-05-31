using System;
using System.Collections.Generic;
using System.Linq;

namespace BioMetrixCore
{
    public class AttendanceClassifier
    {
        // Get time ranges from settings
        private static AttendanceSettings Settings => AttendanceSettings.Instance;

        /// <summary>
        /// Classifies attendance records into check-in, pause, and check-out categories
        /// </summary>
        /// <param name="records">List of attendance records from the device</param>
        /// <returns>Dictionary of classified attendance records by date and user</returns>
        public static Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>> ClassifyAttendance(ICollection<MachineInfo> records)
        {
            var classifiedRecords = new Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>>();

            foreach (var record in records)
            {
                DateTime recordDateTime = DateTime.Parse(record.DateTimeRecord);
                DateTime recordDate = recordDateTime.Date;
                TimeSpan recordTime = recordDateTime.TimeOfDay;
                int userId = record.IndRegID;

                // Create date entry if it doesn't exist
                if (!classifiedRecords.ContainsKey(recordDate))
                {
                    classifiedRecords[recordDate] = new Dictionary<int, ClassifiedAttendance>();
                }

                // Create user entry if it doesn't exist
                if (!classifiedRecords[recordDate].ContainsKey(userId))
                {
                    classifiedRecords[recordDate][userId] = new ClassifiedAttendance
                    {
                        Date = recordDate,
                        UserID = userId
                    };
                }

                // Classify based on time
                if (Settings.CheckInStartTime <= recordTime && recordTime <= Settings.CheckInEndTime)
                {
                    // Check In
                    classifiedRecords[recordDate][userId].CheckInTimes.Add(recordDateTime);
                }
                else if (Settings.PauseStartTime <= recordTime && recordTime <= Settings.PauseEndTime)
                {
                    // For pause, we need to determine if it's start or end of pause
                    // If the first pause record of the day, consider it as start
                    // If there's already a start without an end, consider it as end
                    var pauseStarts = classifiedRecords[recordDate][userId].PauseStartTimes.Count;
                    var pauseEnds = classifiedRecords[recordDate][userId].PauseEndTimes.Count;
                    
                    if (pauseStarts <= pauseEnds)
                    {
                        // More ends than starts or equal, so this is a start
                        classifiedRecords[recordDate][userId].PauseStartTimes.Add(recordDateTime);
                    }
                    else
                    {
                        // More starts than ends, so this is an end
                        classifiedRecords[recordDate][userId].PauseEndTimes.Add(recordDateTime);
                    }
                }
                else if (Settings.CheckOutStartTime <= recordTime && recordTime <= Settings.CheckOutEndTime)
                {
                    // Check Out
                    classifiedRecords[recordDate][userId].CheckOutTimes.Add(recordDateTime);
                }
            }

            // Handle default values for missing records in a comprehensive and integrated way
            ApplyDefaultValues(classifiedRecords);

            return classifiedRecords;
        }

        /// <summary>
        /// Apply default values for missing records in a comprehensive and integrated way
        /// </summary>
        private static void ApplyDefaultValues(Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>> classifiedRecords)
        {
            // Skip if no default options are enabled
            if (!Settings.UseDefaultCheckInTime && !Settings.UseDefaultCheckOutTime && !Settings.UseDefaultPauseTime)
                return;
                
            foreach (var dateEntry in classifiedRecords)
            {
                DateTime date = dateEntry.Key;
                
                foreach (var userEntry in dateEntry.Value)
                {
                    var attendance = userEntry.Value;
                    
                    // First handle check-in as it's the starting point
                    ApplyDefaultCheckIn(attendance, date);
                    
                    // Then handle pause times which depend on check-in
                    ApplyDefaultPauseTimes(attendance, date);
                    
                    // Finally handle check-out which may depend on both check-in and pause
                    ApplyDefaultCheckOut(attendance, date);
                    
                    // Verify time sequence is logical and make adjustments if needed
                    EnsureLogicalTimeSequence(attendance, date);
                }
            }
        }
        
        /// <summary>
        /// Apply default check-in time if needed
        /// </summary>
        private static void ApplyDefaultCheckIn(ClassifiedAttendance attendance, DateTime date)
        {
            if (Settings.UseDefaultCheckInTime && attendance.CheckInTimes.Count == 0)
            {
                DateTime defaultCheckIn = date.Date.Add(Settings.DefaultCheckInTime);
                attendance.CheckInTimes.Add(defaultCheckIn);
                attendance.HasDefaultCheckIn = true;
            }
        }
        
        /// <summary>
        /// Apply default pause times based on various scenarios
        /// </summary>
        private static void ApplyDefaultPauseTimes(ClassifiedAttendance attendance, DateTime date)
        {
            if (!Settings.UseDefaultPauseTime)
                return;
                
            // Case 1: No pause times at all
            if (attendance.PauseStartTimes.Count == 0 && attendance.PauseEndTimes.Count == 0)
            {
                DateTime noon = date.Date.AddHours(12);
                
                attendance.PauseStartTimes.Add(noon.AddMinutes(-(Settings.DefaultPauseTime.TotalMinutes / 2)));
                attendance.HasDefaultPauseStart = true;
                
                attendance.PauseEndTimes.Add(noon.AddMinutes(Settings.DefaultPauseTime.TotalMinutes / 2));
                attendance.HasDefaultPauseEnd = true;
                
                // Hide pause times when both are defaults
                attendance.HidePauseTimes = true;
            }
            // Case 2: Only pause start times exist, no end times
            else if (attendance.PauseStartTimes.Count > 0 && attendance.PauseEndTimes.Count == 0)
            {
                // Get the latest pause start time
                DateTime latestPauseStart = attendance.PauseStartTimes.Max();
                
                // Add a default pause end time based on the latest start time
                DateTime defaultPauseEnd = latestPauseStart.AddMinutes(Settings.DefaultPauseTime.TotalMinutes);
                attendance.PauseEndTimes.Add(defaultPauseEnd);
                attendance.HasDefaultPauseEnd = true;
            }
            // Case 3: Only pause end times exist, no start times
            else if (attendance.PauseStartTimes.Count == 0 && attendance.PauseEndTimes.Count > 0)
            {
                // Get the earliest pause end time
                DateTime earliestPauseEnd = attendance.PauseEndTimes.Min();
                
                // Add a default pause start time based on the earliest end time
                DateTime defaultPauseStart = earliestPauseEnd.AddMinutes(-Settings.DefaultPauseTime.TotalMinutes);
                attendance.PauseStartTimes.Add(defaultPauseStart);
                attendance.HasDefaultPauseStart = true;
            }
        }
        
        /// <summary>
        /// Apply default check-out time, considering existing values
        /// </summary>
        private static void ApplyDefaultCheckOut(ClassifiedAttendance attendance, DateTime date)
        {
            if (!Settings.UseDefaultCheckOutTime || attendance.CheckOutTimes.Count > 0)
                return;
                
            // Determine the appropriate check-out time based on existing records
            DateTime defaultCheckOut;
            
            // If we have pause end time(s), base check-out on the latest pause end
            if (attendance.PauseEndTimes.Count > 0)
            {
                DateTime latestPauseEnd = attendance.PauseEndTimes.Max();
                DateTime standardCheckOut = date.Date.Add(Settings.DefaultCheckOutTime);
                
                // Ensure at least 1 hour of work after pause
                DateTime suggestedCheckOut = latestPauseEnd.AddHours(1);
                
                // Use the later of standard default or calculated time
                defaultCheckOut = standardCheckOut > suggestedCheckOut ? standardCheckOut : suggestedCheckOut;
            }
            // If we have check-in time(s) but no pause records, ensure reasonable work duration
            else if (attendance.CheckInTimes.Count > 0)
            {
                DateTime latestCheckIn = attendance.CheckInTimes.Max();
                DateTime standardCheckOut = date.Date.Add(Settings.DefaultCheckOutTime);
                
                // Ensure at least 4 hours of work total if no pause
                DateTime suggestedCheckOut = latestCheckIn.AddHours(4);
                
                // Use the later of standard default or calculated time
                defaultCheckOut = standardCheckOut > suggestedCheckOut ? standardCheckOut : suggestedCheckOut;
            }
            // No check-in or pause records, just use the standard default
            else
            {
                defaultCheckOut = date.Date.Add(Settings.DefaultCheckOutTime);
            }
            
            attendance.CheckOutTimes.Add(defaultCheckOut);
            attendance.HasDefaultCheckOut = true;
        }
        
        /// <summary>
        /// Ensure all times follow a logical sequence: check-in → pause start → pause end → check-out
        /// </summary>
        private static void EnsureLogicalTimeSequence(ClassifiedAttendance attendance, DateTime date)
        {
            // If we have all the times, verify and adjust the sequence if needed
            if (attendance.CheckInTimes.Count > 0 && 
                attendance.PauseStartTimes.Count > 0 && 
                attendance.PauseEndTimes.Count > 0 && 
                attendance.CheckOutTimes.Count > 0)
            {
                DateTime checkIn = attendance.CheckInTimes.Min();
                DateTime pauseStart = attendance.PauseStartTimes.Min();
                DateTime pauseEnd = attendance.PauseEndTimes.Max();
                DateTime checkOut = attendance.CheckOutTimes.Max();
                
                // Fix pause start if it's before check-in
                if (pauseStart < checkIn)
                {
                    // Remove the invalid pause start
                    attendance.PauseStartTimes.Remove(pauseStart);
                    
                    // Add a new pause start that's 2 hours after check-in
                    DateTime newPauseStart = checkIn.AddHours(2);
                    attendance.PauseStartTimes.Add(newPauseStart);
                    attendance.HasDefaultPauseStart = true;
                }
                
                // Recalculate pause start
                pauseStart = attendance.PauseStartTimes.Min();
                
                // Fix pause end if it's before pause start
                if (pauseEnd < pauseStart)
                {
                    // Remove the invalid pause end
                    attendance.PauseEndTimes.Remove(pauseEnd);
                    
                    // Add a new pause end that's the default duration after pause start
                    DateTime newPauseEnd = pauseStart.AddMinutes(Settings.DefaultPauseTime.TotalMinutes);
                    attendance.PauseEndTimes.Add(newPauseEnd);
                    attendance.HasDefaultPauseEnd = true;
                }
                
                // Recalculate pause end
                pauseEnd = attendance.PauseEndTimes.Max();
                
                // Fix check-out if it's before pause end
                if (checkOut < pauseEnd)
                {
                    // Remove the invalid check-out
                    attendance.CheckOutTimes.Remove(checkOut);
                    
                    // Add a new check-out that's 1 hour after pause end
                    DateTime newCheckOut = pauseEnd.AddHours(1);
                    attendance.CheckOutTimes.Add(newCheckOut);
                    attendance.HasDefaultCheckOut = true;
                }
            }
        }

        /// <summary>
        /// Flattens the classified attendance data into a list for display
        /// </summary>
        /// <param name="classifiedRecords">Dictionary of classified records</param>
        /// <returns>Flat list of classified attendance records</returns>
        public static List<ClassifiedAttendance> FlattenClassifiedRecords(Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>> classifiedRecords)
        {
            var result = new List<ClassifiedAttendance>();

            foreach (var dateEntry in classifiedRecords)
            {
                foreach (var userEntry in dateEntry.Value)
                {
                    result.Add(userEntry.Value);
                }
            }

            return result;
        }
    }
}
