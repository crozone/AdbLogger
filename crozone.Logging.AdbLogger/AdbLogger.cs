using Microsoft.Extensions.Logging;
using System;

namespace crozone.Logging.AdbLogger
{
    /// <summary>
    /// A logger that writes messages to the Android log buffer
    /// </summary>
    public partial class AdbLogger : ILogger
    {
        private readonly Func<string, LogLevel, bool> filter;
        private readonly string name;
        private readonly string tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbLogger"/> class.
        /// </summary>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        /// <param name="name">The name of the logger.</param>
        public AdbLogger(string tag, string name) : this(tag, name, filter: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbLogger"/> class.
        /// </summary>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public AdbLogger(string tag, string name, Func<string, LogLevel, bool> filter)
        {
            this.tag = tag ?? throw new ArgumentNullException(nameof(tag));
            this.name = string.IsNullOrEmpty(name) ? nameof(AdbLogger) : name;
            this.filter = filter;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return logLevel != LogLevel.None && (filter == null || filter(name, logLevel));
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            message = $"{ logLevel }: {message}";

            if (exception != null)
            {
                message += Environment.NewLine + Environment.NewLine + exception.ToString();
            }

            switch (logLevel)
            {
                case LogLevel.Trace:
                    Android.Util.Log.Verbose(tag, message);
                    break;
                case LogLevel.Debug:
                    Android.Util.Log.Debug(tag, message);
                    break;
                case LogLevel.Information:
                    Android.Util.Log.Info(tag, message);
                    break;
                case LogLevel.Warning:
                    Android.Util.Log.Warn(tag, message);
                    break;
                case LogLevel.Error:
                    Android.Util.Log.Error(tag, message);
                    break;
                case LogLevel.Critical:
                    Android.Util.Log.Wtf(tag, message);
                    break;
                case LogLevel.None:
                    break;
                default:
                    Android.Util.Log.Info(tag, message);
                    break;
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }

}