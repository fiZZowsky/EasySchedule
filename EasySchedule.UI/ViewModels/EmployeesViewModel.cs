using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using System.Collections.ObjectModel;

namespace EasySchedule.UI.ViewModels;

public partial class EmployeesViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IProfessionService _professionService;

    private List<Employee> _allEmployees = new();

    public ObservableCollection<Employee> Employees { get; } = new();
    public ObservableCollection<Profession> Professions { get; } = new();

    public ObservableCollection<Profession> FilterProfessions { get; } = new();

    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newSurname = string.Empty;
    [ObservableProperty] private string _newPhoneNumber = string.Empty;
    [ObservableProperty] private Profession? _selectedProfession;

    [ObservableProperty] private Profession? _selectedFilterProfession;

    private readonly Profession _allProfessionPlaceholder = new Profession("Wszystkie");

    public EmployeesViewModel(IEmployeeService employeeService, IProfessionService professionService)
    {
        _employeeService = employeeService;
        _professionService = professionService;
        Title = "Pracownicy";
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
                FilterProfessions.Clear();

                FilterProfessions.Add(_allProfessionPlaceholder);

                foreach (var p in profResult.Value)
                {
                    Professions.Add(p);
                    FilterProfessions.Add(p);
                }

                SelectedFilterProfession = _allProfessionPlaceholder;
            }

            var empResult = await _employeeService.GetAllEmployeesAsync();
            if (empResult.IsSuccess)
            {
                _allEmployees = empResult.Value.ToList();
                ApplyFilter();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnSelectedFilterProfessionChanged(Profession? value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Employees.Clear();

        var filtered = (SelectedFilterProfession == null || SelectedFilterProfession == _allProfessionPlaceholder)
                    ? _allEmployees
                    : _allEmployees.Where(e => e.ProfessionId == SelectedFilterProfession.Id);

        foreach (var emp in filtered)
        {
            Employees.Add(emp);
        }
    }

    [RelayCommand]
    public async Task AddEmployeeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewSurname) || string.IsNullOrWhiteSpace(NewPhoneNumber) || SelectedProfession == null)
        {
            await Shell.Current.DisplayAlertAsync("Błąd", "Uzupełnij wszystkie dane pracownika.", "OK");
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
            await Shell.Current.DisplayAlertAsync("Błąd", result.Errors.First().Message, "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteEmployeeAsync(Employee employee)
    {
        if (employee == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Potwierdzenie", $"Usunąć pracownika {employee.Name} {employee.Surname}?", "Tak", "Nie");
        if (!confirm) return;

        var result = await _employeeService.DeleteEmployeeAsync(employee.Id);
        if (result.IsSuccess)
        {
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Błąd", result.Errors.FirstOrDefault()?.Message, "OK");
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