using System;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Modules.Common.TypeResolution;
using Intent.Templates;

[assembly: InternalsVisibleTo("Intent.Modules.Common.Tests")]

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentProjectItemTemplateBase : IntentTemplateBase
    {
        protected IntentProjectItemTemplateBase(string templateId, IOutputTarget project) : base(templateId, project)
        {
        }

        public IOutputTarget Project => OutputTarget;
    }

    public abstract class IntentProjectItemTemplateBase<TModel> : IntentTemplateBase<TModel>
    {
        protected IntentProjectItemTemplateBase(string templateId, IOutputTarget project, TModel model) : base(templateId, project, model)
        {
        }

        public IOutputTarget Project => OutputTarget;
    }

    public abstract class IntentRoslynProjectItemTemplateBase<TModel> : CSharpTemplateBase<TModel>
    {
        protected IntentRoslynProjectItemTemplateBase(string templateId, IOutputTarget project, TModel model) : base(templateId, project, model)
        {
        }

        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return DefineRoslynDefaultFileMetadata();
        }

        protected abstract RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata();

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            throw new InvalidOperationException("Please contact support@intentarchitect.com if this problem persists.");
        }
    }

    public abstract class IntentRoslynProjectItemTemplateBase : IntentRoslynProjectItemTemplateBase<object>
    {
        protected IntentRoslynProjectItemTemplateBase(string templateId, IOutputTarget project) : base(templateId, project, null)
        {
        }
    }
}
