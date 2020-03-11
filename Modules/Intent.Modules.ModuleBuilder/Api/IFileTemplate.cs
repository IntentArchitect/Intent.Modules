using System;
using System.Collections;
using System.Text;
using Intent.Engine;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IFileTemplate : IModuleBuilderElement
    {
        IModelerModelType GetModelType();
        IModelerReference GetModeler();
        string GetModelTypeName();
        string GetModelerName();
        string FileExtension { get; }

    }
}
