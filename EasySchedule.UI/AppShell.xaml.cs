using EasySchedule.UI.Views;

namespace EasySchedule.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            _ = new MauiIcons.Core.MauiIcon();

            InitializeComponent();

            Routing.RegisterRoute(nameof(EasySchedule.UI.Views.TimeOffsPage), typeof(EasySchedule.UI.Views.TimeOffsPage));
            Routing.RegisterRoute(nameof(GeneratorPage), typeof(GeneratorPage));
            Routing.RegisterRoute(nameof(EditSchedulePage), typeof(EditSchedulePage));
        }
    }
}
