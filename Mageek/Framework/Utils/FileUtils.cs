using System;
using System.IO;
using System.Linq;

namespace MaGeek.Framework.Utils
{
    public static class FileUtils
    {
        public static bool FileContentDiffers(string newHashPath, string oldHashPath)
        {
            return !File.ReadAllBytes(newHashPath).SequenceEqual(File.ReadAllBytes(oldHashPath));
        }

    }
}
