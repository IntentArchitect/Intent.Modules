using System;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFile : IFileBuilderBase<IIndentedFile>
{
    IIndentedFileItems Items { get; }
    IIndentedFile WithItems(Action<IIndentedFileItems> configure);
    IIndentedFile WithIndentation(string indentation);
}