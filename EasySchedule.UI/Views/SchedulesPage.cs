using EasySchedule.Domain.Entities;
using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using EasySchedule.UI.Converters;
using MauiIcons.Core;
using MauiIcons.Material;

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
        var mainGrid = new Grid
        {
            RowDefinitions = {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star) 
            },
            Padding = 15
        };

        var entryName = new Entry { Placeholder = "Nazwa grafiku (np. Lipiec 2026)" };
        entryName.SetBinding(Entry.TextProperty, "NewName");

        var profPicker = new Picker { Title = "Wybierz profesję" };
        profPicker.SetBinding(Picker.ItemsSourceProperty, "Professions");
        profPicker.ItemDisplayBinding = new Binding("Name");
        profPicker.SetBinding(Picker.SelectedItemProperty, "SelectedProfession");

        var startDatePicker = new DatePicker { Format = "dd.MM.yyyy" };
        startDatePicker.SetBinding(DatePicker.DateProperty, "NewStartDate");

        var endDatePicker = new DatePicker { Format = "dd.MM.yyyy" };
        endDatePicker.SetBinding(DatePicker.DateProperty, "NewEndDate");

        var dateContainer = new HorizontalStackLayout
        {
            Spacing = 15,
            Children = {
                new VerticalStackLayout { Children = { new Label { Text = "Od:", FontSize = 12, TextColor = Colors.Gray }, startDatePicker } },
                new VerticalStackLayout { Children = { new Label { Text = "Do:", FontSize = 12, TextColor = Colors.Gray }, endDatePicker } }
            }
        };

        var addBtn = new Button { Text = "UTWÓRZ SZKIELET GRAFIKU", FontAttributes = FontAttributes.Bold, TextColor = Colors.White, CornerRadius = 8 };
        addBtn.SetDynamicResource(Button.BackgroundColorProperty, "Primary");
        addBtn.SetBinding(Button.CommandProperty, "AddScheduleCommand");

        var formCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 15),
            Shadow = new Shadow { Brush = Colors.Black, Offset = new Point(0, 4), Radius = 10, Opacity = 0.05f },
            Content = new VerticalStackLayout { Spacing = 10, Children = { entryName, profPicker, dateContainer, addBtn } }
        };

        var collectionView = new CollectionView
        {
            SelectionMode = SelectionMode.None
        };

        collectionView.SetBinding(ItemsView.ItemsSourceProperty, "Schedules");

        collectionView.ItemTemplate = new DataTemplate(() =>
        {
            var cardGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) },
                RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Auto) },
                Padding = 15,
                RowSpacing = 5,
                ColumnSpacing = 10
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
            ((Label)statusBadge.Content).SetBinding(Label.TextProperty, new Binding("Status", converter: new ScheduleStatusConverter()));

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

            deleteBtn.SetBinding(Button.CommandProperty, new Binding("DeleteScheduleCommand", source: _viewModel));
            deleteBtn.SetBinding(Button.CommandParameterProperty, ".");

            deleteBtn.SetBinding(Button.IsVisibleProperty, new Binding("Status", converter: new DraftToVisibilityConverter()));

            cardGrid.Add(nameLabel, 0, 0);
            cardGrid.Add(datesLabel, 0, 1);

            cardGrid.Add(statusBadge, 1, 0);
            Grid.SetRowSpan(statusBadge, 2);

            cardGrid.Add(deleteBtn, 2, 0);
            Grid.SetRowSpan(deleteBtn, 2);

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

        mainGrid.Add(formCard, 0, 0);
        mainGrid.Add(collectionView, 0, 1);

        Content = mainGrid;
    }
}