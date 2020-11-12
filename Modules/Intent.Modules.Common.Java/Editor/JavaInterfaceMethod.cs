using System.Collections.Generic;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaInterfaceMethod : JavaNode
    {
        private readonly JavaParser.InterfaceMethodDeclarationContext _context;

        public JavaInterfaceMethod(JavaParser.InterfaceMethodDeclarationContext context, JavaNode parent) : base(context, parent)
        {
            _context = context;
            Name = _context.IDENTIFIER().GetText();
        }

        public string Name { get; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            var name = ((JavaParser.InterfaceMethodDeclarationContext)context).IDENTIFIER().GetText();

            IEnumerable<string> parameterTypes = ((JavaParser.InterfaceMethodDeclarationContext)context)
                .formalParameters().formalParameterList()?.GetParameterTypes() ?? new List<string>();
            return $"{name}({string.Join(", ", parameterTypes)})";
        }
    }
}