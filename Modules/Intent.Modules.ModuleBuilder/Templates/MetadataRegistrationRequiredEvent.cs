using System.Collections.Generic;

namespace Intent.Modules.ModuleBuilder.Templates
{
    public class MetadataRegistrationRequiredEvent
    {
        public string Id { get; }
        public IEnumerable<(string Id, string Name)> Targets { get; }
        public string Path { get; }

        public MetadataRegistrationRequiredEvent(string id, IEnumerable<(string Id, string Name)> targets, string path)
        {
            Id = id;
            Targets = targets;
            Path = path;
        }
    }
}