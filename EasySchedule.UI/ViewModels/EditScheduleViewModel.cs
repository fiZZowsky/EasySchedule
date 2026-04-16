using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.ViewModels;

[QueryProperty(nameof(CurrentSchedule), "Schedule")]
[QueryProperty(nameof(InitialAssignments), "Assignments")]
[QueryProperty(nameof(ApplyCallback), "ApplyCallback")]
public partial class EditScheduleViewModel : BaseViewModel
{
    private readonly IEmployeeService _employeeService;
    private readonly IShiftTypeService _shiftTypeService;

    [ObservableProperty] private Schedule _currentSchedule;
    [ObservableProperty] private Action<List<ShiftAssignment>> _applyCallback;

    [ObservableProperty] private List<ShiftAssignment> _initialAssignments;

    private List<ShiftAssignment> _allAssignments = new();

    [ObservableProperty] private DateTime _selectedDate;
    public ObservableCollection<ShiftAssignment> DailyAssignments { get; } = new();

    public ObservableCollection<Employee> AvailableEmployees { get; } = new();
    public ObservableCollection<ShiftType> AvailableShiftTypes { get; } = new();

    [ObservableProperty] private Employee? _selectedEmployee;
    [ObservableProperty] private ShiftType? _selectedShiftType;

    public EditScheduleViewModel(IEmployeeService employeeService, IShiftTypeService shiftTypeService)
    {
        _employeeService = employeeService;
        _shiftTypeService = shiftTypeService;
        Title = "Edycja Grafiku";

        LoadDataCommand.Execute(null);
    }

    partial void OnInitialAssignmentsChanged(List<ShiftAssignment> value)
    {
        if (value != null)
        {
            _allAssignments = value;

            if (CurrentSchedule != null)
                SelectedDate = CurrentSchedule.StartDate.ToDateTime(TimeOnly.MinValue);
            else
                SelectedDate = DateTime.Today;

            RefreshDailyAssignments();
        }
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        var empResult = await _employeeService.GetAllEmployeesAsync();
        if (empResult.IsSuccess)
        {
            AvailableEmployees.Clear();
            foreach (var e in empResult.Value) AvailableEmployees.Add(e);
        }

        var shiftResult = await _shiftTypeService.GetAllShiftTypesAsync();
        if (shiftResult.IsSuccess)
        {
            AvailableShiftTypes.Clear();
            foreach (var st in shiftResult.Value) AvailableShiftTypes.Add(st);
        }

        RefreshDailyAssignments();
    }

    [RelayCommand]
    public void NextDay()
    {
        var maxDate = CurrentSchedule?.EndDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MaxValue;
        if (SelectedDate < maxDate)
        {
            SelectedDate = SelectedDate.AddDays(1);
            RefreshDailyAssignments();
        }
    }

    [RelayCommand]
    public void PreviousDay()
    {
        var minDate = CurrentSchedule?.StartDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue;
        if (SelectedDate > minDate)
        {
            SelectedDate = SelectedDate.AddDays(-1);
            RefreshDailyAssignments();
        }
    }

    private void RefreshDailyAssignments()
    {
        DailyAssignments.Clear();
        var dateOnly = DateOnly.FromDateTime(SelectedDate);

        var dayAssignments = _allAssignments.Where(a => a.Date == dateOnly).ToList();

        foreach (var a in dayAssignments)
        {
            if (a.Employee == null && AvailableEmployees.Any())
            {
                var emp = AvailableEmployees.FirstOrDefault(e => e.Id == a.EmployeeId);
                var st = AvailableShiftTypes.FirstOrDefault(s => s.Id == a.ShiftTypeId);
                if (emp != null && st != null)
                    a.SetReferencesForPreview(emp, st);
            }

            DailyAssignments.Add(a);
        }
    }

    [RelayCommand]
    public void RemoveAssignment(ShiftAssignment assignment)
    {
        if (assignment != null)
        {
            _allAssignments.Remove(assignment);
            RefreshDailyAssignments();
        }
    }

    [RelayCommand]
    public void AddAssignment()
    {
        if (SelectedEmployee == null || SelectedShiftType == null || CurrentSchedule == null)
        {
            Shell.Current.DisplayAlert("Błąd", "Wybierz pracownika i zmianę.", "OK");
            return;
        }

        var dateOnly = DateOnly.FromDateTime(SelectedDate);

        if (_allAssignments.Any(a => a.EmployeeId == SelectedEmployee.Id && a.Date == dateOnly))
        {
            Shell.Current.DisplayAlert("Błąd", "Ten pracownik ma już przypisaną zmianę tego dnia.", "OK");
            return;
        }

        var newAssignment = new ShiftAssignment(CurrentSchedule.Id, SelectedEmployee.Id, SelectedShiftType.Id, dateOnly);
        newAssignment.SetReferencesForPreview(SelectedEmployee, SelectedShiftType);

        _allAssignments.Add(newAssignment);
        RefreshDailyAssignments();

        SelectedEmployee = null;
        SelectedShiftType = null;
    }

    [RelayCommand]
    public async Task ApplyChangesAsync()
    {
        ApplyCallback?.Invoke(_allAssignments);
        await Shell.Current.GoToAsync("..");
    }
}