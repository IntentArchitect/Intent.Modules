using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpFile : ICSharpCodeContext
{
    ICSharpTemplate Template { get; }
    string GetModelType<TModel>(TModel model) where TModel : IMetadataModel, IHasName;
}