using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class ShiftAssignmentService : IShiftAssignmentService
{
    private readonly IShiftAssignmentRepository _assignmentRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IValidator<ShiftAssignment> _validator;

    public ShiftAssignmentService(
        IShiftAssignmentRepository assignmentRepository,
        IScheduleRepository scheduleRepository,
        ITimeOffRepository timeOffRepository,
        IValidator<ShiftAssignment> validator)
    {
        _assignmentRepository = assignmentRepository;
        _scheduleRepository = scheduleRepository;
        _timeOffRepository = timeOffRepository;
        _validator = validator;
    }

    public async Task<Result> AssignShiftAsync(ShiftAssignment assignment)
    {
        var valResult = await _validator.ValidateAsync(assignment);
        if (!valResult.IsValid) return Result.Fail(valResult.Errors.Select(e => new Error(e.ErrorMessage)));

        var schedule = await _scheduleRepository.GetByIdAsync(assignment.ScheduleId);
        if (schedule == null) return Result.Fail("Wskazany grafik nie istnieje.");

        if (assignment.Date < schedule.StartDate || assignment.Date > schedule.EndDate)
            return Result.Fail($"Data zmiany ({assignment.Date}) wykracza poza ramy grafiku ({schedule.StartDate} - {schedule.EndDate}).");

        var employeeTimeOffs = await _timeOffRepository.GetByEmployeeIdAsync(assignment.EmployeeId);
        var isOnLeave = employeeTimeOffs.Any(t => assignment.Date >= t.StartDate && assignment.Date <= t.EndDate);
        if (isOnLeave)
            return Result.Fail("Nie można przypisać zmiany. Pracownik przebywa w tym dniu na zaplanowanym urlopie/zwolnieniu.");

        var existingAssignments = await _assignmentRepository.GetByEmployeeAndDateRangeAsync(assignment.EmployeeId, assignment.Date, assignment.Date);
        if (existingAssignments.Any())
            return Result.Fail("Pracownik ma już przypisaną zmianę na ten dzień.");

        await _assignmentRepository.AddAsync(assignment);
        return Result.Ok();
    }

    public async Task<Result> RemoveAssignmentAsync(int id)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(id);
        if (assignment == null) return Result.Fail("Przypisanie nie istnieje.");

        await _assignmentRepository.DeleteAsync(assignment);
        return Result.Ok();
    }
}