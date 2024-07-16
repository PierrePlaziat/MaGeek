using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaziatTools
{

    public enum LogLevels {Infos, Warning, Error}

    ///// <summary>
    ///// A custom logger
    ///// </summary>
    public static class Logger
    {

        const string ErrorSymbol = " /!\\ ";
        const string WarningSymbol = "  -  ";

        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="sourceFile">Automatically retrieved, dont fill it</param>
        /// <param name="memberName">Automatically retrieved, dont fill it</param>
        public static void Log(string message, LogLevels lvl = LogLevels.Infos, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            
            string msg = string.Concat(
                DateTime.Now, " ", lvl == LogLevels.Error ? ErrorSymbol : lvl == LogLevels.Warning ? WarningSymbol : "     " , " ",
                sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), ".",memberName, " | ", message
            );
            WriteLine(msg);
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
            Log(exception.Message, LogLevels.Error, sourceFile, memberName);
            if (inner && exception.InnerException != null) { Log(exception.InnerException); }
        }

        private static void WriteLine(string message)
        {
            Console.WriteLine(message);
            Trace.WriteLine(message);
            using (StreamWriter file = new StreamWriter("PATH", true))
            {
                file.WriteLine(message);
            }
        }

    }

    /// <summary>
    /// A custom logger
    /// </summary>
    //public class Logger
    //{

    //    private string title;
    //    Queue<string> messages = new Queue<string>();
    //    private bool stopped;

    //    public Logger(string title)
    //    {
    //        this.title = title;
    //        stopped = false;
    //        Loop();
    //    }

    //    private async void Loop()
    //    {
    //        while (!stopped)
    //        {
    //            await Task.Delay(200);
    //            WriteLine(messages.Dequeue());
    //        }
    //    }

    //    private void WriteLine(string message)
    //    {
    //        Console.WriteLine(message);
    //        //Trace.WriteLine(message);
    //        using (StreamWriter file = new StreamWriter("PATH", true))
    //        {
    //            file.WriteLine(message);
    //        }
    //    }


    //    public void Log(string message, LogLevels lvl = LogLevels.Infos , [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
    //    {
    //        string msg = string.Concat(
    //            DateTime.Now," | ",lvl," | ", 
    //            sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), " - ",
    //            memberName, " | ",
    //            message
    //        );
    //        messages.Enqueue(msg);
    //    }

    //    public void Log(Exception exception, bool inner = false, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
    //    {
    //        Log(exception.Message,LogLevels.Error,sourceFile, memberName);
    //        if (inner && exception.InnerException != null ) { Log(exception.InnerException); }
    //    }

    //}

}
