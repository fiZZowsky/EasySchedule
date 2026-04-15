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
    [ObservableProperty] private ObservableCollection<ShiftRequirementViewModel> _staffingRequirements = new();

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

        IsBusy = true;
        try
        {
            var shiftTypesResult = await _shiftTypeService.GetAllShiftTypesAsync();
            var reqResult = await _requirementService.GetRequirementsForScheduleAsync(CurrentSchedule.Id);

            if (shiftTypesResult.IsSuccess && reqResult.IsSuccess)
            {
                StaffingRequirements.Clear();
                foreach (var type in shiftTypesResult.Value)
                {
                    var existing = reqResult.Value.FirstOrDefault(r => r.ShiftTypeId == type.Id && r.SpecificDate == null);

                    StaffingRequirements.Add(new ShiftRequirementViewModel
                    {
                        ShiftTypeId = type.Id,
                        ShiftName = type.Name,
                        Count = existing?.RequiredEmployeeCount ?? 1
                    });
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task RunGeneratorAsync()
    {
        if (CurrentSchedule == null) return;

        IsBusy = true;
        ProposedAssignments.Clear();

        var settings = new ScheduleSettings(MaxConsecutiveDays, MinRestHours, MaxShiftsPerWeek);

        var result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Low);

        if (result.IsFailed)
        {
            bool relaxToMedium = await Shell.Current.DisplayAlertAsync(
                "Generator utknął (Poziom Łagodny)",
                $"{result.Errors.First().Message}\n\nCzy chcesz zignorować zasady o niskim priorytecie, aby spróbować ułożyć grafik?",
                "Tak, nagnij zasady", "Anuluj");

            if (relaxToMedium)
            {
                result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Medium);

                if (result.IsFailed)
                {
                    bool relaxToHigh = await Shell.Current.DisplayAlertAsync(
                        "Generator utknął (Poziom Średni)",
                        $"{result.Errors.First().Message}\n\nCzy chcesz zignorować średnie reguły (np. ciągi dni), byle obsadzić zmiany?",
                        "Tak, wymuś", "Anuluj");

                    if (relaxToHigh)
                    {
                        result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.High);

                        if (result.IsFailed)
                        {
                            await Shell.Current.DisplayAlertAsync(
                                "Ostateczny błąd",
                                $"Nawet przy nagięciu zasad nie można wygenerować grafiku.\n\nPowód: {result.Errors.First().Message}\n\nZbyt mało pracowników lub zbyt wiele urlopów.",
                                "Rozumiem");
                        }
                    }
                }
            }
        }

        if (result.IsSuccess)
        {
            foreach (var a in result.Value) ProposedAssignments.Add(a);
            await Shell.Current.DisplayAlertAsync("Sukces", "Udało się ułożyć propozycję grafiku!", "OK");
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

    [RelayCommand]
    public async Task SaveRequirementsAsync()
    {
        foreach (var req in StaffingRequirements)
        {
            await _requirementService.SetDefaultRequirementAsync(CurrentSchedule.Id, req.ShiftTypeId, req.Count);
        }
        await Shell.Current.DisplayAlertAsync("Sukces", "Zapotrzebowanie zostało zapisane.", "OK");
    }

    public partial class ShiftRequirementViewModel : ObservableObject
    {
        public int ShiftTypeId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        [ObservableProperty] private int _count;
    }
}