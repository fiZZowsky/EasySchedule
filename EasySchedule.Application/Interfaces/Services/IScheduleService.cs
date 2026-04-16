using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IScheduleService
{
    Task<Result<Schedule>> GetScheduleByIdAsync(int id);
    Task<Result<IEnumerable<Schedule>>> GetAllSchedulesAsync();
    Task<Result> AddScheduleAsync(Schedule schedule);
    Task<Result> UpdateScheduleAsync(Schedule schedule);
    Task<Result> DeleteScheduleAsync(int id);
}