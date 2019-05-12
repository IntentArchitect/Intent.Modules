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
    [IntentManaged(Mode.Merge)]
    partial class AngularComponentHtmlTemplate : IntentProjectItemTemplateBase<IComponentModel>, IPostTemplateCreation, IHasDecorators<IOverwriteDecorator>
    {
        private readonly IList<IOverwriteDecorator> _decorators = new List<IOverwriteDecorator>();
        public const string TemplateId = "Angular.AngularComponentHtmlTemplate";

        public AngularComponentHtmlTemplate(IProject project, IComponentModel model) : base(TemplateId, project, model)
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

        public void Created()
        {
            var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            ModuleName = moduleTemplate.ModuleName;
        }

        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            if (source.StartsWith("<!--IntentManaged-->"))
            {
                return GetDecorators().Any() ? GetDecorators().First().GetOverwrite() ?? TransformText() : TransformText();
            }

            return source;
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
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ComponentName.ToAngularFileName()}.component",
                fileExtension: "html",
                defaultLocationInProject: $"Client\\src\\app\\{moduleTemplate.ModuleName.ToAngularFileName()}\\{ComponentName.ToAngularFileName()}"
                    );
        }
    }
}