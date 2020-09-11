using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaSuperClass : JavaNode
    {
        public JavaSuperClass(Java9Parser.SuperclassContext context, JavaNode parent) : base(context, parent)
        {

        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }
    }
}