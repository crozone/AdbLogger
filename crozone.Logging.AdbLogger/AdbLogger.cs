using Microsoft.Extensions.Logging;

using System;

namespace crozone.Logging.AdbLogger
{
    /// <summary>
    /// A logger that writes messages to the Android log buffer
    /// </summary>
    public partial class AdbLogger : ILogger
    {
        private readonly Func<string, LogLevel, bool>? filter;
        private readonly string name;
        private readonly string tag;

        internal IExternalScopeProvider? ScopeProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbLogger"/> class.
        /// </summary>
        /// <param name="tag">The tag to be used in the Android logging utility.</param>
        /// <param name="name">The name of the logger.</param>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public AdbLogger(string tag, string name, IExternalScopeProvider? scopeProvider, Func<string, LogLevel, bool>? filter)
        {
            this.tag = tag ?? throw new ArgumentNullException(nameof(tag));
            this.name = string.IsNullOrEmpty(name) ? nameof(AdbLogger) : name;
            this.ScopeProvider = scopeProvider;
            this.filter = filter;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            // If the filter is null, everything is enabled
            // unless the debugger is not attached
            return logLevel != LogLevel.None && (filter == null || filter(name, logLevel));
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            string message = formatter(state, exception) ?? "";

            Java.Lang.Throwable? throwable = exception != null ? Java.Lang.Throwable.FromException(exception) : null;

            // The throwable parameter can be null, but the log methods aren't annotated for nullability.
            switch (logLevel)
            {
                case LogLevel.Trace:
                    Android.Util.Log.Verbose(tag, throwable!, message);
                    break;
                case LogLevel.Debug:
                    Android.Util.Log.Debug(tag, throwable!, message);
                    break;
                case LogLevel.Information:
                    Android.Util.Log.Info(tag, throwable!, message);
                    break;
                case LogLevel.Warning:
                    Android.Util.Log.Warn(tag, throwable!, message);
                    break;
                case LogLevel.Error:
                    Android.Util.Log.Error(tag, throwable!, message);
                    break;
                case LogLevel.Critical:
                    Android.Util.Log.Wtf(tag, throwable!, message);
                    break;
                case LogLevel.None:
                    break;
                default:
                    break;
            }
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state) ?? NullScope.Instance;
    }
}