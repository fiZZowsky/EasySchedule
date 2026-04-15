using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ShiftRequirementRepository : IShiftRequirementRepository
{
    private readonly AppDbContext _dbContext;

    public ShiftRequirementRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ShiftRequirement>> GetByScheduleIdAsync(int scheduleId)
    {
        return await _dbContext.ShiftRequirements
            .Where(sr => sr.ScheduleId == scheduleId)
            .Include(sr => sr.ShiftType)
            .ToListAsync();
    }

    public async Task<ShiftRequirement?> GetByIdAsync(int id)
    {
        return await _dbContext.ShiftRequirements.FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task AddAsync(ShiftRequirement requirement)
    {
        await _dbContext.ShiftRequirements.AddAsync(requirement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftRequirement requirement)
    {
        _dbContext.ShiftRequirements.Update(requirement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ShiftRequirement requirement)
    {
        _dbContext.ShiftRequirements.Remove(requirement);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ShiftRequirement> requirements)
    {
        _dbContext.ShiftRequirements.RemoveRange(requirements);
        await _dbContext.SaveChangesAsync();
    }
}