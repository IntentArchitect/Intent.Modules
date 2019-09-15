using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModeler : IHasStereotypes, IHasFolder
    {
        string Id { get; }
        string Name { get; }
        IEnumerable<IModelerModelType> ModelTypes { get; }
        string ModuleDependency { get; }
        string ModuleVersion { get; }
    }

    public interface IModelerModelType
    {
        string Id { get; }
        string Name { get; }
        string LoadMethod { get; }
    }
}