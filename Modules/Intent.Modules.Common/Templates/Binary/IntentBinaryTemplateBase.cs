using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.Templates
{
    public abstract class IntentBinaryTemplateBase : IntentTemplateBase, IIntentBinaryTemplate
    {
        public IntentBinaryTemplateBase(string templateId, IOutputTarget outputTarget)
            : base(templateId, outputTarget)
        {
            
        }

        public abstract byte[] RunBinaryTemplate();

        public override string TransformText()
        {
            return string.Empty;
        }
    }

    public abstract class IntentBinaryTemplateBase<TModel> : IntentTemplateBase<TModel>, IIntentBinaryTemplate
    {
        public IntentBinaryTemplateBase(string templateId, IOutputTarget outputTarget, TModel model)
            : base(templateId, outputTarget, model)
        {

        }

        public abstract byte[] RunBinaryTemplate();

        public override string TransformText()
        {
            return string.Empty;
        }
    }

}
