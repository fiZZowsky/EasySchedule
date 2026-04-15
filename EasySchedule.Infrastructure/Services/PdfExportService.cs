using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EasySchedule.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    public async Task<Result<byte[]>> ExportScheduleToPdfAsync(Schedule schedule)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(schedule.Name).FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{schedule.StartDate} - {schedule.EndDate}");
                        });
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Data");
                            header.Cell().Element(CellStyle).Text("Pracownik");
                            header.Cell().Element(CellStyle).Text("Zmiana");
                            header.Cell().Element(CellStyle).Text("Czas");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                        });

                        foreach (var assignment in schedule.ShiftAssignments.OrderBy(a => a.Date))
                        {
                            table.Cell().Element(RowStyle).Text(assignment.Date.ToString("dd.MM.yyyy"));
                            table.Cell().Element(RowStyle).Text($"{assignment.Employee?.Name} {assignment.Employee?.Surname}");
                            table.Cell().Element(RowStyle).Text(assignment.ShiftType?.Name);
                            table.Cell().Element(RowStyle).Text($"{assignment.ShiftType?.Duration.TotalHours}h");

                            static IContainer RowStyle(IContainer container) =>
                                container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Strona ");
                        x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfBytes = document.GeneratePdf();
            return Result.Ok(pdfBytes);
        }
        catch (Exception ex)
        {
            return Result.Fail($"Błąd generowania PDF: {ex.Message}");
        }
    }
}