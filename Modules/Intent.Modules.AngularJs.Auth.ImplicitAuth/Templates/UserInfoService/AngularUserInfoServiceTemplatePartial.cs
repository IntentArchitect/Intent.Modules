using System.Collections.Generic;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.AngularJs.Auth.ImplicitAuth.Templates.UserInfoService
{
    partial class AngularUserInfoServiceTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.UserInfoService";

        public AngularUserInfoServiceTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
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
                BowerPackageInfo.AngularLocalStorage
            };
        }
    }
}
