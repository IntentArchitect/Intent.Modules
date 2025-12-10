using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.TypeResolvers
{
    public class CSharpTypeResolverTests
    {
        [Theory]
        [InlineData("Dictionary<string, int>")]
        [InlineData("System.Collections.Generic.Dictionary<string, int>")]
        [InlineData("System.Collections.Generic.Dictionary<String, System.Int32>")]
        [InlineData("System.Collections.Generic.Dictionary<System.String, System.Int32>")]
        public void ItShouldGetTypeReferenceForGenerics(string input)
        {
            // Arrange
            var project = Substitute.For<ICSharpProject>();
            var package = Substitute.For<IPackage>();

            var typeResolver = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("List<{0}>"),
                defaultNullableFormatter: CSharpNullableFormatter.Create(project));

            // Act
            var result = typeResolver.TryGetTypeReference(input, package, out var typeReference);

            typeReference.ShouldNotBeNull();
            typeReference.Element.Name.ShouldBe("Dictionary");
            typeReference.GenericTypeParameters[0].Element.Name.ShouldBe("string");
            typeReference.GenericTypeParameters[1].Element.Name.ShouldBe("int");
        }

        [Theory]
        [InlineData("IEnumerable<string>")]
        [InlineData("System.Collections.Generic.IEnumerable<string>")]
        [InlineData("System.Collections.Generic.IEnumerable<String>")]
        [InlineData("System.Collections.Generic.IEnumerable<System.String>")]
        [InlineData("List<string>")]
        [InlineData("System.Collections.Generic.List<string>")]
        [InlineData("System.Collections.Generic.List<String>")]
        [InlineData("System.Collections.Generic.List<System.String>")]
        [InlineData("string[]")]
        [InlineData("String[]")]
        [InlineData("System.String[]")]
        public void ItShouldGetTypeReferenceForCollections(string input)
        {
            // Arrange
            var project = Substitute.For<ICSharpProject>();
            var package = Substitute.For<IPackage>();

            var typeResolver = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("List<{0}>"),
                defaultNullableFormatter: CSharpNullableFormatter.Create(project));

            // Act
            var result = typeResolver.TryGetTypeReference(input, package, out var typeReference);

            typeReference.ShouldNotBeNull();
            typeReference.IsCollection.ShouldBeTrue();
            typeReference.Element.Name.ShouldBe("string");
        }

        [Fact]
        public void ItShouldResolveNamespacesForClassProvidersAsGenericParameters()
        {
            // Arrange
            var project = Substitute.For<ICSharpProject>();

            var typeResolver = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("List<{0}>"),
                defaultNullableFormatter: CSharpNullableFormatter.Create(project));

            typeResolver.AddTypeSource(new TypeSource());

            var typeReference = TypeReference.ForTypeDefinition("Type", "Namespace", new[]
            {
                TypeReference.ForTypeSource("GenericType", "GenericTypeNamespace")
            });

            // Act
            var resolvedTypeInfo = typeResolver.Get(typeReference);

            // Assert
            var cSharpResolvedTypeInfo = resolvedTypeInfo.ShouldBeOfType<CSharpResolvedTypeInfo>();
            cSharpResolvedTypeInfo.GetNamespaces().ShouldContain("Namespace");
            cSharpResolvedTypeInfo.GetNamespaces().ShouldContain("GenericTypeNamespace");
        }

        [Fact]
        public void ItShouldAddGenericTypeParametersToTypeSourceTypes()
        {
            // Arrange
            var project = Substitute.For<ICSharpProject>();

            var typeResolver = new CSharpTypeResolver(
                defaultCollectionFormatter: CSharpCollectionFormatter.Create("List<{0}>"),
                defaultNullableFormatter: CSharpNullableFormatter.Create(project));

            typeResolver.AddTypeSource(new TypeSource());

            var typeReference = TypeReference.ForTypeSource("Type", "Namespace", new[]
            {
                TypeReference.ForTypeDefinition("GenericType", "GenericTypeNamespace")
            });

            // Act
            var resolvedTypeInfo = typeResolver.Get(typeReference);

            // Assert
            var cSharpResolvedTypeInfo = resolvedTypeInfo.ShouldBeOfType<CSharpResolvedTypeInfo>();
            cSharpResolvedTypeInfo.ToString().ShouldBe("Type<GenericType>");
            cSharpResolvedTypeInfo.GetNamespaces().ShouldContain("Namespace");
            cSharpResolvedTypeInfo.GetNamespaces().ShouldContain("GenericTypeNamespace");
        }

        private class TypeSource : ITypeSource
        {
            public IResolvedTypeInfo GetType(ITypeReference typeInfo)
            {
                if (typeInfo is not TypeReference { IsForTypeSource: true } typeReference)
                {
                    return null;
                }

                return new ResolvedTypeInfo
                {
                    Template = new ClassProvider
                    {
                        ClassName = typeReference.TypeSourceType,
                        Namespace = typeReference.TypeSourceNamespace
                    }
                };
            }

            public IEnumerable<ITemplateDependency> GetTemplateDependencies() => null;

            public ICollectionFormatter CollectionFormatter => null;

            public INullableFormatter NullableFormatter => null;
        }

        private class TypeReference : ITypeReference
        {
            public static TypeReference ForTypeDefinition(
                string type,
                string @namespace,
                IEnumerable<ITypeReference> genericTypeParameters = null)
            {
                return new TypeReference
                {
                    Element = new CanBeReferencedType
                    {
                        Stereotypes = CSharpStereotype.CreateStereotypes(type, @namespace),
                    },
                    GenericTypeParameters = genericTypeParameters ?? Enumerable.Empty<ITypeReference>()
                };
            }

            public static TypeReference ForTypeSource(
                string type,
                string @namespace,
                IEnumerable<ITypeReference> genericTypeParameters = null)
            {
                return new TypeReference
                {
                    IsForTypeSource = true,
                    TypeSourceType = type,
                    TypeSourceNamespace = @namespace,
                    GenericTypeParameters = genericTypeParameters ?? Enumerable.Empty<ITypeReference>()
                };
            }

            public bool IsForTypeSource { get; set; }
            public string TypeSourceType { get; set; }
            public string TypeSourceNamespace { get; set; }

            public IEnumerable<IStereotype> Stereotypes { get; set; } = Enumerable.Empty<IStereotype>();
            public bool IsNullable { get; set; }
            public bool IsCollection { get; set; }
            public ICanBeReferencedType Element { get; set; }
            public IEnumerable<ITypeReference> GenericTypeParameters { get; set; } = Array.Empty<ITypeReference>();
        }

        private class ResolvedTypeInfo : IResolvedTypeInfo
        {
            public string Name { get; set; }
            public bool IsPrimitive { get; set; }
            public bool IsNullable { get; set; }
            public bool IsCollection { get; set; }
            public ITemplate Template { get; set; }
            public ITypeReference TypeReference { get; set; }
            public IReadOnlyList<IResolvedTypeInfo> GenericTypeParameters { get; set; } = Array.Empty<IResolvedTypeInfo>();
            public INullableFormatter NullableFormatter { get; set; }
            public ICollectionFormatter CollectionFormatter { get; set; }
            public IEnumerable<ITemplate> TemplateDependencies { get; set; } = Enumerable.Empty<ITemplate>();

            public IEnumerable<ITemplate> GetTemplateDependencies() => TemplateDependencies;

            public IResolvedTypeInfo WithIsNullable(bool isNullable)
            {
                IsNullable = isNullable;
                return this;
            }
        }

        private class HasTypeReference : IHasTypeReference
        {
            public static IHasTypeReference Create(ITypeReference typeReference)
            {
                return new HasTypeReference
                {
                    TypeReference = typeReference
                };
            }

            public ITypeReference TypeReference { get; private set; }
        }

        private class CanBeReferencedType : ICanBeReferencedType
        {
            public string Id { get; set; }
            public IEnumerable<IStereotype> Stereotypes { get; set; }
            public string SpecializationType { get; set; }
            public string SpecializationTypeId { get; set; }
            public string Name { get; set; }
            public string DisplayText { get; set; }
            public string Comment { get; set; }
            public ITypeReference TypeReference { get; set; }
            public IPackage Package { get; set; }
        }

        private class CSharpStereotype : IStereotype
        {
            public static IEnumerable<IStereotype> CreateStereotypes(string type, string @namespace)
            {
                yield return new CSharpStereotype(type, @namespace);
            }

            public CSharpStereotype(string type, string @namespace)
            {
                Properties = new[]
                {
                    new Property { Key = "Type", Value = type },
                    new Property { Key = "Namespace", Value = @namespace }
                };
            }

            public IStereotypeProperty GetProperty(string nameOrId) => Properties.Single(x => x.Key == nameOrId);

            public bool TryGetProperty(string nameOrId, out IStereotypeProperty property)
            {
                property = Properties.SingleOrDefault(x => x.Key == nameOrId);
                return property != null;
            }

            public string DefinitionId { get; }
            public IStereotypeDefinition Definition { get; }
            public string Name => "C#";

            public IEnumerable<IStereotypeProperty> Properties { get; }

            private class Property : IStereotypeProperty
            {
                public string PropertyDefinitionId { get; init; }
                public string Key { get; init; }
                public string Value { get; init; }
                public StereotypePropertyControlType ControlType => default;
                public StereotypePropertyOptionsSource OptionsSource => default;
            }
        }

        private class ClassProvider : IClassProvider
        {
            IFileMetadata ITemplate.GetMetadata() => throw new NotImplementedException();

            bool ITemplate.CanRunTemplate() => throw new NotImplementedException();

            string ITemplate.RunTemplate() => throw new NotImplementedException();

            string ITemplate.Id => null;

            public string Namespace { get; set; }
            public string ClassName { get; set; }
        }
    }
}
