using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaziatCore
{

    public enum LogLevels {Infos, Error}

    /// <summary>
    /// A custom logger
    /// </summary>
    public static class Logger
    {

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="sourceFile">Automatically retrieved, dont fill it</param>
        /// <param name="memberName">Automatically retrieved, dont fill it</param>
        public static void Log(string message, LogLevels lvl = LogLevels.Infos , [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            string msg = string.Concat(
                DateTime.Now," | ",lvl," | ", 
                sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), " - ",
                memberName, " | ",
                message
            );
            Console.WriteLine(msg);
            Trace.WriteLine(msg);
        }

        /// <summary>
        /// Log an exception
        /// </summary>
        /// <param name="exception">Exception to log</param>
        /// <param name="inner">Also log inner exceptions recursively</param>
        /// <param name="sourceFile">Automatically retrieved, dont fill it</param>
        /// <param name="memberName">Automatically retrieved, dont fill it</param>
        public static void Log(Exception exception, bool inner = false, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            Log(exception.Message,LogLevels.Error,sourceFile, memberName);
            if (inner && exception.InnerException != null ) { Log(exception.InnerException); }
        }

    }


}
