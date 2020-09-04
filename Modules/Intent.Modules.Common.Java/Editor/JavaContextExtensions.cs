using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Editor
{
    public static class JavaContextExtensions
    {
        public static IList<string> GetParameterTypes(this Java9Parser.FormalParameterListContext formalParameterList)
        {
            return (formalParameterList.formalParameters()?.formalParameter()
                    .Select(x => x.unannType().GetText()) ?? new List<string>())
                .Concat(new[] { formalParameterList.lastFormalParameter().formalParameter().unannType().GetText() }).ToList();
        }
    }
}