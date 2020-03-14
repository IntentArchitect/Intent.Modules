using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICreationOption
    {
        string SpecializationType { get; }

        string Text { get; }

        string Shortcut { get; }

        string DefaultName { get; }

        IconModel Icon { get; }

        IElement Type { get; }

        bool AllowMultiple { get; }
    }
}