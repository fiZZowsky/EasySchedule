using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IScheduleService
{
    Task<Result<IEnumerable<Schedule>>> GetAllSchedulesAsync();
    Task<Result<Schedule>> GetScheduleWithDetailsAsync(int id);
    Task<Result> CreateScheduleAsync(Schedule schedule);
    Task<Result> ChangeScheduleStatusAsync(int id, ScheduleStatus status);
}

public interface IShiftAssignmentService
{
    Task<Result> AssignShiftAsync(ShiftAssignment assignment);
    Task<Result> RemoveAssignmentAsync(int id);
}