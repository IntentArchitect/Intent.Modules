using System.Collections.Generic;
using Intent.SoftwareFactory.MetaModels.Mapping;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Mapping.EntityToDto.Templates.DTOMappingProfile
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
