using EasySchedule.Application.Interfaces.Repositories;
using EasySchedule.Application.Interfaces.Services;
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

            // DI registration for repositories, services, viewmodels and pages
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IProfessionRepository, ProfessionRepository>();

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IProfessionService, ProfessionService>();

            builder.Services.AddTransient<IValidator<Employee>, EmployeeValidator>();
            builder.Services.AddTransient<IValidator<Profession>, ProfessionValidator>();

            builder.Services.AddTransient<EmployeesViewModel>();

            builder.Services.AddTransient<EmployeesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
