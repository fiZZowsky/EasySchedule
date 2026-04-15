using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IShiftAssignmentRepository
{
    Task<IEnumerable<ShiftAssignment>> GetByScheduleIdAsync(int scheduleId);
    Task<IEnumerable<ShiftAssignment>> GetByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate);
    Task<ShiftAssignment?> GetByIdAsync(int id);
    Task AddAsync(ShiftAssignment assignment);
    Task DeleteAsync(ShiftAssignment assignment);
}