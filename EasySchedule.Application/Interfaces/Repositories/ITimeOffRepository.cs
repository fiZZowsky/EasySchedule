using EasySchedule.Domain.Entities;
namespace EasySchedule.Application.Interfaces.Repositories;

public interface ITimeOffRepository
{
    Task<IEnumerable<TimeOff>> GetAllAsync();
    Task<IEnumerable<TimeOff>> GetByEmployeeIdAsync(int employeeId);
    Task<TimeOff?> GetByIdAsync(int id);
    Task AddAsync(TimeOff timeOff);
    Task UpdateAsync(TimeOff timeOff);
    Task DeleteAsync(TimeOff timeOff);
    Task DeleteAllAsync();
}