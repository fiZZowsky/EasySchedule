using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.ViewModels;

public partial class ShiftTypesViewModel : BaseViewModel
{
    private readonly IShiftTypeService _shiftTypeService;

    public ObservableCollection<ShiftType> ShiftTypes { get; } = new();

    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private string _newShortName = string.Empty;
    [ObservableProperty] private TimeSpan _newStartTime = new TimeSpan(7, 0, 0);
    [ObservableProperty] private TimeSpan _newEndTime = new TimeSpan(15, 0, 0); 
    [ObservableProperty] private bool _newIsNightShift;

    public ShiftTypesViewModel(IShiftTypeService shiftTypeService)
    {
        _shiftTypeService = shiftTypeService;
        Title = "Rodzaje Zmian";
    }

    [RelayCommand]
    public async Task LoadShiftTypesAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var result = await _shiftTypeService.GetAllShiftTypesAsync();
            if (result.IsSuccess)
            {
                ShiftTypes.Clear();
                foreach (var st in result.Value) ShiftTypes.Add(st);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddShiftTypeAsync()
    {
        if (string.IsNullOrWhiteSpace(NewName) || string.IsNullOrWhiteSpace(NewShortName))
        {
            await Shell.Current.DisplayAlert("Błąd", "Wypełnij nazwę i skrót.", "OK");
            return;
        }

        var startTime = TimeOnly.FromTimeSpan(NewStartTime);
        var endTime = TimeOnly.FromTimeSpan(NewEndTime);

        var newShift = new ShiftType(NewName, NewShortName, startTime, endTime, NewIsNightShift);
        var result = await _shiftTypeService.AddShiftTypeAsync(newShift);

        if (result.IsSuccess)
        {
            NewName = string.Empty;
            NewShortName = string.Empty;
            await LoadShiftTypesAsync();
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd", result.Errors.First().Message, "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteShiftTypeAsync(ShiftType shiftType)
    {
        if (shiftType == null) return;
        var result = await _shiftTypeService.DeleteShiftTypeAsync(shiftType.Id);
        if (result.IsSuccess) ShiftTypes.Remove(shiftType);
    }
}