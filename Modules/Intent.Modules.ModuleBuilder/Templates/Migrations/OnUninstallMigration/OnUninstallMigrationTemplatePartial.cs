using System;
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

namespace Intent.Modules.ModuleBuilder.Templates.Migrations.OnUninstallMigration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OnUninstallMigrationTemplate : CSharpTemplateBase<OnUninstallMigrationModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Migrations.OnUninstallMigration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OnUninstallMigrationTemplate(IOutputTarget outputTarget, OnUninstallMigrationModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedMerge()
                .AddClass($"{Model.Name}Migration".ToCSharpIdentifier(), @class =>
                {
                    @class.ImplementsInterface(UseType("Intent.Plugins.IModuleOnUninstallMigration"));
                    @class.AddConstructor();

                    @class.AddProperty("string", "ModuleId", prop => prop
                        .AddAttribute("IntentFully")
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation($"\"{model.ParentModule.Name}\""));

                    @class.AddMethod("void", "OnUninstall");
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