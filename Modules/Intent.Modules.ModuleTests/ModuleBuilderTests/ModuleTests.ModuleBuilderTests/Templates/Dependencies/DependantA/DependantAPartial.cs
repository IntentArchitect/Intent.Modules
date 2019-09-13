using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.RoslynProjectItemTemplate.Partial", Version = "1.0")]

namespace ModuleTests.ModuleBuilderTests.Templates.Dependencies.DependantA
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    partial class DependantA : IntentRoslynProjectItemTemplateBase<IClass>, IHasTemplateDependencies
    {
        public const string TemplateId = "ModuleBuilderTests.DependantA";

        public DependantA(IProject project, IClass model) : base(TemplateId, project, model)
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
                fileName: "${Model.Name}Dependant",
                fileExtension: "cs",
                defaultLocationInProject: "DependantA",
                className: "${Model.Name}Dependant",
                @namespace: "${Project.Name}.DependantA"
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully, Signature = Mode.Fully)]
        IEnumerable<ITemplateDependancy> IHasTemplateDependencies.GetTemplateDependencies()
        {
            var templateDependencies = new List<ITemplateDependancy>();
            templateDependencies.AddRange(GetCustomTemplateDependencies());
            return templateDependencies;
        }


        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        private IEnumerable<ITemplateDependancy> GetCustomTemplateDependencies()
        {
            return new ITemplateDependancy[]
            {
            };
        }
    }
}