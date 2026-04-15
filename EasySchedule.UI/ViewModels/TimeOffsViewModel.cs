using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.UI.ViewModels;

[QueryProperty(nameof(CurrentEmployee), "Employee")]
public partial class TimeOffsViewModel : BaseViewModel
{
    private readonly ITimeOffService _timeOffService;

    [ObservableProperty]
    private Employee _currentEmployee;

    public ObservableCollection<TimeOff> TimeOffs { get; } = new();

    [ObservableProperty] private DateTime _newStartDate = DateTime.Today;
    [ObservableProperty] private DateTime _newEndDate = DateTime.Today.AddDays(7);
    [ObservableProperty] private TimeOffType _selectedTimeOffType = TimeOffType.Vacation;

    public List<TimeOffType> TimeOffTypes { get; } = Enum.GetValues(typeof(TimeOffType)).Cast<TimeOffType>().ToList();

    public TimeOffsViewModel(ITimeOffService timeOffService)
    {
        _timeOffService = timeOffService;
    }

    partial void OnCurrentEmployeeChanged(Employee value)
    {
        if (value != null)
        {
            Title = $"Urlopy: {value.Name} {value.Surname}";
            LoadTimeOffsCommand.Execute(null);
        }
    }

    [RelayCommand]
    public async Task LoadTimeOffsAsync()
    {
        if (CurrentEmployee == null || IsBusy) return;

        try
        {
            IsBusy = true;
            var result = await _timeOffService.GetTimeOffsForEmployeeAsync(CurrentEmployee.Id);

            if (result.IsSuccess)
            {
                TimeOffs.Clear();
                foreach (var t in result.Value.OrderBy(x => x.StartDate))
                {
                    TimeOffs.Add(t);
                }
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
        var startDate = DateOnly.FromDateTime(NewStartDate);
        var endDate = DateOnly.FromDateTime(NewEndDate);

        if (endDate < startDate)
        {
            await Shell.Current.DisplayAlertAsync("Błąd", "Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.", "OK");
            return;
        }

        var newTimeOff = new TimeOff(CurrentEmployee.Id, startDate, endDate, SelectedTimeOffType);
        var result = await _timeOffService.AddTimeOffAsync(newTimeOff);

        if (result.IsSuccess)
        {
            NewStartDate = DateTime.Today;
            NewEndDate = DateTime.Today.AddDays(7);

            await LoadTimeOffsAsync();
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
        if (result.IsSuccess) TimeOffs.Remove(timeOff);
    }
}