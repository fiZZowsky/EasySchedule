using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IValidator<Schedule> _validator;

    public ScheduleService(IScheduleRepository scheduleRepository, IValidator<Schedule> validator)
    {
        _scheduleRepository = scheduleRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<Schedule>>> GetAllSchedulesAsync() =>
        Result.Ok(await _scheduleRepository.GetAllAsync());

    public async Task<Result<Schedule>> GetScheduleWithDetailsAsync(int id)
    {
        var schedule = await _scheduleRepository.GetByIdWithAssignmentsAsync(id);
        return schedule == null ? Result.Fail("Grafik nie znaleziony.") : Result.Ok(schedule);
    }

    public async Task<Result> CreateScheduleAsync(Schedule schedule)
    {
        var valResult = await _validator.ValidateAsync(schedule);
        if (!valResult.IsValid) return Result.Fail(valResult.Errors.Select(e => new Error(e.ErrorMessage)));

        await _scheduleRepository.AddAsync(schedule);
        return Result.Ok();
    }

    public async Task<Result> ChangeScheduleStatusAsync(int id, ScheduleStatus status)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);
        if (schedule == null) return Result.Fail("Grafik nie istnieje.");

        schedule.ChangeStatus(status);
        await _scheduleRepository.UpdateAsync(schedule);
        return Result.Ok();
    }
}