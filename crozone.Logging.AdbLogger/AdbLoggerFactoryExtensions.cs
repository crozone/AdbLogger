using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using System;

namespace crozone.Logging.AdbLogger
{
    /// <summary>
    /// Extension methods for the <see cref="ILoggerFactory"/> class.
    /// </summary>
    public static class AdbLoggerFactoryExtensions
    {
        /// <summary>
        /// Adds a debug logger named 'Adb' to the factory.
        /// </summary>
        /// <param name="builder">The extension method argument.</param>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        public static ILoggingBuilder AddAdb(this ILoggingBuilder builder, string tag)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AdbLoggerProvider>(
                (_) => new AdbLoggerProvider(tag)
                ));

            return builder;
        }

        /// <summary>
        /// Adds an Adb logger that is enabled for <see cref="Microsoft.Extensions.Logging.LogLevel"/>.Information or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        public static ILoggerFactory AddAdb(this ILoggerFactory factory, string tag)
        {
            return AddAdb(factory, tag, LogLevel.Information);
        }

        /// <summary>
        /// Adds an Adb logger that is enabled as defined by the filter function.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        public static ILoggerFactory AddAdb(this ILoggerFactory factory, string tag, Func<string, LogLevel, bool> filter)
        {
            factory.AddProvider(new AdbLoggerProvider(tag, filter));
            return factory;
        }

        /// <summary>
        /// Adds an Adb logger that is enabled for <see cref="Microsoft.Extensions.Logging.LogLevel"/>s of minLevel or higher.
        /// </summary>
        /// <param name="factory">The extension method argument.</param>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        /// <param name="minLevel">The minimum <see cref="Microsoft.Extensions.Logging.LogLevel"/> to be logged.</param>
        public static ILoggerFactory AddAdb(this ILoggerFactory factory, string tag, LogLevel minLevel)
        {
            return AddAdb(
               factory,
               tag,
               (_, logLevel) => logLevel >= minLevel);
        }
    }

}