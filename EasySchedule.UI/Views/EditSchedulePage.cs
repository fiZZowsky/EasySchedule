using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;
using MauiIcons.Core;
using MauiIcons.Material;
using System;

namespace EasySchedule.UI.Views;

public class EditSchedulePage : ContentPage
{
    private readonly EditScheduleViewModel _viewModel;

    public EditSchedulePage(EditScheduleViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        BackgroundColor = Color.FromArgb("#F5F5F5");

        BuildUI();
    }

    private void BuildUI()
    {
        var mainGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            Padding = 15,
            RowSpacing = 15
        };

        var dateNavGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };

        var prevBtn = new Button { Text = "<", FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#BDC3C7"), TextColor = Colors.Black, WidthRequest = 50 };
        prevBtn.SetBinding(Button.CommandProperty, nameof(EditScheduleViewModel.PreviousDayCommand));

        var dateLabel = new Label { FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
        dateLabel.SetBinding(Label.TextProperty, new Binding("SelectedDate", stringFormat: "{0:dd MMMM yyyy}"));

        var nextBtn = new Button { Text = ">", FontAttributes = FontAttributes.Bold, BackgroundColor = Color.FromArgb("#BDC3C7"), TextColor = Colors.Black, WidthRequest = 50 };
        nextBtn.SetBinding(Button.CommandProperty, nameof(EditScheduleViewModel.NextDayCommand));

        dateNavGrid.Add(prevBtn, 0);
        dateNavGrid.Add(dateLabel, 1);
        dateNavGrid.Add(nextBtn, 2);

        var assignmentsCollection = new CollectionView { SelectionMode = SelectionMode.None };
        assignmentsCollection.SetBinding(ItemsView.ItemsSourceProperty, nameof(EditScheduleViewModel.DailyAssignments));
        assignmentsCollection.ItemTemplate = new DataTemplate(() =>
        {
            var rowGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Padding = 10 };

            var empLabel = new Label { VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            empLabel.SetBinding(Label.TextProperty, new MultiBinding
            {
                StringFormat = "{0} ({1})",
                Bindings = new[]
                {
                    new Binding("Employee.FullName"),
                    new Binding("ShiftType.Name")
                }
            });

            // ZMIANA: Dodano .IconSize(22) aby powiększyć samą ikonę kosza
            var delBtn = new Button { BackgroundColor = Color.FromArgb("#FEE2E2"), WidthRequest = 35, HeightRequest = 35, CornerRadius = 8, Padding = 0, VerticalOptions = LayoutOptions.Center }
                .Icon(MaterialIcons.Delete)
                .IconColor(Color.FromArgb("#DC2626"))
                .IconSize(22);
            delBtn.SetBinding(Button.CommandProperty, new Binding("RemoveAssignmentCommand", source: _viewModel));
            delBtn.SetBinding(Button.CommandParameterProperty, ".");

            rowGrid.Add(empLabel, 0);
            rowGrid.Add(delBtn, 1);

            return new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                Margin = new Thickness(0, 0, 0, 8),
                StrokeShape = new RoundRectangle { CornerRadius = 8 },
                Content = rowGrid
            };
        });

        var formBorder = new Border
        {
            BackgroundColor = Colors.White,
            StrokeThickness = 0,
            Padding = 15,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label { Text = "Dodaj pracownika do zmiany", FontAttributes = FontAttributes.Bold },

                    new Picker { Title = "Wybierz pracownika", ItemDisplayBinding = new Binding("FullName") }
                        .Apply(p => { p.SetBinding(Picker.ItemsSourceProperty, "AvailableEmployees"); p.SetBinding(Picker.SelectedItemProperty, "SelectedEmployee"); }),

                    new Picker { Title = "Wybierz zmianę", ItemDisplayBinding = new Binding("Name") }
                        .Apply(p => { p.SetBinding(Picker.ItemsSourceProperty, "AvailableShiftTypes"); p.SetBinding(Picker.SelectedItemProperty, "SelectedShiftType"); }),

                    new Button { Text = "Dodaj", BackgroundColor = Color.FromArgb("#3498DB"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold }
                        .Apply(b => b.SetBinding(Button.CommandProperty, "AddAssignmentCommand"))
                }
            }
        };

        var applyBtn = new Button { Text = "ZASTOSUJ ZMIANY", BackgroundColor = Color.FromArgb("#2ECC71"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold, HeightRequest = 50 };
        applyBtn.SetBinding(Button.CommandProperty, nameof(EditScheduleViewModel.ApplyChangesCommand));

        mainGrid.Add(dateNavGrid, 0, 0);
        mainGrid.Add(assignmentsCollection, 0, 1);
        mainGrid.Add(formBorder, 0, 2);
        mainGrid.Add(applyBtn, 0, 3);

        Content = mainGrid;
    }
}

public static class ViewExtensions
{
    public static T Apply<T>(this T view, Action<T> action) where T : View { action(view); return view; }
}