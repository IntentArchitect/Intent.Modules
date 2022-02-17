using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// For adding a connection string to a .NET configuration file. If the configuration file
    /// already contains a connection string with the provided <see cref="Name"/>, it is left
    /// untouched.
    /// </summary>
    /// <remarks>
    /// Templates for appsettings[.&lt;<see cref="RuntimeEnvironment"/>&gt;].json and
    /// Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config files may listen for these
    /// requests and update themselves accordingly.
    /// </remarks>
    public class ConnectionStringRegistrationRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConnectionStringRegistrationRequest"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload so that there is a single constructor with optional parameters.")]
        public ConnectionStringRegistrationRequest(string name, string connectionString, string providerName)
            : this(name, connectionString, providerName, null, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ConnectionStringRegistrationRequest"/>.
        /// </summary>
        [FixFor_Version4("Make the 'runtimeEnvironment' and 'forProjectWithRole' parameters have a default value of null")]
        public ConnectionStringRegistrationRequest(string name, string connectionString, string providerName, string runtimeEnvironment, string forProjectWithRole)
        {
            Name = name;
            ConnectionString = connectionString;
            ProviderName = providerName;
            RuntimeEnvironment = runtimeEnvironment;
            ForProjectWithRole = forProjectWithRole;
        }

        /// <summary>
        /// The name of the connection string.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The connection string value.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Only applicable for Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config files
        /// and is used to populate the "providerName" attribute.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Optional. The specific appsettings.&lt;<see cref="RuntimeEnvironment"/>&gt;.json or
        /// Web/App.&lt;<see cref="RuntimeEnvironment"/>&gt;.config file to apply to, when not
        /// specified then the default appsettings.json or Web/App.config file is applied to.
        /// </summary>
        public string RuntimeEnvironment { get; set; }

        /// <summary>
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// </summary>
        /// <remarks>
        /// Used for disambiguating which appsettings.&lt;<see cref="RuntimeEnvironment"/>&gt;.json
        /// or Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config file to apply to when a
        /// solution has multiple projects each with their own configuration file(s).
        /// </remarks>
        public string ForProjectWithRole { get; set; }

        /// <summary>
        /// Whether or not <see cref="MarkHandled"/> has been called.
        /// </summary>
        public bool WasHandled { get; private set; }

        /// <summary>
        /// Sets <see cref="WasHandled"/> to <see langword="true"/> if it wasn't already.
        /// </summary>
        /// <remarks>
        /// Should be called by handlers so that publishers can query <see cref="WasHandled"/>
        /// and potentially show warnings when its value is <see langword="false"/>.
        /// </remarks>
        public void MarkHandled() => WasHandled = true;
    }
}
