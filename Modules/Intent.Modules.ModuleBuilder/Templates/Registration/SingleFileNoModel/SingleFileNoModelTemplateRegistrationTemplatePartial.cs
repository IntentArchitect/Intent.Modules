using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.SingleFileNoModel
{
    partial class SingleFileNoModelTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel";

        public SingleFileNoModelTemplateRegistrationTemplate(IOutputTarget outputTarget, TemplateRegistrationModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        public IList<string> OutputFolders => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();

        public string FolderPath => string.Join("/", OutputFolders);

        public string FolderNamespace => string.Join(".", OutputFolders);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}Registration",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}");
        }

        private string GetTemplateNameForTemplateId()
        {
            return TemplateName;
        }
    }
}