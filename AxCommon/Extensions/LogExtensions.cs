using System;
using Serilog;

namespace Aximo
{
    public static class LogExtensions
    {

        #region Info Alias

        public static void Info<T0, T1>(this ILogger log, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            log.Information(messageTemplate, propertyValue0, propertyValue1);
        }
        public static void Info(this ILogger log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log.Information(exception, messageTemplate, propertyValues);
        }
        public static void Info<T0, T1, T2>(this ILogger log, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            log.Information(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        public static void Info<T0, T1>(this ILogger log, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            log.Information(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        public static void Info<T>(this ILogger log, Exception exception, string messageTemplate, T propertyValue)
        {
            log.Information(exception, messageTemplate, propertyValue);
        }
        public static void Info(this ILogger log, Exception exception, string messageTemplate)
        {
            log.Information(exception, messageTemplate);
        }
        public static void Info(this ILogger log, string messageTemplate, params object[] propertyValues)
        {
            log.Information(messageTemplate, propertyValues);
        }
        public static void Info<T0, T1, T2>(this ILogger log, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            log.Information(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        public static void Info<T>(this ILogger log, string messageTemplate, T propertyValue)
        {
            log.Information(messageTemplate, propertyValue);
        }
        public static void Info(this ILogger log, string messageTemplate)
        {
            log.Information(messageTemplate);
        }

        #endregion

        #region Warn Alias

        public static void Warn<T0, T1>(this ILogger log, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            log.Warning(messageTemplate, propertyValue0, propertyValue1);
        }
        public static void Warn(this ILogger log, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            log.Warning(exception, messageTemplate, propertyValues);
        }
        public static void Warn<T0, T1, T2>(this ILogger log, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            log.Warning(exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        public static void Warn<T0, T1>(this ILogger log, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            log.Warning(exception, messageTemplate, propertyValue0, propertyValue1);
        }
        public static void Warn<T>(this ILogger log, Exception exception, string messageTemplate, T propertyValue)
        {
            log.Warning(exception, messageTemplate, propertyValue);
        }
        public static void Warn(this ILogger log, Exception exception, string messageTemplate)
        {
            log.Warning(exception, messageTemplate);
        }
        public static void Warn(this ILogger log, string messageTemplate, params object[] propertyValues)
        {
            log.Warning(messageTemplate, propertyValues);
        }
        public static void Warn<T0, T1, T2>(this ILogger log, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            log.Warning(messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        public static void Warn<T>(this ILogger log, string messageTemplate, T propertyValue)
        {
            log.Warning(messageTemplate, propertyValue);
        }
        public static void Warn(this ILogger log, string messageTemplate)
        {
            log.Warning(messageTemplate);
        }

        #endregion

        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Error(exception.ToString());
        }

        public static ILogger ForContext(this ILogger logger, string sourceContext)
        {
            return logger.ForContext("SourceContext", sourceContext);
        }

    }
}
