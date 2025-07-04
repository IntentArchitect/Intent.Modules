using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

namespace Intent.Modules.ModuleBuilder.Templates.Migrations.OnVersionMigration
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class OnVersionMigrationTemplate : CSharpTemplateBase<VersionMigrationModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public OnVersionMigrationTemplate(IOutputTarget outputTarget, VersionMigrationModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentPersistenceSdk);
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedMerge()
                .AddClass(GetClassName(), @class =>
                {
                    @class.ImplementsInterface(UseType("Intent.Plugins.IModuleMigration"));
                    @class.AddConstructor();

                    @class.AddProperty("string", "ModuleId", prop => prop
                        .AddAttribute("IntentFully")
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation($"\"{model.ParentModule.Name}\""));

                    @class.AddProperty("string", "ModuleVersion", prop => prop
                        .AddAttribute("IntentFully")
                        .WithoutSetter()
                        .Getter.WithExpressionImplementation($"\"{Model.Name}\""));

                    @class.AddMethod("void", "Up");
                    @class.AddMethod("void", "Down");
                });
        }

        private string GetClassName()
        {
            var migration = $"Migration_{Model.Name.Replace(".", "_").Replace("-", "_")}".ToCSharpIdentifier();

            var parts = migration.Split('_');
            for (var index = 0; index < parts.Length; index++)
            {
                var part = parts[index];

                // Pad numbers with zeros so that string sorting also works with the file names
                if (int.TryParse(part, out var @int))
                {
                    parts[index] = @int.ToString("D2");
                    continue;
                }

                parts[index] = part.ToPascalCase();
            }

            return string.Join("_", parts);
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