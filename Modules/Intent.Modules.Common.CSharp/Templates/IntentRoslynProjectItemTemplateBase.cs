using System;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

[assembly: InternalsVisibleTo("Intent.Modules.Common.CSharp.Tests")]

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentFileTemplateBase : IntentTemplateBase
    {
        protected IntentFileTemplateBase(string templateId, IOutputTarget project) : base(templateId, project)
        {
        }

        public IOutputTarget Project => OutputTarget;
    }

    public abstract class IntentFileTemplateBase<TModel> : IntentTemplateBase<TModel>
    {
        protected IntentFileTemplateBase(string templateId, IOutputTarget project, TModel model) : base(templateId, project, model)
        {
        }

        public IOutputTarget Project => OutputTarget;
    }
}
