using System.Collections.Generic;
using System.IO;

namespace Ariadne.Extensions.FileSystemInfo
{
    public static class FileSystemInfoExtensions
    {
        public static DirectoryInfo Directory(this DirectoryInfo directoryInfo, string name) => new DirectoryInfo(Path.Combine(directoryInfo.FullName, name));

        public static FileInfo File(this DirectoryInfo directoryInfo, string name) => new FileInfo(Path.Combine(directoryInfo.FullName, name));

        public static string ReadAllText(this FileInfo fileInfo) => System.IO.File.ReadAllText(fileInfo.FullName);

        public static string[] ReadAllLines(this FileInfo fileInfo) => System.IO.File.ReadAllLines(fileInfo.FullName);

        public static string NameWithoutExtension(this FileInfo fileInfo) => Path.GetFileNameWithoutExtension(fileInfo.FullName);

        public static void WriteAllText(this FileInfo fileInfo, string contents) => System.IO.File.WriteAllText(fileInfo.FullName, contents);

        public static void WriteAllLines(this FileInfo fileInfo, IEnumerable<string> contents) => System.IO.File.WriteAllLines(fileInfo.FullName, contents);

        public static FileInfo ChangeExtension(this FileInfo fileInfo, string extension) => new FileInfo(Path.ChangeExtension(fileInfo.FullName, extension));
    }
}
