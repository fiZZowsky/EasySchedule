using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ShiftTypeRepository : IShiftTypeRepository
{
    private readonly AppDbContext _dbContext;

    public ShiftTypeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ShiftType>> GetAllAsync()
    {
        return await _dbContext.ShiftTypes.AsNoTracking().ToListAsync();
    }

    public async Task<ShiftType?> GetByIdAsync(int id)
    {
        return await _dbContext.ShiftTypes.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task AddAsync(ShiftType shiftType)
    {
        await _dbContext.ShiftTypes.AddAsync(shiftType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftType shiftType)
    {
        _dbContext.ShiftTypes.Update(shiftType);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ShiftType shiftType)
    {
        _dbContext.ShiftTypes.Remove(shiftType);
        await _dbContext.SaveChangesAsync();
    }
}