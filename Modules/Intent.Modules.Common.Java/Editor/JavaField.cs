using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaField : JavaNode<JavaParser.FieldDeclarationContext>
    {
        private readonly JavaParser.FieldDeclarationContext _context;

        public JavaField(JavaParser.FieldDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
            Name = Identifier;
        }

        public string Name { get; }
        public override IToken StartToken => Annotations.FirstOrDefault()?.StartToken ?? ((ParserRuleContext)Context.Parent.Parent).Start;

        public override string GetIdentifier(JavaParser.FieldDeclarationContext context)
        {
            return context.variableDeclarators().variableDeclarator(0).variableDeclaratorId().IDENTIFIER().GetText();
        }
    }
}