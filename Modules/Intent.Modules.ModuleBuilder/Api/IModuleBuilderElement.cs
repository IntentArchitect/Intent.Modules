using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleBuilderElement : IHasStereotypes, IHasFolder
    {
        string Id { get; }
        string SpecializationType { get; }
        string Name { get; }
        IEnumerable<IGenericType> GenericTypes { get; }
        IElement ParentElement { get; }
        IElementApplication Application { get; }
        string Comment { get; }
    }
}