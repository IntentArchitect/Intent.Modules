using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Templates;
using ModuleTests.ModuleBuilderTests.Templates.Dependencies.DependantA;
using ModuleTests.ModuleBuilderTests.Templates.Dependencies.DependantB;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Composite
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    partial class Composite : IntentRoslynProjectItemTemplateBase<IClass>, IHasDecorators<ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract>
    {
        public const string TemplateId = "ModuleBuilderTests.Composite";

        public Composite(IProject project, IClass model) : base(TemplateId, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Composite",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.Composite"
            );
        }

        private ICollection<ICompositeContract> _decorators = new List<ICompositeContract>();

        [IntentManaged(Mode.Fully)]
        public void AddDecorator(ModuleTests.ModuleBuilderTests.Templates.Composite.ICompositeContract decorator)
        {
            _decorators.Add(decorator);
        }

        [IntentManaged(Mode.Fully)]
        public IEnumerable<ICompositeContract> GetDecorators()
        {
            return _decorators;
        }

        private string GetDecoratorOutput()
        {
            var outputList = new List<string>();

            outputList.AddRange(GetDecorators().Select(s => s.GetDecoratorText()));

            return string.Join(Environment.NewLine, outputList);
        }
    }
}