using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;
using System;
using System.Collections.Generic;

[assembly: IntentTemplate("ModuleBuilder.Templates.Api.ApiModelInterfaceTemplate", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IFolder : IMetadataModel, IHasStereotypes
    {
        string Name { get; }

        IFolder ParentFolder { get; }

        IList<ICSharpTemplate> CSharpTemplates { get; }

        IList<IFileTemplate> FileTemplates { get; }

        IList<IFolder> Folders { get; }

        IList<IModelerReference> ModelerReferences { get; }

        IList<IDecorator> TemplateDecorators { get; }

        IList<ITypeDefinition> Types { get; }
    }

}