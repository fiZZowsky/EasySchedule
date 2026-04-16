using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.Infrastructure.Repositories;

public class ShiftAssignmentRepository : IShiftAssignmentRepository
{
    private readonly AppDbContext _context;

    public ShiftAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShiftAssignment?> GetByIdAsync(int id)
    {
        return await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<ShiftAssignment>> GetAllAsync()
    {
        return await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftType)
            .ToListAsync();
    }

    public async Task AddAsync(ShiftAssignment assignment)
    {
        await _context.ShiftAssignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftAssignment assignment)
    {
        _context.ShiftAssignments.Update(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var assignment = await _context.ShiftAssignments.FindAsync(id);
        if (assignment != null)
        {
            _context.ShiftAssignments.Remove(assignment);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<ShiftAssignment>> GetByScheduleIdAsync(int scheduleId)
    {
        return await _context.ShiftAssignments
            .Include(a => a.Employee)
            .Include(a => a.ShiftType)
            .Where(a => a.ScheduleId == scheduleId)
            .ToListAsync();
    }

    public async Task DeleteByScheduleIdAsync(int scheduleId)
    {
        var assignments = await _context.ShiftAssignments
            .Where(a => a.ScheduleId == scheduleId)
            .ToListAsync();

        if (assignments.Any())
        {
            _context.ShiftAssignments.RemoveRange(assignments);
            await _context.SaveChangesAsync();
        }
    }
}