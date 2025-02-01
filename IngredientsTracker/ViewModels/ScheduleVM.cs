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

        public string monthSelected { get; set; }

        public ScheduleVM() { }
        public ScheduleVM(ApiService api)
        {
            _api = api;

            CalendarDays = new ObservableCollection<CalendarDay>();
            // Get Current month and year for params
            DateTime today = DateTime.Today;
            monthSelected = today.ToString("MMMM");
            Debug.WriteLine(monthSelected);
            GetDishScheduleForMonth(today);
        }

        // This function will be used for setting the month shown on the calendar as well as getting the dishes for that month
        // So takes month/date as a param
        public async Task GetDishScheduleForMonth(DateTime date)
        {
            // Get Month Length
            int monthLength = DateTime.DaysInMonth(date.Year, date.Month);
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime endDate = new DateTime(date.Year, date.Month, monthLength);

            string response = await _api.GetSchedule(startDate, endDate);

            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return;
            }

            // Loop for month length filing in schedule (leave data blank if nothign for that date in response
            JArray results = (JArray)responseData["results"];
            for (int i = 1; i <= monthLength; i++)
            {
                DateTime day = new DateTime(date.Year, date.Month, i);
                bool hasResultForDay = false;
                foreach (JObject item in results)
                {
                    DateTime itemDate = (DateTime)item["date"];
                    if (itemDate.ToString("MM-dd-yyyy") == day.ToString("MM-dd-yyyy"))
                    {
                        hasResultForDay = true;
                        CalendarDays.Add(new CalendarDay
                        {
                            Date = day,
                            DishId = (int)item["dish_id"],
                            Name = (string)item["name"],
                            Completed = (bool)item["completed"]
                        });
                        break; // end loop
                    }
                }

                if (!hasResultForDay)
                {
                    CalendarDays.Add(new CalendarDay
                    {
                        Date = day,
                        Name = "None Assigned"
                    });
                }
            }
        }
    }
}
