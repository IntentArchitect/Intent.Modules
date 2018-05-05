using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetadataNavigation.ProjectsAndApplications.Templates.AsXml
{
    public partial class Template : IntentProjectItemTemplateBase<Intent.SoftwareFactory.Engine.IApplication>
    {
        public const string Identifier = "MetadataNavigation.ProjectsAndApplications.AsXml";

        public Template(IProject project, IApplication model)
            : base(Identifier, project, model)
        {

        }

        public override DefaultFileMetaData DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "Application-" + Model.ApplicationName,
                fileExtension: "xml",
                defaultLocationInProject: "MetadataNavigation\\ProjectsAndApplications\\AsXml"
                );
        }
    }
}
