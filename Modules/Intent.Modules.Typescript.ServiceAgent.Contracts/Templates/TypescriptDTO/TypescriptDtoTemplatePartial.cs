using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    partial class TypescriptDtoTemplate : IntentTypescriptProjectItemTemplateBase<DTOModel>, ITemplate
    {
        public const string LocalIdentifier = "Intent.Typescript.ServiceAgent.Contracts.DTO.Local";
        public const string RemoteIdentifier = "Intent.Typescript.ServiceAgent.Contracts.DTO.Remote";
        public TypescriptDtoTemplate(string identifier, IProject project, DTOModel model)
            : base(identifier, project, model)
        {
            // For reference purposes only:
            //Namespace = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? "App.Contracts" : $"App.Contracts.{model.BoundedContextName}";
            //Location = model.BoundedContextName == project.ApplicationName().Replace("_Client", "") ? $@"wwwroot\App\DTOs\Generated" : $@"wwwroot\App\DTOs\Generated\{model.BoundedContextName}";
        }

        public string ApplicationName => Model.Application.Name;

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: Model.Name,
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\DTOs\Generated",
                className: "${Name}",
                @namespace: "App.Contracts");
        }
    }
}
