using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// For adding a section or key to a .NET configuration file. If the configuration file already
    /// contains a section or key with the provided <see cref="Key"/>, it is left untouched.
    /// </summary>
    /// <remarks>
    /// Templates for appsettings[.&lt;<see cref="RuntimeEnvironment"/>&gt;].json and
    /// Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config files may listen for these
    /// requests and update themselves accordingly.
    /// </remarks>
    public class AppSettingRegistrationRequest : IForProjectWithRoleRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="AppSettingRegistrationRequest"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload so that there is a single constructor with optional parameters.")]
        public AppSettingRegistrationRequest(string key, string value) : this(key, value, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AppSettingRegistrationRequest"/>.
        /// </summary>
        [FixFor_Version4("Remove this constructor overload so that there is a single constructor with optional parameters.")]
        public AppSettingRegistrationRequest(string key, object value) : this(key, value, null)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AppSettingRegistrationRequest"/>.
        /// </summary>
        [FixFor_Version4("Make the 'runtimeEnvironment' parameter have a default value of null")]
        public AppSettingRegistrationRequest(string key, object value, string runtimeEnvironment, string forProjectWithRole = null)
        {
            Key = key;
            Value = value;
            RuntimeEnvironment = runtimeEnvironment;
            ForProjectWithRole = forProjectWithRole;
        }

        /// <summary>
        /// For appsettings[.&lt;<see cref="RuntimeEnvironment"/>&gt;].json files, this is the top
        /// level section name to add. For Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config
        /// files, this is "key" attribute value for the &lt;settings /&gt; element to add.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value to add for the <see cref="Key"/>.
        /// </summary>
        /// <remarks>
        /// For appsettings[.&lt;<see cref="RuntimeEnvironment"/>&gt;].json files, the object will
        /// be serialized to json (even an anonymous object works fine). For
        /// Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config files, this needs to be a
        /// string as it's placed in the "key" attribute value for the &lt;settings /&gt; element
        /// being added.
        /// </remarks>
        public object Value { get; set; }

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
        /// Used for disambiguating which appsettings[.&lt;<see cref="RuntimeEnvironment"/>&gt;].json
        /// or Web/App[.&lt;<see cref="RuntimeEnvironment"/>&gt;].config file to apply to when a
        /// solution has multiple projects each with their own configuration file(s).
        /// </remarks>
        public string ForProjectWithRole { get; set; }

        /// <inheritdoc />
        public bool WasHandled { get; private set; }

        /// <inheritdoc />
        public void MarkHandled() => WasHandled = true;
    }
}
