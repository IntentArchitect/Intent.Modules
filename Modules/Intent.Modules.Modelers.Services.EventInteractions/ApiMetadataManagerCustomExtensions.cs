using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modelers.Services.EventInteractions;

/// <summary>
/// Extension methods of Event Interactions in the Services designer.
/// </summary>
public static class ApiMetadataManagerCustomExtensions
{
    /// <summary>
    /// Retrieves all DTOs that are explicitly published through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A list of explicitly published DTO models.</returns>
    public static IList<EventingDTOModel> GetExplicitlyPublishedDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlyPublishedDtoModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all DTOs that are explicitly published through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A list of explicitly published DTO models.</returns>
    public static IList<EventingDTOModel> GetExplicitlyPublishedDtoModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetExplicitlyPublishedMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySentIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Enum models that are explicitly published through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly published enum models.</returns>
    public static IReadOnlyCollection<EnumModel> GetExplicitlyPublishedEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlyPublishedEnumModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Enum models that are explicitly published through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly published enum models.</returns>
    public static IReadOnlyCollection<EnumModel> GetExplicitlyPublishedEnumModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetExplicitlyPublishedMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySentIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Message models that are explicitly published through integration events in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly published message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetExplicitlyPublishedMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlyPublishedMessageModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Message models that are explicitly published through integration events in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly published message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetExplicitlyPublishedMessageModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).GetAssociationsOfType(PublishIntegrationEventModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Integration Command models that are explicitly sent in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly sent integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySentIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySentIntegrationCommandModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Integration Command models that are explicitly sent in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly sent integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySentIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).GetAssociationsOfType(SendIntegrationCommandModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Integration Command dispatch models that are explicitly sent in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of integration command dispatch target end models.</returns>
    public static IReadOnlyCollection<SendIntegrationCommandTargetEndModel> GetExplicitlySentIntegrationCommandDispatches(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).Associations.GetSentIntegrationCommands().ToArray();
    }

    /// <summary>
    /// Retrieves all DTO models that are explicitly subscribed to through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly subscribed to DTO models.</returns>
    public static IReadOnlyCollection<EventingDTOModel> GetExplicitlySubscribedToDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToDtoModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all DTO models that are explicitly subscribed to through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly subscribed to DTO models.</returns>
    public static IReadOnlyCollection<EventingDTOModel> GetExplicitlySubscribedToDtoModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetExplicitlySubscribedToMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySubscribedToIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Enum models that are explicitly subscribed to through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly subscribed to enumeration models.</returns>
    public static IReadOnlyCollection<EnumModel> GetExplicitlySubscribedToEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToEnumModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Enum models that are explicitly subscribed to through messages or integration commands in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly subscribed to enumeration models.</returns>
    public static IReadOnlyCollection<EnumModel> GetExplicitlySubscribedToEnumModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetExplicitlySubscribedToMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySubscribedToIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Message models that are explicitly subscribed to through integration events in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly subscribed to message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetExplicitlySubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToMessageModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Message models that are explicitly subscribed to through integration events in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly subscribed to message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetExplicitlySubscribedToMessageModels(this IMetadataManager metadataManager, string applicationId)
    {
        // This looks like a bug.
        return metadataManager.Services(applicationId).GetAssociationsOfType(SubscribeIntegrationEventModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Retrieves all Integration Command models that are explicitly subscribed to in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of explicitly subscribed to integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySubscribedToIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToIntegrationCommandModels(application.Id);
    }
    
    /// <summary>
    /// Retrieves all Integration Command models that are explicitly subscribed to in the specified application.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of explicitly subscribed to integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySubscribedToIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).GetAssociationsOfType(SubscribeIntegrationCommandModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all Messages referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of associated message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetAssociatedMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedMessageModels(application.Id);
    }
    
    /// <summary>
    /// Returns all Messages referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of associated message models.</returns>
    public static IReadOnlyCollection<MessageModel> GetAssociatedMessageModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).Associations
            .SelectMany(x => new[] { x.TargetEnd?.TypeReference?.Element, x.SourceEnd?.TypeReference?.Element })
            .Where(x => x.IsMessageModel())
            .Select(x => x.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all integration command models referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of associated integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetAssociatedIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedIntegrationCommandModels(application.Id);
    }
    
    /// <summary>
    /// Returns all integration command models referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of associated integration command models.</returns>
    public static IReadOnlyCollection<IntegrationCommandModel> GetAssociatedIntegrationCommandModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).Associations
            .SelectMany(x => new[] { x.TargetEnd?.TypeReference?.Element, x.SourceEnd?.TypeReference?.Element })
            .Where(x => x.IsIntegrationCommandModel())
            .Select(x => x.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all enums used by messages referenced by an association in the services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of associated message enum models.</returns>
    public static IReadOnlyCollection<EnumModel> GetAssociatedMessageEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedMessageEnumModels(application.Id);
    }
    
    /// <summary>
    /// Returns all enums used by messages referenced by an association in the services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of associated message enum models.</returns>
    public static IReadOnlyCollection<EnumModel> GetAssociatedMessageEnumModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetAssociatedMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetAssociatedIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all DTOs used by messages referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="application">The application to search within.</param>
    /// <returns>A collection of associated message DTO models.</returns>
    public static IReadOnlyCollection<EventingDTOModel> GetAssociatedMessageDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedMessageDtoModels(application.Id);
    }
    
    /// <summary>
    /// Returns all DTOs used by messages referenced by an association in the Services designer.
    /// </summary>
    /// <param name="metadataManager">The metadata manager.</param>
    /// <param name="applicationId">The ID of the application to search within.</param>
    /// <returns>A collection of associated message DTO models.</returns>
    public static IReadOnlyCollection<EventingDTOModel> GetAssociatedMessageDtoModels(this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.GetAssociatedMessageModels(applicationId)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetAssociatedIntegrationCommandModels(applicationId)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    private static IEnumerable<EventingDTOModel> GetReferencedDtoModels(ITypeReference typeReference)
    {
        if (typeReference.Element?.IsEventingDTOModel() != true)
        {
            yield break;
        }

        var dtoModel = typeReference.Element.AsEventingDTOModel();
        yield return dtoModel;

        if (dtoModel.ParentDtoTypeReference != null)
        {
            foreach (var referencedDtoModel in GetReferencedDtoModels(dtoModel.ParentDtoTypeReference))
            {
                yield return referencedDtoModel;
            }
        }

        foreach (var referencedDtoModel in dtoModel.Fields.SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
        {
            yield return referencedDtoModel;
        }
    }

    private static IEnumerable<EnumModel> GetReferencedEnumModels(ITypeReference typeReference)
    {
        if (typeReference.Element?.IsEnumModel() == true)
        {
            yield return typeReference.Element.AsEnumModel();
            yield break;
        }

        if (typeReference.Element?.IsEventingDTOModel() != true)
        {
            yield break;
        }

        var dtoModel = typeReference.Element.AsEventingDTOModel();

        if (dtoModel.ParentDtoTypeReference != null)
        {
            foreach (var enumModel in GetReferencedEnumModels(dtoModel.ParentDtoTypeReference))
            {
                yield return enumModel;
            }
        }

        foreach (var enumModel in dtoModel.Fields.SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
        {
            yield return enumModel;
        }
    }
}