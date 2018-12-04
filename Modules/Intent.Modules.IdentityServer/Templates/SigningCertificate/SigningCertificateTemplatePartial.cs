using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.IdentityServer.Templates.SigningCertificate
{
    public partial class SigningCertificateTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.IdentityServer.SigningCertificate";

        public SigningCertificateTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "SigningCertificate",
                fileExtension: "cs",
                defaultLocationInProject: "Certificate",
                className: "SigningCertificate",
                @namespace: "${Project.Name}"
                );
        }
    }
}
