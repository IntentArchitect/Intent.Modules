using Intent.SoftwareFactory.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Templates;

namespace Templates.BasicT4Template.Templates.VanillaT4
{
    public partial class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase
    {
        public const string Identifier = "Templates.BasicT4Template.VanillaT4";

        public Template(IProject project)
            : base(Identifier, project)
        {
            ClassName = "VanillaT4";
            Namespace = Project.Name + ".Templates.VanillaT4";
        }

        public string Namespace { get; private set; }
        public string ClassName { get; private set; }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: ClassName,
                fileExtension: "cs",
                defaultLocationInProject: "Templates\\BasicT4Template\\VanillaT4"
                );
        }

        internal string GetTypeFromPartial()
        {
            return "int";
        }

    }
}