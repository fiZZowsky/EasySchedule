using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.UI.ViewModels;

[QueryProperty(nameof(CurrentSchedule), "Schedule")]
public partial class GeneratorViewModel : BaseViewModel
{
    private readonly IScheduleGeneratorService _generatorService;
    private readonly IShiftRequirementService _requirementService;
    private readonly IShiftTypeService _shiftTypeService;
    private readonly IShiftAssignmentService _assignmentService;
    private readonly IPdfExportService _pdfExportService;

    [ObservableProperty] private Schedule _currentSchedule;

    [ObservableProperty] private int _maxConsecutiveDays = 5;
    [ObservableProperty] private int _minRestHours = 11;
    [ObservableProperty] private int _maxShiftsPerWeek = 5;

    public ObservableCollection<ShiftAssignment> ProposedAssignments { get; } = new();
    public ObservableCollection<ShiftRequirement> CurrentRequirements { get; } = new();

    public GeneratorViewModel(
        IScheduleGeneratorService generatorService,
        IShiftRequirementService requirementService,
        IShiftTypeService shiftTypeService,
        IShiftAssignmentService assignmentService,
        IPdfExportService pdfExportService)
    {
        _generatorService = generatorService;
        _requirementService = requirementService;
        _shiftTypeService = shiftTypeService;
        _assignmentService = assignmentService;
        _pdfExportService = pdfExportService;
    }

    partial void OnCurrentScheduleChanged(Schedule value)
    {
        if (value != null)
        {
            Title = $"Generator: {value.Name}";
            LoadDataCommand.Execute(null);
        }
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        if (CurrentSchedule == null) return;

        var reqResult = await _requirementService.GetRequirementsForScheduleAsync(CurrentSchedule.Id);
        if (reqResult.IsSuccess)
        {
            CurrentRequirements.Clear();
            foreach (var req in reqResult.Value) CurrentRequirements.Add(req);
        }
    }

    [RelayCommand]
    public async Task RunGeneratorAsync()
    {
        IsBusy = true;
        var settings = new ScheduleSettings(MaxConsecutiveDays, MinRestHours, MaxShiftsPerWeek);

        var result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Low);

        if (result.IsFailed)
        {
            bool relax = await Shell.Current.DisplayAlertAsync("Generator utknął",
                $"{result.Errors.First().Message}\n\nCzy chcesz zignorować miękkie zasady (np. ciągi dni), aby spróbować ułożyć grafik?", "Tak, nagnij zasady", "Nie, zostaw puste");

            if (relax)
            {
                result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Medium);
            }
        }

        if (result.IsSuccess)
        {
            ProposedAssignments.Clear();
            foreach (var a in result.Value) ProposedAssignments.Add(a);
            await Shell.Current.DisplayAlertAsync("Sukces", "Wygenerowano wstępną propozycję grafiku.", "OK");
        }

        IsBusy = false;
    }

    [RelayCommand]
    public async Task SaveScheduleAsync()
    {
        if (!ProposedAssignments.Any()) return;

        IsBusy = true;
        foreach (var assignment in ProposedAssignments)
        {
            await _assignmentService.AssignShiftAsync(assignment);
        }
        IsBusy = false;

        await Shell.Current.DisplayAlertAsync("Zapisano", "Grafik został zapisany w bazie danych.", "OK");
    }

    [RelayCommand]
    public async Task ExportPdfAsync()
    {
        if (CurrentSchedule == null) return;

        var exportResult = await _pdfExportService.ExportScheduleToPdfAsync(CurrentSchedule);
        if (exportResult.IsSuccess)
        {
            await Shell.Current.DisplayAlertAsync("PDF", "Plik PDF został wygenerowany w pamięci.", "OK");
        }
    }
}