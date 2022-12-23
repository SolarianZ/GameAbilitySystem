using System;

namespace GBG.GameAbilitySystem
{
    public interface ILogger
    {
        void LogInfo(string message, object? context = null);

        void LogWarning(string message, object? context = null);

        void LogError(string message, object? context = null);

        void LogFatal(string message, object? context = null);
    }

    public class DefaultConsoleLogger : ILogger
    {
        public void LogInfo(string message, object? context = null)
        {
            Console.WriteLine($"[INFO] {message}");
        }

        public void LogWarning(string message, object? context = null)
        {
            Console.WriteLine($"[WARN] {message}");
        }

        public void LogError(string message, object? context = null)
        {
            Console.WriteLine($"[ERROR] {message}");
        }

        public void LogFatal(string message, object? context = null)
        {
            Console.WriteLine($"[FATAL] {message}");
        }
    }

    public static class Logger
    {
        private static ILogger _logger = new DefaultConsoleLogger();

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static void LogInfo(string message, object? context = null)
        {
            _logger.LogInfo(message, context);
        }

        public static void LogWarning(string message, object? context = null)
        {
            _logger.LogWarning(message, context);
        }

        public static void LogError(string message, object? context = null)
        {
            _logger.LogError(message, context);
        }

        public static void LogFatal(string message, object? context = null)
        {
            _logger.LogFatal(message, context);
        }
    }
}