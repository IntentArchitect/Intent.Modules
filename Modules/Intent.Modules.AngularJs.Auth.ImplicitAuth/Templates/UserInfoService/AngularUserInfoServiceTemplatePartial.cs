using Intent.Modules.Bower.Contracts;
using Intent.Engine;
using Intent.Templates
using System.Collections.Generic;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.UserInfoService
{
    partial class AngularUserInfoServiceTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.UserInfoService";

        public AngularUserInfoServiceTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "UserInfoService",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth\Services"

                );
        }

        public IEnumerable<IBowerPackageInfo> GetBowerDependencies()
        {
            return new[]
            {
                BowerPackages.AngularLocalStorage
            };
        }
    }
}
