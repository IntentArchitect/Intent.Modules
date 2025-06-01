using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiTraitModel
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ApiTraitModelTemplate : CSharpTemplateBase<IStereotypeDefinition>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiTraitModel";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ApiTraitModelTemplate(IOutputTarget outputTarget, IStereotypeDefinition model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name.ToCSharpIdentifier()}Model", @interface =>
                {
                    if (!string.IsNullOrWhiteSpace(Model.Comment))
                    {
                        @interface.WithComments($"""
                                                 /// <summary>
                                                 /// {Model.Comment}
                                                 /// </summary>
                                                 """);
                    }

                    @interface.ExtendsInterface(UseType("Intent.Modules.Common.IElementWrapper"));
                    @interface.ExtendsInterface(UseType("Intent.Metadata.Models.IMetadataModel"));
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