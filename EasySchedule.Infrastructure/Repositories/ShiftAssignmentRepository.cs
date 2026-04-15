using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ShiftAssignmentRepository : IShiftAssignmentRepository
{
    private readonly AppDbContext _dbContext;

    public ShiftAssignmentRepository(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IEnumerable<ShiftAssignment>> GetByScheduleIdAsync(int scheduleId) =>
        await _dbContext.ShiftAssignments
            .Where(sa => sa.ScheduleId == scheduleId)
            .Include(sa => sa.Employee)
            .Include(sa => sa.ShiftType)
            .ToListAsync();

    public async Task<IEnumerable<ShiftAssignment>> GetByEmployeeAndDateRangeAsync(int employeeId, DateOnly startDate, DateOnly endDate) =>
        await _dbContext.ShiftAssignments
            .Where(sa => sa.EmployeeId == employeeId && sa.Date >= startDate && sa.Date <= endDate)
            .ToListAsync();

    public async Task<ShiftAssignment?> GetByIdAsync(int id) =>
        await _dbContext.ShiftAssignments.FirstOrDefaultAsync(sa => sa.Id == id);

    public async Task AddAsync(ShiftAssignment assignment)
    {
        await _dbContext.ShiftAssignments.AddAsync(assignment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(ShiftAssignment assignment)
    {
        _dbContext.ShiftAssignments.Remove(assignment);
        await _dbContext.SaveChangesAsync();
    }
}