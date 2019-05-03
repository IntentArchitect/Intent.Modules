using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Types.Contracts
{
    public interface ITypeResolverFactory
    {

        string Name { get; }

        int Priority { get; }

        ITypeResolver Create();

        IEnumerable<string> SupportedFileTypes { get;}
    }
}