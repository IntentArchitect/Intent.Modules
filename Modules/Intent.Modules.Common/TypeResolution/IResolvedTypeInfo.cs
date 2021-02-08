using Intent.Templates;

namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Information about the resolved type.
    /// </summary>
    public interface IResolvedTypeInfo
    {
        string Name { get; }
        bool IsPrimitive { get; }
        ITemplate Template { get; }
    }

    public class ResolvedTypeInfo : IResolvedTypeInfo
    {
        public ResolvedTypeInfo(string name, bool isPrimitive, ITemplate template)
        {
            Name = name;
            IsPrimitive = isPrimitive;
            Template = template;
        }

        public string Name { get; }
        public bool IsPrimitive { get; }
        public ITemplate Template { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}