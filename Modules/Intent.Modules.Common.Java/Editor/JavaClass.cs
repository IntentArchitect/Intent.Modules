using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaClass : JavaNode<JavaParser.TypeDeclarationContext>
    {
        public JavaClass(JavaParser.TypeDeclarationContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override string GetIdentifier(JavaParser.TypeDeclarationContext context)
        {
            return context.classDeclaration().IDENTIFIER().GetText();
        }

        public string Name => Identifier;
        public JavaSuperClass SuperClass { get; set; }
        public JavaSuperInterfaces SuperInterfaces { get; set; }
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaMethod> Methods => Children.Where(x => x is JavaMethod).Cast<JavaMethod>().ToList();
        public IReadOnlyList<JavaConstructor> Constructors => Children.Where(x => x is JavaConstructor).Cast<JavaConstructor>().ToList();

        public override void UpdateContext(JavaParser.TypeDeclarationContext context)
        {
            base.UpdateContext(context);
            UpdateSuperClass(context.classDeclaration().typeType());
            UpdateInterfaces(context.classDeclaration().typeList());
        }

        public override void MergeWith(JavaNode node)
        {
            base.MergeWith(node);
            MergeSuperClass((JavaClass)node);
            MergeInterfaces((JavaClass)node);
        }

        private void MergeSuperClass(JavaClass node)
        {
            if (node.SuperClass != null)
            {
                if (SuperClass != null)
                {
                    SuperClass.ReplaceWith(node.SuperClass.GetTextWithComments());
                }
                else
                {
                    File.InsertAfter(TypedContext.classDeclaration().IDENTIFIER().Symbol, node.SuperClass.GetTextWithComments());
                }
            }
            else if (!IsMerged())
            {
                SuperClass?.Remove();
            }
        }

        private void MergeInterfaces(JavaClass node)
        {
            if (node.SuperInterfaces != null)
            {
                if (SuperInterfaces != null)
                {
                    SuperInterfaces.MergeWith(node.SuperInterfaces);
                }
                else
                {
                    var afterToken = TypedContext.classDeclaration().typeType()?.Stop ?? TypedContext.classDeclaration().IDENTIFIER().Symbol;
                    File.InsertAfter(afterToken, node.SuperInterfaces.GetTextWithComments());
                }
            }
            else if (!IsMerged())
            {
                SuperInterfaces?.Remove();
            }
        }

        private void UpdateSuperClass(JavaParser.TypeTypeContext superclass)
        {
            SuperClass = superclass != null ? new JavaSuperClass(superclass, this) : null;
        }

        private void UpdateInterfaces(JavaParser.TypeListContext superinterfaces)
        {
            SuperInterfaces = superinterfaces != null ? new JavaSuperInterfaces(superinterfaces, this) : null;
        }
    }
}