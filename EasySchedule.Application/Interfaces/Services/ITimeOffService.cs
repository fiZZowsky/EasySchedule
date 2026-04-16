using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface ITimeOffService
{
    Task<Result<IEnumerable<TimeOff>>> GetAllTimeOffsAsync();
    Task<Result<IEnumerable<TimeOff>>> GetTimeOffsForEmployeeAsync(int employeeId);
    Task<Result> AddTimeOffAsync(TimeOff timeOff);
    Task<Result> UpdateTimeOffAsync(TimeOff timeOff);
    Task<Result> DeleteTimeOffAsync(int id);
}