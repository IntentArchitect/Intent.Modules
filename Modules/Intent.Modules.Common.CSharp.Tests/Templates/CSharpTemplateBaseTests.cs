using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

// ReSharper disable InconsistentNaming

namespace Intent.Modules.Common.CSharp.Tests.Templates
{
    public class CSharpTemplateBaseTests
    {
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

            private class TestableCSharpTemplateBase : CSharpTemplateBase
            {
                public TestableCSharpTemplateBase() : base(string.Empty, Substitute.For<IOutputTarget>())
                {
                    var fm = Substitute.For<IFileMetadata>();
                    fm.CustomMetadata.Returns(new Dictionary<string, string>());

                    ConfigureFileMetadata(fm);
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
