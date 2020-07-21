using System;
using Serilog;

namespace Yaabm.generic
{
    public static class InternalLog
    {
        public static void Error(Exception exception, string message)
        {
            Log.Error(exception, message);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Info(string message)
        {
            Log.Information(message);
        }
    }
}
