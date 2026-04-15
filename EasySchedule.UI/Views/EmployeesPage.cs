using EasySchedule.UI.ViewModels;
using EasySchedule.Domain.Entities;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class EmployeesPage : ContentPage
{
    private readonly EmployeesViewModel _viewModel;

    public EmployeesPage(EmployeesViewModel viewModel)
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
        formStack.Add(new Label { Text = "Nowy pracownik", FontAttributes = FontAttributes.Bold });

        var nameGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10
        };

        var nameEntry = new Entry { Placeholder = "Imię" };
        nameEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.NewName));
        nameGrid.Add(nameEntry, 0);

        var surnameEntry = new Entry { Placeholder = "Nazwisko" };
        surnameEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.NewSurname));
        nameGrid.Add(surnameEntry, 1);

        var phoneNumberEntry = new Entry { Placeholder = "Numer telefonu" };
        phoneNumberEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.NewPhoneNumber));
        nameGrid.Add(phoneNumberEntry, 2);

        var professionPicker = new Picker { Title = "Wybierz zawód", TextColor = Color.FromArgb("#333333") };
        professionPicker.SetBinding(Picker.ItemsSourceProperty, nameof(EmployeesViewModel.Professions));
        professionPicker.ItemDisplayBinding = new Binding(nameof(Profession.Name));
        professionPicker.SetBinding(Picker.SelectedItemProperty, nameof(EmployeesViewModel.SelectedProfession));

        var addBtn = new Button { Text = "Dodaj pracownika", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetBinding(Button.CommandProperty, nameof(EmployeesViewModel.AddEmployeeCommand));

        formStack.Add(nameGrid);
        formStack.Add(professionPicker);
        formStack.Add(addBtn);

        formBorder.Content = formStack;
        mainGrid.Add(formBorder, 0, 0);

        var collectionView = new CollectionView();
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(EmployeesViewModel.Employees));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            nameLabel.SetBinding(Label.TextProperty, new Binding(".", converter: new EmployeeNameConverter()));

            var professionLabel = new Label { FontSize = 12, TextColor = Color.FromArgb("#1976D2"), FontAttributes = FontAttributes.Bold };
            professionLabel.SetBinding(Label.TextProperty, "Profession.Name");

            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center, Children = { nameLabel, professionLabel } };

            var timeOffBtn = new Button { Text = "Urlopy", HeightRequest = 35, BackgroundColor = Color.FromArgb("#F39C12"), TextColor = Colors.White, CornerRadius = 8, Margin = new Thickness(0, 0, 10, 0) };
            timeOffBtn.SetBinding(Button.CommandProperty, new Binding("NavigateToTimeOffsCommand", source: _viewModel));
            timeOffBtn.SetBinding(Button.CommandParameterProperty, ".");

            var deleteBtn = new ImageButton { Source = "dotnet_bot.png", WidthRequest = 24, VerticalOptions = LayoutOptions.Center };
            deleteBtn.SetBinding(ImageButton.CommandProperty, new Binding(nameof(EmployeesViewModel.DeleteEmployeeCommand), source: _viewModel));
            deleteBtn.SetBinding(ImageButton.CommandParameterProperty, ".");

            var itemGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) } };
            itemGrid.Add(infoStack, 0, 0);
            itemGrid.Add(timeOffBtn, 1, 0);
            itemGrid.Add(deleteBtn, 2, 0);

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

public class EmployeeNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is Employee emp)
        {
            return $"{emp.Name} {emp.Surname}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}