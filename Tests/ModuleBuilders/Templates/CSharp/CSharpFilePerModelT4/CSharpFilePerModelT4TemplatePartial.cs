using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.CSharp.CSharpFilePerModelT4
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class CSharpFilePerModelT4Template : CSharpTemplateBase<ClassModel>
    {
        public const string TemplateId = "ModuleBuilders.CSharp.CSharpFilePerModelT4Template";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public CSharpFilePerModelT4Template(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }
    }
}