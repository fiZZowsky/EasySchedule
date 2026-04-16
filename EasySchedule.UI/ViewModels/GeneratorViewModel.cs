using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace EasySchedule.UI.ViewModels;

[QueryProperty(nameof(CurrentSchedule), "Schedule")]
public partial class GeneratorViewModel : BaseViewModel
{
    private readonly IScheduleGeneratorService _generatorService;
    private readonly IShiftRequirementService _requirementService;
    private readonly IShiftTypeService _shiftTypeService;
    private readonly IShiftAssignmentService _assignmentService;
    private readonly IPdfExportService _pdfExportService;
    private readonly IScheduleService _scheduleService;

    [ObservableProperty] private Schedule _currentSchedule;

    [ObservableProperty] private int _maxConsecutiveDays = 5;
    [ObservableProperty] private int _minRestHours = 11;
    [ObservableProperty] private int _maxShiftsPerWeek = 5;

    public ObservableCollection<ShiftAssignment> ProposedAssignments { get; } = new();

    [ObservableProperty] private ObservableCollection<DateOnly> _calendarDays = new();
    [ObservableProperty] private ObservableCollection<CalendarRow> _calendarRows = new();

    [ObservableProperty] private bool _hasGeneratedSchedule;
    [ObservableProperty] private bool _isEditable;
    [ObservableProperty] private bool _isPublished;

    [ObservableProperty] private ObservableCollection<ShiftRequirementViewModel> _staffingRequirements = new();

    public ObservableCollection<ShiftType> AvailableShiftTypes { get; } = new();
    public ObservableCollection<ShiftRequirementOverrideViewModel> OverrideRequirements { get; } = new();

    [ObservableProperty] private DateTime _overrideDate = DateTime.Today;
    [ObservableProperty] private ShiftType? _selectedOverrideShift;
    [ObservableProperty] private int _overrideCount = 1;

    public GeneratorViewModel(
        IScheduleGeneratorService generatorService,
        IShiftRequirementService requirementService,
        IShiftTypeService shiftTypeService,
        IShiftAssignmentService assignmentService,
        IPdfExportService pdfExportService,
        IScheduleService scheduleService)
    {
        _generatorService = generatorService;
        _requirementService = requirementService;
        _shiftTypeService = shiftTypeService;
        _assignmentService = assignmentService;
        _pdfExportService = pdfExportService;
        _scheduleService = scheduleService;
    }

    partial void OnCurrentScheduleChanged(Schedule value)
    {
        if (value != null)
        {
            Title = $"Generator: {value.Name}";
            OverrideDate = value.StartDate.ToDateTime(TimeOnly.MinValue);
            HasGeneratedSchedule = false;
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
            IsEditable = CurrentSchedule.Status == ScheduleStatus.Draft;
            IsPublished = CurrentSchedule.Status == ScheduleStatus.Published;

            var shiftTypesResult = await _shiftTypeService.GetAllShiftTypesAsync();
            var reqResult = await _requirementService.GetRequirementsForScheduleAsync(CurrentSchedule.Id);
            var existingAssignments = await _assignmentService.GetAssignmentsByScheduleIdAsync(CurrentSchedule.Id);

            if (shiftTypesResult.IsSuccess && reqResult.IsSuccess)
            {
                AvailableShiftTypes.Clear();
                foreach (var st in shiftTypesResult.Value) AvailableShiftTypes.Add(st);

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

                OverrideRequirements.Clear();
                var overrides = reqResult.Value.Where(r => r.SpecificDate != null).OrderBy(r => r.SpecificDate);
                foreach (var o in overrides)
                {
                    OverrideRequirements.Add(new ShiftRequirementOverrideViewModel
                    {
                        ShiftTypeId = o.ShiftTypeId,
                        ShiftName = o.ShiftType?.Name ?? "Zmiana",
                        SpecificDate = o.SpecificDate!.Value,
                        Count = o.RequiredEmployeeCount
                    });
                }

                if (existingAssignments.IsSuccess && existingAssignments.Value.Any())
                {
                    ProposedAssignments.Clear();
                    foreach (var a in existingAssignments.Value)
                    {
                        ProposedAssignments.Add(a);
                    }
                    BuildCalendarMatrix(existingAssignments.Value.ToList());
                    HasGeneratedSchedule = true;
                }
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public void AddOverrideLocal()
    {
        if (SelectedOverrideShift == null)
        {
            Shell.Current.DisplayAlert("Błąd", "Wybierz zmianę.", "OK");
            return;
        }

        var dateOnly = DateOnly.FromDateTime(OverrideDate);
        if (dateOnly < CurrentSchedule.StartDate || dateOnly > CurrentSchedule.EndDate)
        {
            Shell.Current.DisplayAlert("Błąd", "Data wyjątku musi zawierać się w ramach czasowych grafiku.", "OK");
            return;
        }

        var existing = OverrideRequirements.FirstOrDefault(r => r.ShiftTypeId == SelectedOverrideShift.Id && r.SpecificDate == dateOnly);
        if (existing != null)
        {
            existing.Count = OverrideCount;
        }
        else
        {
            OverrideRequirements.Add(new ShiftRequirementOverrideViewModel
            {
                ShiftTypeId = SelectedOverrideShift.Id,
                ShiftName = SelectedOverrideShift.Name,
                SpecificDate = dateOnly,
                Count = OverrideCount
            });
        }
    }

    [RelayCommand]
    public void RemoveOverrideLocal(ShiftRequirementOverrideViewModel item)
    {
        if (item != null) OverrideRequirements.Remove(item);
    }

    [RelayCommand]
    public async Task SaveRequirementsAsync()
    {
        foreach (var req in StaffingRequirements)
        {
            await _requirementService.SetDefaultRequirementAsync(CurrentSchedule.Id, req.ShiftTypeId, req.Count);
        }

        foreach (var req in OverrideRequirements)
        {
            await _requirementService.SetOverrideRequirementAsync(CurrentSchedule.Id, req.ShiftTypeId, req.SpecificDate, req.Count);
        }

        await Shell.Current.DisplayAlert("Sukces", "Zapotrzebowanie (domyślne i wyjątki) zostało zapisane.", "OK");
    }

    [RelayCommand]
    public async Task RunGeneratorAsync()
    {
        if (CurrentSchedule == null) return;
        IsBusy = true;

        HasGeneratedSchedule = false;
        ProposedAssignments.Clear();

        var settings = new ScheduleSettings(MaxConsecutiveDays, MinRestHours, MaxShiftsPerWeek);
        var result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Low);

        if (result.IsFailed)
        {
            bool relaxToMedium = await Shell.Current.DisplayAlert("Generator utknął (Poziom Łagodny)",
                $"{result.Errors.First().Message}\n\nCzy zignorować najlżejsze reguły?", "Tak", "Anuluj");

            if (relaxToMedium)
            {
                result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.Medium);
                if (result.IsFailed)
                {
                    bool relaxToHigh = await Shell.Current.DisplayAlert("Generator utknął (Poziom Średni)",
                        $"{result.Errors.First().Message}\n\nCzy zignorować średnie reguły?", "Tak", "Anuluj");

                    if (relaxToHigh)
                    {
                        result = await _generatorService.GenerateProposalAsync(CurrentSchedule.Id, settings, RuleSeverity.High);
                        if (result.IsFailed)
                        {
                            await Shell.Current.DisplayAlert("Błąd",
                                $"Nie można wygenerować grafiku.\n\nPowód: {result.Errors.First().Message}", "Rozumiem");
                        }
                    }
                }
            }
        }

        if (result.IsSuccess)
        {
            foreach (var a in result.Value) ProposedAssignments.Add(a);

            BuildCalendarMatrix(result.Value);
            HasGeneratedSchedule = true;

            await Shell.Current.DisplayAlert("Sukces", "Udało się ułożyć propozycję grafiku!", "OK");
        }
        IsBusy = false;
    }

    private void BuildCalendarMatrix(List<ShiftAssignment> assignments)
    {
        CalendarDays.Clear();
        CalendarRows.Clear();

        if (!assignments.Any()) return;

        var current = CurrentSchedule.StartDate;
        while (current <= CurrentSchedule.EndDate)
        {
            CalendarDays.Add(current);
            current = current.AddDays(1);
        }

        var groupedByEmployee = assignments.GroupBy(a => a.EmployeeId);

        foreach (var group in groupedByEmployee)
        {
            var employeeName = group.First().Employee?.Surname ?? "Nieznany";
            var row = new CalendarRow { EmployeeName = employeeName };

            foreach (var day in CalendarDays)
            {
                var shiftForDay = group.FirstOrDefault(a => a.Date == day);
                if (shiftForDay != null)
                {
                    row.Cells.Add(new CalendarCell
                    {
                        Date = day,
                        ShiftShortName = shiftForDay.ShiftType?.ShortName ?? "?",
                        BackgroundColor = shiftForDay.ShiftType?.IsNightShift == true ? Color.FromArgb("#2C3E50") : Color.FromArgb("#3498DB")
                    });
                }
                else
                {
                    row.Cells.Add(new CalendarCell { Date = day });
                }
            }
            CalendarRows.Add(row);
        }
    }

    [RelayCommand]
    public async Task SaveDraftAsync()
    {
        if (!ProposedAssignments.Any()) return;
        IsBusy = true;

        await _assignmentService.DeleteAssignmentsByScheduleIdAsync(CurrentSchedule.Id);

        foreach (var a in ProposedAssignments)
        {
            var clean = new ShiftAssignment(a.ScheduleId, a.EmployeeId, a.ShiftTypeId, a.Date);
            await _assignmentService.AssignShiftAsync(clean);
        }

        IsBusy = false;
        await Shell.Current.DisplayAlert("Zapisano", "Grafik zapisany jako Szkic (Draft).", "OK");
    }

    [RelayCommand]
    public async Task PublishScheduleAsync()
    {
        var confirm = await Shell.Current.DisplayAlert("Potwierdzenie",
            "Czy na pewno chcesz opublikować grafik? Po tej operacji edycja nie będzie możliwa.", "Tak", "Nie");

        if (!confirm) return;

        IsBusy = true;

        await SaveDraftAsync();

        CurrentSchedule.Status = ScheduleStatus.Published;
        await _scheduleService.UpdateScheduleAsync(CurrentSchedule);

        IsEditable = false;
        IsPublished = true;

        IsBusy = false;
        await Shell.Current.DisplayAlert("Opublikowano", "Grafik został zatwierdzony i opublikowany.", "OK");
    }

    [RelayCommand]
    public async Task ExportPdfAsync()
    {
        if (CurrentSchedule == null) return;

        IsBusy = true;
        var exportResult = await _pdfExportService.ExportScheduleToPdfAsync(CurrentSchedule, ProposedAssignments);
        IsBusy = false;

        if (exportResult.IsSuccess)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Udostępnij {CurrentSchedule.Name}",
                File = new ShareFile(exportResult.Value)
            });
        }
        else
        {
            await Shell.Current.DisplayAlert("Błąd PDF", exportResult.Errors.First().Message, "OK");
        }
    }

    public partial class ShiftRequirementViewModel : ObservableObject
    {
        public int ShiftTypeId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        [ObservableProperty] private int _count;
    }

    public partial class ShiftRequirementOverrideViewModel : ObservableObject
    {
        public int ShiftTypeId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateOnly SpecificDate { get; set; }
        [ObservableProperty] private int _count;
    }

    public class CalendarCell
    {
        public DateOnly Date { get; set; }
        public string ShiftShortName { get; set; } = string.Empty;
        public Color BackgroundColor { get; set; } = Colors.Transparent;
        public bool HasShift => !string.IsNullOrEmpty(ShiftShortName);
    }

    public class CalendarRow
    {
        public string EmployeeName { get; set; } = string.Empty;
        public List<CalendarCell> Cells { get; set; } = new();
    }
}