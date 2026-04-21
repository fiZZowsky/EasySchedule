using EasySchedule.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EasySchedule.UI
{
    public partial class App : IApplication
    {
        public App(AppDbContext dbContext)
        {
            InitializeComponent();

            this.UserAppTheme = AppTheme.Light;

            dbContext.Database.Migrate();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}