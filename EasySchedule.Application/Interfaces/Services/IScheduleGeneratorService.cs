using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;
using FluentResults;

namespace EasySchedule.Application.Interfaces.Services;

public interface IScheduleGeneratorService
{
    Task<Result<List<ShiftAssignment>>> GenerateProposalAsync(
        int scheduleId,
        ScheduleSettings settings,
        RuleSeverity enforcedSeverity = RuleSeverity.Low);
}