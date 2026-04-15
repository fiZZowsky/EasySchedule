using EasySchedule.UI.ViewModels;
using EasySchedule.Domain.Entities;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class TimeOffsPage : ContentPage
{
    private readonly TimeOffsViewModel _viewModel;

    public TimeOffsPage(TimeOffsViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        BackgroundColor = Color.FromArgb("#F5F5F5");

        this.SetBinding(Page.TitleProperty, nameof(TimeOffsViewModel.Title));

        BuildUI();
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
        formStack.Add(new Label { Text = "Zgłoś nieobecność", FontAttributes = FontAttributes.Bold });

        var dateGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) }, ColumnSpacing = 10 };

        var startPicker = new DatePicker { Format = "dd.MM.yyyy" };
        startPicker.SetBinding(DatePicker.DateProperty, nameof(TimeOffsViewModel.NewStartDate));
        dateGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Od:", FontSize = 12 }, startPicker } }, 0);

        var endPicker = new DatePicker { Format = "dd.MM.yyyy" };
        endPicker.SetBinding(DatePicker.DateProperty, nameof(TimeOffsViewModel.NewEndDate));
        dateGrid.Add(new VerticalStackLayout { Children = { new Label { Text = "Do:", FontSize = 12 }, endPicker } }, 1);

        var typePicker = new Picker { Title = "Wybierz powód" };
        typePicker.SetBinding(Picker.ItemsSourceProperty, nameof(TimeOffsViewModel.TimeOffTypes));
        typePicker.SetBinding(Picker.SelectedItemProperty, nameof(TimeOffsViewModel.SelectedTimeOffType));

        var addBtn = new Button { Text = "Dodaj wolne", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetBinding(Button.CommandProperty, nameof(TimeOffsViewModel.AddTimeOffCommand));

        formStack.Add(dateGrid);
        formStack.Add(typePicker);
        formStack.Add(addBtn);

        formBorder.Content = formStack;
        mainGrid.Add(formBorder, 0, 0);

        var collectionView = new CollectionView();
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(TimeOffsViewModel.TimeOffs));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var dateLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold };
            dateLabel.SetBinding(Label.TextProperty, new Binding(".", converter: new TimeOffDatesConverter()));

            var typeLabel = new Label { FontSize = 12, TextColor = Colors.Gray };
            typeLabel.SetBinding(Label.TextProperty, nameof(TimeOff.Type));

            var deleteBtn = new ImageButton { Source = "dotnet_bot.png", WidthRequest = 24, VerticalOptions = LayoutOptions.Center };
            deleteBtn.SetBinding(ImageButton.CommandProperty, new Binding(nameof(TimeOffsViewModel.DeleteTimeOffCommand), source: _viewModel));
            deleteBtn.SetBinding(ImageButton.CommandParameterProperty, ".");

            var card = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 10),
                Content = new Grid
                {
                    ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                    Children =
                    {
                        new VerticalStackLayout {
                            VerticalOptions = LayoutOptions.Center,
                            Children = { dateLabel, typeLabel }
                        },
                        deleteBtn
                    }
                }
            };

            Grid.SetColumn(card.Content.As<Grid>().Children[0] as BindableObject, 0);
            Grid.SetColumn(card.Content.As<Grid>().Children[1] as BindableObject, 1);

            return card;
        });

        mainGrid.Add(collectionView, 0, 1);
        Content = mainGrid;
    }
}

public class TimeOffDatesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is TimeOff t)
        {
            return $"{t.StartDate:dd.MM.yyyy} - {t.EndDate:dd.MM.yyyy}";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
}