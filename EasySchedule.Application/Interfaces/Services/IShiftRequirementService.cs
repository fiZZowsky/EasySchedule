using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IShiftRequirementService
{
    Task<Result<IEnumerable<ShiftRequirement>>> GetRequirementsForScheduleAsync(int scheduleId);
    Task<Result> SetDefaultRequirementAsync(int scheduleId, int shiftTypeId, int count);
    Task<Result> SetOverrideRequirementAsync(int scheduleId, int shiftTypeId, DateOnly date, int count);
    Task<Result> DeleteRequirementAsync(int id);
}