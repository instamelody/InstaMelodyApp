using System;
using NLog;

namespace InstaMelody.Infrastructure
{
    public static class InstaMelodyLogger
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        public static void Log(string message, LogLevel level)
        {
            logger.Log(level, message);
        }

        /// <summary>
        /// Tests the method.
        /// </summary>
        public static void TestMethod()
        {
            logger.Trace("Sample trace message");
            logger.Debug("Sample debug message");
            logger.Info("Sample informational message");
            logger.Warn("Sample warning message");
            logger.Error("Sample error message");
            logger.Fatal("Sample fatal error message");

            // alternatively you can call the Log() method 
            // and pass log level as the parameter.
            logger.Log(LogLevel.Info, "Sample informational message");
        }
    }
}
