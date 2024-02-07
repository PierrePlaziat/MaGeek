using System;
using System.IO;
using System.Linq;

namespace MageekServices.Tools
{
    public static class FileUtils
    {
        public static bool FileContentDiffers(string newHashPath, string oldHashPath)
        {
            return !File.ReadAllBytes(newHashPath).SequenceEqual(File.ReadAllBytes(oldHashPath));
        }
        public static bool? IsFileOlder(string fileName, TimeSpan thresholdAge)
        {
            if (File.Exists(fileName))
            {
                var lastWrite = File.GetLastWriteTime(fileName);
                var diff = DateTime.Now.Subtract(lastWrite);
                return diff > thresholdAge;
            }
            else
                return null;
        }

    }
}
