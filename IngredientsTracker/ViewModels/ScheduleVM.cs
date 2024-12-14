using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            // Can expand to each month, i.e., January, Februrary etc.. rather than now + 30 days
            for (int i = 1; i <= 30; i++) // Generate a sample month
            {
                CalendarDays.Add(new CalendarDay
                {
                    Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, i),
                    Dish = "Dish number " + i
                });
            }
        }
    }
}
