using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using MauiIcons.Core;
using MauiIcons.Material;

namespace EasySchedule.UI.Views;

public class ShiftTypesPage : ContentPage
{
    private readonly ShiftTypesViewModel _viewModel;

    public ShiftTypesPage(ShiftTypesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        this.SetDynamicResource(BackgroundColorProperty, "AppBackground");
        Title = "Typy Zmian";

        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadShiftTypesCommand.Execute(null);
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Padding = 15
        };

        var entryName = new Entry { Placeholder = "Nazwa (np. Dniówka)" };
        entryName.SetBinding(Entry.TextProperty, "NewName");

        var entryShort = new Entry { Placeholder = "Skrót (np. D)" };
        entryShort.SetBinding(Entry.TextProperty, "NewShortName");

        var switchNight = new Switch { VerticalOptions = LayoutOptions.Center };
        switchNight.SetBinding(Switch.IsToggledProperty, "NewIsNightShift");

        var addBtn = new Button { Text = "DODAJ ZMIANĘ", FontAttributes = FontAttributes.Bold, TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        addBtn.SetBinding(Button.CommandProperty, "AddShiftTypeCommand");

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
                    entryName, entryShort,
                    new HorizontalStackLayout { Spacing = 10, Children = { new Label { Text = "Zmiana nocna?", VerticalOptions = LayoutOptions.Center }, switchNight } },
                    addBtn
                }
            }
        };

        var collectionView = new CollectionView { SelectionMode = SelectionMode.None };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "ShiftTypes");

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(50), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Padding = 15,
                ColumnSpacing = 15
            };

            var shortBadge = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#DBEAFE"),
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                HeightRequest = 40,
                WidthRequest = 40,
                Content = new Label { FontSize = 14, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#1E40AF"), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center }
            };
            ((Label)shortBadge.Content).SetBinding(Label.TextProperty, "ShortName");

            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            nameLabel.SetDynamicResource(Label.TextColorProperty, "TextPrimary");
            nameLabel.SetBinding(Label.TextProperty, "Name");

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

            deleteBtn.SetBinding(Button.CommandProperty, new Binding("DeleteShiftTypeCommand", source: _viewModel));
            deleteBtn.SetBinding(Button.CommandParameterProperty, ".");

            cardGrid.Add(shortBadge, 0, 0);
            cardGrid.Add(nameLabel, 1, 0);
            cardGrid.Add(deleteBtn, 2, 0);

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