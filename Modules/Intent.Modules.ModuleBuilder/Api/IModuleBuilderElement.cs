using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleBuilderElement : IHasStereotypes, IHasFolder
    {
        string Id { get; }
        ModuleBuilderElementType Type { get; }
        string Name { get; }
        IEnumerable<IGenericType> GenericTypes { get; }
        IElement ParentElement { get; }
        IElementApplication Application { get; }
        string Comment { get; }
        bool IsTemplate();
        bool IsDecorator();
    }

    public enum ModuleBuilderElementType
    {
        CSharpTemplate,
        FileTemplate,
        Decorator
    }
}