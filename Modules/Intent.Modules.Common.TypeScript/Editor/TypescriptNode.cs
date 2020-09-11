using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    //public interface ITypeScriptNodeVisitor
    //{
    //    void Visit(TypeScriptClass typeScriptClass);
    //}

    public abstract class TypeScriptNode : IEquatable<TypeScriptNode>
    {
        protected internal Node Node;
        public TypeScriptFileEditor Editor;
        public string NodePath;
        private readonly SyntaxKind _syntaxKind;
        private readonly List<TypeScriptNode> _children = new List<TypeScriptNode>();

        public TypeScriptNode(Node node, TypeScriptFileEditor editor)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _syntaxKind = Node.Kind;
            //File.Register(this);
            NodePath = GetNodePath(node);
            Decorators = Node.Decorators?.Select(x => new TypeScriptDecorator(x, this)).ToList() ?? new List<TypeScriptDecorator>();
        }

        public virtual string Identifier => GetIdentifier(Node);

        public IList<TypeScriptNode> Children => _children;

        public List<T> GetChildren<T>() where T : TypeScriptNode => Children.Where(x => x is T).Cast<T>().ToList();

        public abstract string GetIdentifier(Node node);

        public TypeScriptNode TryGetChild(Node node)
        {
            return Children.SingleOrDefault(x => x.Node.Kind == node.Kind && x.Identifier == x.GetIdentifier(node));
        }

        public void InsertBefore(TypeScriptNode existing, TypeScriptNode node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //InsertChild(_children.IndexOf(existing), node);
            Editor.InsertBefore(existing, node.GetTextWithComments());
        }

        public void InsertBefore(TypeScriptDecorator existing, TypeScriptDecorator node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //InsertDecorator(Decorators.IndexOf(existing), node);
            Editor.InsertBefore(existing, node.GetTextWithComments());
        }

        public void InsertAfter(TypeScriptNode existing, TypeScriptNode node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //InsertChild(_children.IndexOf(existing) + 1, node);
            Editor.InsertAfter(existing, node.GetTextWithComments());
        }

        public void AddChild(TypeScriptNode node)
        {
            node.Editor = Editor;
            _children.Add(node);
        }

        public void InsertChild(int index, TypeScriptNode node)
        {
            node.Editor = Editor;
            _children.Insert(index, node);
        }

        public void RemoveChild(TypeScriptNode toRemove)
        {
            toRemove.Remove();
            _children.Remove(toRemove);
        }

        public void InsertDecorator(int index, TypeScriptDecorator node)
        {
            Decorators.Insert(index, node);
            node.Editor = Editor;
        }

        public bool HasNode(TypeScriptNode node)
        {
            return Children.Contains(node);
        }

        public virtual void AddNode(TypeScriptNode node)
        {
            if (Children.Count == 0)
            {
                var index = GetTextWithComments().LastIndexOf('{') != -1 
                    ? Node.Pos.Value + GetTextWithComments().LastIndexOf('{') + 1
                    : Node.End.Value;
                Editor.Insert(index, node);
            }
            else
            {
                InsertAfter(Children.Last(), node);
            }
        }

        public void ReplaceWith(string text)
        {
            Editor.Replace(this, text);
        }

        public void Remove()
        {
            Editor.Replace(this, "");
        }

        protected string GetNodePath(Node startNode)
        {
            var path = "";
            var current = startNode;
            while (current.Kind != SyntaxKind.SourceFile)
            {
                path = $"{current.Kind}{(current.IdentifierStr != null ? ":" + current.IdentifierStr : "")}{(startNode != current ? "/" : "")}{path}";
                current = (Node)current.Parent;
            }

            return path;
        }

        public IList<TypeScriptDecorator> Decorators { get; private set; }

        public bool HasDecorator(string name)
        {
            return Decorators.Any(x => x.Name == name);
        }

        public TypeScriptDecorator TryGetDecorator(Node node)
        {
            return Decorators.SingleOrDefault(x => x.Node.Kind == node.Kind && x.Identifier == x.GetIdentifier(node));
        }

        public string GetTextWithComments()
        {
            return Node.GetTextWithComments();
        }

        public virtual bool IsIgnored()
        {
            return HasDecorator("IntentIgnore") || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentIgnore()");
        }

        public virtual bool IsMerged()
        {
            return HasDecorator("IntentMerge") || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentMerge()");
        }

        public virtual bool IsManaged()
        {
            return HasDecorator("IntentManage");
        }

        //public virtual void Remove()
        //{
        //    Editor.ReplaceNode(Node, "");
        //    Editor.Unregister(this);
        //    UpdateChanges();
        //}

        //public void ReplaceWith(string replaceWith)
        //{
        //    if (string.IsNullOrWhiteSpace(replaceWith))
        //    {
        //        throw new ArgumentException("Cannot replace a method with an empty string", nameof(replaceWith));
        //    }
        //    if (Node.GetTextWithComments() == replaceWith)
        //    {
        //        return;
        //    }
        //    Change.ChangeNode(Node, replaceWith);
        //    UpdateChanges();
        //    //if (IsIgnored())
        //    //{
        //    //    throw new Exception($"Cannot add method to TypeScript node [{ToString()}] as it has been decorated with @IntentIgnore()");
        //    //}
        //    //File.ReplaceNode(Node, replaceWith);
        //    //Node = File.Ast.GetDescendants().OfKind(Node.Kind).Single(x => x.Pos == Node.Pos);
        //}

        //private void UpdateChanges()
        //{
        //    Editor.UpdateChanges();
        //}
        //public virtual void UpdateChanges()
        //{
        //    if (IsIgnored())
        //    {
        //        throw new Exception($"Cannot add method to TypeScript node [{ToString()}] as it has been decorated with @IntentIgnore()");
        //    }

        //    if (_decorators != null)
        //    {
        //        foreach (var decorator in _decorators)
        //        {
        //            File.Unregister(decorator);
        //        }

        //        _decorators = null;
        //    }
        //    File.UpdateChanges();
        //    //Node = File.Ast.GetDescendants().OfKind(Node.Kind).Single(x => x.Pos == Node.Pos);
        //}

        public void UpdateNode(Node node)
        {
            if (node.Kind != _syntaxKind)
            {
                throw new Exception($"Cannot update node of kind [{_syntaxKind}] to a different SyntaxKind: " + node.Kind);
            }
            Node = node;
            Node.Decorators?.ForEach(x =>
            {
                var existing = this.TryGetDecorator(x);
                if (existing == null)
                {
                    Decorators.Add(new TypeScriptDecorator(x, this));
                }
                else
                {
                    existing.UpdateNode(x);
                }
            });
        }

        //internal virtual void UpdateNode()
        //{
        //    Node = FindNode(File.Ast.RootNode, NodePath) ?? throw new Exception($"[{GetType().Name}] Could not find node for path: " + NodePath);
        //}

        public bool Equals(TypeScriptNode other)
        {
            return Identifier == other?.Identifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptNode)obj);
        }

        public override int GetHashCode()
        {
            return Identifier?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Node.GetTextWithComments();
        }
    }
}