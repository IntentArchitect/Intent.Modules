using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaClass : IEquatable<JavaClass>
    {
        private readonly JavaParser.ClassDeclarationContext _context;

        public JavaClass(JavaParser.ClassDeclarationContext context)
        {
            _context = context;
            Name = _context.GetChild(0).GetChild(1).GetText();
        }

        public string Name { get; }

        public IList<JavaMethod> Methods { get; } = new List<JavaMethod>();

        public bool Equals(JavaClass other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JavaClass) obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}