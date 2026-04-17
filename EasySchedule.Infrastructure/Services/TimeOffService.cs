using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class TimeOffService : ITimeOffService
{
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<TimeOff> _validator;

    public TimeOffService(ITimeOffRepository timeOffRepository, IEmployeeRepository employeeRepository, IValidator<TimeOff> validator)
    {
        _timeOffRepository = timeOffRepository;
        _employeeRepository = employeeRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<TimeOff>>> GetAllTimeOffsAsync()
    {
        var timeOffs = await _timeOffRepository.GetAllAsync();
        return Result.Ok(timeOffs);
    }

    public async Task<Result<IEnumerable<TimeOff>>> GetTimeOffsForEmployeeAsync(int employeeId)
    {
        var timeOffs = await _timeOffRepository.GetByEmployeeIdAsync(employeeId);
        return Result.Ok(timeOffs);
    }

    public async Task<Result> AddTimeOffAsync(TimeOff timeOff)
    {
        var validationResult = await _validator.ValidateAsync(timeOff);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => new Error(e.ErrorMessage)));

        var employee = await _employeeRepository.GetByIdAsync(timeOff.EmployeeId);
        if (employee == null)
            return Result.Fail("Pracownik nie istnieje w bazie.");

        var existingTimeOffs = await _timeOffRepository.GetByEmployeeIdAsync(timeOff.EmployeeId);
        var isOverlapping = existingTimeOffs.Any(t =>
            timeOff.StartDate <= t.EndDate && timeOff.EndDate >= t.StartDate);

        if (isOverlapping)
            return Result.Fail("Wskazany okres nakłada się z już istniejącymi dniami wolnymi tego pracownika.");

        await _timeOffRepository.AddAsync(timeOff);
        return Result.Ok();
    }

    public async Task<Result> UpdateTimeOffAsync(TimeOff timeOff)
    {
        var validationResult = await _validator.ValidateAsync(timeOff);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => new Error(e.ErrorMessage)));

        await _timeOffRepository.UpdateAsync(timeOff);
        return Result.Ok();
    }

    public async Task<Result> DeleteTimeOffAsync(int id)
    {
        var timeOff = await _timeOffRepository.GetByIdAsync(id);
        if (timeOff == null)
            return Result.Fail("Nie znaleziono wpisu o dniach wolnych.");

        await _timeOffRepository.DeleteAsync(timeOff);
        return Result.Ok();
    }

    public async Task<Result> DeleteAllTimeOffsAsync()
    {
        await _timeOffRepository.DeleteAllAsync();
        return Result.Ok();
    }
}