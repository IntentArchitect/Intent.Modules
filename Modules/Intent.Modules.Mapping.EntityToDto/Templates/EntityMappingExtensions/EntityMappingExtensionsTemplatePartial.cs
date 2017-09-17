using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Mapping.EntityToDto.Templates.EntityMappingExtensions
{
    partial class EntityMappingExtensionsTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Mapping.EntityToDto.EntityMappingExtensions";

        public EntityMappingExtensionsTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "EntityMappingExtensions",
                fileExtension: "cs",
                defaultLocationInProject: "Mapping",
                className: "EntityMappingExtensions",
                @namespace: "${Project.Name}.Mapping"
            );
        }
    }
}
