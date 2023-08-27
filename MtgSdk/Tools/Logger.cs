using System.Reflection;
using System.Runtime.CompilerServices;

namespace MageekSdk.Tools
{

    public static class Logger
    {

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Log(string log, LogLvl lvl = LogLvl.Debug, [CallerFilePath]string fileName="" ,[CallerMemberName] string memberName = "")
        {
            string start = "";
            switch (lvl)
            {
                case LogLvl.Debug: start = "     "; break;
                case LogLvl.Warning: start = " /?\\ "; break;
                case LogLvl.Info: start = "     "; break;
                case LogLvl.Error: start = " /!\\ "; break;
                case LogLvl.Fatal: start = " /X\\ "; break;
                case LogLvl.Trace: start = "     "; break;
            }
            string tolog = string.Concat(
                "     [",
                DateTime.Now,
                "]",
                start,
                fileName.Split('\\').Last().Split(".cs").First() , " :: " ,
                memberName , " : " ,
                log
            );
            switch(lvl)
            {
                case LogLvl.Debug: logger.Debug(tolog); break;
                case LogLvl.Warning: logger.Warn(tolog); break;
                case LogLvl.Info: logger.Info(tolog); break;
                case LogLvl.Error: logger.Error(tolog); break;
                case LogLvl.Fatal: logger.Fatal(tolog); break;
                case LogLvl.Trace: logger.Trace(tolog); break;
            }
            System.Diagnostics.Debug.WriteLine(tolog);
        }

    }

    public enum LogLvl { Debug, Warning, Info, Error, Fatal, Trace }

}
