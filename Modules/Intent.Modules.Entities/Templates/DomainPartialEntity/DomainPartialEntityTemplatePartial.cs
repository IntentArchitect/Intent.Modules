using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Packages.Entities.Templates.DomainPartialEntity
{
    partial class DomainPartialEntityTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate
    {
        public const string Identifier = "Intent.Entities.PartialEntity";

        public DomainPartialEntityTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.OnceOff,
                fileName: "${Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Name}",
                @namespace: "${Project.ProjectName}"
                );
        }
    }
}
