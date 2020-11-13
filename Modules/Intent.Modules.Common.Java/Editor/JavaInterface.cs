using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaInterface : JavaNode<JavaParser.TypeDeclarationContext>
    {
        public JavaInterface(JavaParser.TypeDeclarationContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override string GetIdentifier(JavaParser.TypeDeclarationContext context)
        {
            return context.interfaceDeclaration().IDENTIFIER().GetText();
        }

        public string Name => Identifier;
        public JavaExtendsInterfaces SuperInterfaces { get; set; }
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaInterfaceMethod> Methods => Children.Where(x => x is JavaInterfaceMethod).Cast<JavaInterfaceMethod>().ToList();

        public override void UpdateContext(JavaParser.TypeDeclarationContext context)
        {
            base.UpdateContext(context);
            UpdateInterfaces(context.interfaceDeclaration().typeList());
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
                    var afterContext = TypedContext.interfaceDeclaration().IDENTIFIER();
                    File.InsertAfter(afterContext.Symbol, node.SuperInterfaces.GetTextWithComments());
                }
            }
            else if (!IsMerged())
            {
                SuperInterfaces?.Remove();
            }
        }

        private void UpdateInterfaces(JavaParser.TypeListContext extendsInterfaces)
        {
            SuperInterfaces = extendsInterfaces != null ? new JavaExtendsInterfaces(extendsInterfaces, this) : null;
        }
    }
}