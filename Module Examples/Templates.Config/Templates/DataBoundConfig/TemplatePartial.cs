using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates.Config.Templates.DataBoundConfig
{
    partial class Template : Intent.SoftwareFactory.Templates.IntentProjectItemTemplateBase
    {
        public const string Identifier = "Templates.Config.DataBoundConfig";
        private Model _model;

        public Template(IProject project)
            : base(Identifier, project)
        {
            _model = new Model() { Name = "Model Name" };
        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            var result = new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "DataBoundConfig",
                fileExtension: "txt",
                defaultLocationInProject: "Templates\\Config\\DataBoundConfig"
                );

            return result;
        }

        public Model CustomModel
        {
            get
            {
                return _model;
            }
        }

        //You can encapsulate the config through properties like this
        public string Variable3
        {
            get
            {
                return "Variable 3 Value";
            }
        }
    }

    public class Model
    {
        public string Name { get; set; } 
    }
}
