using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using FluentResults;
using FluentValidation;

namespace EasySchedule.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IValidator<Employee> _validator;

    public EmployeeService(IEmployeeRepository employeeRepository, IValidator<Employee> validator)
    {
        _employeeRepository = employeeRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return Result.Ok(employees);
    }

    public async Task<Result<Employee>> GetEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return Result.Fail($"Pracownik o ID {id} nie został znaleziony.");
        }
        return Result.Ok(employee);
    }

    public async Task<Result> AddEmployeeAsync(Employee employee)
    {
        var validationResult = await _validator.ValidateAsync(employee);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        await _employeeRepository.AddAsync(employee);
        return Result.Ok();
    }

    public async Task<Result> UpdateEmployeeAsync(Employee employee)
    {
        var validationResult = await _validator.ValidateAsync(employee);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new Error(e.ErrorMessage)).ToList();
            return Result.Fail(errors);
        }

        var existingEmployee = await _employeeRepository.GetByIdAsync(employee.Id);
        if (existingEmployee == null)
        {
            return Result.Fail("Nie można zaktualizować. Pracownik nie istnieje.");
        }

        await _employeeRepository.UpdateAsync(employee);
        return Result.Ok();
    }

    public async Task<Result> DeleteEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            return Result.Fail("Nie można usunąć. Pracownik nie istnieje.");
        }

        await _employeeRepository.DeleteAsync(employee);
        return Result.Ok();
    }
}