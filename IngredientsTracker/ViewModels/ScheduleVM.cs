﻿using IngredientsTracker.Data;
using IngredientsTracker.Helpers;
using Microsoft.Maui.Controls;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace IngredientsTracker.ViewModels
{
    public partial class ScheduleVM : BindableObject
    {

        private readonly ApiService _api;
        public ObservableCollection<CalendarDay> CalendarDays { get; set; }

        public ObservableCollection<DishModel> Dishes { get; set; }

        private string _monthSelected;
        public string MonthSelected
        {
            get => _monthSelected;
            set
            {
                if (_monthSelected != value)
                {
                    _monthSelected = value;
                    OnPropertyChanged(nameof(MonthSelected));
                }
            }
        }
        private string _yearSelected;
        public string YearSelected
        {
            get => _yearSelected;
            set
            {
                if (_yearSelected != value)
                {
                    _yearSelected = value;
                    OnPropertyChanged(nameof(YearSelected));
                }
            }
        }
        public DateTime selectedDateYearMonth { get; set; }

        public ScheduleVM() { }
        public ScheduleVM(ApiService api)
        {
            _api = api;

            CalendarDays = new ObservableCollection<CalendarDay>();
            Dishes = new ObservableCollection<DishModel>();
            // Get Current month and year for params
            DateTime today = DateTime.Today;
            MonthSelected = today.ToString("MMMM");
            YearSelected = today.ToString("yyyy");
            selectedDateYearMonth = today;
            GetDishScheduleForMonth(today);
        }

        public void GoToPreviousMonth()
        {
            selectedDateYearMonth = selectedDateYearMonth.AddMonths(-1);
            MonthSelected = selectedDateYearMonth.ToString("MMMM");
            YearSelected = selectedDateYearMonth.ToString("yyyy");
            GetDishScheduleForMonth(selectedDateYearMonth);
        }

        public void GoToNextMonth()
        {
            selectedDateYearMonth = selectedDateYearMonth.AddMonths(1);
            MonthSelected = selectedDateYearMonth.ToString("MMMM");
            YearSelected = selectedDateYearMonth.ToString("yyyy");
            GetDishScheduleForMonth(selectedDateYearMonth);
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
            CalendarDays.Clear();
            // Loop for month length filing in schedule (leave data blank if nothign for that date in response
            JArray results = (JArray)responseData["results"];
            for (int i = 1; i <= monthLength; i++)
            {
                DateTime day = new DateTime(date.Year, date.Month, i);
                bool hasResultForDay = false;
                foreach (JObject item in results)
                {
                    DateTime itemDate = (DateTime)item["date"];
                    if (itemDate.ToString("yyyy-MM-dd") == day.ToString("yyyy-MM-dd"))
                    {
                        hasResultForDay = true;
                        CalendarDays.Add(new CalendarDay
                        {
                            Date = day.ToString("yyyy-MM-dd"),
                            DateNumber = day.ToString("dd"),
                            DateDay = day.ToString("dddd"),
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
                        Date = day.ToString("yyyy-MM-dd"),
                        DateNumber = day.ToString("dd"),
                        DateDay = day.ToString("dddd"),
                        Name = "None Assigned"
                    });
                }
            }
        }

        public async Task GetAllDishes()
        {
            Dishes.Clear();

            string response = await _api.GetAllDishes();
            JObject responseData = JObject.Parse(response);
            foreach (JObject dish in responseData["dishes"])
            {
                Dishes.Add(new DishModel
                {
                    Id = (int)dish["id"],
                    Name = (string)dish["name"],
                    User_Id = (int)dish["user_id"]
                });
            }
        }

        public async Task<bool> AddDishToSchedule(DishModel dish, CalendarDay day)
        {
            string response = string.Empty;
            if (day.Name == "None Assigned") // Add new, else edit
            {
                response = await _api.AddDishToSchedule(dish.Id, day.Date);
            } else
            {
                response = await _api.EditDishOnSchedule(dish.Id, day.Date);
            }

            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            JObject responseData = JObject.Parse(response);
            bool success = (bool)responseData["success"];
            if (!success)
            {
                // error message
                return false;
            }

            // Get this day and update
            CalendarDay? item = CalendarDays.FirstOrDefault(x => x.Date == day.Date);
            item.Name = dish.Name;
            item.DishId = dish.Id;
            item.Completed = false;

            return true;
        }
    }
}
