using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Common.Java.Tests;

public class JavaTypeCheckExtensionTests
{
    private static readonly Dictionary<string, ITypeReference> _typeReferences;
    private static readonly Dictionary<string, ICanBeReferencedType> _canBeReferencedTypes;

    static JavaTypeCheckExtensionTests()
    {
        var location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
            "../../../..", "Intent.Modules.Common.Java", "Intent.Metadata", "Module Builder", "Intent.Common.Java", "Elements", "Type-Definition");
        var commonTypeElements = ElementPersistable.LoadFromDirectory(location, "*.xml");

        _canBeReferencedTypes = commonTypeElements.ToDictionary(k => k.Name.ToPascalCase(), v => (ICanBeReferencedType)new CanBeReferenceType(v.Id));
        _typeReferences = commonTypeElements.ToDictionary(k => k.Name.ToPascalCase(), v => (ITypeReference)new TypeReference(_canBeReferencedTypes[v.Name.ToPascalCase()]));
    }

    [Fact]
    public void Element_IsJavaBooleanType_True()
    {
        Assert.True(_canBeReferencedTypes["Boolean"].IsJavaBooleanType());
    }

    [Fact]
    public void Reference_HasJavaBooleanType_True()
    {
        Assert.True(_typeReferences["Boolean"].HasJavaBooleanType());
    }
    
    [Fact]
    public void Element_IsJavaByteType_True()
    {
        Assert.True(_canBeReferencedTypes["Byte"].IsJavaByteType());
    }

    [Fact]
    public void Reference_HasJavaByteType_True()
    {
        Assert.True(_typeReferences["Byte"].HasJavaByteType());
    }
    
    [Fact]
    public void Element_IsJavaCharacterType_True()
    {
        Assert.True(_canBeReferencedTypes["Character"].IsJavaCharacterType());
    }

    [Fact]
    public void Reference_HasJavaCharacterType_True()
    {
        Assert.True(_typeReferences["Character"].HasJavaCharacterType());
    }
    
    [Fact]
    public void Element_IsJavaDoubleType_True()
    {
        Assert.True(_canBeReferencedTypes["Double"].IsJavaDoubleType());
    }

    [Fact]
    public void Reference_HasJavaDoubleType_True()
    {
        Assert.True(_typeReferences["Double"].HasJavaDoubleType());
    }
    
    [Fact]
    public void Element_IsJavaFloatType_True()
    {
        Assert.True(_canBeReferencedTypes["Float"].IsJavaFloatType());
    }

    [Fact]
    public void Reference_HasJavaFloatType_True()
    {
        Assert.True(_typeReferences["Float"].HasJavaFloatType());
    }
    
    [Fact]
    public void Element_IsJavaIntegerType_True()
    {
        Assert.True(_canBeReferencedTypes["Integer"].IsJavaIntegerType());
    }

    [Fact]
    public void Reference_HasJavaIntegerType_True()
    {
        Assert.True(_typeReferences["Integer"].HasJavaIntegerType());
    }
    
    [Fact]
    public void Element_IsJavaLongType_True()
    {
        Assert.True(_canBeReferencedTypes["Long"].IsJavaLongType());
    }

    [Fact]
    public void Reference_HasJavaLongType_True()
    {
        Assert.True(_typeReferences["Long"].HasJavaLongType());
    }
    
    [Fact]
    public void Element_IsJavaMapType_True()
    {
        Assert.True(_canBeReferencedTypes["Map"].IsJavaMapType());
    }

    [Fact]
    public void Reference_HasJavaMapType_True()
    {
        Assert.True(_typeReferences["Map"].HasJavaMapType());
    }
    
    [Fact]
    public void Element_IsJavaShortType_True()
    {
        Assert.True(_canBeReferencedTypes["Short"].IsJavaShortType());
    }

    [Fact]
    public void Reference_HasJavaShortType_True()
    {
        Assert.True(_typeReferences["Short"].HasJavaShortType());
    }
    
    [Fact]
    public void Element_IsJavaStringType_True()
    {
        Assert.True(_canBeReferencedTypes["String"].IsJavaStringType());
    }

    [Fact]
    public void Reference_HasJavaStringType_True()
    {
        Assert.True(_typeReferences["String"].HasJavaStringType());
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