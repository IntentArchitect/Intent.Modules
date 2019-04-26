using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modelers.Services.Api
{
    public interface IServiceModel : IMetaModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }

        string ApplicationName { get; }

        IApplication Application { get; }

        IEnumerable<IOperation> Operations { get; }

        string Comment { get; }
    }
}