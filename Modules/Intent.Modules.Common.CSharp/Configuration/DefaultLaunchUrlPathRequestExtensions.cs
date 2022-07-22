using System;
using Intent.Eventing;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.Modules.Common.CSharp.Configuration;

/// <summary>
/// Extension methods for <see cref="DefaultLaunchUrlPathRequest"/>.
/// </summary>
public static class DefaultLaunchUrlPathRequestExtensions
{
    /// <summary>
    /// Sets the default launch url path in the <c>launchsettings.json</c> file to the provided
    /// <paramref name="urlPath"/>.
    /// </summary>
    /// <remarks>
    /// This needs to be called within the <see cref="IntentTemplateBase.BeforeTemplateExecution"/> method.
    /// This event can only be published once, publishing it a second time will raise an exception.
    /// If the <c>launchsettings.json</c> file already exists, it will not be updated.
    /// </remarks>
    /// <param name="template">
    /// An instance of a type derived from <see cref="IntentTemplateBase"/> with an accessible
    /// instance of <see cref="IApplicationEventDispatcher"/> which is used to make the request.
    /// </param>
    /// <param name="urlPath">
    /// The Path component to be applied to the <c>launchUrl</c> for the default profiles.
    /// <example>/swagger/index.html</example>
    /// </param>
    /// <param name="forProjectWithRole">
    /// Optional. Name of the output target 'Role' which must be present in the project within
    /// the Intent Architect Visual Studio designer.
    /// <remarks>
    /// Used for disambiguating which <c>launchsettings.json</c>
    /// file to apply to when a solution has multiple projects each with their own
    /// <c>launchsettings.json</c> file.
    /// </remarks>
    /// </param>
    public static void PublishDefaultLaunchUrlPathRequest(this IntentTemplateBase template, string urlPath, string forProjectWithRole = null)
    {
        var request = new DefaultLaunchUrlPathRequest(urlPath, forProjectWithRole);
        template.ExecutionContext.EventDispatcher.Publish(request);
        
        if (!request.WasHandled)
        {
            Logging.Log.Warning($"{nameof(PublishDefaultLaunchUrlPathRequest)} for {nameof(urlPath)}='{urlPath}',{nameof(forProjectWithRole)}='{forProjectWithRole}' " +
                                $"was not handled. Ensure in the Visual Studio designer that you have:{Environment.NewLine}" +
                                $" - at least one ASP.NET project{Environment.NewLine}" +
                                (!string.IsNullOrWhiteSpace(forProjectWithRole) ? $" - a matching 'Role' element under the project{Environment.NewLine}" : string.Empty).TrimEnd());
        }
    }
}