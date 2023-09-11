using MageekSdk.Tools;
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
            {
                Logger.Log("File.GetCreationTime(fileName) : " + File.GetCreationTime(fileName));
                Logger.Log("DateTime.Now.Subtract(File.GetCreationTime(fileName)) : " + DateTime.Now.Subtract(File.GetCreationTime(fileName)));
                Logger.Log("thresholdAge : " + thresholdAge);
                Logger.Log("DateTime.Now.Subtract(File.GetCreationTime(fileName)) > thresholdAge : " + (DateTime.Now.Subtract(File.GetCreationTime(fileName)) > thresholdAge));
                return DateTime.Now.Subtract(File.GetCreationTime(fileName)) > thresholdAge;

            }
            else
                return null;
        }

    }
}
