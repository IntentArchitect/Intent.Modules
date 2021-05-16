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

        public ResolvedTypeInfo(IResolvedTypeInfo typeInfo) : this(typeInfo.Name, typeInfo.IsPrimitive, typeInfo.Template)
        {

        }

        public string Name { get; set; }
        public bool IsPrimitive { get; set; }
        public ITemplate Template { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}