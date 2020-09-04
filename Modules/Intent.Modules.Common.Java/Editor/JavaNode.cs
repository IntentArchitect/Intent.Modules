using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.Java.Editor.Parser;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public abstract class JavaNode : IEquatable<JavaNode>
    {
        private readonly List<JavaNode> _children = new List<JavaNode>();

        protected JavaNode(ParserRuleContext context, JavaFile file)
        {
            File = file;
            Context = context;
        }

        protected JavaNode(ParserRuleContext context, JavaNode parent) : this(context, parent.File)
        {
            Parent = parent;
        }

        protected string GetNodePath()
        {
            var path = "";
            var current = Context;
            while (current != null && current != Parent?.Context)
            {
                path = $"{current.GetType().Name}{(!current.Equals(Context) ? "/" : "")}{path}";
                current = current.Parent as ParserRuleContext;
            }

            return $"{path}{(Identifier != null ? $":{Identifier}" : "")}";
        }

        public JavaFile File { get; }

        public string Identifier => GetIdentifier(Context);
        protected abstract string GetIdentifier(ParserRuleContext context);
        public ParserRuleContext Context { get; private set; }
        public JavaNode Parent { get; }

        public IList<JavaNode> Children => _children;

        public JavaNode TryGetChild(ParserRuleContext context)
        {
            foreach (var child in Children)
            {
                if (child.Context.GetType() == context.GetType())
                {
                    if (child.Identifier == child.GetIdentifier(context))
                    {

                    }
                }
            }
            return Children.SingleOrDefault(x => x.Context.GetType() == context.GetType() &&  x.Identifier == x.GetIdentifier(context));
        }

        public bool HasChild(JavaNode node)
        {
            return Children.Contains(node);
        }

        public void AddChild(JavaNode node)
        {
            _children.Add(node);
        }

        public void InsertChild(JavaNode node)
        {
            _children.Add(node);
        }

        public virtual string GetText()
        {
            var ws = File.GetWhitespaceBefore(Context);
            return $"{ws?.Text ?? ""}{Context.GetFullText()}";
            //return Context.GetFullText();
        }

        public void ReplaceWith(string text)
        {
            File.Replace(Context, text);
        }

        public void Remove()
        {
            File.Replace(Context, "");
        }

        public virtual bool IsIgnored()
        {
            return false;
        }

        public void UpdateContext(RuleContext fromContext)
        {
            Context = (ParserRuleContext) fromContext;
            //var path = GetNodePath();
            //var found = FindNodes((ParserRuleContext) fromContext, path);
            //Context = found[0];
            //foreach (var node in Children)
            //{
            //    node.UpdateContext(Context);
            //}

        }

        private IList<ParserRuleContext> FindNodes(ParserRuleContext node, string path)
        {
            var pathParts = path.Split('/');
            var part = pathParts[0];

            var nodeKind = part.Split(':')[0].Split('~')[0];
            var identifier = part.Split(':').Length > 1 ? part.Split(':')[1].Split('~')[0] : null;
            var textIdentifier = part.Split('~').Length > 1 ? part.Split('~')[1] : null;

            if (pathParts.Length == 1)
            {
                if (identifier == null)
                {
                    return new [] { node };
                }
                return node.children.Where(x => x.GetType().Name == nodeKind && (identifier == null || GetIdentifier((ParserRuleContext) x) == identifier) && (textIdentifier == null || x.GetText() == textIdentifier)).Cast<ParserRuleContext>().ToList();
            }

            if (nodeKind == node.GetType().Name)
            {
                return FindNodes(node, path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1));
            }

            if (identifier == null)
            {
                foreach (var descendant in node.children.Where(x => x.GetType().Name == nodeKind))
                {
                    var found = FindNodes((ParserRuleContext) descendant, path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1));
                    if (found?.Count > 0)
                    {
                        return found;
                    }
                }

                return null;
            }

            throw new Exception("Failed to find node at path: "+ path);
            //return FindNodes(node.children.Where(x => x.GetType().Name == nodeKind).FirstOrDefault(x => x.IdentifierStr == identifier), path.Substring(path.IndexOf("/", StringComparison.Ordinal) + 1));
        }

        public bool Equals(JavaNode other)
        {
            return Identifier == other?.Identifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JavaNode)obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return GetText();
        }
    }
}