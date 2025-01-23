using IngredientsTracker.ViewModels;
using static IngredientsTracker.ViewModels.ScheduleVM;

namespace IngredientsTracker;

public partial class Schedule : ContentPage
{
    private ScheduleVM ViewModel;
    public Schedule(Data.Database _db)
	{
		InitializeComponent();
        ViewModel = new ScheduleVM(_db);
        BindingContext = ViewModel;
        PopulateCalendar();
    }

    void PopulateCalendar()
    {
        var firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);

        // Calculate starting position
        int startColumn = (int)firstDay.DayOfWeek; // Sunday = 0, Monday = 1, etc.
        int currentRow = 0;

        foreach (var day in ViewModel.CalendarDays)
        {
            // Create a container for each day
            var dayContainer = new StackLayout { Padding = 5, Spacing = 2 };

            // Add date label
            dayContainer.Children.Add(new Label
            {
                Text = day.Date.Day.ToString(),
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            });

            
            
            dayContainer.Children.Add(new Label
            {
                Text = day.Dish,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 10,
                TextColor = Colors.DarkBlue
            });
            dayContainer.Children.Add(new Button
            {
                Text = "Assign new Dish",
                //Command = AssignDish
                //CommandParameter = thing
            });
            

            // Add to grid
            //CalendarGrid.Children.Add();
            CalendarGrid.Add(dayContainer, startColumn, currentRow);

            // Move to next cell
            startColumn++;
            if (startColumn == 7) // End of the week
            {
                startColumn = 0;
                currentRow++;
            }
        }
    }
}