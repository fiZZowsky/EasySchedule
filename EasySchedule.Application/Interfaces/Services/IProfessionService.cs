using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IProfessionService
{
    Task<Result<IEnumerable<Profession>>> GetAllProfessionsAsync();
    Task<Result<Profession>> GetProfessionAsync(int id);
    Task<Result> AddProfessionAsync(Profession profession);
    Task<Result> UpdateProfessionAsync(Profession profession);
    Task<Result> DeleteProfessionAsync(int id);
}