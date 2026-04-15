using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _dbContext;

    public ScheduleRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<Schedule>> GetAllAsync() =>
        await _dbContext.Schedules.AsNoTracking().OrderByDescending(s => s.StartDate).ToListAsync();

    public async Task<Schedule?> GetByIdAsync(int id) =>
        await _dbContext.Schedules.FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Schedule?> GetByIdWithAssignmentsAsync(int id) =>
        await _dbContext.Schedules
            .Include(s => s.ShiftAssignments)
            .ThenInclude(sa => sa.Employee)
            .Include(s => s.ShiftAssignments)
            .ThenInclude(sa => sa.ShiftType)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(Schedule schedule)
    {
        await _dbContext.Schedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        _dbContext.Schedules.Update(schedule);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Schedule schedule)
    {
        _dbContext.Schedules.Remove(schedule);
        await _dbContext.SaveChangesAsync();
    }
}