using System.Globalization;

namespace FinanceHub.Core.Common.Helpers
{
 /// <summary>
 /// Decimal parsing helpers for Portuguese locale inputs (e.g., "1.234,56").
 /// </summary>
 public static class DecimalHelper
 {
 public static decimal? ParsePtNullable(string? s)
 {
 if (string.IsNullOrWhiteSpace(s)) return null;
 s = s.Trim().Replace(".", string.Empty).Replace(",", ".");
 return decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : (decimal?)null;
 }

 public static decimal ParsePt(string s)
 {
 var v = ParsePtNullable(s);
 return v ??0m;
 }
 }
}
