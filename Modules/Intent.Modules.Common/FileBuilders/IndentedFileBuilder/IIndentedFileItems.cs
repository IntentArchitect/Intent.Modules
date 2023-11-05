using System;
using System.Collections.Generic;

namespace Intent.Modules.Common.FileBuilders.IndentedFileBuilder;

public interface IIndentedFileItems : IIndentedFileItem<IIndentedFileItems>, IList<IIndentedFileItem>
{
    IIndentedFileItems WithItems(Action<IIndentedFileItems> configure);
    IIndentedFileItems WithContent(string content, Action<IIndentedFileContent> configure = null);
    string ToString(string indentation);
}