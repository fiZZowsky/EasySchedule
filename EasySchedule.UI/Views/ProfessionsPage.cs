using Microsoft.Maui.Controls.Shapes;
using EasySchedule.UI.ViewModels;
using EasySchedule.Domain.Entities;

namespace EasySchedule.UI.Views;

public class ProfessionsPage : ContentPage
{
    private readonly ProfessionsViewModel _viewModel;

    public ProfessionsPage(ProfessionsViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        Title = "Zawody i Stanowiska";
        BackgroundColor = Color.FromArgb("#F5F5F5");

        BuildUI();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProfessionsAsync();
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            Padding = 20,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Star }
            }
        };

        var formBorder = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
            Stroke = Colors.Transparent,
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 20),
            Shadow = new Shadow { Brush = Brush.Black, Offset = new Point(0, 4), Radius = 10, Opacity = 0.05f }
        };

        var formStack = new VerticalStackLayout { Spacing = 15 };

        formStack.Children.Add(new Label { Text = "Dodaj nowy zawód", FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") });

        var nameEntry = new Entry { Placeholder = "Nazwa stanowiska (np. Pielęgniarka)", BackgroundColor = Color.FromArgb("#FAFAFA"), TextColor = Color.FromArgb("#333333") };
        nameEntry.SetBinding(Entry.TextProperty, nameof(ProfessionsViewModel.NewProfessionName));
        formStack.Children.Add(nameEntry);

        var switchStack = new HorizontalStackLayout { Spacing = 10, VerticalOptions = LayoutOptions.Center };
        var nightSwitch = new Switch();
        nightSwitch.SetBinding(Switch.IsToggledProperty, nameof(ProfessionsViewModel.NewProfessionCanWorkNights));

        switchStack.Children.Add(nightSwitch);
        switchStack.Children.Add(new Label { Text = "Może pracować na nocki", VerticalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#666666") });
        formStack.Children.Add(switchStack);

        var addButton = new Button { Text = "Dodaj stanowisko", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White, CornerRadius = 8, FontAttributes = FontAttributes.Bold };
        addButton.SetBinding(Button.CommandProperty, nameof(ProfessionsViewModel.AddProfessionCommand));
        formStack.Children.Add(addButton);

        formBorder.Content = formStack;
        mainGrid.Add(formBorder, 0, 0);

        var indicator = new ActivityIndicator { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Color = Color.FromArgb("#2B5B84") };
        indicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ProfessionsViewModel.IsBusy));
        indicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(ProfessionsViewModel.IsBusy));
        mainGrid.Add(indicator, 0, 1);

        var collectionView = new CollectionView();
        collectionView.SetBinding(CollectionView.ItemsSourceProperty, nameof(ProfessionsViewModel.Professions));
        collectionView.SetBinding(CollectionView.IsVisibleProperty, nameof(ProfessionsViewModel.IsNotBusy));

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardBorder = new Border
            {
                BackgroundColor = Colors.White,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Stroke = Colors.Transparent,
                Padding = 15,
                Margin = new Thickness(0, 0, 0, 10),
                Shadow = new Shadow { Brush = Brush.Black, Offset = new Point(0, 2), Radius = 5, Opacity = 0.03f }
            };

            var itemGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            var infoStack = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };

            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#333333") };
            nameLabel.SetBinding(Label.TextProperty, nameof(Profession.Name));

            var cannotWorkNightsLabel = new Label { Text = "Brak uprawnień do nocek", FontSize = 12, TextColor = Color.FromArgb("#E74C3C") };
            cannotWorkNightsLabel.SetBinding(Label.IsVisibleProperty, new Binding(nameof(Profession.CanWorkNightShifts), converter: new InverseBoolConverter()));

            var canWorkNightsLabel = new Label { Text = "Może pracować w nocy", FontSize = 12, TextColor = Color.FromArgb("#2ECC71") };
            canWorkNightsLabel.SetBinding(Label.IsVisibleProperty, nameof(Profession.CanWorkNightShifts));

            infoStack.Children.Add(nameLabel);
            infoStack.Children.Add(cannotWorkNightsLabel);
            infoStack.Children.Add(canWorkNightsLabel);

            itemGrid.Add(infoStack, 0, 0);

            var deleteBtn = new ImageButton
            {
                Source = "dotnet_bot.png",
                WidthRequest = 24,
                HeightRequest = 24,
                BackgroundColor = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center
            };

            deleteBtn.SetBinding(ImageButton.CommandProperty, new Binding("BindingContext.DeleteProfessionCommand", source: this));
            deleteBtn.SetBinding(ImageButton.CommandParameterProperty, ".");

            itemGrid.Add(deleteBtn, 1, 0);

            cardBorder.Content = itemGrid;
            return cardBorder;
        });

        mainGrid.Add(collectionView, 0, 1);

        Content = mainGrid;
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => !(bool)value;
}