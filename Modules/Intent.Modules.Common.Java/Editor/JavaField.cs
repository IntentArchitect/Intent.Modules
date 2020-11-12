using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaField : JavaNode
    {
        private readonly JavaParser.FieldDeclarationContext _context;

        public JavaField(JavaParser.FieldDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
            Name = Identifier;
        }

        public string Name { get; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return ((JavaParser.FieldDeclarationContext)context).variableDeclarators().variableDeclarator(0).variableDeclaratorId().IDENTIFIER().GetText();
        }

        //public override bool IsIgnored()
        //{
        //    var isIgnored = _context.fieldModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
        //    return isIgnored;
        //}
    }
}