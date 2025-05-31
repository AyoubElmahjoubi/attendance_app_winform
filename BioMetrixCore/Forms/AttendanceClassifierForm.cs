using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static iTextSharp.text.Font;

namespace BioMetrixCore
{
    public partial class AttendanceClassifierForm : Form
    {
        private List<UserInfo> allUsers = new List<UserInfo>();
        private List<MachineInfo> allAttendanceRecords = new List<MachineInfo>();
        private Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>> classifiedRecords;
        private Dictionary<int, Dictionary<DateTime, WorkTimeData>> workTimesData;

        // Get settings from AttendanceSettings
        private AttendanceSettings Settings => AttendanceSettings.Instance;

        // Columns for the DataGridView
        private readonly string[] columnsAll = new string[] { 
            "Date", "Employee", "Check In", "Pause Start", "Pause End", "Check Out", "Total Pause Time", "Total Work Time", "Remarque"
        };
        
        private readonly string[] columnsSingle = new string[] { 
            "Date", "Check In", "Pause Start", "Pause End", "Check Out", "Total Pause Time", "Total Work Time", "Remarque"
        };

        public AttendanceClassifierForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            // Set up date pickers
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Today;

            // Set up the datagridview
            SetupDataGridView();

            // Load data and populate combobox
            LoadData();
            PopulateUserComboBox();

            // Set initial filter state
            chkUseStartDate.Checked = true;
            chkUseEndDate.Checked = true;
        }

        private void SetupDataGridView()
        {
            dgvAttendance.AutoGenerateColumns = false;
            dgvAttendance.AllowUserToAddRows = false;
            dgvAttendance.AllowUserToDeleteRows = false;
            dgvAttendance.ReadOnly = true;
            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAttendance.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAttendance.RowHeadersVisible = false;
            dgvAttendance.BackgroundColor = Color.White;
            dgvAttendance.BorderStyle = BorderStyle.None;
            dgvAttendance.EnableHeadersVisualStyles = false;

            // Style the datagridview
            dgvAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvAttendance.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvAttendance.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvAttendance.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgvAttendance.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvAttendance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgvAttendance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAttendance.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);
            dgvAttendance.ColumnHeadersHeight = 40;
        }

        private void LoadData()
        {
            try
            {
                // Load all users
                var deviceManipulator = new DeviceManipulator();
                // Fix: Need instance of Master to access objZkeeper
                var mainForm = Application.OpenForms.OfType<Master>().FirstOrDefault();
                allUsers = mainForm?.objZkeeper != null 
                    ? deviceManipulator.GetAllUserInfo(mainForm.objZkeeper, 1).ToList() 
                    : new List<UserInfo>();

                // Load all attendance records
                allAttendanceRecords = mainForm?.objZkeeper != null 
                    ? deviceManipulator.GetLogData(mainForm.objZkeeper, 1).ToList() 
                    : new List<MachineInfo>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateUserComboBox()
        {
            // Add "All Employees" as the first item
            cmbUsers.Items.Add("All Employees");
            
            // Add all users
            foreach (var user in allUsers.OrderBy(u => u.Name))
            {
                cmbUsers.Items.Add(user.Name);
            }

            // Select "All Employees" by default
            if (cmbUsers.Items.Count > 0)
                cmbUsers.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            // Get selected user ID
            int? selectedUserId = null;
            if (cmbUsers.SelectedIndex > 0) // Not "All Employees"
            {
                string selectedUserName = cmbUsers.SelectedItem.ToString();
                // Fix: Convert string to int? properly
                var selectedUser = allUsers.FirstOrDefault(u => u.Name == selectedUserName);
                if (selectedUser != null && int.TryParse(selectedUser.EnrollNumber, out int enrollNumber))
                {
                    selectedUserId = enrollNumber;
                }
            }

            // Get date range
            DateTime startDate = chkUseStartDate.Checked ? dtpStartDate.Value.Date : DateTime.MinValue;
            DateTime endDate = chkUseEndDate.Checked ? dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1) : DateTime.MaxValue;

            // Filter records based on date range
            var filteredRecords = allAttendanceRecords.Where(r => 
            {
                DateTime recordDate = DateTime.Parse(r.DateTimeRecord);
                return recordDate >= startDate && recordDate <= endDate;
            }).ToList();

            // Further filter by user if selected
            if (selectedUserId.HasValue)
            {
                // Convert selectedUserId to int for comparison with IndRegID (which is an int)
                int userIdInt = selectedUserId.Value;
                filteredRecords = filteredRecords.Where(r => r.IndRegID == userIdInt).ToList();
            }

            // Classify the attendance records
            classifiedRecords = AttendanceClassifier.ClassifyAttendance(filteredRecords);
            
            // Calculate work times
            workTimesData = CalculateDailyWorkTime(classifiedRecords);

            // Display the data in the grid
            DisplayAttendanceData(selectedUserId);
        }

        private void DisplayAttendanceData(int? selectedUserId)
        {
            // Clear the DataGridView
            dgvAttendance.Columns.Clear();
            dgvAttendance.DataSource = null;

            // Create a DataTable to hold the data
            DataTable dataTable = new DataTable();
            
            // Set up columns based on selection
            if (selectedUserId.HasValue)
            {
                // Single user columns
                foreach (var col in columnsSingle)
                {
                    dataTable.Columns.Add(col);
                }
            }
            else
            {
                // All users columns
                foreach (var col in columnsAll)
                {
                    dataTable.Columns.Add(col);
                }
            }

            // Add data rows
            foreach (var dateEntry in classifiedRecords.OrderBy(x => x.Key))
            {
                DateTime date = dateEntry.Key;
                
                foreach (var userEntry in dateEntry.Value)
                {
                    int userId = userEntry.Key;
                    
                    // Skip if we're filtering for a specific user and this isn't it
                    if (selectedUserId.HasValue && userId != selectedUserId.Value)
                        continue;
                    
                    var attendance = userEntry.Value;
                    
                    // Get the user name
                    string userName = allUsers.FirstOrDefault(u => u.EnrollNumber == userId.ToString())?.Name ?? "Unknown User";
                    
                    // Format times
                    string checkInTimes = string.Join(", ", attendance.CheckInTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                    string pauseStartTimes = string.Join(", ", attendance.PauseStartTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                    string pauseEndTimes = string.Join(", ", attendance.PauseEndTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                    
                    // Get work time info
                    var workTimeInfo = workTimesData.ContainsKey(userId) && workTimesData[userId].ContainsKey(date) 
                        ? workTimesData[userId][date] 
                        : new WorkTimeData();
                    
                    string totalWorkTime = workTimeInfo.TotalWorkTime.ToString(@"hh\:mm");
                    string totalPauseTime = workTimeInfo.TotalPauseTime.ToString(@"hh\:mm");
                    string checkOutTimes = string.Join(", ", attendance.CheckOutTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                    string notes = workTimeInfo.Notes;

                    // Create and add row
                    DataRow row = dataTable.NewRow();
                    
                    if (selectedUserId.HasValue)
                    {
                        // Single user format
                        row["Date"] = date.ToString("yyyy-MM-dd");
                        row["Check In"] = checkInTimes;
                        row["Pause Start"] = pauseStartTimes;
                        row["Pause End"] = pauseEndTimes;
                        row["Check Out"] = checkOutTimes;
                        row["Total Pause Time"] = totalPauseTime;
                        row["Total Work Time"] = totalWorkTime;
                        row["Remarque"] = notes;
                    }
                    else
                    {
                        // All users format
                        row["Date"] = date.ToString("yyyy-MM-dd");
                        row["Employee"] = userName;
                        row["Check In"] = checkInTimes;
                        row["Pause Start"] = pauseStartTimes;
                        row["Pause End"] = pauseEndTimes;
                        row["Check Out"] = checkOutTimes;
                        row["Total Pause Time"] = totalPauseTime;
                        row["Total Work Time"] = totalWorkTime;
                        row["Remarque"] = notes;
                    }
                    
                    dataTable.Rows.Add(row);
                }
            }

            // Set up DataGridView columns
            if (selectedUserId.HasValue)
            {
                foreach (var col in columnsSingle)
                {
                    var column = new DataGridViewTextBoxColumn
                    {
                        HeaderText = col,
                        DataPropertyName = col,
                        Name = col
                    };
                    dgvAttendance.Columns.Add(column);
                }
            }
            else
            {
                foreach (var col in columnsAll)
                {
                    var column = new DataGridViewTextBoxColumn
                    {
                        HeaderText = col,
                        DataPropertyName = col,
                        Name = col
                    };
                    dgvAttendance.Columns.Add(column);
                }
            }

            // Bind the DataTable to the DataGridView
            dgvAttendance.DataSource = dataTable;
            
            // Apply conditional formatting
            ApplyConditionalFormatting();
            
            // Highlight cells based on time conditions
            HighlightCells();
        }

        private void HighlightCells()
        {
            // Define colors for different states
            Color lateColor = Color.LightCoral;
            Color earlyColor = Color.LightCoral;
            Color longPauseColor = Color.LightCoral;
            Color defaultPauseColor = Color.LightBlue; // Color for default pause times
            
            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                // Skip header row
                if (row.IsNewRow) continue;
                
                // Check Check-In Time
                if (row.Cells["Check In"].Value != null)
                {
                    string checkInStr = row.Cells["Check In"].Value.ToString();
                    var parts = checkInStr.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var checkInTime = new TimeSpan(hour, minute, 0);
                        if (checkInTime > Settings.CheckInLimit)
                        {
                            row.Cells["Check In"].Style.BackColor = lateColor;
                        }
                    }
                }
                
                // Check Check-Out Time
                if (row.Cells["Check Out"].Value != null)
                {
                    string checkOutStr = row.Cells["Check Out"].Value.ToString();
                    var parts = checkOutStr.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var checkOutTime = new TimeSpan(hour, minute, 0);
                        if (checkOutTime < Settings.CheckOutLimit)
                        {
                            row.Cells["Check Out"].Style.BackColor = earlyColor;
                        }
                    }
                }
                
                // Check Total Pause Time
                if (row.Cells["Total Pause Time"].Value != null)
                {
                    string pauseStr = row.Cells["Total Pause Time"].Value.ToString();
                    var parts = pauseStr.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var pauseTime = new TimeSpan(hour, minute, 0);
                        if (pauseTime > Settings.MaxPauseDuration)
                        {
                            row.Cells["Total Pause Time"].Style.BackColor = longPauseColor;
                        }
                    }
                }
                
                // Get the corresponding record to check for default pause times
                string dateStr = row.Cells["Date"].Value.ToString();
                string employeeStr = row.Cells.Count > 1 && row.Cells["Employee"] != null ? 
                    row.Cells["Employee"].Value.ToString() : null;
                
                // Find the record for this row
                DateTime date = DateTime.Parse(dateStr);
                int? userId = null;
                
                // Try to get the user ID
                if (classifiedRecords.ContainsKey(date))
                {
                    foreach (var userEntry in classifiedRecords[date])
                    {
                        // If employee column exists, match by name/id
                        if (employeeStr != null)
                        {
                            if (userEntry.Value.UserName == employeeStr || userEntry.Key.ToString() == employeeStr)
                            {
                                userId = userEntry.Key;
                                break;
                            }
                        }
                        else
                        {
                            // If no employee column, we have a single user selected
                            userId = userEntry.Key;
                            break;
                        }
                    }
                }
                
                // If we found the record, check for default pause times
                if (userId.HasValue && classifiedRecords.ContainsKey(date) && classifiedRecords[date].ContainsKey(userId.Value))
                {
                    var record = classifiedRecords[date][userId.Value];
                    
                    // Highlight default pause start if applicable
                    if (record.HasDefaultPauseStart)
                    {
                        row.Cells["Pause Start"].Style.BackColor = defaultPauseColor;
                    }
                    
                    // Highlight default pause end if applicable
                    if (record.HasDefaultPauseEnd)
                    {
                        row.Cells["Pause End"].Style.BackColor = defaultPauseColor;
                    }
                }
            }
        }

        private void ApplyConditionalFormatting()
        {
            // Apply conditional formatting to rows based on time rules
            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                // Get values
                string checkInStr = row.Cells["Check In"].Value?.ToString();
                string checkOutStr = row.Cells["Check Out"].Value?.ToString();
                string pauseTimeStr = row.Cells["Total Pause Time"].Value?.ToString();

                // Check-in time check
                if (!string.IsNullOrEmpty(checkInStr))
                {
                    var parts = checkInStr.Split(',')[0].Trim().Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var checkInTime = new TimeSpan(hour, minute, 0);
                        if (checkInTime > Settings.CheckInLimit)
                        {
                            row.Cells["Check In"].Style.BackColor = Color.LightCoral;
                        }
                    }
                }

                // Check-out time check
                if (!string.IsNullOrEmpty(checkOutStr))
                {
                    var lastCheckOut = checkOutStr.Split(',').Last().Trim();
                    var parts = lastCheckOut.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var checkOutTime = new TimeSpan(hour, minute, 0);
                        if (checkOutTime < Settings.CheckOutLimit)
                        {
                            row.Cells["Check Out"].Style.BackColor = Color.LightCoral;
                        }
                    }
                }

                // Pause time check
                if (!string.IsNullOrEmpty(pauseTimeStr))
                {
                    var parts = pauseTimeStr.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int hour) && int.TryParse(parts[1], out int minute))
                    {
                        var pauseTime = new TimeSpan(hour, minute, 0);
                        if (pauseTime > Settings.MaxPauseDuration)
                        {
                            row.Cells["Total Pause Time"].Style.BackColor = Color.LightCoral;
                        }
                    }
                }
            }
        }

        private Dictionary<int, Dictionary<DateTime, WorkTimeData>> CalculateDailyWorkTime(Dictionary<DateTime, Dictionary<int, ClassifiedAttendance>> classifiedRecords)
        {
            var workTimes = new Dictionary<int, Dictionary<DateTime, WorkTimeData>>();

            foreach (var dateEntry in classifiedRecords)
            {
                DateTime date = dateEntry.Key;
                
                foreach (var userEntry in dateEntry.Value)
                {
                    int userId = userEntry.Key;
                    var attendance = userEntry.Value;
                    
                    // Initialize user and date entries if they don't exist
                    if (!workTimes.ContainsKey(userId))
                    {
                        workTimes[userId] = new Dictionary<DateTime, WorkTimeData>();
                    }
                    
                    if (!workTimes[userId].ContainsKey(date))
                    {
                        workTimes[userId][date] = new WorkTimeData();
                    }

                    // Skip if no check-in or check-out records
                    if (attendance.CheckInTimes.Count == 0 || attendance.CheckOutTimes.Count == 0)
                    {
                        workTimes[userId][date].Notes = "Incomplete record (missing check-in or check-out)";
                        continue;
                    }

                    // Get total pause time from the ClassifiedAttendance object
                    TimeSpan totalPauseTime = attendance.TotalPauseTime ?? TimeSpan.Zero;
                    
                    // Get total work time from the ClassifiedAttendance object
                    TimeSpan totalWorkTime = attendance.TotalWorkTime ?? TimeSpan.Zero;
                    
                    // Store the calculated times
                    workTimes[userId][date].TotalWorkTime = totalWorkTime;
                    workTimes[userId][date].TotalPauseTime = totalPauseTime;
                    
                    // Add notes based on time limits
                    var notes = "";
                    
                    // Check if check-in time is after the limit
                    if (attendance.CheckInTimes.Count > 0)
                    {
                        var firstCheckIn = attendance.CheckInTimes.Min();
                        if (firstCheckIn.TimeOfDay > Settings.CheckInLimit)
                        {
                            notes += "Late arrival. ";
                        }
                    }
                    
                    // Check if check-out time is before the limit
                    if (attendance.CheckOutTimes.Count > 0)
                    {
                        var lastCheckOut = attendance.CheckOutTimes.Max();
                        if (lastCheckOut.TimeOfDay < Settings.CheckOutLimit)
                        {
                            notes += "Early departure. ";
                        }
                    }
                    
                    // Check if pause time exceeds the limit
                    if (totalPauseTime > Settings.MaxPauseDuration)
                    {
                        notes += "Extended pause. ";
                    }
                    
                    workTimes[userId][date].Notes = notes;
                }
            }

            return workTimes;
        }

        private void ExportToPdf()
        {
            try
            {
                // Get selected user name
                string selectedUserName = cmbUsers.SelectedItem?.ToString() ?? "All Employees";
                
                // Get date range
                DateTime startDate = chkUseStartDate.Checked ? dtpStartDate.Value.Date : DateTime.MinValue;
                DateTime endDate = chkUseEndDate.Checked ? dtpEndDate.Value.Date : DateTime.MaxValue;
                
                // Create default filename
                string defaultFileName = $"Pointage_{selectedUserName.Replace(' ', '_')}_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.pdf";
                
                // Show save dialog
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                    saveDialog.FileName = defaultFileName;
                    saveDialog.Title = "Save Attendance Report";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Get selected user ID for filter
                        int? selectedUserId = null;
                        if (cmbUsers.SelectedIndex > 0) // Not "All Employees"
                        {
                            string selectedUser = cmbUsers.SelectedItem.ToString();
                            // Fix: Convert string to int properly for comparison
                            var selectedUserObj = allUsers.FirstOrDefault(u => u.Name == selectedUser);
                            if (selectedUserObj != null && int.TryParse(selectedUserObj.EnrollNumber, out int enrollNumber))
                            {
                                selectedUserId = enrollNumber;
                            }
                        }

                        // Generate customized PDF based on our filtered data
                        GenerateAttendanceReport(saveDialog.FileName, selectedUserName, startDate, endDate, selectedUserId);
                        
                        MessageBox.Show($"Report exported successfully to:\n{saveDialog.FileName}", 
                            "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerateAttendanceReport(string filePath, string userFilter, DateTime startDate, DateTime endDate, int? selectedUserId)
        {
            // Create a new document
            using (iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate(), 25, 25, 30, 30))
            {
                // Create a writer to write to the file
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                
                // Open the document for writing
                document.Open();
                
                // Add title
                // Fix: Use correct iTextSharp font syntax
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("Helvetica", 16, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.DARK_GRAY));
                Paragraph title = new Paragraph("Rapport de Pointage", titleFont);
                title.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                title.SpacingAfter = 10;
                document.Add(title);
                
                // Add date range
                iTextSharp.text.Font normalFont = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("Helvetica", 10, iTextSharp.text.Font.NORMAL));
                string dateRange = chkUseStartDate.Checked && chkUseEndDate.Checked 
                    ? $"Période du: {startDate:yyyy-MM-dd} au {endDate:yyyy-MM-dd}" 
                    : "Période: Toutes les dates";
                Paragraph dateRangePara = new Paragraph(dateRange, normalFont);
                dateRangePara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                document.Add(dateRangePara);
                
                // Add employee filter
                Paragraph employeePara = new Paragraph($"Employé(e): {userFilter}", normalFont);
                employeePara.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                employeePara.SpacingAfter = 20;
                document.Add(employeePara);
                
                // Create table
                PdfPTable table = new PdfPTable(selectedUserId.HasValue ? 8 : 9);
                table.WidthPercentage = 100;
                
                // Set relative column widths
                if (selectedUserId.HasValue)
                {
                    table.SetWidths(new float[] { 1.5f, 2f, 1.5f, 1.5f, 1.5f, 2f, 1.5f, 3f });
                }
                else
                {
                    table.SetWidths(new float[] { 1.5f, 2f, 2f, 1.5f, 1.5f, 1.5f, 2f, 1.5f, 3f });
                }
                
                // Add table header row
                // Fix: Use correct iTextSharp font syntax
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("Helvetica", 10, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.WHITE));
                
                // Create header cells
                if (selectedUserId.HasValue)
                {
                    AddTableCell(table, "Date", headerFont, true);
                    AddTableCell(table, "Entrée", headerFont, true);
                    AddTableCell(table, "Pause Début", headerFont, true);
                    AddTableCell(table, "Pause Fin", headerFont, true);
                    AddTableCell(table, "Sortie", headerFont, true);
                    AddTableCell(table, "Temps de pause total", headerFont, true);
                    AddTableCell(table, "Temps de travail total", headerFont, true);
                    AddTableCell(table, "Remarque", headerFont, true);
                }
                else
                {
                    AddTableCell(table, "Date", headerFont, true);
                    AddTableCell(table, "Employé", headerFont, true);
                    AddTableCell(table, "Entrée", headerFont, true);
                    AddTableCell(table, "Pause Début", headerFont, true);
                    AddTableCell(table, "Pause Fin", headerFont, true);
                    AddTableCell(table, "Sortie", headerFont, true);
                    AddTableCell(table, "Temps de pause total", headerFont, true);
                    AddTableCell(table, "Temps de travail total", headerFont, true);
                    AddTableCell(table, "Remarque", headerFont, true);
                }
                
                // Add data rows
                iTextSharp.text.Font cellFont = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("Helvetica", 9, iTextSharp.text.Font.NORMAL));
                iTextSharp.text.Font redFont = new iTextSharp.text.Font(iTextSharp.text.FontFactory.GetFont("Helvetica", 9, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.RED));
                
                foreach (var dateEntry in classifiedRecords.OrderBy(x => x.Key))
                {
                    DateTime date = dateEntry.Key;
                    
                    foreach (var userEntry in dateEntry.Value)
                    {
                        int userId = userEntry.Key;
                        
                        // Skip if we're filtering for a specific user and this isn't it
                        if (selectedUserId.HasValue && userId != selectedUserId.Value)
                            continue;
                        
                        var attendance = userEntry.Value;
                        
                        // Get the user name
                        string userName = allUsers.FirstOrDefault(u => u.EnrollNumber == userId.ToString())?.Name ?? "Unknown User";
                        
                        // Format times
                        string checkInTimes = string.Join(", ", attendance.CheckInTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                        string pauseStartTimes = string.Join(", ", attendance.PauseStartTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                        string pauseEndTimes = string.Join(", ", attendance.PauseEndTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                        
                        // Get work time info
                        var workTimeInfo = workTimesData.ContainsKey(userId) && workTimesData[userId].ContainsKey(date) 
                            ? workTimesData[userId][date] 
                            : new WorkTimeData();
                        
                        string totalWorkTime = workTimeInfo.TotalWorkTime.ToString(@"hh\:mm");
                        string totalPauseTime = workTimeInfo.TotalPauseTime.ToString(@"hh\:mm");
                        string checkOutTimes = string.Join(", ", attendance.CheckOutTimes.OrderBy(t => t).Select(t => t.ToString("HH:mm")));
                        string notes = workTimeInfo.Notes;

                        // Check for violations to determine text color
                        bool lateArrival = false;
                        bool earlyDeparture = false;
                        bool longPause = false;

                        // Check check-in time
                        if (attendance.CheckInTimes.Count > 0)
                        {
                            var firstCheckIn = attendance.CheckInTimes.Min();
                            lateArrival = firstCheckIn.TimeOfDay > Settings.CheckInLimit;
                        }

                        // Check check-out time
                        if (attendance.CheckOutTimes.Count > 0)
                        {
                            var lastCheckOut = attendance.CheckOutTimes.Max();
                            earlyDeparture = lastCheckOut.TimeOfDay < Settings.CheckOutLimit;
                        }

                        // Check pause time
                        longPause = workTimeInfo.TotalPauseTime > Settings.MaxPauseDuration;

                        // Add data to table
                        if (selectedUserId.HasValue)
                        {
                            // Single user format
                            AddTableCell(table, date.ToString("yyyy-MM-dd"), cellFont);
                            AddTableCell(table, checkInTimes, lateArrival ? redFont : cellFont);
                            AddTableCell(table, pauseStartTimes, cellFont);
                            AddTableCell(table, pauseEndTimes, cellFont);
                            AddTableCell(table, checkOutTimes, earlyDeparture ? redFont : cellFont);
                            AddTableCell(table, totalPauseTime, longPause ? redFont : cellFont);
                            AddTableCell(table, totalWorkTime, cellFont);
                            AddTableCell(table, notes, cellFont);
                        }
                        else
                        {
                            // All users format
                            AddTableCell(table, date.ToString("yyyy-MM-dd"), cellFont);
                            AddTableCell(table, userName, cellFont);
                            AddTableCell(table, checkInTimes, lateArrival ? redFont : cellFont);
                            AddTableCell(table, pauseStartTimes, cellFont);
                            AddTableCell(table, pauseEndTimes, cellFont);
                            AddTableCell(table, checkOutTimes, earlyDeparture ? redFont : cellFont);
                            AddTableCell(table, totalPauseTime, longPause ? redFont : cellFont);
                            AddTableCell(table, totalWorkTime, cellFont);
                            AddTableCell(table, notes, cellFont);
                        }
                    }
                }
                
                document.Add(table);
                
                // Add generated timestamp
                Paragraph footer = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}", normalFont);
                footer.Alignment = iTextSharp.text.Element.ALIGN_RIGHT;
                footer.SpacingBefore = 20;
                document.Add(footer);
                
                // Close the document
                document.Close();
            }
        }

        private void AddTableCell(PdfPTable table, string text, iTextSharp.text.Font font, bool isHeader = false)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            
            if (isHeader)
            {
                cell.BackgroundColor = new iTextSharp.text.BaseColor(20, 25, 72);
            }
            
            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
            cell.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
            cell.Padding = 5;
            
            table.AddCell(cell);
        }

        #region Event Handlers
        
        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            ExportToPdf();
        }

        private void chkUseStartDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpStartDate.Enabled = chkUseStartDate.Checked;
        }

        private void chkUseEndDate_CheckedChanged(object sender, EventArgs e)
        {
            dtpEndDate.Enabled = chkUseEndDate.Checked;
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Optional: Auto-apply filter when user selection changes
            // ApplyFilters();
        }
        
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsForm = new AttendanceSettingsForm();
            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                // Refresh the data with new settings
                if (classifiedRecords != null)
                {
                    ApplyFilters();
                }
            }
        }
        
        #endregion
    }

    // Helper class to store work time data
    public class WorkTimeData
    {
        public TimeSpan TotalWorkTime { get; set; } = TimeSpan.Zero;
        public TimeSpan TotalPauseTime { get; set; } = TimeSpan.Zero;
        public string Notes { get; set; } = string.Empty;
    }
}
