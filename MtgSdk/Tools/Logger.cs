using System.Reflection;
using System.Runtime.CompilerServices;

namespace MageekSdk.Tools
{

    public static class Logger
    {

        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Log(string log, LogLvl lvl = LogLvl.Debug,[CallerMemberName] string memberName = "")
        {
            string tolog =
                MethodBase.GetCurrentMethod()?.DeclaringType?.Name + " :: " +
                memberName + " : " +
                log;
            switch (lvl)
            {
                case LogLvl.Debug: logger.Debug(tolog); break;
                case LogLvl.Warning: logger.Warn(tolog); break;
                case LogLvl.Info: logger.Info(tolog); break;
                case LogLvl.Error: logger.Error(tolog); break;
                case LogLvl.Fatal: logger.Fatal(tolog); break;
                case LogLvl.Trace: logger.Trace(tolog); break;
            }

        }

    }

    public enum LogLvl { Debug, Warning, Info, Error, Fatal, Trace }

}
