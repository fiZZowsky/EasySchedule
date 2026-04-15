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

        var staffingLabel = new Label { Text = "Wymagana obsada (domyślna)", FontAttributes = FontAttributes.Bold };

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
            Content = staffingList
        };

        var overridesLabel = new Label { Text = "Wyjątki dzienne (np. święta)", FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 10, 0, 0) };

        var overrideFormGrid = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Auto) }, ColumnSpacing = 10 };

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

            row.Add(dLabel, 0);
            row.Add(sLabel, 1);
            row.Add(cLabel, 2);
            row.Add(delBtn, 3);
            return row;
        }));

        var saveReqBtn = new Button { Text = "ZAPISZ OBSADĘ I WYJĄTKI", BackgroundColor = Color.FromArgb("#34495E"), TextColor = Colors.White, Margin = new Thickness(0, 15, 0, 0) };
        saveReqBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.SaveRequirementsCommand));

        var overridesCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Content = new VerticalStackLayout { Children = { new Label { Text = "Dodaj modyfikację dla wybranego dnia:", FontSize = 12, Margin = new Thickness(0, 0, 0, 5) }, overrideFormGrid, overridesList, saveReqBtn } }
        };

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

        var resultsLabel = new Label { Text = "Propozycja przypisań:", FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 10, 0, 0) };

        var resultsList = new CollectionView { HeightRequest = 400 };
        resultsList.SetBinding(CollectionView.ItemsSourceProperty, nameof(GeneratorViewModel.ProposedAssignments));
        resultsList.ItemTemplate = new DataTemplate(() =>
        {
            var cell = new Grid { Padding = 10, ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };

            var dateLabel = new Label { VerticalOptions = LayoutOptions.Center };
            dateLabel.SetBinding(Label.TextProperty, new Binding("Date", stringFormat: "{0:dd.MM}"));

            var empLabel = new Label { FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(10, 0) };
            empLabel.SetBinding(Label.TextProperty, "Employee.Surname");

            var shiftShortNameLabel = new Label { FontSize = 10 };
            shiftShortNameLabel.SetBinding(Label.TextProperty, "ShiftType.ShortName");

            var shiftLabel = new Border { BackgroundColor = Color.FromArgb("#E3F2FD"), Padding = 5, StrokeShape = new RoundRectangle { CornerRadius = 5 }, Content = shiftShortNameLabel };

            cell.Add(dateLabel, 0);
            cell.Add(empLabel, 1);
            cell.Add(shiftLabel, 2);
            return cell;
        });

        var actionsStack = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center };

        var saveBtn = new Button { Text = "Zapisz w bazie", BackgroundColor = Color.FromArgb("#2B5B84"), TextColor = Colors.White };
        saveBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.SaveScheduleCommand));

        var pdfBtn = new Button { Text = "Eksport PDF", BackgroundColor = Colors.DarkRed, TextColor = Colors.White };
        pdfBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.ExportPdfCommand));

        actionsStack.Add(saveBtn);
        actionsStack.Add(pdfBtn);

        mainStack.Add(staffingLabel);
        mainStack.Add(staffingCard);

        mainStack.Add(overridesLabel);
        mainStack.Add(overridesCard);

        mainStack.Add(settingsCard);

        mainStack.Add(resultsLabel);
        mainStack.Add(resultsList);
        mainStack.Add(actionsStack);

        mainScroll.Content = mainStack;
        Content = mainScroll;
    }
}