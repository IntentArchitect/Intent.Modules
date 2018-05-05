using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.Config.Templates.TemplateVariables
{
    partial class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase
    {
        public const string Identifier = "Templates.Config.TemplateVariables";

        public Template(IProject project)
            : base(Identifier, project)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            var result = new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "TemplateVariables",
                fileExtension: "txt",
                defaultLocationInProject: "Templates\\Config\\TemplateVariables"
                );

            return result;
        }

        //You can encapsulate the config through properties like this
        public string Variable1
        {
            get
            {
                return FileMetaData.CustomMetaData["Variable1"];
            }
        }
    }
}
