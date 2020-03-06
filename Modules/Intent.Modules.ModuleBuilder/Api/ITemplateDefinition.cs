using System;
using System.Collections;
using System.Text;
using Intent.Engine;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ITemplateDefinition : IModuleBuilderElement
    {
        IModelerModelType GetModelType();
        IModelerReference GetModeler();
        string GetModelTypeName();
        string GetModelerName();
    }
}
