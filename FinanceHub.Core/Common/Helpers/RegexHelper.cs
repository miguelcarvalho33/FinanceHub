using System.Text.RegularExpressions;

namespace FinanceHub.Core.Common.Helpers
{
 /// <summary>
 /// Common compiled regex utilities.
 /// </summary>
 public static partial class RegexHelper
 {
 [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
 public static partial Regex MultiSpace();
 }
}
