using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain;

namespace EasySchedule.UI.ViewModels;

public partial class EmployeesViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _surname = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    private Employee? _employeeBeingEdited;

    public EmployeesViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [RelayCommand]
    public async Task LoadEmployeesAsync()
    {
        var employeesFromDb = await _employeeService.GetAllEmployeesAsync();
        Employees.Clear();
        foreach (var emp in employeesFromDb)
        {
            Employees.Add(emp);
        }
    }

    [RelayCommand]
    public async Task SaveEmployeeAsync()
    {
        try
        {
            if (IsEditing && _employeeBeingEdited != null)
            {
                _employeeBeingEdited.Name = Name;
                _employeeBeingEdited.Surname = Surname;
                _employeeBeingEdited.PhoneNumber = PhoneNumber;

                await _employeeService.UpdateEmployeeAsync(_employeeBeingEdited);

                var index = Employees.IndexOf(_employeeBeingEdited);
                if (index >= 0)
                {
                    Employees.RemoveAt(index);
                    Employees.Insert(index, _employeeBeingEdited);
                }
            }
            else
            {
                var newEmployee = new Employee
                {
                    Name = Name,
                    Surname = Surname,
                    PhoneNumber = PhoneNumber
                };

                await _employeeService.AddEmployeeAsync(newEmployee);

                Employees.Add(newEmployee);
            }

            ClearForm();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd zapisu: {ex.Message}");
        }
    }

    [RelayCommand]
    public void EditEmployee(Employee employee)
    {
        _employeeBeingEdited = employee;
        Name = employee.Name;
        Surname = employee.Surname;
        PhoneNumber = employee.PhoneNumber;
        IsEditing = true;
    }

    [RelayCommand]
    public async Task DeleteEmployeeAsync(Employee employee)
    {
        await _employeeService.DeleteEmployeeAsync(employee);
        await LoadEmployeesAsync();
    }

    [RelayCommand]
    public void ClearForm()
    {
        Name = string.Empty;
        Surname = string.Empty;
        PhoneNumber = string.Empty;
        IsEditing = false;
        _employeeBeingEdited = null;
    }
}