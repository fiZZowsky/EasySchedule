using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Application.Interfaces.Services;
using EasySchedule.Application.Rules;
using EasySchedule.Application.Validators;
using EasySchedule.Domain.Entities;
using EasySchedule.Infrastructure.Persistence;
using EasySchedule.Infrastructure.Repositories;
using EasySchedule.Infrastructure.Services;
using EasySchedule.UI.ViewModels;
using EasySchedule.UI.Views;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EasySchedule.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "EasySchedule.db3");
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Filename={dbPath}"));

            // DI registration
            // Repositories
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IProfessionRepository, ProfessionRepository>();
            builder.Services.AddScoped<ITimeOffRepository, TimeOffRepository>();
            builder.Services.AddScoped<IShiftTypeRepository, ShiftTypeRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IShiftAssignmentRepository, ShiftAssignmentRepository>();
            builder.Services.AddScoped<IShiftRequirementRepository, ShiftRequirementRepository>();

            // Services
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IProfessionService, ProfessionService>();
            builder.Services.AddScoped<ITimeOffService, TimeOffService>();
            builder.Services.AddScoped<IShiftTypeService, ShiftTypeService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IShiftAssignmentService, ShiftAssignmentService>();
            builder.Services.AddScoped<IScheduleGeneratorService, ScheduleGeneratorService>();
            builder.Services.AddScoped<IShiftRequirementService, ShiftRequirementService>();
            builder.Services.AddScoped<IPdfExportService, PdfExportService>();

            // Validators
            builder.Services.AddTransient<IValidator<Employee>, EmployeeValidator>();
            builder.Services.AddTransient<IValidator<Profession>, ProfessionValidator>();
            builder.Services.AddTransient<IValidator<TimeOff>, TimeOffValidator>();
            builder.Services.AddTransient<IValidator<ShiftType>, ShiftTypeValidator>();
            builder.Services.AddTransient<IValidator<Schedule>, ScheduleValidator>();
            builder.Services.AddTransient<IValidator<ShiftAssignment>, ShiftAssignmentValidator>();

            // Rules
            builder.Services.AddTransient<IScheduleRule, TimeOffRule>();
            builder.Services.AddTransient<IScheduleRule, MinRestHoursRule>();
            builder.Services.AddTransient<IScheduleRule, MaxConsecutiveDaysRule>();
            builder.Services.AddTransient<IScheduleRule, NightShiftRule>();

            // ViewModels
            builder.Services.AddTransient<ProfessionsViewModel>();
            builder.Services.AddTransient<ShiftTypesViewModel>();

            // Views
            builder.Services.AddTransient<ProfessionsPage>();
            builder.Services.AddTransient<ShiftTypesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
