using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.TypeScript;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    partial class TypescriptDtoTemplate : TypeScriptTemplateBase<DTOModel>, ITemplate
    {
        public const string LocalIdentifier = "Intent.Typescript.ServiceAgent.Contracts.DTO.Local";
        public const string RemoteIdentifier = "Intent.Typescript.ServiceAgent.Contracts.DTO.Remote";
        public TypescriptDtoTemplate(string identifier, IProject project, DTOModel model)
            : base(identifier, project, model)
        {
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, TypescriptDtoTemplate.LocalIdentifier));
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, TypescriptDtoTemplate.RemoteIdentifier));
            // For reference purposes only:
            //Namespace = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? "App.Contracts" : $"App.Contracts.{model.BoundedContextName}";
            //Location = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? $"wwwroot/App/DTOs/Generated" : $@"wwwroot/App/DTOs/Generated/{model.BoundedContextName}";
        }
        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new ITemplateDependency[0]; // disable adding on imports when merged
        }

        public string ApplicationName => Model.Application.Name;

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: Model.Name,
                relativeLocation: "",
                className: "${Model.Name}",
                @namespace: "App.Contracts");
        }
    }
}
