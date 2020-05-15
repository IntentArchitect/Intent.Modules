namespace Intent.Modules.Common.VisualStudio
{
    public class AssemblyRedirectInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string PublicKey { get; set; }

        public AssemblyRedirectInfo(string name, string version, string publicKey)
        {
            Name = name;
            Version = version;
            PublicKey = publicKey;
        }
    }
}
