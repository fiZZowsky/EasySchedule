using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IScheduleRepository
{
    Task<IEnumerable<Schedule>> GetAllAsync();
    Task<Schedule?> GetByIdAsync(int id);
    Task<Schedule?> GetByIdWithAssignmentsAsync(int id);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(int scheduleId);
}