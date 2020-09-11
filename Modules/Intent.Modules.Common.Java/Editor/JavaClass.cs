using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Intent.Modules.Common.Java.Editor.Parser;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaClass : JavaNode
    {
        public JavaClass(Java9Parser.NormalClassDeclarationContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.NormalClassDeclarationContext)context).identifier().GetText();
        }

        public string Name => Identifier;
        public JavaSuperClass SuperClass { get; set; }
        public JavaSuperInterfaces SuperInterfaces { get; set; }
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaClassMethod> Methods => Children.Where(x => x is JavaClassMethod).Cast<JavaClassMethod>().ToList();
        public IReadOnlyList<JavaConstructor> Constructors => Children.Where(x => x is JavaConstructor).Cast<JavaConstructor>().ToList();

        public override void UpdateContext(RuleContext context)
        {
            base.UpdateContext(context);
            UpdateSuperClass(((Java9Parser.NormalClassDeclarationContext)context).superclass());
            UpdateInterfaces(((Java9Parser.NormalClassDeclarationContext)context).superinterfaces());
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
                    SuperClass.ReplaceWith(node.SuperClass.GetText());
                }
                else
                {
                    File.InsertAfter(((Java9Parser.NormalClassDeclarationContext)Context).identifier().Stop, node.SuperClass.GetText());
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
                    var afterContext = ((Java9Parser.NormalClassDeclarationContext)Context).superclass() ?? (ParserRuleContext)((Java9Parser.NormalClassDeclarationContext)Context).identifier();
                    File.InsertAfter(afterContext.Stop, node.SuperInterfaces.GetText());
                }
            }
            else if (!IsMerged())
            {
                SuperInterfaces?.Remove();
            }
        }

        private void UpdateSuperClass(Java9Parser.SuperclassContext superclass)
        {
            SuperClass = superclass != null ? new JavaSuperClass(superclass, this) : null;
        }

        private void UpdateInterfaces(Java9Parser.SuperinterfacesContext superinterfaces)
        {
            SuperInterfaces = superinterfaces != null ? new JavaSuperInterfaces(superinterfaces, this) : null;
        }
    }
}