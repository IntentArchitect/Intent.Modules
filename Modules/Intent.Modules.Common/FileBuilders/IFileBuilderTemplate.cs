using Intent.Templates;

namespace Intent.Modules.Common.FileBuilders;

public interface IFileBuilderTemplate : ITemplate
{
    IFileBuilderBase File { get; }
}