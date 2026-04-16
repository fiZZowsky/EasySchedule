using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class ProfessionsPage : ContentPage
{
    private readonly ProfessionsViewModel _viewModel;

    public ProfessionsPage(ProfessionsViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        this.SetDynamicResource(BackgroundColorProperty, "AppBackground");
        Title = "Zarządzanie Profesjami";

        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadProfessionsAsync();
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            Padding = 15
        };

        var entryName = new Entry { Placeholder = "Nazwa profesji (np. Pielęgniarka)" };
        entryName.SetBinding(Entry.TextProperty, "NewName");

        var addBtn = new Button
        {
            Text = "DODAJ PROFESJĘ",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            CornerRadius = 8
        };
        addBtn.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        addBtn.SetBinding(Button.CommandProperty, "AddProfessionCommand");

        var formCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 15),
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 10, Opacity = 0.05f },
            Content = new VerticalStackLayout { Spacing = 10, Children = { entryName, addBtn } }
        };

        var collectionView = new CollectionView { SelectionMode = SelectionMode.None };
        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Professions");

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
                Padding = 15
            };

            var nameLabel = new Label { FontSize = 16, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
            nameLabel.SetDynamicResource(Label.TextColorProperty, "TextPrimary");
            nameLabel.SetBinding(Label.TextProperty, "Name");

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
            deleteBtn.SetBinding(Button.CommandProperty, new Binding("DeleteProfessionCommand", source: _viewModel));
            deleteBtn.SetBinding(Button.CommandParameterProperty, ".");

            cardGrid.Add(nameLabel, 0, 0);
            cardGrid.Add(deleteBtn, 1, 0);

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