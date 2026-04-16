using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasySchedule.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    public async Task<Result<string>> ExportScheduleToPdfAsync(Schedule schedule, IEnumerable<ShiftAssignment> assignments)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var safeName = string.Concat(schedule.Name.Split(System.IO.Path.GetInvalidFileNameChars())).Replace(" ", "_");
        var fileName = $"Grafik_{safeName}.pdf";
        var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var filePath = System.IO.Path.Combine(directory, fileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);

            pdf.SetDefaultPageSize(PageSize.A4.Rotate());

            using var document = new Document(pdf);
            document.SetMargins(15, 10, 15, 10);

            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            document.Add(new Paragraph(schedule.Name.ToUpper())
                .SetFont(boldFont)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14));

            document.Add(new Paragraph($"{schedule.StartDate:dd.MM.yyyy} - {schedule.EndDate:dd.MM.yyyy}")
                .SetFont(regularFont)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(10)
                .SetMarginBottom(10));

            var assignmentsList = assignments?.ToList() ?? new List<ShiftAssignment>();

            var days = new List<DateOnly>();
            var tempDate = schedule.StartDate;
            while (tempDate <= schedule.EndDate)
            {
                days.Add(tempDate);
                tempDate = tempDate.AddDays(1);
            }

            var employees = assignmentsList
                .Where(a => a.Employee != null)
                .Select(a => a.Employee!)
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .OrderBy(e => e.Surname)
                .ToList();

            if (!employees.Any())
            {
                document.Add(new Paragraph("Brak danych do wyświetlenia."));
                document.Close();
                return Result.Ok(filePath);
            }

            int totalCols = days.Count + 1;
            float[] columnWidths = new float[totalCols];
            columnWidths[0] = 80f;
            for (int i = 1; i < totalCols; i++) columnWidths[i] = 20f;

            var table = new Table(UnitValue.CreatePointArray(columnWidths)).UseAllAvailableWidth();

            Color headerColor = new DeviceRgb(44, 62, 80);
            Color weekendColor = new DeviceRgb(231, 76, 60);

            table.AddHeaderCell(new Cell().Add(new Paragraph("Pracownik").SetFont(boldFont).SetFontSize(8).SetFontColor(ColorConstants.WHITE))
                .SetBackgroundColor(headerColor)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE));

            foreach (var day in days)
            {
                var dayNum = day.Day.ToString();
                var isWeekend = day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday;

                table.AddHeaderCell(new Cell().Add(new Paragraph(dayNum).SetFont(boldFont).SetFontSize(8).SetFontColor(ColorConstants.WHITE))
                    .SetBackgroundColor(isWeekend ? weekendColor : headerColor)
                    .SetTextAlignment(TextAlignment.CENTER));
            }

            foreach (var emp in employees)
            {
                table.AddCell(new Cell().Add(new Paragraph($"{emp.Surname} {emp.Name[0]}.").SetFont(regularFont).SetFontSize(8))
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE));

                foreach (var day in days)
                {
                    var shift = assignmentsList.FirstOrDefault(a => a.Date == day && a.EmployeeId == emp.Id);
                    var cell = new Cell().SetPadding(0).SetHeight(18f);

                    if (shift != null && shift.ShiftType != null)
                    {
                        Color shiftBg = shift.ShiftType.IsNightShift
                            ? new DeviceRgb(44, 62, 80)
                            : new DeviceRgb(52, 152, 219);

                        cell.SetBackgroundColor(shiftBg)
                            .Add(new Paragraph(shift.ShiftType.ShortName)
                                .SetFont(boldFont)
                                .SetFontSize(7)
                                .SetFontColor(ColorConstants.WHITE)
                                .SetTextAlignment(TextAlignment.CENTER));
                    }
                    else
                    {
                        cell.Add(new Paragraph("").SetFontSize(7));
                    }
                    table.AddCell(cell);
                }
            }

            document.Add(table);
            document.Close();
            return Result.Ok(filePath);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Błąd PDF: {ex.Message}");
        }
    }
}