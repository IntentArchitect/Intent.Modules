using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.CommonTypes.Templates.AgnosticTypesTypeScript
{
    partial class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase<IClass>
    {
        public const string Identifier = "Templates.CommonTypes.AgnosticTypesTypeScript";

        public Template(IProject project, IClass model)
            : base(Identifier, project, model)
        {
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            var result = new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: Model.Name,
                fileExtension: "ts",
                defaultLocationInProject: "Templates\\CommonTypes\\AgnosticTypesTypeScript"
                );

            return result;
        }
    }
}
