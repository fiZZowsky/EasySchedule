using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Infrastructure.Services;

public class ShiftRequirementService : IShiftRequirementService
{
    private readonly IShiftRequirementRepository _requirementRepository;
    private readonly IScheduleRepository _scheduleRepository;

    public ShiftRequirementService(
        IShiftRequirementRepository requirementRepository,
        IScheduleRepository scheduleRepository)
    {
        _requirementRepository = requirementRepository;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<IEnumerable<ShiftRequirement>>> GetRequirementsForScheduleAsync(int scheduleId)
    {
        var requirements = await _requirementRepository.GetByScheduleIdAsync(scheduleId);
        return Result.Ok(requirements);
    }

    public async Task<Result> SetDefaultRequirementAsync(int scheduleId, int shiftTypeId, int count)
    {
        if (count < 0) return Result.Fail("Liczba pracowników nie może być ujemna.");

        var existing = (await _requirementRepository.GetByScheduleIdAsync(scheduleId))
            .FirstOrDefault(r => r.ShiftTypeId == shiftTypeId && r.SpecificDate == null);

        if (existing != null)
        {
            existing.UpdateCount(count);
            await _requirementRepository.UpdateAsync(existing);
        }
        else
        {
            await _requirementRepository.AddAsync(new ShiftRequirement(scheduleId, shiftTypeId, count));
        }

        return Result.Ok();
    }

    public async Task<Result> SetOverrideRequirementAsync(int scheduleId, int shiftTypeId, DateOnly date, int count)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null) return Result.Fail("Grafik nie istnieje.");
        if (date < schedule.StartDate || date > schedule.EndDate)
            return Result.Fail("Data wyjątku musi mieścić się w zakresie grafiku.");

        var existing = (await _requirementRepository.GetByScheduleIdAsync(scheduleId))
            .FirstOrDefault(r => r.ShiftTypeId == shiftTypeId && r.SpecificDate == date);

        if (existing != null)
        {
            existing.UpdateCount(count);
            await _requirementRepository.UpdateAsync(existing);
        }
        else
        {
            await _requirementRepository.AddAsync(new ShiftRequirement(scheduleId, shiftTypeId, count, date));
        }

        return Result.Ok();
    }

    public async Task<Result> DeleteRequirementAsync(int id)
    {
        var req = await _requirementRepository.GetByIdAsync(id);
        if (req == null) return Result.Fail("Nie znaleziono wymagania.");

        await _requirementRepository.DeleteAsync(req);
        return Result.Ok();
    }
}