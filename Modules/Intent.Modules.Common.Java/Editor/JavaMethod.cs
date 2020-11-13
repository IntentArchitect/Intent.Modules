using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaMethod : JavaNode<JavaParser.MethodDeclarationContext>
    {
        private readonly JavaParser.MethodDeclarationContext _context;

        public JavaMethod(JavaParser.MethodDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
            Name = _context.IDENTIFIER().GetText();
        }

        public string Name { get; }
        public JavaReturnType ReturnType { get; private set; }
        public override IToken StartToken => Annotations.FirstOrDefault()?.StartToken ?? ((ParserRuleContext)Context.Parent.Parent).Start;

        public override string GetIdentifier(JavaParser.MethodDeclarationContext context)
        {
            var name = context.IDENTIFIER().GetText();
            if (HasIntentInstructions() && IsBodyIgnored())
            {
                return name;
            }
            var parameterTypes = context
                .formalParameters().formalParameterList()?.GetParameterTypes() ?? new List<string>();
            return $"{name}({string.Join(", ", parameterTypes)})";
        }

        public override void MergeWith(JavaNode node)
        {
            base.MergeWith(node);
            ReturnType.ReplaceWith(((JavaMethod)node).ReturnType.GetTextWithComments());
        }

        public override void UpdateContext(JavaParser.MethodDeclarationContext context)
        {
            base.UpdateContext(context);
            ReturnType = new JavaReturnType(context.typeTypeOrVoid(), this);
        }

        protected override void AddFirst(JavaNode node)
        {
            File.InsertAfter(((JavaParser.MethodDeclarationContext)Context).formalParameters().LPAREN().Symbol, node.GetText().Trim());
        }

        public override void InsertBefore(JavaNode existing, JavaNode node)
        {
            if (node is JavaParameter)
            {
                base.InsertBefore(existing, node.GetTextWithComments().Trim() + ", ");
                return;
            }
            base.InsertBefore(existing, node);
        }

        public override void InsertAfter(JavaNode existing, JavaNode node)
        {
            if (node is JavaParameter)
            {
                base.InsertAfter(existing, ", " + node.GetTextWithComments().Trim());
                return;
            }
            base.InsertAfter(existing, node);
        }

        public override bool CanAdd()
        {
            return base.CanAdd() || IsBodyIgnored();
        }

        public override bool CanUpdate()
        {
            return base.CanUpdate() || IsBodyIgnored();
        }

        public override bool CanRemove()
        {
            return base.CanRemove() || IsBodyIgnored();
        }

        private bool IsBodyIgnored()
        {
            return this.HasAnnotation("@IntentIgnoreBody");
        }
    }
}