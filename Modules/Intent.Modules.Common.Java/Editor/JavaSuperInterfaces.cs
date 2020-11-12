using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaSuperInterfaces : JavaInterfacesBase
    {
        public JavaSuperInterfaces(JavaParser.TypeListContext context, JavaNode parent) : base(context, parent)
        {
        }

        protected override void UpdateSourceFile()
        {
            // whitespace coming from interface nodes
            var overwriteWith = Interfaces.Any() ? $" implements{string.Join(",", Interfaces.Select(x => x.GetTextWithComments()))}" : "";
            ReplaceWith(overwriteWith);
        }

        public override IToken StartToken => ((JavaParser.ClassDeclarationContext)Context.Parent).IMPLEMENTS().Symbol;
    }

    public class JavaExtendsInterfaces : JavaInterfacesBase
    {
        public JavaExtendsInterfaces(JavaParser.TypeListContext context, JavaNode parent) : base(context, parent)
        {
        }

        protected override void UpdateSourceFile()
        {
            // whitespace coming from interface nodes
            var overwriteWith = Interfaces.Any() ? $" extends{string.Join(",", Interfaces.Select(x => x.GetTextWithComments()))}" : "";
            ReplaceWith(overwriteWith);
        }

        public override IToken StartToken => ((JavaParser.InterfaceDeclarationContext)Context.Parent).EXTENDS().Symbol;
    }

    public abstract class JavaInterfacesBase : JavaNode
    {
        public JavaInterfacesBase(ParserRuleContext context, JavaNode parent) : base(context, parent)
        {
        }
        
        public IList<JavaInterfaceType> Interfaces = new List<JavaInterfaceType>();

        public override string GetIdentifier(ParserRuleContext context)
        {
            return "";
        }

        public override void MergeWith(JavaNode node)
        {
            var index = 0;
            foreach (var @interface in ((JavaSuperInterfaces)node).Interfaces)
            {
                if (!Interfaces.Contains(@interface))
                {
                    Interfaces.Insert(index, @interface);
                    index++;
                }
                else
                {
                    index = Interfaces.IndexOf(@interface) + 1;
                }
            }
            if (!this.IsMerged())
            {
                var toRemoves = Interfaces.Where(x => !x.IsIgnored()).Except(((JavaSuperInterfaces)node).Interfaces).ToList();
                foreach (var toRemove in toRemoves)
                {
                    Interfaces.Remove(toRemove);
                }
            }

            UpdateSourceFile();
        }

        protected abstract void UpdateSourceFile();

        public override void UpdateContext(RuleContext context)
        {
            base.UpdateContext(context);
            Interfaces.Clear();
            ParseTreeWalker.Default.Walk(new JavaInterfacesListener(this), Context);
        }

        public override bool IsIgnored()
        {
            return Parent.IsIgnored();
        }

        public override bool IsMerged()
        {
            return Parent.IsMerged();
        }

        private class JavaInterfacesListener : JavaParserBaseListener
        {
            private int _index;
            private readonly JavaInterfacesBase _interfaces;

            public JavaInterfacesListener(JavaInterfacesBase interfaces)
            {
                _interfaces = interfaces;
            }

            public override void EnterTypeList(JavaParser.TypeListContext context)
            {
                foreach (var typeTypeContext in context.typeType())
                {
                    var node = _interfaces.Interfaces.SingleOrDefault(x => x.Context.GetType() == typeTypeContext.GetType() && x.Identifier == x.GetIdentifier(typeTypeContext));
                    if (node == null)
                    {
                        node = new JavaInterfaceType(typeTypeContext, _interfaces);
                        _interfaces.Interfaces.Insert(_index, node);
                    }
                    else
                    {
                        node.UpdateContext(typeTypeContext);
                    }

                    if (_index < _interfaces.Interfaces.Count)
                    {
                        _index++;
                    }
                }
                
            }
        }
    }
}