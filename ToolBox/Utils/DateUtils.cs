using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.Utils
{
    public static class DateUtils
    {
        public static DateTime NextWeekDay(this DateTime date, DayOfWeek weekDay, bool includeDate = false)
        {
            int daysToAdd = (int)weekDay - (int)date.DayOfWeek + 7;
            return date.AddDays(includeDate ? daysToAdd%7: daysToAdd);
        }

        public static int Years(this TimeSpan span)
        {
            var date = new DateTime(1, 1, 1);
            var age = date + span;
            return age.Year - 1;
        }

        public static int Months(this TimeSpan span)
        {
            var date = new DateTime(1, 1, 1);
            var age = date + span;
            return age.Month - 1;
        }
    }
}
