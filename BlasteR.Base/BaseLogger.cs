using System;
using System.Runtime.CompilerServices;

namespace BlasteR.Base
{
    public enum LogLevel
    {
        Debug,
        Info,
        Error
    }

    public static class BaseLogger
    {
        public delegate DateTime LogMethodStartDelegate(object caller = null, [CallerMemberName] string methodName = "");
        public delegate void LogMethodEndDelegate(object caller = null, DateTime? startOfTheMethod = null, [CallerMemberName] string methodName = "");
        public delegate void LogDelegate(LogLevel logLevel, string message, Exception exception = null);

        public static event LogMethodStartDelegate OnLogMethodStart;
        public static event LogMethodEndDelegate OnLogMethodEnd;
        public static event LogDelegate OnLog;

        public static DateTime LogMethodStart(object caller = null, [CallerMemberName] string methodName = "")
        {
            if (OnLogMethodStart != null)
                return OnLogMethodStart(caller, methodName);
            else
                return DateTime.Now;
        }

        public static void LogMethodEnd(object caller = null, DateTime? startOfTheMethod = null, [CallerMemberName] string methodName = "")
        {
            if (OnLogMethodEnd != null)
                OnLogMethodEnd(caller, startOfTheMethod, methodName);
        }

        public static void Log(LogLevel logLevel, string message, Exception exception = null)
        {
            if (OnLog != null)
                OnLog(logLevel, message, exception);
        }
    }
}
