using Intent.Modules.Application.Contracts.Legacy.DTO;
using Intent.Modules.RichDomain.Templates.EntityStateInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Mapping;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.RichDomain;

namespace Intent.Modules.Mapping.EntityToDto.Templates.DTOMappingProfile
{
    public class DTOMappingTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<MappingModel>>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IDeclareUsings
    {
        public const string Identifier = "Intent.Mapping.EntityToDto.DTOMappingProfile";

        private readonly List<string> _additionalNamespaces;

        public DTOMappingTemplate(IProject project, IEnumerable<MappingModel> models, IEnumerable<EnumDefinition> enums)
            : base (Identifier, project, models)
        {
            // TODO: GCB - This depedency should not be necessary - investigate how to determine additional namepaces from MappingModel
            _additionalNamespaces = enums.Select(x => x.Stereotypes.GetTagValue<string>("CommonType", "Namespace", null)).Where(x => x != null).ToList();
        }

        public IEnumerable<string> DeclareUsings()
        {
            return _additionalNamespaces.Select(x => $"using {x};").ToArray();
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                    overwriteBehaviour: OverwriteBehaviour.Always,
                    fileName: "DtoMappingProfile",
                    fileExtension: "cs",
                    defaultLocationInProject: "Mapping",
                    className: "DtoMappingProfile",
                    @namespace: "${Project.Name}.Mapping"
                );
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public override string TransformText()
        {
            var genericMappingTemplate = new GenericMappingTemplate(this.Namespace, this.ClassName, Model);
            genericMappingTemplate.DeclareUsings = this.ResolveAllUsings(Project);
            return genericMappingTemplate.RunTemplate();
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DTOTemplate.Identifier),
                TemplateDependancy.OnTemplate(DomainEntityStateInterfaceTemplate.Identifier)
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return base.GetNugetDependencies()
                .Union(
                new []
                {
                    NugetPackages.AutoMapper,
                    NugetPackages.IntentFrameworkAutoMapper
                });
        }
    }
}
