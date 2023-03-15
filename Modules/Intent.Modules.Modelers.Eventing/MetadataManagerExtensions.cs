using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Modelers.Eventing;

public static class MetadataManagerExtensions
{
    public static IList<EventingDTOModel> GetPublishedDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetPublishedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<EnumModel> GetPublishedEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetPublishedMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<MessageModel> GetPublishedMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Eventing(application).GetApplicationModels()
            .SelectMany(
                s => s.PublishedMessages(),
                (_, m) => m.Association.TargetEnd.Element.AsMessageModel())
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<EventingDTOModel> GetSubscribedToDtoModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetSubscribedToMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedDtoModels(x.TypeReference))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<EnumModel> GetSubscribedToEnumModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.GetSubscribedToMessageModels(application)
            .SelectMany(x => x.Properties)
            .SelectMany(x => GetReferencedEnumModels(x.TypeReference))
            .Distinct()
            .ToArray();
    }

    public static IReadOnlyCollection<MessageModel> GetSubscribedToMessageModels(this IMetadataManager metadataManager, IApplication application)
    {
        return metadataManager.Eventing(application).GetApplicationModels()
            .SelectMany(
                s => s.SubscribedMessages(),
                (_, m) => m.Association.TargetEnd.Element.AsMessageModel())
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