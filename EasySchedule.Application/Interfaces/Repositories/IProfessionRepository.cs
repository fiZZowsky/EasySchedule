using EasySchedule.Domain.Entities;

namespace EasySchedule.Application.Interfaces.Repositories;

public interface IProfessionRepository
{
    Task<IEnumerable<Profession>> GetAllAsync();
    Task<Profession?> GetByIdAsync(int id);
    Task<Profession?> GetByIdWithEmployeesAsync(int id);
    Task AddAsync(Profession profession);
    Task UpdateAsync(Profession profession);
    Task DeleteAsync(Profession profession);
}