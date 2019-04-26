using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;


namespace Intent.Modules.AspNetCore.Docker.Templates.DockerFile
{
    [Description(DockerfileTemplate.Identifier)]
    public class DockerfileTemplateRegistration : NoModelTemplateRegistrationBase
    {
        public override string TemplateId => DockerfileTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new DockerfileTemplate(project);
        }
    }
}
