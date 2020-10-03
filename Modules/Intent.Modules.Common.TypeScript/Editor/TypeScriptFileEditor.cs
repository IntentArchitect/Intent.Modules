using System.Collections.Generic;
using System.Net.Mail;
using Intent.Modules.Common.TypeScript.Editor.Parsing;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFileEditor
    {
        private string _source;
        public TypeScriptAST Ast;

        public ChangeAST Change;

        private IList<TypeScriptNode> _registeredNodes = new List<TypeScriptNode>();

        public TypeScriptFileEditor(string source)
        {
            _source = source;
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
            File = new TypeScriptFile(Ast.RootNode, this);
            var walker = GetTreeWalker(File);
            walker.WalkTree();
        }

        public virtual TypeScriptFileTreeWalker GetTreeWalker(TypeScriptNode node)
        {
            return new TypeScriptFileTreeWalker(node, this);
        }

        public TypeScriptFile File { get; }

        public void InsertAfter(TypeScriptNode node, string text)
        {
            InsertAfter(node.Node, text);
        }

        public void InsertAfter(Node node, string text)
        {
            Change.InsertAfter(node, text);
            UpdateNodes();
        }

        public void InsertBefore(TypeScriptNode node, string text)
        {
            InsertBefore(node.Node, text);
        }

        public void InsertBefore(Node node, string text)
        {
            Change.InsertBefore(node, text);
            UpdateNodes();
        }

        public void Insert(int index, TypeScriptNode node)
        {
            Insert(index, node.GetTextWithComments());
        }

        public void Insert(int index, string text)
        {
            _source = _source.Insert(index, text);
            UpdateNodes();
        }

        public void Replace(int start, int end, string text)
        {
            _source = _source
                .Remove(start, end - start)
                .Insert(start, text);
            UpdateNodes();
        }

        public void Replace(TypeScriptNode node, string text)
        {
            if (node.GetTextWithComments() == text)
            {
                return;
            }
            Replace(node.StartIndex, node.EndIndex, text);
        }

        public void UpdateNodes()
        {
            _source = Change.GetChangedSource(_source);
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
            File.UpdateNode(Ast.RootNode);
            var listener = GetTreeWalker(File);
            listener.WalkTree();
        }

        public string GetSource()
        {
            return _source;
        }

        public void Register(TypeScriptNode node)
        {
            _registeredNodes.Add(node);
        }

        public void Unregister(TypeScriptNode node)
        {
            _registeredNodes.Remove(node);
        }

        public override string ToString()
        {
            return _source;
        }
    }
}