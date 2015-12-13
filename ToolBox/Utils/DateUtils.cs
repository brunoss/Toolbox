using System;

namespace ToolBox.Utils
{
    public class Age
    {
        public int Years { get; set; }
        public int Months { get; set; }
    }
    public static class DateUtils
    {
        public static DateTime NextWeekDay(this DateTime date, DayOfWeek weekDay, bool includeDate = false)
        {
            int daysToAdd = (int)weekDay - (int)date.DayOfWeek + 7;
            return date.AddDays(includeDate ? daysToAdd%7: daysToAdd);
        }
        

        public static Age Age(this TimeSpan span)
        {

            var date = new DateTime(1, 1, 1);
            var age = date + span;
            return new Age {Years = age.Year - 1, Months = age.Month - 1};
        }
    }
}
