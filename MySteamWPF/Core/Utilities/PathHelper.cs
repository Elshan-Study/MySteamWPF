using System;
using System.IO;

namespace MySteamWPF.Core.Utilities
{
    public static class PathHelper
    {
        public static string ResolvePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return string.Empty;
            
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDir, relativePath);
        }
    }
}