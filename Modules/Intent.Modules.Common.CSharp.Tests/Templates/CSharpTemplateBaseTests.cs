// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Templates
{
    public class CSharpTemplateBaseTests
    {
        public class AddTypeSource_String
        {
            [Fact]
            public void ItShouldUseTheDefaultTypeCollectionFormat()
            {
                // Arrange
                var template = new TestableCsharpTemplate();
                template.SetDefaultTypeCollectionFormat("List<{0}>");
                template.AddTypeSource(template.Id);

                var typeReference = Substitute.For<ITypeReference>();
                typeReference.Element.Returns(Substitute.For<IElement>());
                typeReference.IsCollection.Returns(true);

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

                // Act
                var result = csharpTemplateBase.NormalizeNamespace(foreignType);

                // Assert
                result.ShouldBe("Dictionary<String, String>");
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
