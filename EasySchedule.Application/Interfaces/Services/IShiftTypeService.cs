using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IShiftTypeService
{
    Task<Result<IEnumerable<ShiftType>>> GetAllShiftTypesAsync();
    Task<Result<ShiftType>> GetShiftTypeAsync(int id);
    Task<Result> AddShiftTypeAsync(ShiftType shiftType);
    Task<Result> UpdateShiftTypeAsync(ShiftType shiftType);
    Task<Result> DeleteShiftTypeAsync(int id);
}