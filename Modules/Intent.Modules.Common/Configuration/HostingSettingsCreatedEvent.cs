namespace Intent.Modules.Common.Configuration
{
    /// <summary>
    /// Subscribe to this event to be notified of hosting settings published by a Module in this application.
    /// </summary>
    public class HostingSettingsCreatedEvent
    {
        public HostingSettingsCreatedEvent(string applicationUrl, int port, int sslPort)
        {
            ApplicationUrl = applicationUrl;
            Port = port;
            SslPort = sslPort;
        }

        public string ApplicationUrl { get; }
        public int Port { get; }
        public int SslPort { get; }
    }
}