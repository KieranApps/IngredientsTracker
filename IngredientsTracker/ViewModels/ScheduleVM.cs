using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace IngredientsTracker.ViewModels
{
    partial class ScheduleVM : BindableObject
    {

        private readonly ApiService _api;
        public ObservableCollection<CalendarDay> CalendarDays { get; set; }

        public ScheduleVM() { }
        public ScheduleVM(ApiService api)
        {
            _api = api;

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
