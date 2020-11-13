using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.Java.Editor.Parser;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public abstract class JavaNode<TContext> : JavaNode
        where TContext : ParserRuleContext
    {
        protected JavaNode(TContext context, JavaFile file) : base(context, file)
        {
        }

        protected JavaNode(TContext context, JavaNode parent) : base(context, parent)
        {
        }

        public TContext TypedContext => (TContext) Context;

        public abstract string GetIdentifier(TContext context);

        public override string GetIdentifier(ParserRuleContext context)
        {
            return GetIdentifier((TContext) context);
        }

        public override void UpdateContext(RuleContext context)
        {
            base.UpdateContext(context);
            UpdateContext((TContext)context);
        }

        public virtual void UpdateContext(TContext context)
        {
        }
    }

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
        public virtual IToken StartToken => Annotations.FirstOrDefault()?.StartToken ?? Context.Start;
        public virtual IToken StopToken => Context.Stop;

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
            InsertBefore(existing, node.GetTextWithComments());
        }

        public virtual void InsertBefore(JavaNode existing, JavaAnnotation node)
        {
            File.InsertBefore(existing.StartToken, node.GetText());
        }

        protected virtual void InsertBefore(JavaNode existing, string text)
        {
            File.InsertBefore(existing, text);
        }

        public virtual void InsertAfter(JavaNode existing, JavaNode node)
        {
            if (HasChild(node))
            {
                throw new InvalidOperationException("Child already exists: " + node.ToString());
            }
            //Children.Insert(Children.IndexOf(existing) + 1, node);
            InsertAfter(existing, node.GetTextWithComments());
        }

        protected virtual void InsertAfter(JavaNode existing, string text)
        {
            File.InsertAfter(existing, text);
        }

        public bool HasChild(JavaNode node)
        {
            return Children.Contains(node);
        }

        public void InsertChild(int index, JavaNode node)
        {
            _children.Insert(index, node);
        }

        public JavaAnnotation TryGetAnnotation(JavaParser.AnnotationContext context)
        {
            return Annotations.SingleOrDefault(x => x.Context.GetType() == context.GetType() && x.Identifier == x.GetIdentifier(context));
        }

        public void InsertAnnotation(int index, JavaAnnotation annotation)
        {
            Annotations.Insert(index, annotation);
        }

        public virtual string GetTextWithComments()
        {
            var ws = File.GetCommentsAndWhitespaceBefore(StartToken);
            return $"{ws.Text}{GetText()}";
            //return $"{ws.Text}{Context.GetFullText()}";
            //return Context.GetFullText();
        }

        public virtual string GetText()
        {
            return Context.Start.InputStream.GetText(Interval.Of(StartToken.StartIndex, StopToken.StopIndex));
        }

        public virtual void ReplaceWith(string text)
        {
            File.Replace(this, text);
        }

        public virtual void Remove()
        {
            File.Replace(this, "");
        }

        public virtual bool IsIgnored()
        {
            return HasAnnotation("@IntentIgnore");// || GetComments().Contains($"//@{IntentDecorators.IntentIgnore}");
            //return Annotations.Any(x => x.Identifier.StartsWith("@IntentIgnore"));// || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentIgnore()");
        }

        public virtual bool CanAdd()
        {
            return !HasIntentInstructions() || IsManaged() || IsMerged() || HasAnnotation("IntentCanAdd");// || GetComments().Contains($"//@IntentCanAdd");
        }

        public virtual bool CanUpdate()
        {
            return !HasIntentInstructions() || IsManaged() || IsMerged() || HasAnnotation("IntentCanUpdate");// || GetComments().Contains($"//@IntentCanUpdate");
        }

        public virtual bool CanRemove()
        {
            return !HasIntentInstructions() || IsManaged() || HasAnnotation("IntentCanRemove");// || GetComments().Contains($"//@IntentCanRemove");
        }

        public virtual bool IsManaged()
        {
            return HasAnnotation("@IntentManage");
            //return Annotations.Any(x => x.Identifier.StartsWith("@IntentManage"));// || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentMerge()");
        }

        public virtual bool IsMerged()
        {
            return HasAnnotation("@IntentMerge");
            //return Annotations.Any(x => x.Identifier.StartsWith("@IntentMerge"));// || Node.GetTextWithComments().TrimStart().StartsWith("//@IntentMerge()");
        }

        protected virtual bool HasIntentInstructions()
        {
            return Annotations.Any(x => x.Identifier.StartsWith("@Intent"));// || GetComments().Contains("//@Intent");
        }

        protected bool CanReplaceInsteadOfMerge()
        {
            return (!Annotations.Any() && !Children.Any()) || (Children.All(x => !x.HasIntentInstructions()) && !HasIntentInstructions());
        }

        protected bool HasAnnotation(string name)
        {
            return Annotations.Any(x => x.Identifier.Split('(').First().Equals(name));
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
            return GetTextWithComments();
        }

        protected void MergeNodeCollections(JavaNode outputNode, Func<JavaNode, IList<JavaNode>> getCollection)
        {
            var index = 0;
            var highestFoundIndex = 0;
            foreach (var node in getCollection(outputNode))
            {
                var existing = getCollection(this).SingleOrDefault(x => x.Equals(node));
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
                        this.InsertBefore(getCollection(this)[0], (dynamic)node);
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
                    if (existing.CanUpdate() && existing.CanReplaceInsteadOfMerge())
                    {
                        if (existing.GetTextWithComments() == node.GetTextWithComments())
                        {
                            continue;
                        }
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }

                    existing.MergeWith(node);
                }
            }

            if (CanRemove())
            {
                var toRemove = getCollection(this).Where(x => !(x is JavaImport) && !x.IsIgnored()).Except(getCollection(outputNode)).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        protected virtual void AddFirst(JavaAnnotation node)
        {
            File.InsertBefore(StartToken, node.GetText().TrimStart() + Environment.NewLine);
        }

        protected virtual void AddFirst(JavaNode node)
        {
            File.InsertBefore(Context.Stop ?? Context.Start, node.GetTextWithComments() + Environment.NewLine);
        }
    }
}