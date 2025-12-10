#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.UserChanges;
using ITypeReference = Intent.UserChanges.ITypeReference;

namespace Intent.Modules.Common;

public static class UserChangeFactoryExtensions
{
    public static ICreateElement CreateChildElement(
        this IUserChangeFactory factory,
        IElement parent,
        string elementId,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference)
    {
        return factory.CreateElement(
            applicationId: parent.Package.ApplicationId,
            designerId: parent.Package.DesignerId,
            elementId: elementId,
            name: name,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.Id,
            typeReference: typeReference);
    }

    public static ICreateElement CreateChildElement(
        this IUserChangeFactory factory,
        IElementWrapper parent,
        string elementId,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference)
    {
        return CreateChildElement(
            factory: factory,
            parent: parent.InternalElement,
            elementId: elementId,
            name: name,
            specialization: specialization,
            specializationId: specializationId,
            typeReference: typeReference);
    }

    public static IRenameElement RenameElement(this IUserChangeFactory factory, IElement element, string newName)
    {
        return factory.RenameElement(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            element.Id,
            newName: newName);
    }

    public static IRenameElement RenameElement(this IUserChangeFactory factory, IElementWrapper wrappedElement, string newName)
    {
        return RenameElement(factory, wrappedElement.InternalElement, newName);
    }

    public static IChangeElementTypeReference ChangeElementTypeReference(this IUserChangeFactory factory, IElement element, ITypeReference? typeReference)
    {
        return factory.ChangeElementTypeReference(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id,
            typeReference: typeReference);
    }

    public static IChangeElementTypeReference ChangeElementTypeReference(this IUserChangeFactory factory, IElementWrapper wrappedElement, ITypeReference? typeReference)
    {
        return ChangeElementTypeReference(
            factory: factory,
            element: wrappedElement.InternalElement,
            typeReference: typeReference);
    }

    public static IUpdateElementMetadata UpdateElementMetadata(this IUserChangeFactory factory, IElement element, Dictionary<string, string> metadata)
    {
        return factory.UpdateElementMetadata(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id,
            metadata: metadata);
    }

    public static IUpdateElementMetadata UpdateElementMetadata(this IUserChangeFactory factory, IElementWrapper wrappedElement, Dictionary<string, string> metadata)
    {
        return UpdateElementMetadata(
            factory: factory,
            element: wrappedElement.InternalElement,
            metadata: metadata);
    }
}