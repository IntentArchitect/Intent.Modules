using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaInterfaceType : JavaNode
    {
        public JavaInterfaceType(JavaParser.TypeTypeContext context, JavaNode parent) : base(context, parent)
        {

        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }
    }
}