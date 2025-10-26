using System.Security.Cryptography;
using System.Text;
using FinanceHub.Core.Models;

namespace FinanceHub.Core.Common.Helpers
{
 /// <summary>
 /// Hashing helper for domain entities.
 /// </summary>
 public static class HashingHelper
 {
 public static string ComputeTransactionHash(Transaction transaction)
 {
 var input = $"{transaction.MovementDate:yyyy-MM-dd}-{transaction.Amount:F2}-{transaction.OriginalDescription}-{transaction.Bank}";
 using var sha256 = SHA256.Create();
 var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
 return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
 }
 }
}
