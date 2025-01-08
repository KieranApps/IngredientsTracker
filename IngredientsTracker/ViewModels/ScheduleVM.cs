using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngredientsTracker.ViewModels
{
    partial class ScheduleVM : BindableObject
    {
        private readonly Database.Database _db;


        public ObservableCollection<CalendarDay> CalendarDays { get; set; }

        public ScheduleVM() { }

        public class CalendarDay
        {
            public DateTime Date { get; set; }
            public string Dish { get; set; } // Can be expanded to a list if multiple dishes per day
        }

       

        public ScheduleVM(Database.Database db)
        {
            _db = db;

            CalendarDays = new ObservableCollection<CalendarDay>();
            // Get Current month and year for params
            DateTime today = DateTime.Today;
            GetDishScheduleForMonth(today);
        }

        // This function will be used for setting the month shown on the calendar as well as getting the dishes for that month
        // So takes month/date as a param
        public async Task GetDishScheduleForMonth(DateTime date)
        {
            // Get Current Month
            int monthLength = DateTime.DaysInMonth(date.Year, date.Month);

            var dishScheduledForMonth = await _db.GetDishForDayScheduled(date.Year, date.Month, monthLength);
            if (dishScheduledForMonth == null)
            {
                // Whole month is empty
                for (int i = 1; i <= monthLength; i++)
                {
                    CalendarDays.Add(new CalendarDay
                    {
                        Date = new DateTime(date.Year, date.Month, i), // Update to use
                        Dish = "Dish: " + i
                    });
                }
                return;
            }
            for (int i = 1; i <= monthLength; i++)
            {
                // Get dish
                DateTime dishDate = new DateTime(date.Year, date.Month, i);
                
                // Find current day in dishScheduledForMonth (if exists) and add to CalendarDays
                // Will have to loop through manually as its a custom type
                foreach (var el in dishScheduledForMonth)
                {
                    if ()
                    {

                    }
                    CalendarDays.Add(new CalendarDay
                    {
                        Date = new DateTime(date.Year, date.Month, i), // Update to use
                        Dish = el.DishId.ToString() // Dish = "Dish: " + i
                    });

                }
            }
        }
    }
}
