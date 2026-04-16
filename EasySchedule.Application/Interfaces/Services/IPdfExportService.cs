using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IPdfExportService
{
    Task<Result<string>> ExportScheduleToPdfAsync(Schedule schedule, IEnumerable<ShiftAssignment> assignments);
}