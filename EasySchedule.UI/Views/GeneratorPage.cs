using EasySchedule.UI.ViewModels;
using Microsoft.Maui.Controls.Shapes;

namespace EasySchedule.UI.Views;

public class GeneratorPage : ContentPage
{
    private readonly GeneratorViewModel _viewModel;

    public GeneratorPage(GeneratorViewModel viewModel)
    {
        _viewModel = viewModel;
        BindingContext = _viewModel;
        BackgroundColor = Color.FromArgb("#F5F5F5");

        this.SetBinding(Page.TitleProperty, nameof(GeneratorViewModel.Title));
        BuildUI();
    }

    private void BuildUI()
    {
        var mainScroll = new ScrollView();
        var mainStack = new VerticalStackLayout { Padding = 20, Spacing = 15 };

        var staffingList = new VerticalStackLayout { Spacing = 5 };
        BindableLayout.SetItemsSource(staffingList, _viewModel.StaffingRequirements);
        BindableLayout.SetItemTemplate(staffingList, new DataTemplate(() =>
        {
            var row = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) }, Padding = 5 };
            var nameLabel = new Label { VerticalOptions = LayoutOptions.Center };
            nameLabel.SetBinding(Label.TextProperty, "ShiftName");
            var countStepper = new Stepper { Minimum = 0, Maximum = 10 };
            countStepper.SetBinding(Stepper.ValueProperty, "Count");
            var countLabel = new Label { VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) };
            countLabel.SetBinding(Label.TextProperty, "Count");
            row.Add(nameLabel, 0);
            row.Add(new HorizontalStackLayout { Children = { countLabel, countStepper } }, 1);
            return row;
        }));

        var staffingCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Content = new VerticalStackLayout { Children = { new Label { Text = "Wymagana obsada (domyślna)", FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 5) }, staffingList } }
        };
        staffingCard.SetBinding(Border.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        var datePicker = new DatePicker { Format = "dd.MM" };
        datePicker.SetBinding(DatePicker.DateProperty, nameof(GeneratorViewModel.OverrideDate));

        var shiftPicker = new Picker { Title = "Zmiana" };
        shiftPicker.SetBinding(Picker.ItemsSourceProperty, nameof(GeneratorViewModel.AvailableShiftTypes));
        shiftPicker.ItemDisplayBinding = new Binding("Name");
        shiftPicker.SetBinding(Picker.SelectedItemProperty, nameof(GeneratorViewModel.SelectedOverrideShift));

        var overrideStepper = new Stepper { Minimum = 0, Maximum = 10 };
        overrideStepper.SetBinding(Stepper.ValueProperty, nameof(GeneratorViewModel.OverrideCount));

        var overrideCountLbl = new Label { VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0) };
        overrideCountLbl.SetBinding(Label.TextProperty, nameof(GeneratorViewModel.OverrideCount));

        var addOverrideBtn = new Button { Text = "+", BackgroundColor = Color.FromArgb("#2ECC71"), TextColor = Colors.White, WidthRequest = 40, HeightRequest = 40, CornerRadius = 20, FontAttributes = FontAttributes.Bold };
        addOverrideBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.AddOverrideLocalCommand));

        var overrideFormGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10 };
        overrideFormGrid.Add(datePicker, 0);
        overrideFormGrid.Add(shiftPicker, 1);
        overrideFormGrid.Add(new HorizontalStackLayout { Children = { overrideCountLbl, overrideStepper }, VerticalOptions = LayoutOptions.Center }, 2);
        overrideFormGrid.Add(addOverrideBtn, 3);

        var overridesList = new VerticalStackLayout { Spacing = 5, Margin = new Thickness(0, 10, 0, 0) };
        BindableLayout.SetItemsSource(overridesList, _viewModel.OverrideRequirements);
        BindableLayout.SetItemTemplate(overridesList, new DataTemplate(() =>
        {
            var row = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10, Padding = 5 };
            var dLabel = new Label { VerticalOptions = LayoutOptions.Center };
            dLabel.SetBinding(Label.TextProperty, new Binding("SpecificDate", stringFormat: "{0:dd.MM.yy}"));
            var sLabel = new Label { VerticalOptions = LayoutOptions.Center, FontAttributes = FontAttributes.Bold };
            sLabel.SetBinding(Label.TextProperty, "ShiftName");
            var cLabel = new Label { VerticalOptions = LayoutOptions.Center, TextColor = Color.FromArgb("#1976D2") };
            cLabel.SetBinding(Label.TextProperty, new Binding("Count", stringFormat: "Osób: {0}"));
            var delBtn = new ImageButton { Source = "dotnet_bot.png", WidthRequest = 20, HeightRequest = 20, VerticalOptions = LayoutOptions.Center };
            delBtn.SetBinding(ImageButton.CommandProperty, new Binding(nameof(GeneratorViewModel.RemoveOverrideLocalCommand), source: _viewModel));
            delBtn.SetBinding(ImageButton.CommandParameterProperty, ".");
            row.Add(dLabel, 0); row.Add(sLabel, 1); row.Add(cLabel, 2); row.Add(delBtn, 3);
            return row;
        }));

        var saveReqBtn = new Button { Text = "ZAPISZ OBSADĘ I WYJĄTKI", BackgroundColor = Color.FromArgb("#34495E"), TextColor = Colors.White, Margin = new Thickness(0, 15, 0, 0) };
        saveReqBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.SaveRequirementsCommand));

        var overridesCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Content = new VerticalStackLayout { Children = { new Label { Text = "Wyjątki dzienne (np. święta)", FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 5) }, overrideFormGrid, overridesList, saveReqBtn } }
        };
        overridesCard.SetBinding(Border.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        var maxConsecutiveDaysStepper = new Stepper { Minimum = 1, Maximum = 14 };
        maxConsecutiveDaysStepper.SetBinding(Stepper.ValueProperty, nameof(GeneratorViewModel.MaxConsecutiveDays));

        var maxConsecutiveDaysLabel = new Label { FontSize = 12 };
        maxConsecutiveDaysLabel.SetBinding(Label.TextProperty, nameof(GeneratorViewModel.MaxConsecutiveDays));

        var minRestHoursStepper = new Stepper { Minimum = 8, Maximum = 24 };
        minRestHoursStepper.SetBinding(Stepper.ValueProperty, nameof(GeneratorViewModel.MinRestHours));

        var minRestHoursLabel = new Label { FontSize = 12 };
        minRestHoursLabel.SetBinding(Label.TextProperty, nameof(GeneratorViewModel.MinRestHours));

        var generateBtn = new Button { Text = "GENERUJ GRAFIK", BackgroundColor = Color.FromArgb("#2ECC71"), TextColor = Colors.White, FontAttributes = FontAttributes.Bold };
        generateBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.RunGeneratorCommand));

        var settingsCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = {
                    new Label { Text = "Parametry algorytmu", FontAttributes = FontAttributes.Bold },
                    new Label { Text = "Max dni pod rząd:", FontSize = 12 },
                    maxConsecutiveDaysStepper,
                    maxConsecutiveDaysLabel,
                    new Label { Text = "Min. odpoczynek (h):", FontSize = 12 },
                    minRestHoursStepper,
                    minRestHoursLabel,
                    generateBtn
                }
            }
        };
        settingsCard.SetBinding(Border.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        var pdfBtn = new Button
        {
            Text = "E",
            BackgroundColor = Colors.DarkRed,
            TextColor = Colors.White,
            WidthRequest = 35,
            HeightRequest = 35,
            CornerRadius = 8,
            Padding = 0,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center
        };
        pdfBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.ExportPdfCommand));

        var resultsHeaderGrid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) },
            Margin = new Thickness(0, 20, 0, 5)
        };
        resultsHeaderGrid.SetBinding(Grid.IsVisibleProperty, nameof(GeneratorViewModel.HasGeneratedSchedule));

        var resultsLabel = new Label { Text = "Podgląd grafiku:", FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };

        resultsHeaderGrid.Add(resultsLabel);
        Grid.SetColumn(resultsLabel, 0);

        resultsHeaderGrid.Add(pdfBtn);
        Grid.SetColumn(pdfBtn, 1);

        var matrixGrid = new Grid
        {
            ColumnDefinitions = { new ColumnDefinition(100), new ColumnDefinition(GridLength.Star) },
            RowDefinitions = { new RowDefinition(50), new RowDefinition(GridLength.Star) },
            HeightRequest = 400,
            BackgroundColor = Colors.White,
            Margin = new Thickness(0, 0, 0, 15),
            IsClippedToBounds = true
        };
        matrixGrid.SetBinding(Grid.IsVisibleProperty, nameof(GeneratorViewModel.HasGeneratedSchedule));

        matrixGrid.Add(new Border { BackgroundColor = Color.FromArgb("#ECF0F1"), Stroke = Color.FromArgb("#BDC3C7") }, 0, 0);

        var dateHeaderStack = new HorizontalStackLayout { Spacing = 2 };
        BindableLayout.SetItemsSource(dateHeaderStack, _viewModel.CalendarDays);
        BindableLayout.SetItemTemplate(dateHeaderStack, new DataTemplate(() =>
        {
            var dayLabel = new Label { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, FontSize = 12, FontAttributes = FontAttributes.Bold };
            dayLabel.SetBinding(Label.TextProperty, new Binding(".", stringFormat: "{0:dd.MM}"));
            return new Border { WidthRequest = 50, BackgroundColor = Color.FromArgb("#ECF0F1"), Stroke = Color.FromArgb("#BDC3C7"), Content = dayLabel };
        }));

        var topScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = dateHeaderStack,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never
        };
        matrixGrid.Add(topScroll, 1, 0);

        var nameColumnStack = new VerticalStackLayout { Spacing = 2 };
        BindableLayout.SetItemsSource(nameColumnStack, _viewModel.CalendarRows);
        BindableLayout.SetItemTemplate(nameColumnStack, new DataTemplate(() =>
        {
            var nameLabel = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            nameLabel.SetBinding(Label.TextProperty, "EmployeeName");
            return new Border { HeightRequest = 40, BackgroundColor = Color.FromArgb("#FAFAFA"), Stroke = Color.FromArgb("#BDC3C7"), Content = nameLabel };
        }));

        var cellsMatrixStack = new VerticalStackLayout { Spacing = 2 };
        BindableLayout.SetItemsSource(cellsMatrixStack, _viewModel.CalendarRows);
        BindableLayout.SetItemTemplate(cellsMatrixStack, new DataTemplate(() =>
        {
            var rowStack = new HorizontalStackLayout { Spacing = 2 };
            rowStack.SetBinding(BindableLayout.ItemsSourceProperty, "Cells");
            BindableLayout.SetItemTemplate(rowStack, new DataTemplate(() =>
            {
                var shiftLabel = new Label { TextColor = Colors.White, FontSize = 12, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                shiftLabel.SetBinding(Label.TextProperty, "ShiftShortName");
                shiftLabel.SetBinding(Label.IsVisibleProperty, "HasShift");
                var cellBorder = new Border { WidthRequest = 50, HeightRequest = 40, Stroke = Color.FromArgb("#ECF0F1") };
                cellBorder.SetBinding(Border.BackgroundColorProperty, "BackgroundColor");
                cellBorder.Content = shiftLabel;
                return cellBorder;
            }));
            return rowStack;
        }));

        var leftScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Vertical,
            Content = nameColumnStack,
            VerticalScrollBarVisibility = ScrollBarVisibility.Never
        };

        var mainContentScroll = new ScrollView
        {
            Orientation = ScrollOrientation.Both,
            Content = cellsMatrixStack
        };

        double currentX = 0;
        double currentY = 0;

        mainContentScroll.Scrolled += (s, e) =>
        {
            if (Math.Abs(currentX - e.ScrollX) > 0.5 || Math.Abs(currentY - e.ScrollY) > 0.5)
            {
                currentX = e.ScrollX;
                currentY = e.ScrollY;
                topScroll.ScrollToAsync(currentX, 0, false);
                leftScroll.ScrollToAsync(0, currentY, false);
            }
        };

        topScroll.Scrolled += (s, e) =>
        {
            if (Math.Abs(currentX - e.ScrollX) > 0.5)
            {
                currentX = e.ScrollX;
                mainContentScroll.ScrollToAsync(currentX, currentY, false);
            }
        };

        leftScroll.Scrolled += (s, e) =>
        {
            if (Math.Abs(currentY - e.ScrollY) > 0.5)
            {
                currentY = e.ScrollY;
                mainContentScroll.ScrollToAsync(currentX, currentY, false);
            }
        };

        matrixGrid.Add(leftScroll, 0, 1);
        matrixGrid.Add(mainContentScroll, 1, 1);

        var actionsStack = new VerticalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Fill };
        actionsStack.SetBinding(VerticalStackLayout.IsVisibleProperty, nameof(GeneratorViewModel.HasGeneratedSchedule));

        var editBtn = new Button { Text = "Edytuj Grafik", BackgroundColor = Color.FromArgb("#F39C12"), TextColor = Colors.White };
        editBtn.SetBinding(Button.CommandProperty, "GoToEditPageCommand");
        editBtn.SetBinding(Button.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        var saveDraftBtn = new Button { Text = "Zapisz Szkic", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White };
        saveDraftBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.SaveDraftCommand));
        saveDraftBtn.SetBinding(Button.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        var publishBtn = new Button { Text = "Zatwierdź Grafik", BackgroundColor = Color.FromArgb("#27AE60"), TextColor = Colors.White };
        publishBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.PublishScheduleCommand));
        publishBtn.SetBinding(Button.IsVisibleProperty, nameof(GeneratorViewModel.IsEditable));

        actionsStack.Add(editBtn);
        actionsStack.Add(saveDraftBtn);
        actionsStack.Add(publishBtn);

        mainStack.Add(staffingCard);
        mainStack.Add(overridesCard);
        mainStack.Add(settingsCard);
        mainStack.Add(resultsHeaderGrid);
        mainStack.Add(matrixGrid);
        mainStack.Add(actionsStack);

        mainScroll.Content = mainStack;
        Content = mainScroll;
    }
}