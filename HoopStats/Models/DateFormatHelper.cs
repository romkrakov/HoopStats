using System;

namespace HoopStats.Models
{
    /// <summary>
    /// Helper class for formatting dates in a consistent way across the application.
    /// Use this class instead of extension methods when working with dynamic objects.
    /// </summary>
    public static class DateFormatHelper
    {
        /// <summary>
        /// Formats a date in the standard application format (dd/MM/yyyy)
        /// This method can be used with dynamic objects where extension methods may not work.
        /// </summary>
        /// <param name="dateTime">The DateTime to format</param>
        /// <returns>Formatted date string</returns>
        public static string FormatDate(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }
        
        /// <summary>
        /// Formats a date in the standard application format (dd/MM/yyyy)
        /// This overload is safer for use with dynamic objects and views.
        /// </summary>
        /// <param name="dateTimeObj">The DateTime object (can be dynamic)</param>
        /// <returns>Formatted date string</returns>
        public static string FormatDate(object dateTimeObj)
        {
            if (dateTimeObj is DateTime dateTime)
            {
                return dateTime.ToString("dd/MM/yyyy");
            }
            
            return string.Empty;
        }
    }
}
