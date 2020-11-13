using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Editor
{
    public static class JavaContextExtensions
    {
        public static IList<string> GetParameterTypes(this JavaParser.FormalParameterListContext formalParameterList)
        {
            return (formalParameterList.formalParameter()
                .Select(x => x.typeType().GetText()))
                .ToList();
        }

        public static IList<JavaParameter> GetParameters(this JavaParser.FormalParameterListContext formalParameterList, JavaNode parent)
        {
            return formalParameterList.formalParameter()
                .Select((x, index) => new JavaParameter(x, parent, index))
                .ToList();
        }
    }
}