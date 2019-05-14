using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    public static class DTOModelExtensions
    {
        public static IEnumerable<string> GetConsumers<T>(this T dto, string stereotypeName, string stereotypePropertyName) where T : IHasFolder, IHasStereotypes
        {
            return dto.HasStereotype(stereotypeName)
                ? dto.GetStereotypeProperty(stereotypeName, stereotypePropertyName, "").Split(',').Select(x => x.Trim())
                : dto.GetStereotypeInFolders(stereotypeName).GetProperty(stereotypePropertyName, "").Split(',').Select(x => x.Trim());
        }
    }
}