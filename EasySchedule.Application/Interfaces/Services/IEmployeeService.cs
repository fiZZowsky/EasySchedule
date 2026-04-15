using EasySchedule.Domain.Entities;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IEmployeeService
{
    Task<Result<Employee>> GetEmployeeAsync(int id);
    Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync();
    Task<Result> AddEmployeeAsync(Employee employee);
    Task<Result> UpdateEmployeeAsync(Employee employee);
    Task<Result> DeleteEmployeeAsync(int id);
}