using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class ProfessionService : IProfessionService
{
    private readonly IProfessionRepository _professionRepository;
    private readonly IValidator<Profession> _validator;

    public ProfessionService(IProfessionRepository professionRepository, IValidator<Profession> validator)
    {
        _professionRepository = professionRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<Profession>>> GetAllProfessionsAsync()
    {
        var professions = await _professionRepository.GetAllAsync();
        return Result.Ok(professions);
    }

    public async Task<Result<Profession>> GetProfessionAsync(int id)
    {
        var profession = await _professionRepository.GetByIdAsync(id);
        if (profession == null)
        {
            return Result.Fail($"Zawód o ID {id} nie został znaleziony.");
        }
        return Result.Ok(profession);
    }

    public async Task<Result> AddProfessionAsync(Profession profession)
    {
        var validationResult = await _validator.ValidateAsync(profession);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        await _professionRepository.AddAsync(profession);
        return Result.Ok();
    }

    public async Task<Result> UpdateProfessionAsync(Profession profession)
    {
        var validationResult = await _validator.ValidateAsync(profession);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        var existingProfession = await _professionRepository.GetByIdAsync(profession.Id);
        if (existingProfession == null)
        {
            return Result.Fail("Nie można zaktualizować. Zawód nie istnieje.");
        }

        await _professionRepository.UpdateAsync(profession);
        return Result.Ok();
    }

    public async Task<Result> DeleteProfessionAsync(int id)
    {
        var profession = await _professionRepository.GetByIdWithEmployeesAsync(id);

        if (profession == null)
        {
            return Result.Fail("Nie można usunąć. Zawód nie istnieje.");
        }

        if (profession.Employees.Any())
        {
            return Result.Fail($"Nie można usunąć zawodu '{profession.Name}', ponieważ jest do niego przypisanych {profession.Employees.Count} pracowników.");
        }

        await _professionRepository.DeleteAsync(profession);
        return Result.Ok();
    }
}