namespace FinanceHub.Core.Common.Helpers
{
 /// <summary>
 /// File system helper operations.
 /// </summary>
 public static class FileHelper
 {
 public static void EnsureDirectory(string path)
 {
 if (!Directory.Exists(path)) Directory.CreateDirectory(path);
 }

 public static void MoveFile(string sourcePath, string destinationFolder)
 {
 EnsureDirectory(destinationFolder);
 var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(sourcePath));
 File.Move(sourcePath, destinationPath, true);
 }
 }
}
