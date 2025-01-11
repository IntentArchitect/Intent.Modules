using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;

#nullable enable

namespace Intent.Modules.Metadata.WebApi.Models;

public static class HttpEndpointExecutionContextExtensions
{
    /// <summary>
    /// Gets details of endpoints where <see cref="IHttpEndpointModel.SecurityModels"/> contains
    /// both directly and indirectly applied Security stereotypes (i.e. on parent elements
    /// or the package) and whether the setting "Secure by default" is enabled.
    /// </summary>
    /// <param name="context">This context provides access to Application Settings where this <paramref name="element"/> comes from.</param>
    /// <param name="element">The element (Service / Folder) containing endpoints represented by Operations, Commands and Query element types.</param>
    /// <param name="defaultBasePath">The default base API path, typically captured in the application settings.</param>
    /// <param name="httpEndpointCollectionModel">The HTTP endpoint models.</param>
    /// <returns>Whether the provided elements (as endpoints) were found.</returns>
    public static bool TryGetHttpEndpointCollection(
        this ISoftwareFactoryExecutionContext context,
        IElement element,
        string? defaultBasePath,
        [NotNullWhen(true)] out IHttpEndpointCollectionModel? httpEndpointCollectionModel)
    {
        var securedByDefault = GetSecuredByDefaultSetting(context, element.Package.ApplicationId);
        return HttpEndpointModelFactory.TryGetCollection(element, defaultBasePath, securedByDefault, out httpEndpointCollectionModel);
    }

    /// <summary>
    /// Gets details of endpoints where <see cref="IHttpEndpointModel.SecurityModels"/> contains
    /// both directly and indirectly applied Security stereotypes (i.e. on parent elements
    /// or the package) and whether the setting "Secure by default" is enabled.
    /// </summary>
    /// <param name="context">This context provides access to Application Settings where this <paramref name="element"/> comes from.</param>
    /// <param name="package">The package containing endpoints, that contains Operations, Commands and Query element types.</param>
    /// <param name="defaultBasePath">The default base API path, typically captured in the application settings.</param>
    /// <param name="httpEndpointCollectionModel">The HTTP endpoint models.</param>
    /// <returns>Whether the provided Operations/Commands/Queries were found to represent endpoints.</returns>
    public static bool TryGetHttpEndpointCollection(
        this ISoftwareFactoryExecutionContext context,
        IPackage package,
        string? defaultBasePath,
        [NotNullWhen(true)] out IHttpEndpointCollectionModel? httpEndpointCollectionModel)
    {
        var securedByDefault = GetSecuredByDefaultSetting(context, package.ApplicationId);
        return HttpEndpointModelFactory.TryGetCollection(package, defaultBasePath, securedByDefault, out httpEndpointCollectionModel);
    }
    
    /// <summary>
    /// Gets details of an endpoint where <see cref="IHttpEndpointModel.SecurityModels"/> contains
    /// both directly and indirectly applied Security stereotypes (i.e. on parent elements
    /// or the package) and whether the setting "Secure by default" is enabled.
    /// </summary>
    /// <param name="context">This context provides access to Application Settings where this <paramref name="element"/> comes from.</param>
    /// <param name="element">The element representing the endpoint, works with Operation, Command and Query element types.</param>
    /// <param name="defaultBasePath">The default base API path, typically captured in the application settings.</param>
    /// <param name="httpEndpointModel">The HTTP endpoint model.</param>
    /// <returns>Whether the provided <paramref name="element"/> was found to represent an endpoint.</returns>
    public static bool TryGetHttpEndpoint(
        this ISoftwareFactoryExecutionContext context,
        IElement element,
        string? defaultBasePath,
        [NotNullWhen(true)] out IHttpEndpointModel? httpEndpointModel)
    {
        var securedByDefault = GetSecuredByDefaultSetting(context, element.Package.ApplicationId);
        return HttpEndpointModelFactory.TryGetEndpoint(element, defaultBasePath, securedByDefault, out httpEndpointModel);
    }

    private static bool GetSecuredByDefaultSetting(ISoftwareFactoryExecutionContext context, string applicationId)
    {
        // There might be cases where an applicationId is missing, then just ignore.
        if (applicationId == null!)
        {
            return false;
        }
        
        // Until we migrate this setting over to the WebAPI module form the ASP.NET Core Controllers module,
        // we'll need to make use of the soft-linking for that setting.
        var defaultApiSecuritySetting = context.GetSolutionConfig().GetApplicationConfig(applicationId)
            .ModuleSetting
            .FirstOrDefault(p => p.Id == "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb") // APISettings
            ?.GetSetting("061a559a-0d54-4eb1-8c70-ed0baa238a59"); //DefaultAPISecurityOptions
        
        return defaultApiSecuritySetting?.Value == "secured";
    }
}