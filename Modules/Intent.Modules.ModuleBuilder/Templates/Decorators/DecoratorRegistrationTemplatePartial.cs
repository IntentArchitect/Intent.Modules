using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.ModuleBuilder.Templates.Decorators
{
    partial class DecoratorRegistrationTemplate : IntentRoslynProjectItemTemplateBase<IClass>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.DecoratorRegistration.Template";

        public DecoratorRegistrationTemplate(IProject project, IClass model) : base(TemplateId, project, model)
        {
        }

        public IList<string> FolderBaseList => new[] { "Decorators" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Decorators") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Registration",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}Registration",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DecoratorTemplate.TemplateId)
            };
        }

        private IHasClassDetails GetDecoratorTemplate(IClass model)
        {
            return Project.FindTemplateInstance<IHasClassDetails>(DecoratorTemplate.TemplateId, model);
        }

        private string GetDecoratorTemplateFullName(IClass model)
        {
            return GetDecoratorTemplate(model).FullTypeName();
        }
    }
}
