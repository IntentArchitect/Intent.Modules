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
    partial class Composite : IntentRoslynProjectItemTemplateBase<IClass>, IHasTemplateDependencies
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

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        private string GetDependantATemplateFullName()
        {
            var templateDependency = TemplateDependancy.OnTemplate("ModuleBuilderTests.DependantA");
            var template = Project.FindTemplateInstance<IHasClassDetails>(templateDependency);
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        private string GetDependantBTemplateFullName()
        {
            var templateDependency = TemplateDependancy.OnTemplate("ModuleBuilderTests.DependantB");
            var template = Project.FindTemplateInstance<IHasClassDetails>(templateDependency);
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }
    }
}