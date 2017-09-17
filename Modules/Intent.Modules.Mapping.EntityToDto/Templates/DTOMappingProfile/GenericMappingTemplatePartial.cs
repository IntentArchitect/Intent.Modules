using Intent.SoftwareFactory.MetaModels.Mapping;
using System.Collections.Generic;

namespace Intent.Modules.Mapping.EntityToDto.Templates.DTOMappingProfile
{
    partial class GenericMappingTemplate
    {
        public GenericMappingTemplate(string @namespace, string profileName, IEnumerable<MappingModel> models)
        {
            Namespace = @namespace;
            ProfileName = profileName;
            Models = models;
        }

        public string RunTemplate() => TransformText();

        public string ProfileName { get; private set; }
        public string DeclareUsings { get; set; }
        public string Namespace { get; private set; }
        public IEnumerable<MappingModel> Models { get; private set; }

    }
}
