using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Contracts
{

    public static class OperationExtensions
    {
        // Should come from APi generation
        public static bool IsAsync(this OperationModel operation)
        {
            return operation.HasStereotype("Asynchronous");
        }

        
    }
}
