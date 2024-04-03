using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpStringInterpolation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class CSharpStringInterpolationTemplate : CSharpTemplateBase<CSharpTemplateModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CSharpStringInterpolationTemplate(IOutputTarget outputTarget, CSharpTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }
        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}")
            {
                AutoFormat = false
            };

        }

        private bool IsForInterface()
        {
            return Model.Name.RemoveSuffix("Template").EndsWith("Interface");
        }
    }
}