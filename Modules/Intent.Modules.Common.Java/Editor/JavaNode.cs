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
            UpdateContext(context);
        }

        protected JavaNode(ParserRuleContext context, JavaNode parent) : this(context, parent.File)
        {
            Parent = parent;
        }

        public JavaFile File { get; protected set; }
        public string Identifier => GetIdentifier(Context);
        public abstract string GetIdentifier(ParserRuleContext context);
        public ParserRuleContext Context { get; private set; }
        public JavaNode Parent { get; }
        public IList<JavaNode> Children => _children;
        public IList<JavaAnnotation> Annotations { get; } = new List<JavaAnnotation>();
        public IToken StartToken => File.GetCommentsAndWhitespaceBefore(Context.Start).StartToken ?? Context.Start;
        public IToken StopToken => Context.Stop;

        public JavaNode TryGetChild(ParserRuleContext context)
        {
            return Children.SingleOrDefault(x => x.Context.GetType() == context.GetType() && x.Identifier == x.GetIdentifier(context));
        }

        public virtual void InsertBefore(JavaNode existing, JavaNode node)
        {
            if (HasChild(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //Children.Insert(Children.IndexOf(existing), node);
            File.InsertBefore(existing, node.GetText());
        }

        public virtual void InsertAfter(JavaNode existing, JavaNode node)
        {
            if (HasChild(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //Children.Insert(Children.IndexOf(existing) + 1, node);
            File.InsertAfter(existing, node.GetText());
        }

        public bool HasChild(JavaNode node)
        {
            return Children.Contains(node);
        }

        public void InsertChild(int index, JavaNode node)
        {
            _children.Insert(index, node);
        }

        public JavaAnnotation TryGetAnnotation(Java9Parser.AnnotationContext context)
        {
            return Annotations.SingleOrDefault(x => x.Context.GetType() == context.GetType() && x.Identifier == x.GetIdentifier(context));
        }

        public void InsertAnnotation(int index, JavaAnnotation annotation)
        {
            Annotations.Insert(index, annotation);
        }

        public virtual string GetText()
        {
            var ws = File.GetCommentsAndWhitespaceBefore(Context.Start);
            return $"{ws.Text}{Context.GetFullText()}";
            //return Context.GetFullText();
        }

        public void ReplaceWith(string text)
        {
            File.Replace(this, text);
        }

        public void Remove()
        {
            File.Replace(this, "");
        }

        public virtual bool IsIgnored()
        {
            return Annotations.Any(x => x.Identifier.StartsWith("@IntentIgnore"));// || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentIgnore()");
        }

        public virtual bool IsMerged()
        {
            return Annotations.Any(x => x.Identifier.StartsWith("@IntentMerge"));// || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentMerge()");
        }

        public virtual void UpdateContext(RuleContext context)
        {
            Context = (ParserRuleContext)context;
        }

        public virtual void MergeWith(JavaNode node)
        {
            MergeNodeCollections(node, x => x.Annotations.ToList<JavaNode>());
            MergeNodeCollections(node, x => x.Children.ToList<JavaNode>());
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

        protected void MergeNodeCollections(JavaNode outputNode, Func<JavaNode, IList<JavaNode>> getCollection)
        {
            var index = 0;
            foreach (var node in getCollection(outputNode))
            {
                var existing = getCollection(this).SingleOrDefault(x => x.Context.GetType() == node.Context.GetType() && x.Identifier == x.GetIdentifier(node.Context));
                //this.TryGetAnnotation((Java9Parser.AnnotationContext)node.Context);
                if (existing == null)
                {
                    // toAdd:
                    //var text = node.GetTextWithComments();
                    if (getCollection(this).Count == 0)
                    {
                        if (node is JavaAnnotation annotation)
                        {
                            this.AddFirst(annotation);
                        }
                        else
                        {
                            this.AddFirst(node);
                        }
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

                    if (getCollection(existing).All(x => !x.IsIgnored()) && !existing.IsMerged())
                    {
                        if (existing.GetText() != node.GetText())
                        {
                            existing.ReplaceWith(node.GetText()); // Overwrite
                        }
                        continue;
                    }

                    existing.MergeWith(node);
                }
            }

            if (!this.IsMerged())
            {
                var toRemove = getCollection(this).Where(x => !x.IsIgnored()).Except(getCollection(outputNode)).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        protected virtual void AddFirst(JavaAnnotation node)
        {
            File.InsertBefore(this, node.GetText());
        }

        protected virtual void AddFirst(JavaNode node)
        {
            File.InsertBefore(Context.Stop ?? Context.Start, node.GetText());
        }
    }
}