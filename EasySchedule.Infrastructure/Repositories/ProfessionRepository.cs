using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ProfessionRepository : IProfessionRepository
{
    private readonly AppDbContext _dbContext;

    public ProfessionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Profession>> GetAllAsync()
    {
        return await _dbContext.Professions.AsNoTracking().ToListAsync();
    }

    public async Task<Profession?> GetByIdAsync(int id)
    {
        return await _dbContext.Professions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Profession?> GetByIdWithEmployeesAsync(int id)
    {
        return await _dbContext.Professions
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Profession profession)
    {
        await _dbContext.Professions.AddAsync(profession);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profession profession)
    {
        _dbContext.Professions.Update(profession);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Profession profession)
    {
        _dbContext.Professions.Remove(profession);
        await _dbContext.SaveChangesAsync();
    }
}