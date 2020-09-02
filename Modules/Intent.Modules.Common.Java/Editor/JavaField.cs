using System.Linq;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaField : JavaNode
    {
        public JavaClass Parent { get; }
        private readonly Java9Parser.FieldDeclarationContext _context;

        public JavaField(Java9Parser.FieldDeclarationContext context, JavaClass parent) : base(context, parent.File)
        {
            Parent = parent;
            _context = context;
            Name = _context.variableDeclaratorList().variableDeclarator(0).variableDeclaratorId().identifier().GetText();
            Identifier = Name; // plus parameter types
        }

        public string Name { get; }
        public override string Identifier { get; }

        public override bool IsIgnored()
        {
            var isIgnored = _context.fieldModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
            return isIgnored;
        }
    }
}