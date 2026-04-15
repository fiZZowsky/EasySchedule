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

            var shiftLabel = new Border
            {
                BackgroundColor = Color.FromArgb("#E3F2FD"),
                Padding = 5,
                StrokeShape = new RoundRectangle { CornerRadius = 5 },
                Content = shiftShortNameLabel
            };

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

        var staffingLabel = new Label { Text = "Wymagana obsada (liczba osób)", FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 10, 0, 0) };

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

            var rightStack = new HorizontalStackLayout { Children = { countLabel, countStepper } };

            row.Add(nameLabel, 0);
            row.Add(rightStack, 1);
            return row;
        }));

        var saveReqBtn = new Button
        {
            Text = "ZAPISZ OBSADĘ",
            BackgroundColor = Color.FromArgb("#34495E"),
            TextColor = Colors.White,
            Margin = new Thickness(0, 10, 0, 0)
        };
        saveReqBtn.SetBinding(Button.CommandProperty, nameof(GeneratorViewModel.SaveRequirementsCommand));

        var staffingCard = new Border
        {
            BackgroundColor = Colors.White,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Padding = 15,
            Margin = new Thickness(0, 0, 0, 10),
            Content = new VerticalStackLayout { Children = { staffingList, saveReqBtn } }
        };

        mainStack.Add(settingsCard);
        mainStack.Add(resultsLabel);
        mainStack.Add(resultsList);
        mainStack.Add(actionsStack);
        mainStack.Add(staffingLabel);
        mainStack.Add(staffingCard);
        mainStack.Add(settingsCard);

        mainScroll.Content = mainStack;
        Content = mainScroll;
    }
}