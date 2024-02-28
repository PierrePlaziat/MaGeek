using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaziatTools
{

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
        public static void Log(string message, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            string msg = string.Concat(
                "[",DateTime.Now,"] ",
                sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), " :: ",
                memberName, " : ",
                message
            );
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
            string msg = string.Concat(
                "[", DateTime.Now, "] ",
                sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), " :: ",
                memberName, " : ERROR - ",
                exception.Message
            );
            Trace.WriteLine(msg);
            if (inner && exception.InnerException != null ) { Log(exception.InnerException); }
        }

    }


}
