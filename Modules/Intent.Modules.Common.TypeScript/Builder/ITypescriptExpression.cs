using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Builder;

public interface ITypescriptExpression
{
    ITypescriptReferenceable Reference { get; }
    string GetText(string indentation);
    string ToString();
}
