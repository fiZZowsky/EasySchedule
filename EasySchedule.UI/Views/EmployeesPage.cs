using System.Globalization;
using EasySchedule.UI.ViewModels;
using EasySchedule.Domain;

namespace EasySchedule.UI.Views;

public class EmployeesPage : ContentPage
{
    private readonly EmployeesViewModel _viewModel;

    public EmployeesPage(EmployeesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        Title = "Zarządzaj Pracownikami";

        BuildUI();
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        var formLayout = new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 10,
            BackgroundColor = Color.FromArgb("#F5F5F5")
        };

        var titleLabel = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold };
        titleLabel.SetBinding(Label.TextProperty, new Binding(nameof(EmployeesViewModel.IsEditing), converter: new BoolToTitleConverter()));

        var nameEntry = new Entry { Placeholder = "Imię" };
        nameEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.Name));

        var surnameEntry = new Entry { Placeholder = "Nazwisko" };
        surnameEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.Surname));

        var phoneEntry = new Entry { Placeholder = "Numer telefonu", Keyboard = Keyboard.Telephone };
        phoneEntry.SetBinding(Entry.TextProperty, nameof(EmployeesViewModel.PhoneNumber));

        var buttonsLayout = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };

        var saveButton = new Button { Text = "Zapisz", WidthRequest = 120 };
        saveButton.SetBinding(Button.CommandProperty, nameof(EmployeesViewModel.SaveEmployeeCommand));

        var cancelButton = new Button { Text = "Anuluj", BackgroundColor = Colors.Gray, WidthRequest = 120 };
        cancelButton.SetBinding(Button.IsVisibleProperty, nameof(EmployeesViewModel.IsEditing));
        cancelButton.SetBinding(Button.CommandProperty, nameof(EmployeesViewModel.ClearFormCommand));

        buttonsLayout.Children.Add(saveButton);
        buttonsLayout.Children.Add(cancelButton);

        formLayout.Children.Add(titleLabel);
        formLayout.Children.Add(nameEntry);
        formLayout.Children.Add(surnameEntry);
        formLayout.Children.Add(phoneEntry);
        formLayout.Children.Add(buttonsLayout);

        mainGrid.Add(formLayout, 0, 0);

        var collectionView = new CollectionView { Margin = new Thickness(20) };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, nameof(EmployeesViewModel.Employees));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var infoLayout = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };

            var nameLabel = new Label { FontAttributes = FontAttributes.Bold, FontSize = 16 };
            var nameSpan = new Span(); nameSpan.SetBinding(Span.TextProperty, nameof(Employee.Name));
            var spaceSpan = new Span { Text = " " };
            var surnameSpan = new Span(); surnameSpan.SetBinding(Span.TextProperty, nameof(Employee.Surname));
            nameLabel.FormattedText = new FormattedString { Spans = { nameSpan, spaceSpan, surnameSpan } };

            var phoneLabel = new Label { TextColor = Colors.Gray };
            phoneLabel.SetBinding(Label.TextProperty, nameof(Employee.PhoneNumber));

            infoLayout.Children.Add(nameLabel);
            infoLayout.Children.Add(phoneLabel);
            cardGrid.Add(infoLayout, 0, 0);

            var editButton = new Button { Text = "Edytuj", HeightRequest = 35, Margin = new Thickness(0, 0, 10, 0), Padding = new Thickness(10, 5) };
            editButton.SetBinding(Button.CommandProperty, new Binding("BindingContext.EditEmployeeCommand", source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(ContentPage))));
            editButton.SetBinding(Button.CommandParameterProperty, ".");
            cardGrid.Add(editButton, 1, 0);

            var deleteButton = new Button { Text = "Usuń", BackgroundColor = Colors.DarkRed, HeightRequest = 35, Padding = new Thickness(10, 5) };
            deleteButton.SetBinding(Button.CommandProperty, new Binding("BindingContext.DeleteEmployeeCommand", source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(ContentPage))));
            deleteButton.SetBinding(Button.CommandParameterProperty, ".");
            cardGrid.Add(deleteButton, 2, 0);

            return new Frame
            {
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(15),
                BorderColor = Colors.LightGray,
                CornerRadius = 8,
                Content = cardGrid
            };
        });

        mainGrid.Add(collectionView, 0, 1);

        Content = mainGrid;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadEmployeesAsync();
    }
}

public class BoolToTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isEditing && isEditing)
            return "Edytuj pracownika";

        return "Dodaj pracownika";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}