using Microsoft.Extensions.Logging;
using System;

namespace crozone.Logging.AdbLogger
{
    /// <summary>
    /// The provider for the <see cref="AdbLogger"/>.
    /// </summary>
    [ProviderAlias("Adb")]
    public class AdbLoggerProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, bool> filter;
        private readonly string tag;

        public AdbLoggerProvider(string tag)
        {
            this.tag = tag;
            filter = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugLoggerProvider"/> class.
        /// </summary>
        /// <param name="filter">The function used to filter events based on the log level.</param>
        public AdbLoggerProvider(string tag, Func<string, LogLevel, bool> filter)
        {
            this.tag = tag;
            this.filter = filter;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string name)
        {
            return new AdbLogger(tag, name, filter);
        }

        public void Dispose()
        {
        }
    }
}