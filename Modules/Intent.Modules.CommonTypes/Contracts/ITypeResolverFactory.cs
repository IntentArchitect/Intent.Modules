using Intent.MetaModel.Common;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.CommonTypes.Contracts
{
    public interface ITypeResolverFactory
    {

        string Name { get; }

        int Priotiry { get; }

        ITypeResolver Create();

        IEnumerable<string> SupportedFileTypes { get;}
    }
}