using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.ViewModels;

public partial class ProfessionsViewModel : BaseViewModel
{
    private readonly IProfessionService _professionService;

    public ObservableCollection<Profession> Professions { get; } = new();

    [ObservableProperty] private string _newProfessionName = string.Empty;
    [ObservableProperty] private bool _newProfessionCanWorkNights = true;

    public ProfessionsViewModel(IProfessionService professionService)
    {
        _professionService = professionService;
        Title = "Zawody i Stanowiska";
    }

    [RelayCommand]
    public async Task LoadProfessionsAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var result = await _professionService.GetAllProfessionsAsync();

            if (result.IsSuccess)
            {
                Professions.Clear();
                foreach (var profession in result.Value)
                {
                    Professions.Add(profession);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Błąd", result.Errors.FirstOrDefault()?.Message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task AddProfessionAsync()
    {
        if (string.IsNullOrWhiteSpace(NewProfessionName))
        {
            await Shell.Current.DisplayAlert("Uwaga", "Podaj nazwę zawodu.", "OK");
            return;
        }

        var newProfession = new Profession(NewProfessionName, "", NewProfessionCanWorkNights);
        var result = await _professionService.AddProfessionAsync(newProfession);

        if (result.IsSuccess)
        {
            NewProfessionName = string.Empty;
            NewProfessionCanWorkNights = true;

            await LoadProfessionsAsync();
        }
        else
        {
            var error = result.Errors.FirstOrDefault()?.Message ?? "Nieznany błąd.";
            await Shell.Current.DisplayAlert("Błąd dodawania", error, "OK");
        }
    }

    [RelayCommand]
    public async Task DeleteProfessionAsync(Profession profession)
    {
        if (profession == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć zawód: {profession.Name}?", "Tak", "Nie");
        if (!confirm) return;

        var result = await _professionService.DeleteProfessionAsync(profession.Id);
        if (result.IsSuccess)
        {
            Professions.Remove(profession);
        }
        else
        {
            await Shell.Current.DisplayAlert("Nie można usunąć", result.Errors.FirstOrDefault()?.Message, "OK");
        }
    }
}