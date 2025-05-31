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

            // Handle default pause times if enabled in settings
            if (Settings.UseDefaultPauseTime)
            {
                foreach (var dateEntry in classifiedRecords)
                {
                    foreach (var userEntry in dateEntry.Value)
                    {
                        var attendance = userEntry.Value;
                        
                        // Check if we have check-in and check-out but no pause
                        if (attendance.CheckInTimes.Count > 0 && attendance.CheckOutTimes.Count > 0 && 
                            (attendance.PauseStartTimes.Count == 0 || attendance.PauseEndTimes.Count == 0))
                        {
                            // Get earliest check-in and latest check-out
                            DateTime firstCheckIn = attendance.CheckInTimes.Min();
                            DateTime lastCheckOut = attendance.CheckOutTimes.Max();
                            
                            // Only add default pause if there's enough time between check-in and check-out
                            if ((lastCheckOut - firstCheckIn).TotalHours >= 5)
                            {
                                // Calculate default pause time (noon + settings.defaultPauseTime/2 before and after)
                                DateTime noon = attendance.Date.AddHours(12);
                                
                                // Add default pause start and end
                                if (attendance.PauseStartTimes.Count == 0)
                                {
                                    attendance.PauseStartTimes.Add(noon.AddMinutes(-(Settings.DefaultPauseTime.TotalMinutes / 2)));
                                    attendance.HasDefaultPauseStart = true;
                                }
                                
                                if (attendance.PauseEndTimes.Count == 0)
                                {
                                    attendance.PauseEndTimes.Add(noon.AddMinutes(Settings.DefaultPauseTime.TotalMinutes / 2));
                                    attendance.HasDefaultPauseEnd = true;
                                }
                            }
                        }
                    }
                }
            }

            return classifiedRecords;
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
