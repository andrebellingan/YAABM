using System;
using NLog;

namespace Yaabm.generic
{
    public static class InternalLog
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Error(Exception exception, string message)
        {
            Logger.Error(exception, message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }
    }
}
