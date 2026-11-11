using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Templates
{
    public class GetNamespaceTests
    {
        [Fact]
        public void NoCSharpStereotypeUsesOutputTargetNamespace()
        {
            var template = CreateTemplate(new TestModel());

            template.GetNamespace().ShouldBe("My.Project");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void BlankStereotypeNamespaceUsesOutputTargetNamespace(string stereotypeNamespace)
        {
            var template = CreateTemplate(new TestModel(stereotypeNamespace));

            template.GetNamespace().ShouldBe("My.Project");
            template.GetNamespace("Sub").ShouldBe("My.Project.Sub");
        }

        [Fact]
        public void StereotypeNamespaceOverridesOutputTargetNamespace()
        {
            var template = CreateTemplate(new TestModel("Intent.Probe.Ns"));

            template.GetNamespace().ShouldBe("Intent.Probe.Ns");
        }

        [Fact]
        public void StereotypeNamespaceIgnoresAdditionalFolders()
        {
            var template = CreateTemplate(new TestModel("Intent.Probe.Ns"));

            template.GetNamespace("Sub").ShouldBe("Intent.Probe.Ns");
        }

        private static IIntentTemplate<TestModel> CreateTemplate(TestModel model)
        {
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.Name.Returns("My.Project");
            outputTarget.Metadata.Returns(new Dictionary<string, object>());
            outputTarget.GetTargetPath().Returns(new[] { outputTarget });

            var template = Substitute.For<IIntentTemplate<TestModel>>();
            template.OutputTarget.Returns(outputTarget);
            template.Model.Returns(model);
            return template;
        }

        public class TestModel : IHasStereotypes
        {
            public TestModel() { }

            public TestModel(string cSharpNamespace)
            {
                Stereotypes = new IStereotype[] { new CSharpStereotype(cSharpNamespace) };
            }

            public IEnumerable<IStereotype> Stereotypes { get; } = Enumerable.Empty<IStereotype>();
        }

        private class CSharpStereotype : IStereotype
        {
            private readonly Property _namespace;

            public CSharpStereotype(string cSharpNamespace) => _namespace = new Property { Key = "Namespace", Value = cSharpNamespace };

            public string DefinitionId => null;
            public IStereotypeDefinition Definition => null;
            public string Name => "C#";
            public IEnumerable<IStereotypeProperty> Properties => new[] { _namespace };
            public IStereotypeProperty GetProperty(string nameOrId) => _namespace;

            public bool TryGetProperty(string nameOrId, out IStereotypeProperty property)
            {
                property = nameOrId == "Namespace" ? _namespace : null;
                return property != null;
            }

            private class Property : IStereotypeProperty
            {
                public string PropertyDefinitionId => null;
                public string Key { get; init; }
                public string Value { get; init; }
                public StereotypePropertyControlType ControlType => default;
                public StereotypePropertyOptionsSource OptionsSource => default;
            }
        }
    }
}
