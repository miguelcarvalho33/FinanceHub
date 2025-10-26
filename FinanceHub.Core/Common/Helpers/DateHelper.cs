using System.Globalization;

namespace FinanceHub.Core.Common.Helpers
{
 /// <summary>
 /// Date parsing helpers for statements.
 /// </summary>
 public static class DateHelper
 {
 public static DateTime FromDayMonth(int day, int month, int year)
 => new DateTime(year, month, day);

 public static DateTime FromStringDayMonth(string ddmm, int year)
 {
 return DateTime.ParseExact(ddmm + "-" + year.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
 }
 }
}
