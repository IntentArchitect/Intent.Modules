using System.Collections.Generic;
using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// A request for the key/value pair to be added to the environment variables of the application's launchSettings.json file.
    /// </summary>
    public class EnvironmentVariableRegistrationRequest : IForProjectWithRoleRequest
    {
        /// <summary>
        /// Creates the <see cref="EnvironmentVariableRegistrationRequest"/>.
        /// </summary>
        /// <param name="key">The key string for the variable.</param>
        /// <param name="value">The value object for the variable.</param>
        /// <param name="targetProfiles">The target profiles that should register this environment variable. If null, the variable will be registered with all profiles.</param>
        [FixFor_Version4("Remove this constructor overload and solely use the other one.")]
        public EnvironmentVariableRegistrationRequest(string key, object value, IEnumerable<string> targetProfiles = null)
        {
            Key = key;
            Value = value;
            TargetProfiles = targetProfiles;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnvironmentVariableRegistrationRequest"/>.
        /// </summary>
        /// <param name="key">The key string for the variable.</param>
        /// <param name="value">The value object for the variable.</param>
        /// <param name="targetProfiles">The target profiles that should register this environment variable. If null, the variable will be registered with all profiles.</param>
        /// <param name="forProjectWithRole">The <see cref="ForProjectWithRole"/> value.</param>
        public EnvironmentVariableRegistrationRequest(string key, object value, IEnumerable<string> targetProfiles = null, string forProjectWithRole = null)
        {
            Key = key;
            Value = value;
            TargetProfiles = targetProfiles;
            ForProjectWithRole = forProjectWithRole;
        }

        /// <summary>
        /// The key string for the variable.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value object for the variable. 
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The target profiles that should register this environment variable. If null, the variable will be registered with all profiles.
        /// </summary>
        public IEnumerable<string> TargetProfiles { get; set; }

        /// <summary>
        /// Optional. Name of the output target 'Role' which must be present in the project within
        /// the Intent Architect Visual Studio designer.
        /// </summary>
        /// <remarks>
        /// Used for disambiguating which <c>launchSettings.json</c> file to apply to when a solution has multiple
        /// projects each with their own <c>launchSettings.json</c> file.
        /// </remarks>
        public string ForProjectWithRole { get; set; }

        /// <inheritdoc />
        public bool WasHandled { get; private set; }

        /// <inheritdoc />
        public void MarkHandled() => WasHandled = true;
    }
}