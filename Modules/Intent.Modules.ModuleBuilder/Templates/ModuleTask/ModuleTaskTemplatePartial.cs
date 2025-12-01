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
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    @class.ImplementsInterface(UseType("Intent.Plugins.IModuleTask"));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Merge());
                    });

                    @class.AddProperty("string", "TaskTypeId", property =>
                    {
                        property.WithoutSetter().Getter.WithExpressionImplementation($"\"{model.InternalElement.Package.Name}.{model.Name}\"");
                    });

                    @class.AddProperty("string", "TaskTypeName", property =>
                    {
                        property.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        property.WithoutSetter().Getter.WithExpressionImplementation($"\"{model.Name.RemoveSuffix("Task").ToSentenceCase()}\"");
                    });

                    @class.AddProperty("int", "Order", property =>
                    {
                        property.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
                        property.WithoutSetter().Getter.WithExpressionImplementation("0");
                    });

                    @class.AddMethod("string", "Execute", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.IgnoreBody());
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