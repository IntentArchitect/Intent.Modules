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

        protected TypeScriptNode(Node node, TypeScriptFileEditor editor)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _syntaxKind = Node.Kind;
            NodePath = GetNodePath(node);
            //Decorators = Node.Decorators?.Select(x => new TypeScriptDecorator(x, this)).ToList() ?? new List<TypeScriptDecorator>();
            UpdateNode(node);
        }

        public virtual string Identifier => GetIdentifier(Node);

        public IList<TypeScriptNode> Children => _children;

        public List<T> GetChildren<T>() where T : TypeScriptNode => Children.Where(x => x is T).Cast<T>().ToList();

        public abstract string GetIdentifier(Node node);

        public TypeScriptNode TryGetChild(Node node)
        {
            return Children.SingleOrDefault(x => x.Node.Kind == node.Kind && x.Identifier == x.GetIdentifier(node));
        }

        public virtual void InsertBefore(TypeScriptNode existing, TypeScriptNode node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            Editor.InsertBefore(existing, node.GetTextWithComments());
        }

        public virtual void InsertBefore(TypeScriptDecorator existing, TypeScriptDecorator node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            Editor.InsertBefore(existing, node.GetTextWithComments());
        }

        public virtual void InsertAfter(TypeScriptNode existing, TypeScriptNode node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
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

        [Obsolete]
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

        public virtual void Remove()
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

        public IList<TypeScriptDecorator> Decorators { get; } = new List<TypeScriptDecorator>();

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

        public virtual void UpdateNode(Node node)
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

        public virtual void MergeWith(TypeScriptNode node)
        {
            MergeNodeCollections(node, x => x.Decorators.ToList<TypeScriptNode>());
            MergeNodeCollections(node, x => x.Children.ToList<TypeScriptNode>());
        }

        public virtual bool IsManaged()
        {
            return HasDecorator("IntentManage");
        }

        protected void MergeNodeCollections(TypeScriptNode outputNode, Func<TypeScriptNode, IList<TypeScriptNode>> getCollection)
        {
            var index = 0;
            foreach (var node in getCollection(outputNode))
            {
                var existing = getCollection(this).SingleOrDefault(x => x.Node.Kind == node.Node.Kind && x.Identifier == x.GetIdentifier(node.Node));
                if (existing == null)
                {
                    // toAdd:
                    if (getCollection(this).Count == 0)
                    {
                        this.AddFirst((dynamic)node);
                    }
                    else if (index == 0)
                    {
                        this.InsertBefore(getCollection(this)[0], node);
                    }
                    else if (getCollection(this).Count > index)
                    {
                        this.InsertAfter(getCollection(this)[index - 1], node);
                    }
                    else
                    {
                        this.InsertAfter(getCollection(this).Last(), node);
                    }

                    index++;
                }
                else
                {
                    // toUpdate:
                    var existingIndex = getCollection(this).IndexOf(existing);
                    index = (existingIndex > index) ? existingIndex + 1 : index + 1;

                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    //if (!getCollection(existing).Any() || getCollection(existing).All(x => !x.IsIgnored()) && !existing.IsMerged())
                    if (existing.CanReplaceInsteadOfMerge() && existing.GetTextWithComments() != node.GetTextWithComments())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }

                    existing.MergeWith(node);
                }
            }

            if (!this.IsMerged())
            {
                var toRemove = getCollection(this).Where(x => !(x is TypeScriptFileImport) && !x.IsIgnored()).Except(getCollection(outputNode)).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        protected bool CanReplaceInsteadOfMerge()
        {
            return (!Decorators.Any() && !Children.Any()) || (Children.All(x => !x.IsIgnored()) && !IsMerged());
        }

        public virtual void AddFirst(TypeScriptNode node)
        {
            var index = GetTextWithComments().LastIndexOf('{') != -1
                ? Node.Pos.Value + GetTextWithComments().LastIndexOf('{') + 1
                : Node.End.Value;
            Editor.Insert(index, node);
        }

        protected virtual void InsertOrUpdateChildNode(Node node, int index, Func<TypeScriptNode> createNode)
        {
            var existing = TryGetChild(node);
            if (existing == null)
            {
                InsertChild(index, createNode());
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void AddFirst(TypeScriptDecorator node)
        {
            Editor.InsertBefore(Node, node.GetTextWithComments());
        }

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