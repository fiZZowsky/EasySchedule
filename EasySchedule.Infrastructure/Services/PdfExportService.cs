using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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

        var safeName = string.Concat(schedule.Name.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
        var fileName = $"Grafik_{safeName}.pdf";

        var directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var filePath = Path.Combine(directory, fileName);

        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph($"Grafik: {schedule.Name}")
                .SetFont(boldFont)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            document.Add(new Paragraph($"Okres: {schedule.StartDate:dd.MM.yyyy} - {schedule.EndDate:dd.MM.yyyy}")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14));

            document.Add(new Paragraph("\n"));

            var sortedAssignments = assignments?.OrderBy(a => a.Date).ThenBy(a => a.Employee?.Surname).ToList() ?? new List<ShiftAssignment>();

            if (!sortedAssignments.Any())
            {
                document.Add(new Paragraph("Brak przypisanych zmian w tym grafiku."));
            }
            else
            {
                float[] columnWidths = { 1, 3, 2 };
                var table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();

                table.AddHeaderCell(new Cell().Add(new Paragraph("Data").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Pracownik").SetFont(boldFont)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Zmiana").SetFont(boldFont)));

                foreach (var a in sortedAssignments)
                {
                    var dateStr = a.Date.ToString("dd.MM");
                    var empName = $"{a.Employee?.Name} {a.Employee?.Surname}";
                    var shiftName = a.ShiftType?.Name ?? "Nieznana";

                    table.AddCell(new Cell().Add(new Paragraph(dateStr)));
                    table.AddCell(new Cell().Add(new Paragraph(empName)));
                    table.AddCell(new Cell().Add(new Paragraph(shiftName)));
                }

                document.Add(table);
            }

            document.Close();
            return Result.Ok(filePath);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Błąd generowania PDF ({ex.GetType().Name}): {ex.Message}\n{ex.StackTrace}");
        }
    }
}