using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.ModuleTask
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModuleTaskTemplate : CSharpTemplateBase<ModuleTaskModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.Templates.ModuleTask";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModuleTaskTemplate(IOutputTarget outputTarget, ModuleTaskModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.ImplementsInterface(UseType("Intent.Plugins.IModuleTask"));
                    @class.AddConstructor(ctor =>
                    {
                        //ctor.AddParameter("string", "exampleParam", param =>
                        //{
                        //    param.IntroduceReadonlyField();
                        //});
                    });

                    @class.AddProperty("string", "TaskTypeId", property =>
                    {
                        property.WithInitialValue($"\"{model.InternalElement.Package.Name}.{model.Name}\"");
                    });

                    @class.AddProperty("string", "TaskTypeName", property =>
                    {
                        property.WithInitialValue($"\"{model.Name.RemoveSuffix("Task").ToSentenceCase()}\"");
                    });

                    @class.AddProperty("int", "Order", property =>
                    {
                        property.WithInitialValue("0");
                    });

                    @class.AddMethod("string", "Execute", method =>
                    {
                        method.AddParameter("params string[]", "args");
                        method.AddStatement("// IntentInitialGen");
                        method.AddStatement($"// TODO: Implement {method.Class.Name}.{method.Name}(...) functionality");
                        method.AddStatement("throw new NotImplementedException(\"Implement your handler logic here...\");");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}