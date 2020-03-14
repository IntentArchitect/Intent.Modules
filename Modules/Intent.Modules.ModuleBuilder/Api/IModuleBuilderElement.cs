using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleBuilderElement : IMetadataModel, IHasStereotypes, IHasFolder
    {
        //ModuleBuilderElementType Type { get; }
        //string Name { get; }
        //IElementApplication Application { get; }
        //string Comment { get; }
        //bool IsTemplate();
        //bool IsDecorator();
    }

    public enum ModuleBuilderElementType
    {
        CSharpTemplate,
        FileTemplate,
        Decorator
    }
}