using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.ViewModels;

public partial class SchedulesViewModel : BaseViewModel
{
    private readonly IScheduleService _scheduleService;
    private readonly IProfessionService _professionService;

    public ObservableCollection<Schedule> Schedules { get; } = new();
    public ObservableCollection<Profession> Professions { get; } = new();

    [ObservableProperty] private string _newName = string.Empty;

    [ObservableProperty] private DateTime _newStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
    [ObservableProperty] private DateTime _newEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(2).AddDays(-1);

    [ObservableProperty] private Profession? _selectedProfession;

    public SchedulesViewModel(IScheduleService scheduleService, IProfessionService professionService)
    {
        _scheduleService = scheduleService;
        _professionService = professionService;
        Title = "Zarządzanie Grafikami";
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

            var schedResult = await _scheduleService.GetAllSchedulesAsync();
            if (schedResult.IsSuccess)
            {
                Schedules.Clear();
                foreach (var s in schedResult.Value.OrderByDescending(x => x.StartDate))
                    Schedules.Add(s);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddScheduleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName))
        {
            await Shell.Current.DisplayAlert("Błąd", "Podaj nazwę grafiku.", "OK");
            return;
        }

        if (SelectedProfession == null)
        {
            await Shell.Current.DisplayAlert("Błąd", "Wybierz zawód dla tego grafiku.", "OK");
            return;
        }

        var startDate = DateOnly.FromDateTime(NewStartDate);
        var endDate = DateOnly.FromDateTime(NewEndDate);

        if (endDate < startDate)
        {
            await Shell.Current.DisplayAlert("Błąd", "Data zakończenia nie może być przed datą rozpoczęcia.", "OK");
            return;
        }

        var newSchedule = new Schedule(NewName, startDate, endDate, SelectedProfession.Id);
        var result = await _scheduleService.CreateScheduleAsync(newSchedule);

        if (result.IsSuccess)
        {
            NewName = string.Empty;
            SelectedProfession = null;
            await LoadDataAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", result.Errors.First().Message, "OK");
        }
    }

    [RelayCommand]
    public async Task OpenGeneratorAsync(Schedule schedule)
    {
        if (schedule == null) return;

        await Shell.Current.DisplayAlert("Info", $"Tutaj wkrótce otworzymy generator dla grafiku: {schedule.Name}", "OK");
    }
}