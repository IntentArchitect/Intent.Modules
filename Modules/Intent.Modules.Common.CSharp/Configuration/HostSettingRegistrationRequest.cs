namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// For adding a section or key to an Azure Functions host.json file. If the file already
    /// has the provided <see cref="Key"/> defined, it is left untouched.
    /// </summary>
    /// <remarks>
    /// Templates for host.json files may listen for these requests and update themselves
    /// accordingly.
    /// </remarks>
    public class HostSettingRegistrationRequest : IForProjectWithRoleRequest
    {
        /// <summary>
        /// Creates a new instance of <see cref="HostSettingRegistrationRequest"/>.
        /// </summary>
        public HostSettingRegistrationRequest(
            string key,
            object value,
            string forProjectWithRole = null)
        {
            Key = key;
            Value = value;
            ForProjectWithRole = forProjectWithRole;
        }

        /// <summary>
        /// The key within the JSON file under which the the <see cref="Value"/> will be applied.
        /// May be separated with a colon (:) to target nested objects.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The value to set the <see cref="Key"/> to.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// </summary>
        /// <remarks>
        /// Used for disambiguating which host.json file to apply to when a solution has multiple
        /// projects each with their own host.json file.
        /// </remarks>
        public string ForProjectWithRole { get; }

        /// <inheritdoc />
        public bool WasHandled { get; private set; }

        /// <inheritdoc />
        public void MarkHandled() => WasHandled = true;
    }
}
