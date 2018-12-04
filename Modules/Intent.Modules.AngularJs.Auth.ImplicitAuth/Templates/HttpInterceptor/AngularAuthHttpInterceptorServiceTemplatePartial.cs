using Intent.Modules.Bower.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth.Templates.HttpInterceptor
{
    partial class AngularAuthHttpInterceptorServiceTemplate : IntentProjectItemTemplateBase<object>, ITemplate, IHasBowerDependencies
    {
        public const string Identifier = "Intent.AngularJs.Auth.ImplicitAuth.HttpInterceptor";

        public AngularAuthHttpInterceptorServiceTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "AuthHttpInterceptorService",
                fileExtension: "ts",
                defaultLocationInProject: @"wwwroot\App\Auth\Services"
                );  
        }

        public IEnumerable<IBowerPackageInfo> GetBowerDependencies()
        {
            return new[]
            {
                BowerPackages.OidcTokenManager
            };
        }
    }
}
