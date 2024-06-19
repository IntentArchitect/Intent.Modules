using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpFile : ICSharpCodeContext, IFileBuilderBase
{
    ICSharpTemplate Template { get; }
    string GetModelType<TModel>(TModel model) where TModel : IMetadataModel, IHasName;
}