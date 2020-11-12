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

        public static IList<JavaParameter> GetParameters(this Java9Parser.FormalParameterListContext formalParameterList, JavaNode parent)
        {
            return (formalParameterList.formalParameters()?.formalParameter()
                    .Select(x => new JavaParameter(x, parent)) ?? new List<JavaParameter>())
                .Concat(new[] { new JavaParameter(formalParameterList.lastFormalParameter().formalParameter(), parent) }).ToList();
        }
    }
}