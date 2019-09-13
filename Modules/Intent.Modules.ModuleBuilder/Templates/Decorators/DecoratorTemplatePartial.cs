using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Decorators
{
    partial class DecoratorTemplate : IntentRoslynProjectItemTemplateBase<IDecoratorDefinition>
    {
        public const string TemplateId = "Intent.ModuleBuilder.DecoratorTemplate";

        public DecoratorTemplate(IProject project, IDecoratorDefinition model) : base(DecoratorTemplate.TemplateId, project, model)
        {
        }

        public IList<string> FolderBaseList => new[] { "Decorators" }.Concat(Model.GetFolderPath(false).Where((p, i) => (i == 0 && p.Name != "Decorators") || i > 0).Select(x => x.Name)).ToList();
        public string FolderPath => string.Join("/", FolderBaseList);
        public string FolderNamespace => string.Join(".", FolderBaseList);

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "${FolderPath}/${Model.Name}",
                className: "${Model.Name}",
                @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
            );
        }

        public string GetIdentifier()
        {
            return $"{Project.ApplicationName()}.{Model.Name}";
        }

        private bool HasDeclaresUsings()
        {
            return Model.GetStereotypeProperty<bool>("Decorator Settings", "Declare Usings");
        }

        private string GetConfiguredInterfaces()
        {
            var interfaceList = new List<string>();

            if (HasDeclaresUsings())
            {
                interfaceList.Add("IDeclareUsings");
            }

            return interfaceList.Any() ? (", " + string.Join(", ", interfaceList)) : string.Empty;
        }
    }
}
