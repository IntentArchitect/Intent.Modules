using System.Collections.Generic;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaInterfaceMethod : JavaNode
    {
        private readonly Java9Parser.InterfaceMethodDeclarationContext _context;

        public JavaInterfaceMethod(Java9Parser.InterfaceMethodDeclarationContext context, JavaNode parent) : base(context, parent)
        {
            _context = context;
            Name = _context.methodHeader().methodDeclarator().identifier().GetText();
        }

        public string Name { get; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            var name = ((Java9Parser.InterfaceMethodDeclarationContext)context).methodHeader().methodDeclarator().identifier().GetText();

            IEnumerable<string> parameterTypes = ((Java9Parser.InterfaceMethodDeclarationContext)context)
                .methodHeader().methodDeclarator().formalParameterList()?.GetParameterTypes() ?? new List<string>();
            return $"{name}({string.Join(", ", parameterTypes)})";
        }
    }
}