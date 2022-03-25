using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// A request for the key/value pair to be added to the environment variables of the application's launchSettings.json file.
    /// </summary>
    public class EnvironmentVariableRegistrationRequest
    {
        /// <summary>
        /// Creates the <see cref="EnvironmentVariableRegistrationRequest"/>.
        /// </summary>
        /// <param name="key">The key string for the variable.</param>
        /// <param name="value">The value object for the variable.</param>
        /// <param name="targetProfiles">The target profiles that should register this environment variable. If null, the variable will be registered with all profiles.</param>
        public EnvironmentVariableRegistrationRequest(string key, object value, IEnumerable<string> targetProfiles = null)
        {
            Key = key;
            Value = value;
            TargetProfiles = targetProfiles;
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
    }
}