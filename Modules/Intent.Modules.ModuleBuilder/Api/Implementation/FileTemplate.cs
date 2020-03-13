using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelImplementationTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class FileTemplate : ModuleBuilderElementBase, IFileTemplate
    {

        public FileTemplate(IElement element) : base(element)
        {
        }

        public string FileExtension => _element.GetStereotypeProperty(ModelExtensions.FileTemplateSettingsStereotype, "File Extension", "unknown");

        public override string ToString()
        {
            return $"[{nameof(FileTemplate)}: {Name}]";
        }
    }
}