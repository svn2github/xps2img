using System;
using System.IO;

namespace Xps2Img.Shared.Utils
{
    public static class PathUtils
    {
        public static bool TryGetAbsolutePath(string path, out string absolutePath)
        {
            try
            {
                absolutePath = GetAbsolutePath(path);
                return true;
            }
            catch
            {
                absolutePath = path;
                return false;
            }
        }

        public static bool TryGetAbsolutePath(string basePath, string path, out string absolutePath)
        {
            try
            {
                absolutePath = GetAbsolutePath(basePath, path);
                return true;
            }
            catch
            {
                absolutePath = path;
                return false;
            }
        }

        public static string GetAbsolutePath(string path)
        {
            return GetAbsolutePath(null, path);
        }

        public static string GetAbsolutePath(string basePath, string path)
        {
            var currentDirectory = Path.GetFullPath(".");

            if (String.IsNullOrEmpty(path))
            {
                return currentDirectory;
            }

            basePath = basePath == null ? currentDirectory : GetAbsolutePath(null, basePath);

            var root = Path.GetPathRoot(path);

            var directorySeparatorChar = Path.DirectorySeparatorChar.ToString();
            var altDirectorySeparatorChar = Path.AltDirectorySeparatorChar.ToString();

            // ReSharper disable once AssignNullToNotNullAttribute
            var finalPath =
                !Path.IsPathRooted(path) || String.Equals(directorySeparatorChar, root, StringComparison.Ordinal) ||
                String.Equals(altDirectorySeparatorChar, root, StringComparison.Ordinal)
                    ? path.StartsWith(directorySeparatorChar) || path.StartsWith(altDirectorySeparatorChar)
                        ? Path.Combine(Path.GetPathRoot(basePath),
                            path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                        : Path.Combine(basePath, path)
                    : path;

            return Path.GetFullPath(finalPath);
        }
    }
}
