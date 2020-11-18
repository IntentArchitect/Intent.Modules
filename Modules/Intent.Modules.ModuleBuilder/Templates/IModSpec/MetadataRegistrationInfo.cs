using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class MetadataRegistrationInfo
    {
        public string Target { get; }
        public string Path { get; }
        public string Id { get; }

        public MetadataRegistrationInfo(string target, string path, string id)
        {
            Target = target;
            Path = PathHelper.NormalizePath(path);
            Id = id;
        }
    }
}