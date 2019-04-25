using System.Collections.Generic;

namespace Intent.Modules.Common.Templates
{
    public interface IHasAdditionalHeaderInformation
    {
        IEnumerable<string> GetAdditionalHeaderInformation();
    }
}
