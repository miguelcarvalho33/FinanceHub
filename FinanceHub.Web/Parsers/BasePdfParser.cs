using System.Globalization;
using FinanceHub.Core.Common.Helpers;

namespace FinanceHub.Web.Parsers
{
 /// <summary>
 /// Base class for PDF parsers providing shared helpers.
 /// </summary>
 public abstract class BasePdfParser
 {
 protected static decimal? ParsePtDecimal(string s) => DecimalHelper.ParsePtNullable(s);

 protected static string NormalizeText(string? s)
 => string.IsNullOrWhiteSpace(s) ? string.Empty : RegexHelper.MultiSpace().Replace(s, " ").Trim();

 protected static IEnumerable<string> SplitLines(string text)
 => text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
 }
}
