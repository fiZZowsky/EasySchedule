using EasySchedule.Domain;

namespace EasySchedule.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<Employee?> GetEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
    }
}
