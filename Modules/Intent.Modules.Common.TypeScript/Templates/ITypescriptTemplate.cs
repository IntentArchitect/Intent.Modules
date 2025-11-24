using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Templates;

public interface ITypescriptTemplate : IIntentTemplate
{
    ITypescriptCodeContext RootCodeContext { get; }
}
