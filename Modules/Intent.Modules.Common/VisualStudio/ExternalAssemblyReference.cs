namespace Intent.Modules.Common.VisualStudio
{
    public class ExternalAssemblyReference : IAssemblyReference
    {
        public string Library { get; }
        public string HintPath { get; }

        public ExternalAssemblyReference(string library, string hintPath)
        {
            Library = library;
            HintPath = hintPath;
        }

        public bool HasHintPath()
        {
            return true;
        }

        public bool Equals(IAssemblyReference other)
        {
            if (other == null)
                return false;
            return Library == other.Library;
        }
    }
}
