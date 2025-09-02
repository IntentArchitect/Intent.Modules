// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using Intent.Utils;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Templates
{
    public class CSharpTemplateBaseTests
    {
        public class AddTypeSource_String
        {
            [Fact(Skip = "Needs investigation")]
            public void ItShouldUseTheDefaultTypeCollectionFormat()
            {
                // Arrange
                var template = new TestableCsharpTemplate();
                template.SetDefaultTypeCollectionFormat("List<{0}>");
                template.AddTypeSource(template.Id);

                var typeReference = Substitute.For<ITypeReference>();
                typeReference.Element.Returns(Substitute.For<IElement>());
                typeReference.IsCollection.Returns(true);

                Logging.SetTracing(Substitute.For<ITracing>());

                // Act
                var typeName = template.GetTypeName(typeReference);

                // Assert
                typeName.ShouldBe("List<TypeName>");
            }

            private class TestableCsharpTemplate : CSharpTemplateBase<object>
            {
                private static ITemplate _instance;

                public TestableCsharpTemplate()
                    : base(
                        "1",
                        GetOutputTarget(),
                        Substitute.For<IMetadataModel>())
                {
                    if (_instance != null) throw new Exception();
                    _instance = this;

                    var customMetadata = new Dictionary<string, string>();
                    foreach (var (key, value) in GetTemplateFileConfig().CustomMetadata)
                    {
                        customMetadata.Add(key, value);
                    }

                    var fileMetadata = Substitute.For<IFileMetadata>();
                    fileMetadata.CustomMetadata.Returns(customMetadata);
                    fileMetadata.GetFullLocationPath().Returns($"{Guid.NewGuid()}");
                    ConfigureFileMetadata(fileMetadata);
                }

                private static IOutputTarget GetOutputTarget()
                {
                    var ec = Substitute.For<ISoftwareFactoryExecutionContext>();
                    ec.FindTemplateInstance(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(_ => _instance);

                    var outputTarget = Substitute.For<IOutputTarget>();
                    outputTarget.ExecutionContext.Returns(ec);

                    return outputTarget;
                }

                public override string TransformText()
                {
                    return string.Empty;
                }

                protected override CSharpFileConfig DefineFileConfig()
                {
                    return new CSharpFileConfig("TypeName", "Namespace");
                }
            }
        }

        public class NormalizeNamespace_String
        {
            [Fact]
            public void ItShouldHandleNullableReferenceTypeCorrectly()
            {
                // Arrange
                var foreignType = "List<LeadStatusReasonLink>?";
                var csharpTemplateBase = new TestableCSharpTemplateBase();
                Logging.SetTracing(Substitute.For<ITracing>());

                // Act
                var result = csharpTemplateBase.NormalizeNamespace(foreignType);

                // Assert
                result.ShouldBe(foreignType);
            }

            [Fact]
            public void ItShouldHandleNullableTypesCorrectly()
            {
                // Arrange
                var foreignType = "bool?";
                var csharpTemplateBase = new TestableCSharpTemplateBase();
                Logging.SetTracing(Substitute.For<ITracing>());

                // Act
                var result = csharpTemplateBase.NormalizeNamespace(foreignType);

                // Assert
                result.ShouldBe(foreignType);
            }

            [Fact]
            public void ItShouldHandleUntrimmedGenericTypeParameter()
            {
                // Arrange
                var foreignType = "System.Collection.Generics.Dictionary<System.String, System.String>";
                var csharpTemplateBase = new TestableCSharpTemplateBase();
                csharpTemplateBase.AddUsing("System");
                csharpTemplateBase.AddUsing("System.Collection.Generics");
                Logging.SetTracing(Substitute.For<ITracing>());

                // Act
                var result = csharpTemplateBase.NormalizeNamespace(foreignType);

                // Assert
                result.ShouldBe("Dictionary<String, String>");
            }

            [Fact]
            public void ItShouldHandleVeryComplexTypesCorrectly()
            {
                // Arrange
                var foreignType = "Media.Api.Application.Common.Pagination.PagedResult<System.Collections.Generic.Dictionary<System.Guid, System.Collections.Generic.Dictionary<string, byte[]>>>";
                var csharpTemplateBase = new TestableCSharpTemplateBase();
                csharpTemplateBase.AddUsing("System");
                csharpTemplateBase.AddUsing("System.Collections.Generic");
                csharpTemplateBase.AddUsing("Media.Api.Application.Common.Pagination");
                Logging.SetTracing(Substitute.For<ITracing>());

                // Act
                var result = csharpTemplateBase.NormalizeNamespace(foreignType);

                // Assert
                result.ShouldBe("PagedResult<Dictionary<Guid, Dictionary<string, byte[]>>>");
            }

            // [Fact]
            // public void TypesWithSameNamesButDifferentNamespacesAreDoneCorrectly()
            // {
            //     var csharpTemplateBase = new TestableCSharpTemplateBase();
            //     csharpTemplateBase.Namespace = "AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories";
            //     csharpTemplateBase.AddUsing("System");
            //     csharpTemplateBase.AddUsing("System.Collections.Generic");
            //     csharpTemplateBase.AddUsing("Microsoft.Azure.CosmosRepository");
            //     csharpTemplateBase.AddUsing("AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories");
            //     
            //     // Act
            //     var externalType = csharpTemplateBase.NormalizeNamespace("Microsoft.Azure.CosmosRepository.IRepository<TDocument>");
            //     var internalType = csharpTemplateBase.NormalizeNamespace("AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories.IRepository<TDocument>");
            //
            //     // Assert
            //     externalType.ShouldBe("IRepository<TDocument>");
            //     internalType.ShouldBe("");
            // }

            private class TestableCSharpTemplateBase : CSharpTemplateBase
            {
                public TestableCSharpTemplateBase() : base(string.Empty, Substitute.For<IOutputTarget>())
                {
                    var fileMetadata = Substitute.For<IFileMetadata>();
                    fileMetadata.CustomMetadata.Returns(new Dictionary<string, string>());
                    fileMetadata.GetFullLocationPath().Returns($"{Guid.NewGuid()}");

                    ConfigureFileMetadata(fileMetadata);
                }

                public string Namespace
                {
                    get => base.Namespace;
                    set => FileMetadata.CustomMetadata["Namespace"] = value;
                }

                public override string TransformText()
                {
                    throw new NotImplementedException();
                }

                protected override CSharpFileConfig DefineFileConfig()
                {
                    throw new NotImplementedException();
                    //return new CSharpFileConfig(string.Empty, string.Empty);
                }
            }
        }

        public class UseType_Usings
        { 
            [Fact]
            public void ItShouldAddCorrectUsingsWithNestedGenericWithQualifiedArgument()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();
                // Act
                var resolvedType = template.UseType("Microsoft.Azure.CosmosRepository.IRepository<NestedOne.Entity<NestedOne.EntityTwo>>");

                // Assert
                resolvedType.ShouldBe("IRepository<Entity<EntityTwo>>");
                template.DeclareUsings().ShouldBe(["Microsoft.Azure.CosmosRepository", "NestedOne"]);
            }

            [Fact]
            public void ItShouldAddCorrectUsingsWithNestedGenericWithUnQualifiedArgument()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();
                var useTypeString = "IRepository<Entity<EntityTwo>>";

                // Act
                var resolvedType = template.UseType(useTypeString);

                // Assert
                resolvedType.ShouldBe(useTypeString);
                template.DependencyUsings.ShouldBe("");
            }

            [Fact]
            public void ItShouldAddCorrectUsingsWithNestedGenericWithPartialQualifiedArgument()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();

                // Act
                var resolvedType = template.UseType("Microsoft.Azure.CosmosRepository.IRepository<Entity>");

                // Assert
                resolvedType.ShouldBe("IRepository<Entity>");
                template.DependencyUsings.ShouldBe("using Microsoft.Azure.CosmosRepository;");
            }

            [Fact]
            public void ItShouldNotAddUsingsUnqualifiedName()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();

                // Act
                var resolvedType = template.UseType("IRepository");

                // Assert
                resolvedType.ShouldBe("IRepository");
                template.DependencyUsings.ShouldBe("");
            }

            [Fact]
            public void ItShouldNotAddUsingsUnqualifiedGenericName()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();

                // Act
                var resolvedType = template.UseType("IRepository<Entity>");

                // Assert
                resolvedType.ShouldBe("IRepository<Entity>");
                template.DependencyUsings.ShouldBe("");
            }



            [Fact]
            public void ItShouldNotAddUsingsQualifiedName()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();

                // Act
                var resolvedType = template.UseType("MyNamespace.IRepository");

                // Assert
                resolvedType.ShouldBe("IRepository");
                template.DependencyUsings.ShouldBe("using MyNamespace;");
            }

            [Fact]
            public void ItShouldRecursivelySimplifyGenericArguments()
            {
                // Arrange
                var template = new TestableCSharpTemplateBase();

                // Act
                var resolvedType = template.UseType("Namespace1.Type1<Namespace2.Type2, Namespace3.Type3>");

                // Assert
                resolvedType.ShouldBe("Type1<Type2, Type3>");
                template.DeclareUsings().ShouldBe(["Namespace1", "Namespace2", "Namespace3"]);
            }

            private class TestableCSharpTemplateBase : CSharpTemplateBase
            {
                public TestableCSharpTemplateBase() : base(string.Empty, Substitute.For<IOutputTarget>())
                {
                    var fileMetadata = Substitute.For<IFileMetadata>();
                    fileMetadata.CustomMetadata.Returns(new Dictionary<string, string>());
                    fileMetadata.GetFullLocationPath().Returns($"{Guid.NewGuid()}");

                    ConfigureFileMetadata(fileMetadata);
                }

                public override string TransformText()
                {
                    throw new NotImplementedException();
                }

                protected override CSharpFileConfig DefineFileConfig()
                {
                    throw new NotImplementedException();
                    //return new CSharpFileConfig(string.Empty, string.Empty);
                }
            }
        }
    }
}
