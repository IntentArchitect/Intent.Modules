namespace Intent.Modules.Common.CSharp.Configuration;

/// <summary>
/// Sets the default launch url path in the <c>launchsettings.json</c> file to the provided
/// <see cref="UrlPath"/>.
/// </summary>
public class DefaultLaunchUrlPathRequest : IForProjectWithRoleRequest
{
    /// <summary>
    /// Creates a new instance of <see cref="DefaultLaunchUrlPathRequest"/>.
    /// </summary>
    /// <remarks>
    /// Use <see cref="DefaultLaunchUrlPathRequestExtensions.PublishDefaultLaunchUrlRequest"/>
    /// to perform publishing of this event.
    /// </remarks>
    internal DefaultLaunchUrlPathRequest(string urlPath, string forProjectWithRole)
    {
        UrlPath = urlPath;
        ForProjectWithRole = forProjectWithRole;
    }

    /// <summary>
    /// The Path component to be applied to the <c>launchUrl</c> for the default profiles.
    /// </summary>
    public string UrlPath { get; }
    
    /// <inheritdoc />
    public string ForProjectWithRole { get; }
    
    /// <inheritdoc />
    public bool WasHandled { get; private set; }
    
    /// <inheritdoc />
    public void MarkHandled() => WasHandled = true;
}