using EasySchedule.Domain.Entities;
using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class SchedulesPage : ContentPage
{
    private readonly SchedulesViewModel _viewModel;

    public SchedulesPage(SchedulesViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;

        this.SetDynamicResource(BackgroundColorProperty, "AppBackground");
        Title = "Grafiki Miesięczne";

        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadDataCommand.Execute(null);

        _viewModel.SelectedSchedule = null;
    }

    private void BuildUI()
    {
        var mainGrid = new Grid();

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.None,
            Margin = new Thickness(15, 15, 15, 80)
        };

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Schedules");

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
            nameLabel.SetBinding(Label.TextProperty, "Name");

            var datesLabel = new Label { FontSize = 13 };
            datesLabel.SetDynamicResource(Label.TextColorProperty, "TextSecondary");
            datesLabel.SetBinding(Label.TextProperty, new MultiBinding
            {
                StringFormat = "{0:dd.MM.yyyy} do {1:dd.MM.yyyy}",
                Bindings = new[] { new Binding("StartDate"), new Binding("EndDate") }
            });

            var statusBadge = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#DBEAFE"),
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Padding = new Thickness(8, 4),
                VerticalOptions = LayoutOptions.Center,
                Content = new Label
                {
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.FromArgb("#1E40AF")
                }
            };
            ((Label)statusBadge.Content).SetBinding(Label.TextProperty, "Status");

            cardGrid.Add(nameLabel, 0, 0);
            cardGrid.Add(datesLabel, 0, 1);
            cardGrid.Add(statusBadge, 1, 0);
            Grid.SetRowSpan(statusBadge, 2);

            var cardBorder = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Margin = new Thickness(0, 0, 0, 12),
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                Shadow = new Shadow
                {
                    Brush = Colors.Black,
                    Offset = new Point(0, 4),
                    Radius = 10,
                    Opacity = 0.05f
                },
                Content = cardGrid
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += async (s, e) =>
            {
                var border = (Border)s!;
                var schedule = (Schedule)border.BindingContext;

                await border.ScaleTo(0.97, 100, Easing.CubicOut);
                await border.ScaleTo(1.0, 100, Easing.CubicIn);

                _viewModel.SelectedSchedule = schedule;
                if (_viewModel.GoToGeneratorCommand.CanExecute(null))
                {
                    _viewModel.GoToGeneratorCommand.Execute(null);
                }
            };
            cardBorder.GestureRecognizers.Add(tapGesture);

            return cardBorder;
        });

        var fab = new Button
        {
            Text = "+",
            FontSize = 32,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            WidthRequest = 64,
            HeightRequest = 64,
            CornerRadius = 32,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(20),
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 8, Opacity = 0.2f }
        };
        fab.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        fab.SetBinding(Button.CommandProperty, "AddScheduleCommand");

        mainGrid.Add(collectionView);
        mainGrid.Add(fab);

        Content = mainGrid;
    }
}