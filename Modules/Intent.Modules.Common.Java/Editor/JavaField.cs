using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaField : JavaNode
    {
        private readonly Java9Parser.FieldDeclarationContext _context;

        public JavaField(Java9Parser.FieldDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
            Name = Identifier;
        }

        public string Name { get; }

        protected override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.FieldDeclarationContext)context).variableDeclaratorList().variableDeclarator(0).variableDeclaratorId().identifier().GetText();
        }

        public override bool IsIgnored()
        {
            var isIgnored = _context.fieldModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
            return isIgnored;
        }
    }
}