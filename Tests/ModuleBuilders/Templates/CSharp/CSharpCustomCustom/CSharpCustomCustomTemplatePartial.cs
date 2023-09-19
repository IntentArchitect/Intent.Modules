using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.CSharp.CSharpCustomCustom
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CSharpCustomCustomTemplate : CSharpTemplateBase<object>
    {
        public const string TemplateId = "ModuleBuilders.CSharp.CSharpCustomCustomTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public CSharpCustomCustomTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"CSharpCustomCustom",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public override string TransformText()
        {
            throw new NotImplementedException("Implement custom template here");
        }
    }
}