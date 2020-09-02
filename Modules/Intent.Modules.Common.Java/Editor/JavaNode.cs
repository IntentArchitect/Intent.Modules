using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public abstract class JavaNode : IEquatable<JavaNode>
    {

        protected JavaNode(ParserRuleContext context, JavaFile file)
        {
            File = file;
            Context = context;
        }

        public JavaFile File { get; }
        public abstract string Identifier { get; }
        public ParserRuleContext Context { get; }

        public IList<JavaNode> Children { get; } = new List<JavaNode>();

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