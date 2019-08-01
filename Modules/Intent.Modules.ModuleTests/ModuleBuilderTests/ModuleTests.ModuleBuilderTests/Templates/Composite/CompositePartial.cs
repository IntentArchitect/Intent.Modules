using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Composite
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    partial class Composite : IntentRoslynProjectItemTemplateBase<IClass>, IDeclareUsings, IHasTemplateDependencies
    {
        public const string TemplateId = "ModuleBuilderTests.Composite";

        public Composite(IProject project, IClass model) : base(TemplateId, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Composite",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.Composite"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            var templateDependencies = new List<ITemplateDependancy>();
            templateDependencies.Add(TemplateDependancy.OnTemplate("ModuleBuilderTests.DependantA"));
            templateDependencies.Add(TemplateDependancy.OnTemplate("ModuleBuilderTests.DependantB"));
            return templateDependencies;
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public IEnumerable<string> DeclareUsings()
        {
            return new string[]
            {
                // Specify list of Namespaces here, example:
                "System.Linq"
            };
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        private IHasClassDetails GetDependantATemplate(IClass model)
        {
            return Project.FindTemplateInstance<IHasClassDetails>("ModuleBuilderTests.DependantA", model);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        private IntentProjectItemTemplateBase<IClass> GetDependantBTemplate(IClass model)
        {
            return Project.FindTemplateInstance<IntentProjectItemTemplateBase<IClass>>("ModuleBuilderTests.DependantB", model);
        }
    }
}