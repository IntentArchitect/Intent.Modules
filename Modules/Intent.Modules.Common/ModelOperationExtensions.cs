#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using System.Diagnostics;
using Intent.CodeToModelOperations;
using Intent.Metadata.Models;
using IAssociationEnd = Intent.CodeToModelOperations.IAssociationEnd;
using ITypeReference = Intent.CodeToModelOperations.ITypeReference;
// ReSharper disable InvokeAsExtensionMember

namespace Intent.Modules.Common;

public static class ModelOperationExtensions
{
    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        ICreateElement parent,
        string newElementId,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference)
    {
        return factory.CreateElement(
            applicationId: parent.ApplicationId,
            designerId: parent.DesignerId,
            newElementId: newElementId,
            name: name,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.NewElementId,
            typeReference: typeReference);
    }

    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        IElement parent,
        string newElementId,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference)
    {
        return factory.CreateElement(
            applicationId: parent.Package.ApplicationId,
            designerId: parent.Package.DesignerId,
            newElementId: newElementId,
            name: name,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.Id,
            typeReference: typeReference);
    }

    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        IElementWrapper parent,
        string newElementId,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference)
    {
        return CreateChildElement(
            factory: factory,
            parent: parent.InternalElement,
            newElementId: newElementId,
            name: name,
            specialization: specialization,
            specializationId: specializationId,
            typeReference: typeReference);
    }

    public static IRenameElement RenameElement(this ICodeToModelOperationFactory factory, IElement element, string newName)
    {
        return factory.RenameElement(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            element.Id,
            newName: newName);
    }

    public static IRenameElement RenameElement(this ICodeToModelOperationFactory factory, IElementWrapper wrappedElement, string newName)
    {
        return RenameElement(factory, wrappedElement.InternalElement, newName);
    }

    public static IChangeElementTypeReference ChangeElementTypeReference(this ICodeToModelOperationFactory factory, IElement element, ITypeReference? typeReference)
    {
        return factory.ChangeElementTypeReference(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id,
            typeReference: typeReference);
    }

    public static IChangeElementTypeReference ChangeElementTypeReference(this ICodeToModelOperationFactory factory, IElementWrapper wrappedElement, ITypeReference? typeReference)
    {
        return ChangeElementTypeReference(
            factory: factory,
            element: wrappedElement.InternalElement,
            typeReference: typeReference);
    }

    public static IUpdateElementMetadata UpdateElementMetadata(this ICodeToModelOperationFactory factory, IElement element, Dictionary<string, string> metadata)
    {
        return factory.UpdateElementMetadata(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id,
            metadata: metadata);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        ICreateElement ownerEndElement,
        string? ownerEndName,
        bool ownerEndIsNullable,
        bool ownerEndIsCollection,
        ICreateElement targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        bool isBidirectional)
    {
        return CreateAssociation(
            factory: factory,
            applicationId: ownerEndElement.ApplicationId,
            designerId: ownerEndElement.DesignerId,
            specialization: specialization,
            specializationId: specializationId,
            ownerEndElementId: ownerEndElement.NewElementId,
            ownerEndName: ownerEndName,
            ownerEndIsNullable: ownerEndIsNullable,
            ownerEndIsCollection: ownerEndIsCollection,
            targetEndElementId: ownerEndElement.NewElementId,
            targetEndName: targetEndName,
            targetEndIsNullable: targetEndIsNullable,
            targetEndIsCollection: targetEndIsCollection,
            isBidirectional: isBidirectional);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        IElementWrapper ownerEndElement,
        string? ownerEndName,
        bool ownerEndIsNullable,
        bool ownerEndIsCollection,
        IElementWrapper targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        bool isBidirectional)
    {
        return CreateAssociation(
            factory: factory,
            specialization: specialization,
            specializationId: specializationId,
            ownerEndElement: ownerEndElement.InternalElement,
            ownerEndName: ownerEndName,
            ownerEndIsNullable: ownerEndIsNullable,
            ownerEndIsCollection: ownerEndIsCollection,
            targetEndElement: targetEndElement.InternalElement,
            targetEndName: targetEndName,
            targetEndIsNullable: targetEndIsNullable,
            targetEndIsCollection: targetEndIsCollection,
            isBidirectional: isBidirectional);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        IElement ownerEndElement,
        string? ownerEndName,
        bool ownerEndIsNullable,
        bool ownerEndIsCollection,
        IElement targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        bool isBidirectional)
    {
        return CreateAssociation(
            factory: factory,
            applicationId: ownerEndElement.Package.ApplicationId,
            designerId: ownerEndElement.Package.DesignerId,
            specialization: specialization,
            specializationId: specializationId,
            ownerEndElementId: ownerEndElement.Id,
            ownerEndName: ownerEndName,
            ownerEndIsNullable: ownerEndIsNullable,
            ownerEndIsCollection: ownerEndIsCollection,
            targetEndElementId: targetEndElement.Id,
            targetEndName: targetEndName,
            targetEndIsNullable: targetEndIsNullable,
            targetEndIsCollection: targetEndIsCollection,
            isBidirectional: isBidirectional);
    }

    private static ICreateAssociation CreateAssociation(
        ICodeToModelOperationFactory factory,
        string applicationId,
        string designerId,
        string specialization,
        string specializationId,
        string ownerEndElementId,
        string? ownerEndName,
        bool ownerEndIsNullable,
        bool ownerEndIsCollection,
        string targetEndElementId,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        bool isBidirectional)
    {
        return factory.CreateAssociation(
            applicationId: applicationId,
            designerId: designerId,
            specialization: specialization,
            specializationId: specializationId,
            ownerEnd: factory.AssociationEnd(
                elementId: ownerEndElementId,
                name: ownerEndName,
                multiplicity: GetMultiplicity(
                    isNullable: ownerEndIsNullable,
                    isCollection: ownerEndIsCollection)),
            targetEnd: factory.AssociationEnd(
                elementId: targetEndElementId,
                name: targetEndName,
                multiplicity: GetMultiplicity(
                    isNullable: targetEndIsNullable,
                    isCollection: targetEndIsCollection)),
            isBidirectional: isBidirectional);
    }

    public static IUpdateElementMetadata UpdateElementMetadata(this ICodeToModelOperationFactory factory, IElementWrapper wrappedElement, Dictionary<string, string> metadata)
    {
        return UpdateElementMetadata(
            factory: factory,
            element: wrappedElement.InternalElement,
            metadata: metadata);
    }

    public static IDeleteElement DeleteElement(this ICodeToModelOperationFactory factory, IElementWrapper element)
    {
        return DeleteElement(
            factory: factory,
            element: element.InternalElement);
    }

    public static IDeleteElement DeleteElement(this ICodeToModelOperationFactory factory, IElement element)
    {
        return factory.DeleteElement(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id);
    }

    private static string GetMultiplicity(bool isNullable, bool isCollection)
    {
        return (isNullable, isCollection) switch
        {
            (isNullable: false, isCollection: false) => "1",
            (isNullable: false, isCollection: true) => "1..*",
            (isNullable: true, isCollection: false) => "0..1",
            (isNullable: true, isCollection: true) => "0..*",
        };
    }

    //private class AssociationEnd(
    //    string elementId,
    //    string? name,
    //    bool isNullable,
    //    bool isCollection) : IAssociationEnd
    //{
    //    public string ElementId { get; set; } = elementId;
    //    public string? Name { get; set; } = name;
    //    public string Multiplicity { get; set; } = (isNullable, isCollection) switch
    //    {
    //        (isNullable: false, isCollection: false) => "1",
    //        (isNullable: false, isCollection: true) => "1..*",
    //        (isNullable: true, isCollection: false) => "0..1",
    //        (isNullable: true, isCollection: true) => "0..*",
    //    };
    //}
}