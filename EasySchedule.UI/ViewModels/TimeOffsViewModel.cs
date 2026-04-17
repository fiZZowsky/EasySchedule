using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using System.Collections.ObjectModel;

namespace EasySchedule.UI.ViewModels;

public partial class TimeOffsViewModel : BaseViewModel
{
    private readonly ITimeOffService _timeOffService;
    private readonly IEmployeeService _employeeService;

    public ObservableCollection<TimeOff> TimeOffs { get; } = new();
    public ObservableCollection<Employee> Employees { get; } = new();
    public List<TimeOffType> TimeOffTypes { get; } = Enum.GetValues(typeof(TimeOffType)).Cast<TimeOffType>().ToList();

    [ObservableProperty] private Employee? _selectedEmployee;
    [ObservableProperty] private TimeOffType _selectedType;
    [ObservableProperty] private DateTime _newStartDate = DateTime.Today;
    [ObservableProperty] private DateTime _newEndDate = DateTime.Today;

    [ObservableProperty] private bool _hasTimeOffs;

    public TimeOffsViewModel(ITimeOffService timeOffService, IEmployeeService employeeService)
    {
        _timeOffService = timeOffService;
        _employeeService = employeeService;
        Title = "Urlopy i Zwolnienia";
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            var empResult = await _employeeService.GetAllEmployeesAsync();
            if (empResult.IsSuccess)
            {
                Employees.Clear();
                foreach (var e in empResult.Value) Employees.Add(e);
            }

            var toResult = await _timeOffService.GetAllTimeOffsAsync();
            if (toResult.IsSuccess)
            {
                TimeOffs.Clear();
                foreach (var to in toResult.Value) TimeOffs.Add(to);

                HasTimeOffs = TimeOffs.Any();
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddTimeOffAsync()
    {
        if (SelectedEmployee == null) return;

        var timeOff = new TimeOff(
            SelectedEmployee.Id,
            DateOnly.FromDateTime(NewStartDate),
            DateOnly.FromDateTime(NewEndDate),
            SelectedType
        );

        var result = await _timeOffService.AddTimeOffAsync(timeOff);

        if (result.IsSuccess)
        {
            SelectedEmployee = null;
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Błąd", result.Errors.First().Message, "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteTimeOffAsync(TimeOff timeOff)
    {
        if (timeOff == null) return;
        var result = await _timeOffService.DeleteTimeOffAsync(timeOff.Id);
        if (result.IsSuccess)
        {
            await LoadDataAsync();
        }
    }

    [RelayCommand]
    public async Task DeleteAllTimeOffsAsync()
    {
        bool confirm = await Shell.Current.DisplayAlertAsync("Usuń wszystko", "Czy na pewno chcesz usunąć WSZYSTKIE wpisy urlopów i zwolnień? Tej operacji nie można cofnąć.", "Tak, usuń", "Anuluj");

        if (!confirm) return;

        var result = await _timeOffService.DeleteAllTimeOffsAsync();
        if (result.IsSuccess)
        {
            await LoadDataAsync();
        }
    }
}