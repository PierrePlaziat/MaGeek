namespace PlaziatTools
{

    /// <summary>
    /// Common file manipulations
    /// </summary>
    public static class FileUtils
    {


        /// <summary>
        /// Check if the content of two files are identical
        /// </summary>
        /// <param name="file1">Path to a file</param>
        /// <param name="file2">Path to a file</param>
        /// <returns>Same content in both files</returns>
        public static bool ContentDiffers(string file1, string file2)
        {
            return !File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2));
        }

        /// <summary>
        /// Check if a file is created since before or after a given duration 
        /// </summary>
        /// <param name="file">Path to a file</param>
        /// <param name="duration">A given duration</param>
        /// <returns>The file older than the duration</returns>
        public static bool? IsFileOlder(string file, TimeSpan duration)
        {
            if (File.Exists(file))
            {
                var lastWrite = File.GetLastWriteTime(file);
                var diff = DateTime.Now.Subtract(lastWrite);
                return diff > duration;
            }
            else
                return true;
        }

    }

}
