using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Builder;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.TypeScript.TypeScriptCustomBuilder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class TypeScriptCustomBuilderTemplate : TypeScriptTemplateBase<object>, ITypescriptFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.TypeScript.TypeScriptCustomBuilder";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TypeScriptCustomBuilderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            TypescriptFile = new TypescriptFile(this.GetFolderPath())
                .AddClass($"TypeScriptCustomBuilder", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "exampleParam", param =>
                        {
                            param.WithPrivateReadonlyFieldAssignment();
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public TypescriptFile TypescriptFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return TypescriptFile.GetConfig($"TypeScriptCustomBuilder");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return TypescriptFile.ToString();
        }
    }
}