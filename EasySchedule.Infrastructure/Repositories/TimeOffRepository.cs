using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class TimeOffRepository : ITimeOffRepository
{
    private readonly AppDbContext _dbContext;
    public TimeOffRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<TimeOff>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _dbContext.TimeOffs
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<TimeOff?> GetByIdAsync(int id) => await _dbContext.TimeOffs.FirstOrDefaultAsync(t => t.Id == id);

    public async Task AddAsync(TimeOff timeOff)
    {
        await _dbContext.TimeOffs.AddAsync(timeOff);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TimeOff timeOff)
    {
        _dbContext.TimeOffs.Update(timeOff);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TimeOff timeOff)
    {
        _dbContext.TimeOffs.Remove(timeOff);
        await _dbContext.SaveChangesAsync();
    }
}