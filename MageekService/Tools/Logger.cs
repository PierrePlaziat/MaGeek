using System.Reflection;
using System.Runtime.CompilerServices;

namespace MageekService.Tools
{

    public static class Logger
    {

        public static void Log(string log, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
        {
            string start = "";
            string tolog = string.Concat(
                "     [",
                DateTime.Now,
                "]",
                start,
                fileName.Split('\\').Last().Split(".cs").First(), " :: ",
                memberName, " : ",
                log
            );
            System.Diagnostics.Debug.WriteLine(tolog);
        }

        public static void Log(Exception e, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
        {
            string start = "";
            string tolog = string.Concat(
                "     [",
                DateTime.Now,
                "]",
                start,
                fileName.Split('\\').Last().Split(".cs").First(), " :: ",
                memberName, " : /!\\ ERROR /!\\ ",
                e.Message
            );
            System.Diagnostics.Debug.WriteLine(tolog);
        }

    }


}
