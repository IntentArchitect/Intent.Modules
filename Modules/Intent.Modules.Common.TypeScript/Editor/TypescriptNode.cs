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

    public class IntentDecorators
    {
        public const string IntentIgnore = "IntentIgnore";
        public const string IntentMerge = "IntentMerge";
        public const string IntentManage = "IntentManage";
    }

    public abstract class TypeScriptNode : IEquatable<TypeScriptNode>
    {
        protected internal Node Node;
        public TypeScriptNode Parent;
        public TypeScriptFileEditor Editor;
        public string NodePath;
        private readonly SyntaxKind _syntaxKind;
        private readonly List<TypeScriptNode> _children = new List<TypeScriptNode>();

        protected TypeScriptNode(Node node, TypeScriptNode parent) : this(node, parent.Editor)
        {
            Parent = parent;
        }

        protected TypeScriptNode(Node node, TypeScriptFileEditor editor)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Editor = editor;
            _syntaxKind = Node.Kind;
            NodePath = GetNodePath(node);
            UpdateNode(node);
        }

        public virtual string Identifier => GetIdentifier(Node);
        public virtual string ExplicitIdentifier => GetExplicitIdentifier();

        public IList<TypeScriptNode> Children => _children;

        public virtual int StartIndex => Node.Pos.Value;
        public virtual int EndIndex => Node.End.Value;

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
            InsertBefore(existing, node.GetTextWithComments());
        }

        public virtual void InsertBefore(TypeScriptNode existing, string text)
        {
            Editor.InsertBefore(existing, text);
        }

        public virtual void InsertAfter(TypeScriptNode existing, TypeScriptNode node)
        {
            if (HasNode(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            InsertAfter(existing, node.GetTextWithComments());
        }

        public virtual void InsertAfter(TypeScriptNode existing, string text)
        {
            Editor.InsertAfter(existing, text);
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

        //[Obsolete]
        //public virtual void AddNode(TypeScriptNode node)
        //{
        //    if (Children.Count == 0)
        //    {
        //        var index = GetTextWithComments().LastIndexOf('{') != -1
        //            ? Node.Pos.Value + GetTextWithComments().LastIndexOf('{') + 1
        //            : Node.End.Value;
        //        Editor.Insert(index, node);
        //    }
        //    else
        //    {
        //        InsertAfter(Children.Last(), node);
        //    }
        //}

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

        public virtual bool HasIntentInstructions()
        {
            return Decorators.Any(x => x.Name.StartsWith("Intent")) || GetComments().Contains("//@Intent");
        }

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

        public string GetComments()
        {
            return Node.GetTextWithComments().Substring(0, Node.GetTextWithComments().Length - Node.GetText().Length);
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

        protected void MergeNodeCollections(TypeScriptNode outputNode, Func<TypeScriptNode, IList<TypeScriptNode>> getCollection)
        {
            var index = 0;
            var highestFoundIndex = 0;
            foreach (var node in getCollection(outputNode))
            {
                var existing = getCollection(this).SingleOrDefault(x => x.IsSameNodeAs(node));
                if (existing == null)
                {
                    if (!CanAdd())
                    {
                        continue;
                    }
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
                    if (existingIndex >= index)
                    {
                        index = existingIndex + 1;
                        highestFoundIndex = index;
                    }
                    else
                    {
                        index = highestFoundIndex + 1;
                    }

                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    //if (!getCollection(existing).Any() || getCollection(existing).All(x => !x.IsIgnored()) && !existing.IsMerged())
                    if (existing.CanUpdate() && existing.CanReplaceInsteadOfMerge() && existing.GetTextWithComments() != node.GetTextWithComments())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }

                    existing.MergeWith(node);
                }
            }

            if (CanRemove())
            {
                var toRemove = getCollection(this).Where(x => !(x is TypeScriptFileImport) && !x.IsIgnored()).Except(getCollection(outputNode)).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        protected virtual string GetExplicitIdentifier()
        {
            var identifierDecorator = Decorators.FirstOrDefault(x => x.Name.StartsWith("@Intent"));
            if (identifierDecorator != null)
            {
                var identifier = identifierDecorator.Children.FirstOrDefault()?.Node.GetText();
                return identifier;
            }

            var comments = GetComments();
            var commentLines = comments.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var commentLine in commentLines)
            {
                if (commentLine.IndexOf("@Intent") != -1 && commentLine.IndexOf(")") != -1)
                {
                    var startIndex = commentLine.IndexOf('(', commentLine.IndexOf("@Intent")) + 1;
                    var length = commentLine.IndexOf(")") - startIndex;
                    return length == 0 ? null : commentLine.Substring(startIndex, length);
                }
            }

            return null;
        }

        private bool IsSameNodeAs(TypeScriptNode node)
        {
            return this.Equals(node);
            //return Node.Kind == node.Node.Kind && Identifier == GetIdentifier(node.Node);
        }

        public virtual bool IsIgnored()
        {
            return HasDecorator(IntentDecorators.IntentIgnore) || GetComments().Contains($"//@{IntentDecorators.IntentIgnore}");
        }

        public virtual bool CanAdd()
        {
            return !HasIntentInstructions() || IsManaged() || IsMerged() || HasDecorator("IntentCanAdd") || GetComments().Contains($"//@IntentCanAdd");
        }

        public virtual bool CanUpdate()
        {
            return !HasIntentInstructions() || IsManaged() || IsMerged() || HasDecorator("IntentCanUpdate") || GetComments().Contains($"//@IntentCanUpdate");
        }

        public virtual bool CanRemove()
        {
            return !HasIntentInstructions() || IsManaged() || HasDecorator("IntentCanRemove") || GetComments().Contains($"//@IntentCanRemove");
        }

        protected bool CanReplaceInsteadOfMerge()
        {
            return (!Decorators.Any() && !Children.Any()) || (Children.All(x => !x.IsIgnored()) && !HasIntentInstructions());
        }

        public virtual void AddFirst(TypeScriptNode node)
        {
            var index = GetTextWithComments().LastIndexOf('{') != -1
                ? Node.Pos.Value + GetTextWithComments().LastIndexOf('{') + 1
                : Math.Max(0, Node.End.Value - 1);
            Editor.Insert(index, node);
        }

        public virtual void InsertOrUpdateChildNode(Node node, int index, Func<TypeScriptNode> createNode)
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
            if (Node.Kind != other?.Node.Kind)
            {
                return false;
            }
            if (ExplicitIdentifier != null || other?.ExplicitIdentifier != null)
            {
                return ExplicitIdentifier == other.ExplicitIdentifier;
            }
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

        private bool IsMerged()
        {
            return HasDecorator(IntentDecorators.IntentMerge) || GetComments().Contains($"//@{IntentDecorators.IntentMerge}");
        }

        private bool IsManaged()
        {
            return HasDecorator(IntentDecorators.IntentManage) || GetComments().Contains($"//@{IntentDecorators.IntentManage}");
        }
    }
}