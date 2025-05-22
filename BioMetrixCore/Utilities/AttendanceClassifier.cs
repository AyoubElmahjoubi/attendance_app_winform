using System;
using System.Collections.Generic;
using System.Linq;

namespace BioMetrixCore
{
    public class AttendanceClassifier
    {
        // Time ranges for classification
        private static readonly TimeSpan CheckInStartTime = new TimeSpan(8, 30, 0);
        private static readonly TimeSpan CheckInEndTime = new TimeSpan(10, 30, 0);
        
        private static readonly TimeSpan PauseStartTime = new TimeSpan(13, 0, 0);
        private static readonly TimeSpan PauseEndTime = new TimeSpan(14, 30, 0);
        
        private static readonly TimeSpan CheckOutStartTime = new TimeSpan(17, 0, 0);
        private static readonly TimeSpan CheckOutEndTime = new TimeSpan(20, 0, 0);

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
                if (CheckInStartTime <= recordTime && recordTime <= CheckInEndTime)
                {
                    // Check In
                    classifiedRecords[recordDate][userId].CheckInTimes.Add(recordDateTime);
                }
                else if (PauseStartTime <= recordTime && recordTime <= PauseEndTime)
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
                else if (CheckOutStartTime <= recordTime && recordTime <= CheckOutEndTime)
                {
                    // Check Out
                    classifiedRecords[recordDate][userId].CheckOutTimes.Add(recordDateTime);
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
