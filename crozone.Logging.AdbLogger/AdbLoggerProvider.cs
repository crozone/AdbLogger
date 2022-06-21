using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;

namespace crozone.Logging.AdbLogger
{
    /// <summary>
    /// The provider for the <see cref="AdbLogger"/>.
    /// </summary>
    [ProviderAlias("Adb")]
    public class AdbLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly Func<string, LogLevel, bool>? filter;
        private readonly string tag;
        private readonly ConcurrentDictionary<string, AdbLogger> loggers;
        private IExternalScopeProvider scopeProvider = NullExternalScopeProvider.Instance;

        public AdbLoggerProvider(string tag) : this(tag, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdbLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public AdbLoggerProvider(string tag, Func<string, LogLevel, bool>? filter)
        {
            this.tag = tag;
            this.filter = filter;

            loggers = new ConcurrentDictionary<string, AdbLogger>();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return loggers.TryGetValue(name, out AdbLogger? logger)
                ? logger
                : loggers.GetOrAdd(name, new AdbLogger(tag, name, scopeProvider, filter));
        }

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider;

            foreach (System.Collections.Generic.KeyValuePair<string, AdbLogger> logger in loggers)
            {
                logger.Value.ScopeProvider = scopeProvider;
            }
        }
    }
}