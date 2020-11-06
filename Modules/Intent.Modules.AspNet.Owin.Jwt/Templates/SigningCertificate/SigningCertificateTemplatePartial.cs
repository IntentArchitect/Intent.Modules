using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.AspNet.Owin.Jwt.Templates.SigningCertificate
{
    public partial class SigningCertificateTemplate : CSharpTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Owin.Jwt.SigningCertificate";

        public SigningCertificateTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "SigningCertificate",
                fileExtension: "cs",
                relativeLocation: "Certificate",
                className: "SigningCertificate",
                @namespace: "${Project.Name}"
                );
        }
    }
}
