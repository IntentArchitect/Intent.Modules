using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaPackage : JavaNode
    {
        public JavaPackage(JavaParser.PackageDeclarationContext context, JavaFile file) : base(context, file)
        {
        }
        
        public override string GetIdentifier(ParserRuleContext context)
        {
            return GetTextWithComments();
        }
    }
}