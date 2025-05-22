using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BioMetrixCore
{
    public class AttendanceDetailView : UserControl
    {
        private DataGridView dgvClassifiedAttendance;
        private Button btnExportPdf;
        private Panel pnlControls;
        private List<ClassifiedAttendance> attendanceRecords;

        public AttendanceDetailView(List<ClassifiedAttendance> records)
        {
            this.attendanceRecords = records;
            InitializeComponent();
            PopulateGrid();
        }

        private void InitializeComponent()
        {
            this.dgvClassifiedAttendance = new DataGridView();
            this.btnExportPdf = new Button();
            this.pnlControls = new Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClassifiedAttendance)).BeginInit();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvClassifiedAttendance
            // 
            this.dgvClassifiedAttendance.AllowUserToAddRows = false;
            this.dgvClassifiedAttendance.AllowUserToDeleteRows = false;
            this.dgvClassifiedAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClassifiedAttendance.BackgroundColor = System.Drawing.Color.White;
            this.dgvClassifiedAttendance.BorderStyle = BorderStyle.None;
            this.dgvClassifiedAttendance.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClassifiedAttendance.Dock = DockStyle.Fill;
            this.dgvClassifiedAttendance.Location = new System.Drawing.Point(0, 40);
            this.dgvClassifiedAttendance.Name = "dgvClassifiedAttendance";
            this.dgvClassifiedAttendance.ReadOnly = true;
            this.dgvClassifiedAttendance.Size = new System.Drawing.Size(800, 410);
            this.dgvClassifiedAttendance.TabIndex = 0;
            // 
            // btnExportPdf
            // 
            this.btnExportPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportPdf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(208)))), ((int)(((byte)(154)))));
            this.btnExportPdf.FlatAppearance.BorderSize = 0;
            this.btnExportPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportPdf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportPdf.ForeColor = System.Drawing.Color.White;
            this.btnExportPdf.Location = new System.Drawing.Point(693, 8);
            this.btnExportPdf.Name = "btnExportPdf";
            this.btnExportPdf.Size = new System.Drawing.Size(96, 26);
            this.btnExportPdf.TabIndex = 1;
            this.btnExportPdf.Text = "Export PDF";
            this.btnExportPdf.UseVisualStyleBackColor = false;
            this.btnExportPdf.Click += new System.EventHandler(this.btnExportPdf_Click);
            //
            // pnlControls
            //
            this.pnlControls.Controls.Add(this.btnExportPdf);
            this.pnlControls.Dock = DockStyle.Top;
            this.pnlControls.Height = 40;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(800, 40);
            this.pnlControls.TabIndex = 2;
            // 
            // AttendanceDetailView
            // 
            this.Controls.Add(this.dgvClassifiedAttendance);
            this.Controls.Add(this.pnlControls);
            this.Name = "AttendanceDetailView";
            this.Size = new System.Drawing.Size(800, 450);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClassifiedAttendance)).EndInit();
            this.pnlControls.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void PopulateGrid()
        {
            // Create a view model for the grid
            var viewModel = attendanceRecords.Select(record => new {
                Date = record.Date.ToShortDateString(),
                UserID = record.UserID,
                UserName = record.UserName ?? "Unknown",
                FirstCheckIn = FormatTime(record.CheckInTimes.OrderBy(t => t).FirstOrDefault()),
                LastCheckOut = FormatTime(record.CheckOutTimes.OrderByDescending(t => t).FirstOrDefault()),
                PauseStart = FormatTime(record.PauseStartTimes.OrderBy(t => t).FirstOrDefault()),
                PauseEnd = FormatTime(record.PauseEndTimes.OrderBy(t => t).FirstOrDefault()),
                PauseCount = Math.Min(record.PauseStartTimes.Count, record.PauseEndTimes.Count),
                TotalWorkTime = record.TotalWorkTime.HasValue ? 
                                string.Format("{0:D2}:{1:D2}", 
                                (int)record.TotalWorkTime.Value.TotalHours,
                                record.TotalWorkTime.Value.Minutes) : "N/A"
            }).ToList();

            dgvClassifiedAttendance.DataSource = viewModel;
            
            // Format the grid
            dgvClassifiedAttendance.BorderStyle = BorderStyle.None;
            dgvClassifiedAttendance.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvClassifiedAttendance.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvClassifiedAttendance.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvClassifiedAttendance.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;

            // Column headers styling
            dgvClassifiedAttendance.EnableHeadersVisualStyles = false;
            dgvClassifiedAttendance.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvClassifiedAttendance.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgvClassifiedAttendance.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private string FormatTime(DateTime time)
        {
            return time != DateTime.MinValue ? time.ToShortTimeString() : "N/A";
        }
        
        private void btnExportPdf_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                saveFileDialog.Title = "Save Attendance Report";
                saveFileDialog.DefaultExt = "pdf";
                saveFileDialog.FileName = $"AttendanceReport_{DateTime.Now:yyyyMMdd}";
                
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    PdfReportGenerator.GenerateAttendanceReport(attendanceRecords, saveFileDialog.FileName);
                }
            }
        }
    }
}
