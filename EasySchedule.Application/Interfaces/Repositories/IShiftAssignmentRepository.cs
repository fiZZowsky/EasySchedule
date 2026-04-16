using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IShiftAssignmentRepository
{
    Task<ShiftAssignment?> GetByIdAsync(int id);
    Task<IEnumerable<ShiftAssignment>> GetAllAsync();
    Task AddAsync(ShiftAssignment assignment);
    Task UpdateAsync(ShiftAssignment assignment);
    Task DeleteAsync(int id);
    Task<IEnumerable<ShiftAssignment>> GetByScheduleIdAsync(int scheduleId);
    Task DeleteByScheduleIdAsync(int scheduleId);
}