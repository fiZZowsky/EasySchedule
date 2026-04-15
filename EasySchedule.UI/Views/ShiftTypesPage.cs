using EasySchedule.UI.ViewModels;
using EasySchedule.Domain.Entities;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class ShiftTypesPage : ContentPage
{
    private readonly ShiftTypesViewModel _viewModel;

    public ShiftTypesPage(ShiftTypesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        BackgroundColor = Color.FromArgb("#F5F5F5");

        BuildUI();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadShiftTypesAsync();
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
        formStack.Add(new Label { Text = "Nowy szablon zmiany", FontAttributes = FontAttributes.Bold });

        var nameEntry = new Entry { Placeholder = "Nazwa (np. Poranna)" };
        nameEntry.SetBinding(Entry.TextProperty, nameof(ShiftTypesViewModel.NewName));

        var shortNameEntry = new Entry { Placeholder = "Skrót (np. P1)", MaxLength = 5 };
        shortNameEntry.SetBinding(Entry.TextProperty, nameof(ShiftTypesViewModel.NewShortName));

        var timeGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 10 };

        var startPicker = new TimePicker();
        startPicker.SetBinding(TimePicker.TimeProperty, nameof(ShiftTypesViewModel.NewStartTime));
        timeGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Start", FontSize = 12 }, startPicker } }, 0);

        var endPicker = new TimePicker();
        endPicker.SetBinding(TimePicker.TimeProperty, nameof(ShiftTypesViewModel.NewEndTime));
        timeGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Koniec", FontSize = 12 }, endPicker } }, 1);

        // Standardowe wiązanie Switcha
        var nightSwitchControl = new Switch();
        nightSwitchControl.SetBinding(Switch.IsToggledProperty, nameof(ShiftTypesViewModel.NewIsNightShift));

        var nightSwitch = new HorizontalStackLayout
        {
            Spacing = 10,
            Children = {
                nightSwitchControl,
                new Label { Text = "Zmiana nocna", VerticalOptions = LayoutOptions.Center }
            }
        };

        var addBtn = new Button { Text = "Dodaj zmianę", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetBinding(Button.CommandProperty, nameof(ShiftTypesViewModel.AddShiftTypeCommand));

        formStack.Add(nameEntry);
        formStack.Add(shortNameEntry);
        formStack.Add(timeGrid);
        formStack.Add(nightSwitch);
        formStack.Add(addBtn);

        formBorder.Content = formStack;
        mainGrid.Add(formBorder, 0, 0);

        var collectionView = new CollectionView();
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(ShiftTypesViewModel.ShiftTypes));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var shortNameLabel = new Label { VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#1976D2") };
            shortNameLabel.SetBinding(Label.TextProperty, nameof(ShiftType.ShortName));

            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            nameLabel.SetBinding(Label.TextProperty, nameof(ShiftType.Name));

            var durationLabel = new Label { FontSize = 12, TextColor = Colors.Gray };
            durationLabel.SetBinding(Label.TextProperty, new Binding(nameof(ShiftType.Duration), converter: new DurationConverter()));

            var deleteBtn = new ImageButton { Source = "dotnet_bot.png", WidthRequest = 24, VerticalOptions = LayoutOptions.Center };
            deleteBtn.SetBinding(ImageButton.CommandProperty, new Binding(nameof(ShiftTypesViewModel.DeleteShiftTypeCommand), source: _viewModel));
            deleteBtn.SetBinding(ImageButton.CommandParameterProperty, ".");

            var card = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 10),
                Content = new Grid
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Children =
                    {
                        new Border {
                            BackgroundColor = Color.FromArgb("#E3F2FD"), Padding = 10, StrokeShape = new RoundRectangle { CornerRadius = 8 },
                            Content = shortNameLabel
                        },
                        new VerticalStackLayout {
                            Margin = new Thickness(15, 0), VerticalOptions = LayoutOptions.Center,
                            Children = {
                                nameLabel,
                                durationLabel
                            }
                        },
                        deleteBtn
                    }
                }
            };

            Grid.SetColumn(card.Content.As<Grid>().Children[0] as BindableObject, 0);
            Grid.SetColumn(card.Content.As<Grid>().Children[1] as BindableObject, 1);
            Grid.SetColumn(card.Content.As<Grid>().Children[2] as BindableObject, 2);

            return card;
        });

        mainGrid.Add(collectionView, 0, 1);
        Content = mainGrid;
    }
}

public class DurationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is TimeSpan duration)
        {
            return $"Czas trwania: {duration.TotalHours}h";
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public static class GridExtensions
{
    public static T As<T>(this object obj) where T : class => obj as T;
}