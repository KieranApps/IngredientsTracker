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
        }
    }
}
