using System;

namespace HoopStats.Models
{
    public static class DateTimeExtensions
    {
        public static string ToCustomDateString(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
    }
}
