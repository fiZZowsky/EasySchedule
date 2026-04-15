using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class ShiftTypeService : IShiftTypeService
{
    private readonly IShiftTypeRepository _shiftTypeRepository;
    private readonly IValidator<ShiftType> _validator;

    public ShiftTypeService(IShiftTypeRepository shiftTypeRepository, IValidator<ShiftType> validator)
    {
        _shiftTypeRepository = shiftTypeRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<ShiftType>>> GetAllShiftTypesAsync()
    {
        var shiftTypes = await _shiftTypeRepository.GetAllAsync();
        return Result.Ok(shiftTypes);
    }

    public async Task<Result<ShiftType>> GetShiftTypeAsync(int id)
    {
        var shiftType = await _shiftTypeRepository.GetByIdAsync(id);
        if (shiftType == null)
        {
            return Result.Fail($"Rodzaj zmiany o ID {id} nie został znaleziony.");
        }
        return Result.Ok(shiftType);
    }

    public async Task<Result> AddShiftTypeAsync(ShiftType shiftType)
    {
        var validationResult = await _validator.ValidateAsync(shiftType);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        await _shiftTypeRepository.AddAsync(shiftType);
        return Result.Ok();
    }

    public async Task<Result> UpdateShiftTypeAsync(ShiftType shiftType)
    {
        var validationResult = await _validator.ValidateAsync(shiftType);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        var existingShiftType = await _shiftTypeRepository.GetByIdAsync(shiftType.Id);
        if (existingShiftType == null)
        {
            return Result.Fail("Nie można zaktualizować. Wskazany rodzaj zmiany nie istnieje.");
        }

        await _shiftTypeRepository.UpdateAsync(shiftType);
        return Result.Ok();
    }

    public async Task<Result> DeleteShiftTypeAsync(int id)
    {
        var shiftType = await _shiftTypeRepository.GetByIdAsync(id);
        if (shiftType == null)
        {
            return Result.Fail("Nie można usunąć. Rodzaj zmiany nie istnieje.");
        }

        await _shiftTypeRepository.DeleteAsync(shiftType);
        return Result.Ok();
    }
}