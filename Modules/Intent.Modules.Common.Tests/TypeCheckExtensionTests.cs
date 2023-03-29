using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.CrossPlatform.IO;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Xunit;

namespace Intent.Modules.Common.Tests;

public class TypeCheckExtensionTests
{
    private static readonly Dictionary<string, ITypeReference> _typeReferences;
    private static readonly Dictionary<string, ICanBeReferencedType> _canBeReferencedTypes;

    static TypeCheckExtensionTests()
    {
        var location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "../../../..", "Intent.Modules.Common.Types", "Intent.Metadata", "Module Builder", "Intent.Common.Types", "Elements", "Type-Definition");
        var commonTypeElements = ElementPersistable.LoadFromDirectory(location, "*.xml");

        _canBeReferencedTypes = commonTypeElements.ToDictionary(k => k.Name.ToPascalCase(), v => (ICanBeReferencedType)new CanBeReferenceType(v.Id));
        _typeReferences = commonTypeElements.ToDictionary(k => k.Name.ToPascalCase(), v => (ITypeReference)new TypeReference(_canBeReferencedTypes[v.Name.ToPascalCase()]));
    }

    [Fact]
    public void Element_IsBinaryType_True()
    {
        Assert.True(_canBeReferencedTypes["Binary"].IsBinaryType());
    }

    [Fact]
    public void Reference_HasBinaryType_True()
    {
        Assert.True(_typeReferences["Binary"].HasBinaryType());
    }
    
    [Fact]
    public void Element_IsBoolType_True()
    {
        Assert.True(_canBeReferencedTypes["Bool"].IsBoolType());
    }

    [Fact]
    public void Reference_HasBoolType_True()
    {
        Assert.True(_typeReferences["Bool"].HasBoolType());
    }
    
    [Fact]
    public void Element_IsByteType_True()
    {
        Assert.True(_canBeReferencedTypes["Byte"].IsByteType());
    }

    [Fact]
    public void Reference_HasByteType_True()
    {
        Assert.True(_typeReferences["Byte"].HasByteType());
    }
    
    [Fact]
    public void Element_IsCharType_True()
    {
        Assert.True(_canBeReferencedTypes["Char"].IsCharType());
    }

    [Fact]
    public void Reference_HasCharType_True()
    {
        Assert.True(_typeReferences["Char"].HasCharType());
    }

    [Fact]
    public void Element_IsDateType_True()
    {
        Assert.True(_canBeReferencedTypes["Date"].IsDateType());
    }

    [Fact]
    public void Reference_HasDateType_True()
    {
        Assert.True(_typeReferences["Date"].HasDateType());
    }

    [Fact]
    public void Element_IsDateTimeType_True()
    {
        Assert.True(_canBeReferencedTypes["Datetime"].IsDateTimeType());
    }

    [Fact]
    public void Reference_HasDateTimeType_True()
    {
        Assert.True(_typeReferences["Datetime"].HasDateTimeType());
    }

    [Fact]
    public void Element_IsDateTimeOffsetType_True()
    {
        Assert.True(_canBeReferencedTypes["Datetimeoffset"].IsDateTimeOffsetType());
    }

    [Fact]
    public void Reference_HasDateTimeOffsetType_True()
    {
        Assert.True(_typeReferences["Datetimeoffset"].HasDateTimeOffsetType());
    }

    [Fact]
    public void Element_IsDecimalType_True()
    {
        Assert.True(_canBeReferencedTypes["Decimal"].IsDecimalType());
    }

    [Fact]
    public void Reference_HasDecimalType_True()
    {
        Assert.True(_typeReferences["Decimal"].HasDecimalType());
    }

    [Fact]
    public void Element_IsDoubleType_True()
    {
        Assert.True(_canBeReferencedTypes["Double"].IsDoubleType());
    }

    [Fact]
    public void Reference_HasDoubleType_True()
    {
        Assert.True(_typeReferences["Double"].HasDoubleType());
    }

    [Fact]
    public void Element_IsFloatType_True()
    {
        Assert.True(_canBeReferencedTypes["Float"].IsFloatType());
    }

    [Fact]
    public void Reference_HasFloatType_True()
    {
        Assert.True(_typeReferences["Float"].HasFloatType());
    }

    [Fact]
    public void Element_IsGuidType_True()
    {
        Assert.True(_canBeReferencedTypes["Guid"].IsGuidType());
    }

    [Fact]
    public void Reference_HasGuidType_True()
    {
        Assert.True(_typeReferences["Guid"].HasGuidType());
    }

    [Fact]
    public void Element_IsIntType_True()
    {
        Assert.True(_canBeReferencedTypes["Int"].IsIntType());
    }

    [Fact]
    public void Reference_HasIntType_True()
    {
        Assert.True(_typeReferences["Int"].HasIntType());
    }

    [Fact]
    public void Element_IsLongType_True()
    {
        Assert.True(_canBeReferencedTypes["Long"].IsLongType());
    }

    [Fact]
    public void Reference_HasLongType_True()
    {
        Assert.True(_typeReferences["Long"].HasLongType());
    }

    [Fact]
    public void Element_IsObjectType_True()
    {
        Assert.True(_canBeReferencedTypes["Object"].IsObjectType());
    }

    [Fact]
    public void Reference_HasObjectType_True()
    {
        Assert.True(_typeReferences["Object"].HasObjectType());
    }

    [Fact]
    public void Element_IsShortType_True()
    {
        Assert.True(_canBeReferencedTypes["Short"].IsShortType());
    }

    [Fact]
    public void Reference_HasShortType_True()
    {
        Assert.True(_typeReferences["Short"].HasShortType());
    }
    
    [Fact]
    public void Element_IsStringType_True()
    {
        Assert.True(_canBeReferencedTypes["String"].IsStringType());
    }

    [Fact]
    public void Reference_HasStringType_True()
    {
        Assert.True(_typeReferences["String"].HasStringType());
    }
}

class TypeReference : ITypeReference
{
    public TypeReference(ICanBeReferencedType element)
    {
        Element = element;
    }

    public IEnumerable<IStereotype> Stereotypes { get; }
    public bool IsNullable { get; }
    public bool IsCollection { get; }
    public ICanBeReferencedType Element { get; }
    public IEnumerable<ITypeReference> GenericTypeParameters { get; }
}

class CanBeReferenceType : ICanBeReferencedType
{
    public CanBeReferenceType(string id)
    {
        Id = id;
    }

    public string Id { get; }
    public IEnumerable<IStereotype> Stereotypes { get; }
    public string SpecializationType { get; }
    public string SpecializationTypeId { get; }
    public string Name { get; }
    public string Comment { get; }
    public ITypeReference TypeReference { get; }
    public IPackage Package { get; }
}