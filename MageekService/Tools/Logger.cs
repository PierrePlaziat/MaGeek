//using System.Runtime.CompilerServices;

//namespace MageekService.Tools
//{

//    public static class Logger
//    {

//        public static void Log(string log, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
//        {
//            string msg = string.Concat(
//                fileName.Split('/').Last().Split(".cs").First(), " :: ",
//                memberName, " : ",
//                log
//            );
//            Console.WriteLine(msg);
//        }

//        public static void Log(Exception e, [CallerFilePath] string fileName = "", [CallerMemberName] string memberName = "")
//        {
//            string msg = string.Concat(
//                fileName.Split('\\').Last().Split(".cs").First(), " :: ",
//                memberName, " : /!\\ ERROR /!\\ ",
//                e.Message
//            );
//            Console.WriteLine(msg);
//        }

//    }


//}
