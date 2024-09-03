using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modelers.Services.EventInteractions;

/// <summary>
/// Extension methods of event interactions in the services designer.
/// </summary>
public static class ApiMetadataManagerCustomExtensions
{
    public static IList<EventingDTOModel> GetExplicitlyPublishedDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlyPublishedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySentIntegrationCommandModels(application)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<EnumModel> GetExplicitlyPublishedEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlyPublishedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySentIntegrationCommandModels(application)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<MessageModel> GetExplicitlyPublishedMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).GetAssociationsOfType(PublishIntegrationEventModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySentIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).GetAssociationsOfType(SubscribeIntegrationCommandModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<SendIntegrationCommandTargetEndModel> GetExplicitlySentIntegrationCommandDispatches(
        this IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId)
            .Associations
            .GetSentIntegrationCommands()
            .ToArray();
    }

    public static IReadOnlyCollection<EventingDTOModel> GetExplicitlySubscribedToDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySubscribedToIntegrationCommandModels(application)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedDtoModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<EnumModel> GetExplicitlySubscribedToEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetExplicitlySubscribedToMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetExplicitlySubscribedToIntegrationCommandModels(application)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<MessageModel> GetExplicitlySubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).GetAssociationsOfType(SubscribeIntegrationEventModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<IntegrationCommandModel> GetExplicitlySubscribedToIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).GetAssociationsOfType(SendIntegrationCommandModel.SpecializationTypeId)
            .Select(x => x.TargetEnd.TypeReference.Element.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all messages referenced by an association in the services designer.
    /// </summary>
    public static IReadOnlyCollection<MessageModel> GetAssociatedMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).Associations
            .SelectMany(x => new[] { x.TargetEnd?.TypeReference?.Element, x.SourceEnd?.TypeReference?.Element })
            .Where(x => x.IsMessageModel())
            .Select(x => x.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all messages referenced by an association in the services designer.
    /// </summary>
    public static IReadOnlyCollection<IntegrationCommandModel> GetAssociatedIntegrationCommandModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Services(application).Associations
            .SelectMany(x => new[] { x.TargetEnd?.TypeReference?.Element, x.SourceEnd?.TypeReference?.Element })
            .Where(x => x.IsIntegrationCommandModel())
            .Select(x => x.AsIntegrationCommandModel())
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all enums used by messages referenced by an association in the services designer.
    /// </summary>
    public static IReadOnlyCollection<EnumModel> GetAssociatedMessageEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Concat(metadataManager.GetAssociatedIntegrationCommandModels(application)
                .SelectMany(x => x.Properties)
                .SelectMany(x => GetReferencedEnumModels(x.TypeReference)))
            .Distinct()
            .ToArray();
    }

    /// <summary>
    /// Returns all DTOs used by messages referenced by an association in the services designer.
    /// </summary>
    public static IReadOnlyCollection<EventingDTOModel> GetAssociatedMessageDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetAssociatedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Concat(metadataManager.GetAssociatedIntegrationCommandModels(application)
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