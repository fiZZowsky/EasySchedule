using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Application.Validators;
using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Infrastructure.Services;

public class ShiftAssignmentService : IShiftAssignmentService
{
    private readonly IShiftAssignmentRepository _repository;
    private readonly ShiftAssignmentValidator _validator;

    public ShiftAssignmentService(IShiftAssignmentRepository repository)
    {
        _repository = repository;
        _validator = new ShiftAssignmentValidator();
    }

    public async Task<Result<ShiftAssignment>> GetShiftAssignmentByIdAsync(int id)
    {
        var assignment = await _repository.GetByIdAsync(id);
        if (assignment == null)
            return Result.Fail("Nie znaleziono przypisania zmiany.");
        return Result.Ok(assignment);
    }

    public async Task<Result<IEnumerable<ShiftAssignment>>> GetAllShiftAssignmentsAsync()
    {
        var assignments = await _repository.GetAllAsync();
        return Result.Ok(assignments);
    }

    public async Task<Result> AssignShiftAsync(ShiftAssignment assignment)
    {
        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        await _repository.AddAsync(assignment);
        return Result.Ok();
    }

    public async Task<Result> UpdateShiftAssignmentAsync(ShiftAssignment assignment)
    {
        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage));

        var existing = await _repository.GetByIdAsync(assignment.Id);
        if (existing == null)
            return Result.Fail("Nie znaleziono przypisania zmiany do aktualizacji.");

        await _repository.UpdateAsync(assignment);
        return Result.Ok();
    }

    public async Task<Result> DeleteShiftAssignmentAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return Result.Fail("Nie znaleziono przypisania zmiany do usunięcia.");

        await _repository.DeleteAsync(id);
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<ShiftAssignment>>> GetAssignmentsByScheduleIdAsync(int scheduleId)
    {
        var assignments = await _repository.GetByScheduleIdAsync(scheduleId);
        return Result.Ok(assignments);
    }

    public async Task<Result> DeleteAssignmentsByScheduleIdAsync(int scheduleId)
    {
        await _repository.DeleteByScheduleIdAsync(scheduleId);
        return Result.Ok();
    }
}