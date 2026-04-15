using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IShiftRequirementRepository
{
    Task<IEnumerable<ShiftRequirement>> GetByScheduleIdAsync(int scheduleId);
    Task<ShiftRequirement?> GetByIdAsync(int id);
    Task AddAsync(ShiftRequirement requirement);
    Task UpdateAsync(ShiftRequirement requirement);
    Task DeleteAsync(ShiftRequirement requirement);
    Task DeleteRangeAsync(IEnumerable<ShiftRequirement> requirements);
}