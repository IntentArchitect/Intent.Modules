using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaMethod : JavaNode
    {
        private readonly Java9Parser.MethodDeclarationContext _context;

        public JavaMethod(Java9Parser.MethodDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
            Name = _context.methodHeader().methodDeclarator().identifier().GetText();
        }

        public string Name { get; }
        public IList<JavaParameter> Parameters { get; set; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            var name = ((Java9Parser.MethodDeclarationContext)context).methodHeader().methodDeclarator().identifier().GetText();
            if (HasIntentInstructions() && IsBodyIgnored())
            {
                return name;
            }
            var parameterTypes = ((Java9Parser.MethodDeclarationContext)context)
                .methodHeader().methodDeclarator().formalParameterList()?.GetParameterTypes() ?? new List<string>();
            return $"{name}({string.Join(", ", parameterTypes)})";
        }

        public override void MergeWith(JavaNode node)
        {
            base.MergeWith(node);
            //MergeParameters((JavaClass)node);
        }

        public override void UpdateContext(RuleContext context)
        {
            base.UpdateContext(context);
            UpdateParameters(((Java9Parser.MethodDeclarationContext)context).methodHeader().methodDeclarator().formalParameterList());
        }

        private void UpdateParameters(Java9Parser.FormalParameterListContext formalParameterList)
        {
            if (formalParameterList == null)
            {
                return;
            }

            Parameters = formalParameterList.GetParameters(this);
        }

        protected override void AddFirst(JavaNode node)
        {
            File.InsertBefore(((Java9Parser.MethodDeclarationContext)Context).methodHeader().Stop, node.GetText().Trim());
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

    public class JavaParameter : JavaNode
    {
        public JavaParameter(Java9Parser.FormalParameterContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            var type = ((Java9Parser.FormalParameterContext)context).unannType().GetText();
            return type;
        }
    }
}