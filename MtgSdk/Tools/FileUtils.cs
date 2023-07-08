using System;
using System.IO;
using System.Linq;

namespace MtgSqliveSdk.Framework
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
                return DateTime.Now - File.GetCreationTime(fileName) > thresholdAge;
            else
                return null;
        }

    }
}
