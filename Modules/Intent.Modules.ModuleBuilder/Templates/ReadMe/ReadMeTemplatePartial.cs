using System.IO;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    partial class ReadMeTemplate : IntentTemplateBase
    {
        public const string TemplateId = "Intent.ModuleBuilder.ReadMe";

        public ReadMeTemplate(IOutputTarget outputTarget) : base(TemplateId, outputTarget)
        {
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "README",
                fileExtension: "txt",
                relativeLocation: ""
            );
        }

        public string GetModuleOutputLocation()
        {
            return "./" + OutputTarget.Application.RootLocation.GetRelativePath(Path.Combine(OutputTarget.GetTargetPath()[0].Location, "../Intent.Modules")).Normalize();
        }
    }
}