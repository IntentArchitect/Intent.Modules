#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.CodeToModelOperations;
using Intent.Metadata.Models;
using ITypeReference = Intent.CodeToModelOperations.ITypeReference;
// ReSharper disable InvokeAsExtensionMember

namespace Intent.Modules.Common;

public static class ModelOperationExtensions
{
    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        ICreateElement parent,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference = null,
        string? newElementId = null,
        string? defaultValue = null,
        string? comment = null)
    {
        return factory.CreateElement(
            applicationId: parent.ApplicationId,
            designerId: parent.DesignerId,
            newElementId: newElementId ?? Guid.NewGuid().ToString(),
            name: name,
            defaultValue: defaultValue,
            comment: comment,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.NewElementId,
            typeReference: typeReference);
    }

    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        IElement parent,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference = null,
        string? newElementId = null,
        string? defaultValue = null,
        string? comment = null)
    {
        return factory.CreateElement(
            applicationId: parent.Package.ApplicationId,
            designerId: parent.Package.DesignerId,
            newElementId: newElementId ?? Guid.NewGuid().ToString(),
            name: name,
            defaultValue: defaultValue,
            comment: comment,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.Id,
            typeReference: typeReference);
    }

    public static ICreateElement CreateChildElement(
        this ICodeToModelOperationFactory factory,
        IElementWrapper parent,
        string name,
        string? specialization,
        string specializationId,
        ITypeReference? typeReference = null,
        string? newElementId = null,
        string? defaultValue = null,
        string? comment = null)
    {
        return factory.CreateElement(
            applicationId: parent.InternalElement.Package.ApplicationId,
            designerId: parent.InternalElement.Package.DesignerId,
            newElementId: newElementId ?? Guid.NewGuid().ToString(),
            name: name,
            defaultValue: defaultValue,
            comment: comment,
            specialization: specialization,
            specializationId: specializationId,
            parentId: parent.InternalElement.Id,
            typeReference: typeReference);
    }

    public static IUpdateElement UpdateElement(
        this ICodeToModelOperationFactory factory,
        IElement element,
        string? name = null,
        ITypeReference? typeReference = null,
        string? defaultValue = null,
        string? comment = null)
    {
        return factory.UpdateElement(
            applicationId: element.Package.ApplicationId,
            designerId: element.Package.DesignerId,
            elementId: element.Id,
            name: name,
            defaultValue: defaultValue,
            comment: comment,
            typeReference: typeReference);
    }

    public static IUpdateElement UpdateElement(
        this ICodeToModelOperationFactory factory,
        IElementWrapper element,
        string? name = null,
        ITypeReference? typeReference = null,
        string? defaultValue = null,
        string? comment = null)
    {
        return factory.UpdateElement(
            applicationId: element.InternalElement.Package.ApplicationId,
            designerId: element.InternalElement.Package.DesignerId,
            elementId: element.InternalElement.Id,
            name: name,
            defaultValue: defaultValue,
            comment: comment,
            typeReference: typeReference);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        ICreateElement targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        ICreateElement ownerEndElement,
        string? ownerEndName = null,
        bool ownerEndIsNullable = false,
        bool ownerEndIsCollection = false,
        bool isBidirectional = false)
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
            targetEndElementId: targetEndElement.NewElementId,
            targetEndName: targetEndName,
            targetEndIsNullable: targetEndIsNullable,
            targetEndIsCollection: targetEndIsCollection,
            isBidirectional: isBidirectional);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        IElementWrapper targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        IElementWrapper ownerEndElement,
        string? ownerEndName,
        bool ownerEndIsNullable = false,
        bool ownerEndIsCollection = false,
        bool isBidirectional = false)
    {
        return CreateAssociation(
            factory: factory,
            applicationId: ownerEndElement.InternalElement.Package.ApplicationId,
            designerId: ownerEndElement.InternalElement.Package.DesignerId,
            specialization: specialization,
            specializationId: specializationId,
            ownerEndElementId: ownerEndElement.InternalElement.Id,
            ownerEndName: ownerEndName,
            ownerEndIsNullable: ownerEndIsNullable,
            ownerEndIsCollection: ownerEndIsCollection,
            targetEndElementId: targetEndElement.InternalElement.Id,
            targetEndName: targetEndName,
            targetEndIsNullable: targetEndIsNullable,
            targetEndIsCollection: targetEndIsCollection,
            isBidirectional: isBidirectional);
    }

    public static ICreateAssociation CreateAssociation(
        this ICodeToModelOperationFactory factory,
        string specialization,
        string specializationId,
        IElement targetEndElement,
        string? targetEndName,
        bool targetEndIsNullable,
        bool targetEndIsCollection,
        IElement ownerEndElement,
        string? ownerEndName = null,
        bool ownerEndIsNullable = false,
        bool ownerEndIsCollection = false,
        bool isBidirectional = false)
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

    [return: NotNullIfNotNull(nameof(typeReference))]
    public static IGenericArgument? GenericArgument(this ICodeToModelOperationFactory factory, Metadata.Models.ITypeReference? typeReference)
    {
        if (typeReference == null)
        {
            return null;
        }

        return factory.GenericArgument(
            typeId: typeReference.ElementId,
            isCollection: typeReference.IsCollection,
            isNullable: typeReference.IsNullable,
            genericArguments: typeReference.GenericTypeParameters
                .Select(genericArgument => GenericArgument(factory, genericArgument))
                .ToList());
    }

    [return: NotNullIfNotNull(nameof(typeReference))]
    public static ITypeReference? TypeReference(this ICodeToModelOperationFactory factory, Intent.Metadata.Models.ITypeReference? typeReference)
    {
        if (typeReference == null)
        {
            return null;
        }

        return factory.TypeReference(
            typeId: typeReference.ElementId,
            isCollection: typeReference.IsCollection,
            isNullable: typeReference.IsNullable,
            genericArguments: typeReference.GenericTypeParameters
                .Select(genericArgument => GenericArgument(factory, genericArgument))
                .ToList());
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
}