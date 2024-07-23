using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PlaziatTools
{

    public enum LogLevels {Infos, Warning, Error}

    public static class Logger
    {

        const string errorSymbol = " /!\\ ";
        const string WarningSymbol = "  -  ";
        static Queue<string> messages = new Queue<string>();

        static bool stopped = true;

        public static void Log(string message, LogLevels lvl = LogLevels.Infos, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            string provenance = string.Concat(sourceFile.Split('\\').Last().Split('/').Last().Split(".cs").First(), ".", memberName);
            if (provenance.Count() > 30) provenance = provenance.Substring(0,30);
            provenance = provenance.PadRight(30,' ');
            string msg = string.Concat(
                DateTime.Now, " ", lvl == LogLevels.Error ? errorSymbol : lvl == LogLevels.Warning ? WarningSymbol : "     " , " ",
                provenance, " | ", message
            );
            WriteLine(msg).ConfigureAwait(false);
        }

        public static void Log(Exception exception, bool inner = false, [CallerFilePath] string sourceFile = "", [CallerMemberName] string memberName = "")
        {
            Log(exception.Message, LogLevels.Error, sourceFile, memberName);
            if (inner && exception.InnerException != null) { Log(exception.InnerException); }
        }

        private async static Task WriteLine(string message)
        {
            Console.WriteLine(message);
            Trace.WriteLine(message);
            using (StreamWriter file = new StreamWriter("PATH", true))
            {
                await file.WriteLineAsync(message);
            }
        }


    }

}
