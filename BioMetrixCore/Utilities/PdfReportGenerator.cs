using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace BioMetrixCore
{
    public class PdfReportGenerator
    {
        public static void GenerateAttendanceReport(List<ClassifiedAttendance> attendanceRecords, string filePath)
        {
            try
            {
                // Create a new Document
                using (Document document = new Document(PageSize.A4, 50, 50, 50, 50))
                {
                    // Create a writer to write to the file
                    using (PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create)))
                    {
                        // Open the document for writing
                        document.Open();

                        // Add title
                        Font titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, BaseColor.DARK_GRAY);
                        Paragraph title = new Paragraph("Attendance Report", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        title.SpacingAfter = 20;
                        document.Add(title);

                        // Add generation date
                        Font normalFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);
                        Font boldFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
                        Paragraph dateGenerated = new Paragraph($"Generated on: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", normalFont);
                        dateGenerated.Alignment = Element.ALIGN_RIGHT;
                        dateGenerated.SpacingAfter = 20;
                        document.Add(dateGenerated);

                        // Group attendance records by date
                        var recordsByDate = attendanceRecords
                            .OrderBy(r => r.Date)
                            .GroupBy(r => r.Date.Date);

                        foreach (var dateGroup in recordsByDate)
                        {
                            // Add date section
                            Paragraph dateParagraph = new Paragraph($"Date: {dateGroup.Key.ToString("yyyy-MM-dd")}", boldFont);
                            dateParagraph.SpacingBefore = 15;
                            dateParagraph.SpacingAfter = 10;
                            document.Add(dateParagraph);

                            // Create a table for this date
                            PdfPTable table = new PdfPTable(7);
                            table.WidthPercentage = 100;
                            table.SetWidths(new float[] { 1.5f, 3, 2, 2, 2, 2, 2.5f });

                            // Add table headers
                            AddTableHeader(table, boldFont, "User ID", "Name", "Check In", "Pause Start", 
                                           "Pause End", "Check Out", "Total Hours");

                            // Add data rows
                            foreach (var record in dateGroup.OrderBy(r => r.UserID))
                            {
                                string checkIn = FormatTime(record.CheckInTimes.OrderBy(t => t).FirstOrDefault());
                                string pauseStart = FormatTime(record.PauseStartTimes.OrderBy(t => t).FirstOrDefault());
                                string pauseEnd = FormatTime(record.PauseEndTimes.OrderBy(t => t).FirstOrDefault());
                                string checkOut = FormatTime(record.CheckOutTimes.OrderByDescending(t => t).FirstOrDefault());
                                string totalWorkTime = record.TotalWorkTime.HasValue ?
                                                       string.Format("{0:D2}:{1:D2}",
                                                       (int)record.TotalWorkTime.Value.TotalHours,
                                                       record.TotalWorkTime.Value.Minutes) : "N/A";

                                AddTableRow(table, normalFont, 
                                    record.UserID.ToString(),
                                    record.UserName ?? "Unknown",
                                    checkIn,
                                    pauseStart,
                                    pauseEnd,
                                    checkOut,
                                    totalWorkTime);
                            }

                            document.Add(table);

                            // Add a separator
                            LineSeparator line = new LineSeparator();
                            document.Add(new Chunk(line));
                        }

                        // Add summary section
                        Paragraph summaryTitle = new Paragraph("Summary", boldFont);
                        summaryTitle.SpacingBefore = 20;
                        summaryTitle.SpacingAfter = 10;
                        document.Add(summaryTitle);

                        // Create a summary table
                        PdfPTable summaryTable = new PdfPTable(4);
                        summaryTable.WidthPercentage = 100;
                        summaryTable.SetWidths(new float[] { 1.5f, 3, 2, 3 });

                        // Add summary headers
                        AddTableHeader(summaryTable, boldFont, "User ID", "Name", "Days Present", "Avg. Work Hours");

                        // Group by user for summary
                        var userGroups = attendanceRecords
                            .GroupBy(r => r.UserID);

                        foreach (var userGroup in userGroups.OrderBy(g => g.Key))
                        {
                            var records = userGroup.ToList();
                            string userName = records.FirstOrDefault()?.UserName ?? "Unknown";
                            int daysPresent = records.Count;
                            
                            // Calculate average work hours
                            TimeSpan totalWorkTime = TimeSpan.Zero;
                            int daysWithValidTime = 0;
                            
                            foreach (var record in records)
                            {
                                if (record.TotalWorkTime.HasValue)
                                {
                                    totalWorkTime += record.TotalWorkTime.Value;
                                    daysWithValidTime++;
                                }
                            }
                            
                            string avgWorkHours = daysWithValidTime > 0 ?
                                string.Format("{0:F2} hours", totalWorkTime.TotalHours / daysWithValidTime) : "N/A";

                            AddTableRow(summaryTable, normalFont,
                                userGroup.Key.ToString(),
                                userName,
                                daysPresent.ToString(),
                                avgWorkHours);
                        }

                        document.Add(summaryTable);

                        // Add footer
                        Paragraph footer = new Paragraph("BioMatrix Attendance System", normalFont);
                        footer.Alignment = Element.ALIGN_CENTER;
                        footer.SpacingBefore = 30;
                        document.Add(footer);
                    }
                }

                MessageBox.Show($"Report successfully generated at:\n{filePath}", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating PDF report: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void AddTableHeader(PdfPTable table, Font font, params string[] headers)
        {
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, font));
                cell.BackgroundColor = new BaseColor(240, 240, 240);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
            }
        }

        private static void AddTableRow(PdfPTable table, Font font, params string[] values)
        {
            foreach (var value in values)
            {
                PdfPCell cell = new PdfPCell(new Phrase(value, font));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Padding = 5;
                table.AddCell(cell);
            }
        }

        private static string FormatTime(DateTime time)
        {
            return time != DateTime.MinValue ? time.ToString("HH:mm") : "N/A";
        }
    }
}
