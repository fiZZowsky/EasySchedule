using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using MauiIcons.Core;
using MauiIcons.Material;

namespace EasySchedule.UI.Views;

public class EmployeesPage : ContentPage
{
    private readonly EmployeesViewModel _viewModel;

    public EmployeesPage(EmployeesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        this.SetDynamicResource(BackgroundColorProperty, "AppBackground");
        Title = "Pracownicy";

        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDataCommand.Execute(null);
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            RowDefinitions = {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto), 
                new RowDefinition(GridLength.Star)
            },
            Padding = 15
        };

        var entryName = new Entry { Placeholder = "Imię" };
        entryName.SetBinding(Entry.TextProperty, "NewName");

        var entrySurname = new Entry { Placeholder = "Nazwisko" };
        entrySurname.SetBinding(Entry.TextProperty, "NewSurname");

        var entryPhoneNumber = new Entry { Placeholder = "Numer telefonu" };
        entryPhoneNumber.SetBinding(Entry.TextProperty, "NewPhoneNumber");

        var profPicker = new Picker { Title = "Wybierz profesję" };
        profPicker.SetBinding(Picker.ItemsSourceProperty, "Professions");
        profPicker.ItemDisplayBinding = new Binding("Name");
        profPicker.SetBinding(Picker.SelectedItemProperty, "SelectedProfession");

        var addBtn = new Button { Text = "DODAJ PRACOWNIKA", FontAttributes = FontAttributes.Bold, TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        addBtn.SetBinding(Button.CommandProperty, "AddEmployeeCommand");

        var formCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 15),
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 10, Opacity = 0.05f },
            Content = new VerticalStackLayout { Spacing = 10, Children = { entryName, entrySurname, entryPhoneNumber, profPicker, addBtn } }
        };

        var filterPicker = new Picker
        {
            Title = "Filtruj po profesji",
            WidthRequest = 200,
            HorizontalOptions = LayoutOptions.Start,
            FontSize = 14
        };
        filterPicker.SetBinding(Picker.ItemsSourceProperty, "FilterProfessions");
        filterPicker.ItemDisplayBinding = new Binding("Name");
        filterPicker.SetBinding(Picker.SelectedItemProperty, "SelectedFilterProfession");

        var filterContainer = new VerticalStackLayout
        {
            Margin = new Thickness(5, 0, 0, 15),
            Children = {
                new Label { Text = "Profesja:", FontSize = 12, TextColor = Colors.Gray, Margin = new Thickness(2,0,0,2) },
                filterPicker
            }
        };

        var collectionView = new CollectionView { SelectionMode = SelectionMode.None };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Employees");

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) },
                Padding = 15,
                RowSpacing = 5
            };

            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            nameLabel.SetDynamicResource(Label.TextColorProperty, "TextPrimary");
            nameLabel.SetBinding(Label.TextProperty, new MultiBinding
            {
                StringFormat = "{0} {1}",
                Bindings = new[] { new Binding("Name"), new Binding("Surname") }
            });

            var phoneLabel = new Label { FontSize = 12, TextColor = Colors.Gray };
            phoneLabel.SetBinding(Label.TextProperty, "PhoneNumber");

            var profBadge = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#F1F5F9"),
                StrokeShape = new RoundRectangle { CornerRadius = 6 },
                Padding = new Thickness(8, 4),
                HorizontalOptions = LayoutOptions.Start,
                Content = new Label { FontSize = 12, TextColor = Color.FromArgb("#475569") }
            };
            ((Label)profBadge.Content).SetBinding(Label.TextProperty, "Profession.Name");

            var deleteBtn = new Button
            {
                BackgroundColor = Color.FromArgb("#FEE2E2"),
                WidthRequest = 35,
                HeightRequest = 35,
                CornerRadius = 8,
                Padding = 0,
                VerticalOptions = LayoutOptions.Center
            }
            .Icon(MaterialIcons.Delete)
            .IconColor(Color.FromArgb("#DC2626"))
            .IconSize(22);

            deleteBtn.SetBinding(Button.CommandProperty, new Binding("DeleteEmployeeCommand", source: _viewModel));
            deleteBtn.SetBinding(Button.CommandParameterProperty, ".");

            cardGrid.Add(nameLabel, 0, 0);
            cardGrid.Add(profBadge, 0, 1);
            cardGrid.Add(phoneLabel, 0, 2);
            cardGrid.Add(deleteBtn, 1, 0);
            Grid.SetRowSpan(deleteBtn, 2);

            return new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Margin = new Thickness(0, 0, 0, 10),
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 2), Radius = 8, Opacity = 0.03f },
                Content = cardGrid
            };
        });

        mainGrid.Add(formCard, 0, 0);
        mainGrid.Add(filterContainer, 0, 1);
        mainGrid.Add(collectionView, 0, 2);

        Content = mainGrid;
    }
}