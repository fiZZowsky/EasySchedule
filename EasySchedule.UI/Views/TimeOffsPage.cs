using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class TimeOffsPage : ContentPage
{
    private readonly TimeOffsViewModel _viewModel;

    public TimeOffsPage(TimeOffsViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        this.SetDynamicResource(BackgroundColorProperty, "AppBackground");
        Title = "Urlopy i Zwolnienia";

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
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Padding = 15
        };

        var empPicker = new Picker { Title = "Wybierz pracownika" };
        empPicker.SetBinding(Picker.ItemsSourceProperty, "Employees");
        empPicker.ItemDisplayBinding = new Binding("Surname");
        empPicker.SetBinding(Picker.SelectedItemProperty, "SelectedEmployee");

        var typePicker = new Picker { Title = "Rodzaj wolnego" };
        typePicker.SetBinding(Picker.ItemsSourceProperty, "TimeOffTypes");
        typePicker.SetBinding(Picker.SelectedItemProperty, "SelectedType");

        var startPicker = new DatePicker { Format = "dd.MM.yyyy" };
        startPicker.SetBinding(DatePicker.DateProperty, "NewStartDate");

        var endPicker = new DatePicker { Format = "dd.MM.yyyy" };
        endPicker.SetBinding(DatePicker.DateProperty, "NewEndDate");

        var addBtn = new Button { Text = "DODAJ ZWOLNIENIE/URLOP", FontAttributes = FontAttributes.Bold, TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        addBtn.SetBinding(Button.CommandProperty, "AddTimeOffCommand");

        var formCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 15),
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 10, Opacity = 0.05f },
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = {
                    empPicker, typePicker,
                    new HorizontalStackLayout { Spacing = 15, Children = { new Label{Text="Od:", VerticalOptions=LayoutOptions.Center}, startPicker, new Label{Text="Do:", VerticalOptions=LayoutOptions.Center}, endPicker } },
                    addBtn
                }
            }
        };

        var collectionView = new CollectionView { SelectionMode = SelectionMode.None };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "TimeOffs");

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
                Bindings = new[] { new Binding("Employee.Name"), new Binding("Employee.Surname") }
            });

            var datesLabel = new Label { FontSize = 13 };
            datesLabel.SetDynamicResource(Label.TextColorProperty, "TextSecondary");
            datesLabel.SetBinding(Label.TextProperty, new MultiBinding
            {
                StringFormat = "{0:dd.MM.yyyy} - {1:dd.MM.yyyy} ({2})",
                Bindings = new[] { new Binding("StartDate"), new Binding("EndDate"), new Binding("Type") }
            });

            var deleteBtn = new Button
            {
                Text = "Usuń",
                BackgroundColor = Color.FromArgb("#FEE2E2"),
                TextColor = Color.FromArgb("#DC2626"),
                FontSize = 12,
                CornerRadius = 6,
                HeightRequest = 32,
                Padding = new Thickness(10, 0)
            };
            deleteBtn.SetBinding(Button.CommandProperty, new Binding("DeleteTimeOffCommand", source: _viewModel));
            deleteBtn.SetBinding(Button.CommandParameterProperty, ".");

            cardGrid.Add(nameLabel, 0, 0);
            cardGrid.Add(datesLabel, 0, 1);
            cardGrid.Add(deleteBtn, 1, 0);
            Grid.SetRowSpan(deleteBtn, 2);

            var cardBorder = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Margin = new Thickness(0, 0, 0, 10),
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 2), Radius = 8, Opacity = 0.03f },
                Content = cardGrid
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                var border = (Border)s!;
                await border.ScaleTo(0.97, 100, Easing.CubicOut);
                await border.ScaleTo(1.0, 100, Easing.CubicIn);
            };
            cardBorder.GestureRecognizers.Add(tapGesture);

            return cardBorder;
        });

        mainGrid.Add(formCard, 0, 0);
        mainGrid.Add(collectionView, 0, 1);
        Content = mainGrid;
    }
}