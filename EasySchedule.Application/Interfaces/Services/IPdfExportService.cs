using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IPdfExportService
{
    Task<Result<byte[]>> ExportScheduleToPdfAsync(Schedule schedule);
}