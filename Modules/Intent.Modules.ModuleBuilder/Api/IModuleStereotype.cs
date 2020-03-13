using System.Collections;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModuleStereotype : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string ParentFolderId { get; }
        IEnumerable<string> TargetTypes { get; }
        bool DisplayIcon { get; }
        string DisplayFunction { get; }
        bool AutoAdd { get; }
        IElement Parent { get; }
        IModulePackage GetPackage();
        IList<IModuleStereotypeProperty> Properties { get; }
    }
}