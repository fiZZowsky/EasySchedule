using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using FluentResults;

namespace EasySchedule.Infrastructure.Services;

public class ScheduleGeneratorService : IScheduleGeneratorService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IShiftTypeRepository _shiftTypeRepository;
    private readonly ITimeOffRepository _timeOffRepository;
    private readonly IShiftRequirementRepository _shiftRequirementRepository;
    private readonly IEnumerable<IScheduleRule> _rules;

    public ScheduleGeneratorService(
        IScheduleRepository scheduleRepository,
        IEmployeeRepository employeeRepository,
        IShiftTypeRepository shiftTypeRepository,
        ITimeOffRepository timeOffRepository,
        IShiftRequirementRepository shiftRequirementRepository,
        IEnumerable<IScheduleRule> rules)
    {
        _scheduleRepository = scheduleRepository;
        _employeeRepository = employeeRepository;
        _shiftTypeRepository = shiftTypeRepository;
        _timeOffRepository = timeOffRepository;
        _shiftRequirementRepository = shiftRequirementRepository;
        _rules = rules;
    }

    public async Task<Result<List<ShiftAssignment>>> GenerateProposalAsync(
        int scheduleId,
        ScheduleSettings settings,
        RuleSeverity enforcedSeverity = RuleSeverity.Low)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
        if (schedule == null) return Result.Fail("Grafik nie istnieje.");

        var allEmployees = await _employeeRepository.GetAllAsync();
        var employees = allEmployees.Where(e => e.ProfessionId == schedule.ProfessionId).ToList();
        var shiftTypes = await _shiftTypeRepository.GetAllAsync();

        if (!employees.Any() || !shiftTypes.Any())
            return Result.Fail("Brakuje pracowników lub zdefiniowanych rodzajów zmian w bazie.");

        var scheduleRequirements = await _shiftRequirementRepository.GetByScheduleIdAsync(scheduleId);

        var allTimeOffs = new List<TimeOff>();
        foreach (var emp in employees)
        {
            allTimeOffs.AddRange(await _timeOffRepository.GetByEmployeeIdAsync(emp.Id));
        }

        var proposedAssignments = new List<ShiftAssignment>();
        var activeRules = _rules.Where(r => (int)r.Severity <= (int)enforcedSeverity).ToList();

        for (var date = schedule.StartDate; date <= schedule.EndDate; date = date.AddDays(1))
        {
            foreach (var shift in shiftTypes)
            {
                var requirement = scheduleRequirements.FirstOrDefault(r => r.ShiftTypeId == shift.Id && r.SpecificDate == date)
                               ?? scheduleRequirements.FirstOrDefault(r => r.ShiftTypeId == shift.Id && r.SpecificDate == null);

                int requiredCount = requirement?.RequiredEmployeeCount ?? 1;

                if (requiredCount == 0)
                    continue;

                var availableCandidates = new List<Employee>();

                foreach (var employee in employees)
                {
                    var testAssignment = new ShiftAssignment(schedule.Id, employee.Id, shift.Id, date);

                    testAssignment.SetReferencesForPreview(employee, shift);

                    var employeeCurrentShifts = proposedAssignments.Where(a => a.EmployeeId == employee.Id).ToList();

                    bool isPassed = true;
                    foreach (var rule in activeRules)
                    {
                        if (!rule.IsValid(testAssignment, employeeCurrentShifts, settings, allTimeOffs.Where(t => t.EmployeeId == employee.Id)))
                        {
                            isPassed = false;
                            break;
                        }
                    }

                    if (isPassed)
                    {
                        availableCandidates.Add(employee);
                    }
                }

                if (availableCandidates.Count < requiredCount)
                {
                    return Result.Fail($"Nie udało się obsadzić zmiany '{shift.Name}' w dniu {date}. Potrzeba {requiredCount} pracowników, a reguły spełnia tylko {availableCandidates.Count} (Poziom: {enforcedSeverity}). Spróbuj zrelaksować zasady.");
                }

                var selectedEmployees = availableCandidates
                                    .OrderBy(c => proposedAssignments.Count(a => a.EmployeeId == c.Id))
                                    .ThenBy(c => Guid.NewGuid())
                                    .Take(requiredCount)
                                    .ToList();

                foreach (var selectedEmployee in selectedEmployees)
                {
                    var finalAssignment = new ShiftAssignment(schedule.Id, selectedEmployee.Id, shift.Id, date);

                    finalAssignment.SetReferencesForPreview(selectedEmployee, shift);

                    proposedAssignments.Add(finalAssignment);
                }
            }
        }

        return Result.Ok(proposedAssignments);
    }
}