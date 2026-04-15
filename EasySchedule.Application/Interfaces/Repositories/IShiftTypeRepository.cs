using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IShiftTypeRepository
{
    Task<IEnumerable<ShiftType>> GetAllAsync();
    Task<ShiftType?> GetByIdAsync(int id);
    Task AddAsync(ShiftType shiftType);
    Task UpdateAsync(ShiftType shiftType);
    Task DeleteAsync(ShiftType shiftType);
}