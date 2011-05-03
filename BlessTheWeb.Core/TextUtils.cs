using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlessTheWeb.Core
{
    public static class TextUtils
    {
        public static string DayOfMonth(DateTime dateConfessed)
        {
            if (dateConfessed.Day > 10 && dateConfessed.Day < 14)
                return (dateConfessed.Day + "th");
            if (dateConfessed.Day.ToString().EndsWith("1"))
                return (dateConfessed.Day + "st");
            if (dateConfessed.Day.ToString().EndsWith("2"))
                return dateConfessed.Day + "nd";
            if (dateConfessed.Day.ToString().EndsWith("3"))
                return dateConfessed.Day + "rd";
            return dateConfessed.Day + "th";
        }
    }
}
