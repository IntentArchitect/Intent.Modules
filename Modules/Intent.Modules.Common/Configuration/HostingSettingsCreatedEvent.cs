namespace Intent.Modules.Common.Configuration
{
    /// <summary>
    /// Subscribe to this event to be notified of hosting settings published by a Module in this application.
    /// </summary>
    public class HostingSettingsCreatedEvent
    {
        /// <summary>
        /// Creates a new instance of <see cref="HostingSettingsCreatedEvent"/>.
        /// </summary>
        public HostingSettingsCreatedEvent(string applicationUrl, int port, int sslPort)
        {
            ApplicationUrl = applicationUrl;
            Port = port;
            SslPort = sslPort;
        }

        /// <summary>
        /// Application URL
        /// </summary>
        public string ApplicationUrl { get; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// SSL port
        /// </summary>
        public int SslPort { get; }
    }
}