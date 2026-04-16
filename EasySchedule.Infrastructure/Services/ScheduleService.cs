using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Application.Validators;
using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;
    private readonly ScheduleValidator _validator;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
        _validator = new ScheduleValidator();
    }

    public async Task<Result<Schedule>> GetScheduleByIdAsync(int id)
    {
        var schedule = await _repository.GetByIdAsync(id);
        if (schedule == null)
            return Result.Fail("Nie znaleziono grafiku.");
        return Result.Ok(schedule);
    }

    public async Task<Result<IEnumerable<Schedule>>> GetAllSchedulesAsync()
    {
        var schedules = await _repository.GetAllAsync();
        return Result.Ok(schedules);
    }

    public async Task<Result> AddScheduleAsync(Schedule schedule)
    {
        var validationResult = await _validator.ValidateAsync(schedule);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        await _repository.AddAsync(schedule);
        return Result.Ok();
    }

    public async Task<Result> UpdateScheduleAsync(Schedule schedule)
    {
        var validationResult = await _validator.ValidateAsync(schedule);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        var existing = await _repository.GetByIdAsync(schedule.Id);
        if (existing == null)
            return Result.Fail("Nie znaleziono grafiku do aktualizacji.");

        existing.Name = schedule.Name;
        existing.StartDate = schedule.StartDate;
        existing.EndDate = schedule.EndDate;
        existing.ProfessionId = schedule.ProfessionId;
        existing.Status = schedule.Status;

        await _repository.UpdateAsync(existing);
        return Result.Ok();
    }

    public async Task<Result> DeleteScheduleAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return Result.Fail("Nie znaleziono grafiku do usunięcia.");

        await _repository.DeleteAsync(id);
        return Result.Ok();
    }
}