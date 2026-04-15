using EasySchedule.UI.ViewModels;
using EasySchedule.Domain.Entities;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class SchedulesPage : ContentPage
{
    private readonly SchedulesViewModel _viewModel;

    public SchedulesPage(SchedulesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        BackgroundColor = Color.FromArgb("#F5F5F5");

        BuildUI();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            Padding = 20,
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) }
        };

        var formBorder = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 20),
            Shadow = new Shadow { Opacity = 0.05f, Radius = 10, Offset = new Point(0, 4) }
        };

        var formStack = new VerticalStackLayout { Spacing = 10 };
        formStack.Add(new Label { Text = "Utwórz nowy grafik", FontAttributes = FontAttributes.Bold, FontSize = 16 });

        var nameEntry = new Entry { Placeholder = "Nazwa (np. Grafik Pielęgniarek - Maj)" };
        nameEntry.SetBinding(Entry.TextProperty, nameof(SchedulesViewModel.NewName));

        var dateGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 10 };

        var startPicker = new DatePicker { Format = "dd.MM.yyyy" };
        startPicker.SetBinding(DatePicker.DateProperty, nameof(SchedulesViewModel.NewStartDate));
        dateGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Data rozpoczęcia", FontSize = 12 }, startPicker } }, 0);

        var endPicker = new DatePicker { Format = "dd.MM.yyyy" };
        endPicker.SetBinding(DatePicker.DateProperty, nameof(SchedulesViewModel.NewEndDate));
        dateGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Data zakończenia", FontSize = 12 }, endPicker } }, 1);

        var professionPicker = new Picker { Title = "Wybierz zawód", TextColor = Color.FromArgb("#333333") };
        professionPicker.SetBinding(Picker.ItemsSourceProperty, nameof(SchedulesViewModel.Professions));
        professionPicker.ItemDisplayBinding = new Binding(nameof(Profession.Name));
        professionPicker.SetBinding(Picker.SelectedItemProperty, nameof(SchedulesViewModel.SelectedProfession));

        var addBtn = new Button { Text = "Utwórz szkielet grafiku", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White, CornerRadius = 8, FontAttributes = FontAttributes.Bold };
        addBtn.SetBinding(Button.CommandProperty, nameof(SchedulesViewModel.AddScheduleCommand));

        formStack.Add(nameEntry);
        formStack.Add(dateGrid);
        formStack.Add(professionPicker);
        formStack.Add(addBtn);

        formBorder.Content = formStack;
        mainGrid.Add(formBorder, 0, 0);

        var collectionView = new CollectionView();
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(SchedulesViewModel.Schedules));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            nameLabel.SetBinding(Label.TextProperty, nameof(Schedule.Name));

            var datesLabel = new Label { FontSize = 12, TextColor = Colors.Gray };
            datesLabel.SetBinding(Label.TextProperty, new Binding(".", converter: new ScheduleDatesConverter()));

            var statusLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#E67E22") };
            statusLabel.SetBinding(Label.TextProperty, new Binding("Status", converter: new ScheduleStatusConverter()));

            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Children = { nameLabel, datesLabel, statusLabel } };

            var generatorBtn = new Button { Text = "Otwórz kalendarz", HeightRequest = 35, BackgroundColor = Color.FromArgb("#2ECC71"), TextColor = Colors.White, CornerRadius = 8, FontAttributes = FontAttributes.Bold };
            generatorBtn.SetBinding(Button.CommandProperty, new Binding(nameof(SchedulesViewModel.OpenGeneratorCommand), source: _viewModel));
            generatorBtn.SetBinding(Button.CommandParameterProperty, ".");

            var itemGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
            itemGrid.Add(infoStack, 0, 0);
            itemGrid.Add(generatorBtn, 1, 0);

            var card = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 10),
                Content = itemGrid
            };

            return card;
        });

        mainGrid.Add(collectionView, 0, 1);
        Content = mainGrid;
    }
}

public class ScheduleDatesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is Schedule s) return $"{s.StartDate:dd.MM.yyyy} - {s.EndDate:dd.MM.yyyy}";
        return string.Empty;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}

public class ScheduleStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is EasySchedule.Domain.Enums.ScheduleStatus status)
        {
            return status switch
            {
                EasySchedule.Domain.Enums.ScheduleStatus.Draft => "Szkic (W przygotowaniu)",
                EasySchedule.Domain.Enums.ScheduleStatus.Published => "Opublikowany",
                EasySchedule.Domain.Enums.ScheduleStatus.Archived => "Archiwalny",
                _ => status.ToString()
            };
        }
        return string.Empty;
    }
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}