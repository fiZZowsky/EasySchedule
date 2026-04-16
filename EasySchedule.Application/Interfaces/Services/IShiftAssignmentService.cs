using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IShiftAssignmentService
{
    Task<Result<ShiftAssignment>> GetShiftAssignmentByIdAsync(int id);
    Task<Result<IEnumerable<ShiftAssignment>>> GetAllShiftAssignmentsAsync();
    Task<Result> AssignShiftAsync(ShiftAssignment assignment);
    Task<Result> UpdateShiftAssignmentAsync(ShiftAssignment assignment);
    Task<Result> DeleteShiftAssignmentAsync(int id);
    Task<Result<IEnumerable<ShiftAssignment>>> GetAssignmentsByScheduleIdAsync(int scheduleId);
    Task<Result> DeleteAssignmentsByScheduleIdAsync(int scheduleId);
}