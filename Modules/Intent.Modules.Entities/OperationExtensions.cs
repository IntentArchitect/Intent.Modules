using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;

namespace Intent.Modules.Entities
{
    public static class OperationExtensions
    {
        public static bool IsAsync(this IOperation operation)
        {
            return operation.HasStereotype("Asynchronous");
        }
    }
}
