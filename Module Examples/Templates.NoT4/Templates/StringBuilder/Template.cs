using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Engine;

namespace Templates.NoT4Template.Templates.StringBuilder
{
    public class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase
    {
        public const string Identifier = "Templates.NoT4.StringBuilder";

        public Template(IProject project)
            : base (Identifier, project)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "NoT4",
                fileExtension: "txt",
                defaultLocationInProject: "Templates\\NoT4Template\\StringBuilder"
                );
        }

        public override string TransformText()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            result.AppendLine("Sample not using T4");
            result.AppendLine("~~~~~~~~~~~~~~~~~~~");
            result.AppendLine();
            result.AppendLine("Obvoiously this could have been any type of out put,");
            result.AppendLine("and you could have bound the template to a Model");
            result.AppendLine();
            result.AppendLine($"This file is in the project : {Project.Name}");

            return result.ToString();
        }
    }
}
