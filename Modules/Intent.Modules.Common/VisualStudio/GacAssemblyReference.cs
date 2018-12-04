namespace Intent.Modules.Common.VisualStudio
{
    public class GacAssemblyReference : IAssemblyReference
    {
        public GacAssemblyReference(string library)
        {
            Library = library;
        }

        public bool Equals(IAssemblyReference other)
        {
            if (other == null)
                return false;
            return Library == other.Library;
        }

        public string Library { get; }
        public string HintPath { get; }
        public bool HasHintPath()
        {
            return false;
        }
    }
}
