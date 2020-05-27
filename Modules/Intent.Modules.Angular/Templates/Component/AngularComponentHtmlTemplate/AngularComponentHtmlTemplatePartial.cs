using System;
using System.IO;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Component.Layouts.PaginatedSearchLayout;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Component.AngularComponentHtmlTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Ignore)]
    partial class AngularComponentHtmlTemplate : IntentProjectItemTemplateBase<ComponentModel>, IHasDecorators<IOverwriteDecorator>
    {
        private readonly IList<IOverwriteDecorator> _decorators = new List<IOverwriteDecorator>();
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Component.AngularComponentHtmlTemplate";

        public AngularComponentHtmlTemplate(IProject project, ComponentModel model) : base(TemplateId, project, model)
        {
        }

        public string ComponentName
        {
            get
            {
                if (Model.Name.EndsWith("Component", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Model.Name.Substring(0, Model.Name.Length - "Component".Length);
                }
                return Model.Name;
            }
        }

        public string ModuleName { get; private set; }

        public override void OnCreated()
        {
            var moduleTemplate = Project.FindTemplateInstance<Module.AngularModuleTemplate.AngularModuleTemplate>(Module.AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            ModuleName = moduleTemplate.ModuleName;
        }

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            if (File.Exists(fullFileName))
            {
                var source = File.ReadAllText(fullFileName);
                if (source.StartsWith("<!--IntentManaged-->"))
                {
                    return GetDecorators().Any() ? GetDecorators().First().GetOverwrite() ?? base.RunTemplate() : base.RunTemplate();
                }
                else
                {
                    return source;
                }
            }

            return base.RunTemplate();
        }

        public void AddDecorator(IOverwriteDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IOverwriteDecorator> GetDecorators()
        {
            return _decorators;
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : base.RunTemplate();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            var moduleTemplate = Project.FindTemplateInstance<Module.AngularModuleTemplate.AngularModuleTemplate>(Module.AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ComponentName.ToKebabCase()}.component",
                fileExtension: "html",
                defaultLocationInProject: $"ClientApp/src/app/{moduleTemplate.ModuleName.ToKebabCase()}/{ComponentName.ToKebabCase()}"
                    );
        }
    }
}