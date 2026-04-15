namespace EasySchedule.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(EasySchedule.UI.Views.TimeOffsPage), typeof(EasySchedule.UI.Views.TimeOffsPage));
        }
    }
}
