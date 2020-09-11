using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaInterface : JavaNode
    {
        public JavaInterface(Java9Parser.NormalInterfaceDeclarationContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.NormalInterfaceDeclarationContext)context).identifier().GetText();
        }

        public string Name => Identifier;
        public JavaExtendsInterfaces SuperInterfaces { get; set; }
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaInterfaceMethod> Methods => Children.Where(x => x is JavaInterfaceMethod).Cast<JavaInterfaceMethod>().ToList();

        public override void UpdateContext(RuleContext context)
        {
            base.UpdateContext(context);
            UpdateInterfaces(((Java9Parser.NormalInterfaceDeclarationContext)context).extendsInterfaces());
        }

        public override void MergeWith(JavaNode node)
        {
            base.MergeWith(node);
            MergeInterfaces((JavaInterface)node);
        }

        private void MergeInterfaces(JavaInterface node)
        {
            if (node.SuperInterfaces != null)
            {
                if (SuperInterfaces != null)
                {
                    SuperInterfaces.MergeWith(node.SuperInterfaces);
                }
                else
                {
                    var afterContext = ((Java9Parser.NormalInterfaceDeclarationContext)Context).identifier();
                    File.InsertAfter(afterContext.Stop, node.SuperInterfaces.GetText());
                }
            }
            else if (!IsMerged())
            {
                SuperInterfaces?.Remove();
            }
        }

        private void UpdateInterfaces(Java9Parser.ExtendsInterfacesContext extendsInterfaces)
        {
            SuperInterfaces = extendsInterfaces != null ? new JavaExtendsInterfaces(extendsInterfaces, this) : null;
        }
    }
}