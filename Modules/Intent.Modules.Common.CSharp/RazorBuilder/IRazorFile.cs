#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Collections.Generic;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.RazorBuilder;

public interface IRazorFile : ICSharpFile, IRazorFileNodeBase<IRazorFile>, IFileBuilderBase<IRazorFile>
{
    static IRazorFile Create<TModel>(RazorTemplateBase<TModel> template, string className) =>
        new RazorFile<TModel>(template, className);

    IList<IRazorDirective> Directives { get; }

    IRazorFile AddUsing(string @namespace);

    IRazorFile WithNamespace(string @namespace);

    IRazorFile WithOverwriteBehaviour(OverwriteBehaviour overwriteBehaviour);

    IRazorFile Configure(Action<IRazorFile> configure);

    new RazorFileConfig GetConfig();

    string ToString();
}
