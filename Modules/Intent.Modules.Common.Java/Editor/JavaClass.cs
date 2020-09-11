using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
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
        public string SuperClass { get; set; }
        public IList<JavaInterfaceType> Interfaces = new List<JavaInterfaceType>();
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaMethod> Methods => Children.Where(x => x is JavaMethod).Cast<JavaMethod>().ToList();
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
            MergeNodeCollections(node, x => ((JavaClass)x).Interfaces.ToList<JavaNode>());
        }

        protected override void AddFirst(JavaNode node)
        {
            if (node is JavaInterfaceType)
            {
                var afterContext = ((Java9Parser.NormalClassDeclarationContext)Context).superclass() ?? (ParserRuleContext)((Java9Parser.NormalClassDeclarationContext)Context).identifier();
                File.InsertAfter(afterContext.Stop, " implements" + node.GetText());
            }
            else
            {
                base.AddFirst(node);
            }
        }

        public override void InsertBefore(JavaNode existing, JavaNode node)
        {
            if (node is JavaInterfaceType)
            {
                File.InsertBefore(existing, $"{node.GetText()},");
            }
            else
            {
                base.InsertBefore(existing, node);
            }
        }

        public override void InsertAfter(JavaNode existing, JavaNode node)
        {
            if (node is JavaInterfaceType)
            {
                File.InsertAfter(existing, $",{node.GetText()}");
            }
            else
            {
                base.InsertAfter(existing, node);
            }
        }

        private void UpdateSuperClass(Java9Parser.SuperclassContext superclass)
        {
            SuperClass = superclass?.classType().identifier().GetText();
        }


        private void UpdateInterfaces(Java9Parser.SuperinterfacesContext interfaces)
        {
            if (interfaces == null)
            {
                Interfaces.Clear();
            }
            ParseTreeWalker.Default.Walk(new JavaInterfacesListener(this), Context);
        }
    }

    public class JavaInterfacesListener : Java9BaseListener
    {
        private int _index;
        private readonly JavaClass _class;

        public JavaInterfacesListener(JavaClass @class)
        {
            _class = @class;
        }

        public override void EnterInterfaceType(Java9Parser.InterfaceTypeContext context)
        {
            var node = _class.Interfaces.SingleOrDefault(x => x.Context.GetType() == context.GetType() && x.Identifier == x.GetIdentifier(context));
            if (node == null)
            {
                node = new JavaInterfaceType(context, _class);
                _class.Interfaces.Insert(_index, node);
            }
            else
            {
                node.UpdateContext(context);
            }

            if (_index < _class.Interfaces.Count)
            {
                _index++;
            }
        }
    }
    public class JavaInterfaceType : JavaNode
    {
        public JavaInterfaceType(Java9Parser.InterfaceTypeContext context, JavaNode parent) : base(context, parent)
        {

        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }

        //public override string GetText()
        //{
        //    var previousTokenIndex = ((ParserRuleContext) Context.Parent).children.IndexOf(Context) - 1;
        //    if (previousTokenIndex >= 0 && Context.Parent.GetChild(previousTokenIndex) is TerminalNodeImpl)
        //    {
        //        return Context.Parent.GetChild(previousTokenIndex).GetText() + base.GetText();
        //    }
        //    return base.GetText();
        //}
    }
}