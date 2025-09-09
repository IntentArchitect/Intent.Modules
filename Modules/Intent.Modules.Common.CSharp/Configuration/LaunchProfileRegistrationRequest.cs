using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Configuration
{
    /// <summary>
    /// For adding a profile to a <c>launchSettings.json</c>.
    /// </summary>
    /// <remarks>
    /// Templates for <c>launchSettings.json</c> files may listen for these
    /// requests and update themselves accordingly.
    /// </remarks>
    public class LaunchProfileRegistrationRequest : IForProjectWithRoleRequest
    {
        /// <summary>
        /// The name of the profile to add.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value to populate in the <c>commandName</c> field.
        /// </summary>
        public string CommandName { get; set; }
        
        /// <summary>
        /// The value to populate in the <c>commandLineArgs</c> field.
        /// </summary>
        public string CommandLineArgs { get; set; }

        /// <summary>
        /// The value to populate in the <c>launchBrowser</c> field.
        /// </summary>
        public bool LaunchBrowser { get; set; }

        /// <summary>
        /// The value to populate in the <c>launchUrl</c> field.
        /// </summary>
        public string LaunchUrl { get; set; }

        /// <summary>
        /// The value to populate in the <c>applicationUrl</c> field.
        /// </summary>
        public string ApplicationUrl { get; set; }

        /// <summary>
        /// The value to populate in the <c>publishAllPorts</c> field.
        /// </summary>
        public bool PublishAllPorts { get; set; }

        /// <summary>
        /// The value to populate in the <c>useSSL</c> field.
        /// </summary>
        public bool UseSsl { get; set; } = true;

        /// <summary>
        /// Set to true to display a message when the project is building.
        /// </summary>
        public bool? DotnetRunMessages { get; set; }

        /// <summary>
        /// The url to enable debugging on a Blazor WebAssembly application.
        /// </summary>
        public string InspectUri { get; set; }

        /// <summary>
        /// Key/value pairs of environment variables to be added to profile.
        /// </summary>
        public Dictionary<string, string> EnvironmentVariables { get; set; }
        
        /// <summary>
        /// The value to populate in the <c>executablePath</c> field.
        /// </summary>
        public string ExecutablePath { get; set; }
        
        /// <summary>
        /// The value to populate in the <c>workingDirectory</c> field.
        /// </summary>
        public string WorkingDirectory { get; set; }

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
