using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain;

namespace EasySchedule.UI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeAsync(int id)
        {
            return await employeeRepository.GetByIdAsync(id);
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.Name) || string.IsNullOrWhiteSpace(employee.Surname))
            {
                throw new ArgumentException("Imię i nazwisko pracownika są wymagane.");
            }

            await employeeRepository.AddAsync(employee);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (employee.Id <= 0)
            {
                throw new ArgumentException("Nieprawidłowe ID pracownika.");
            }

            await employeeRepository.UpdateAsync(employee);
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            await employeeRepository.DeleteAsync(employee);
        }
    }
}
