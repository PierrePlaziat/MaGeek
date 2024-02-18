using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MageekCore.Tools
{

    public static class Logger
    {

        public static void Log(string log, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
        {
            string msg = string.Concat(
                "[",DateTime.Now,"] ",
                fileName.Split('\\').Last().Split('/').Last().Split(".cs").First(), " :: ",
                memberName, " : ",
                log
            );
            Trace.WriteLine(msg);
        }

        public static void Log(Exception e, bool showInner = false, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
        {
            string msg = string.Concat(
                "[", DateTime.Now, "] ",
                fileName.Split('\\').Last().Split('/').Last().Split(".cs").First(), " :: ",
                memberName, " : ERROR - ",
                e.Message
            );
            Trace.WriteLine(msg);
            if (showInner && e.InnerException != null ) { Log(e.InnerException); }
        }

    }


}
