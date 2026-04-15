using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.ViewModels;

public partial class EmployeesViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IProfessionService _professionService;

    public ObservableCollection<Employee> Employees { get; } = new();
    public ObservableCollection<Profession> Professions { get; } = new();

    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newSurname = string.Empty;
    [ObservableProperty] private string _newPhoneNumber = string.Empty;
    [ObservableProperty] private Profession? _selectedProfession;

    public EmployeesViewModel(IEmployeeService employeeService, IProfessionService professionService)
    {
        _employeeService = employeeService;
        _professionService = professionService;
        Title = "Personel";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            var profResult = await _professionService.GetAllProfessionsAsync();
            if (profResult.IsSuccess)
            {
                Professions.Clear();
                foreach (var p in profResult.Value) Professions.Add(p);
            }

            var empResult = await _employeeService.GetAllEmployeesAsync();
            if (empResult.IsSuccess)
            {
                Employees.Clear();
                foreach (var e in empResult.Value) Employees.Add(e);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewSurname))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj imię i nazwisko pracownika.", "OK");
            return;
        }

        if (SelectedProfession == null)
        {
            await Shell.Current.DisplayAlert("Błąd", "Wybierz zawód pracownika.", "OK");
            return;
        }

        var newEmployee = new Employee(NewName, NewSurname, NewPhoneNumber, SelectedProfession.Id);
        var result = await _employeeService.AddEmployeeAsync(newEmployee);

        if (result.IsSuccess)
        {
            NewName = string.Empty;
            NewSurname = string.Empty;
            NewPhoneNumber = string.Empty;
            SelectedProfession = null;
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", result.Errors.First().Message, "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteEmployeeAsync(Employee employee)
    {
        if (employee == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", $"Usunąć pracownika {employee.Name} {employee.Surname}?", "Tak", "Nie");
        if (!confirm) return;

        var result = await _employeeService.DeleteEmployeeAsync(employee.Id);
        if (result.IsSuccess)
        {
            Employees.Remove(employee);
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", result.Errors.FirstOrDefault()?.Message, "OK");
        }
    }

    [RelayCommand]
    public async Task NavigateToTimeOffsAsync(Employee employee)
    {
        if (employee == null) return;

        var navigationParameter = new Dictionary<string, object>
    {
        { "Employee", employee }
    };

        await Shell.Current.GoToAsync(nameof(EasySchedule.UI.Views.TimeOffsPage), navigationParameter);
    }
}