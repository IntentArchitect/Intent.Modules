using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class ApiMetadataDesignerExtensionsTemplate : CSharpTemplateBase<IList<DesignerModel>>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiMetadataDesignerExtensions";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public ApiMetadataDesignerExtensionsTemplate(IOutputTarget outputTarget, IList<DesignerModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(Model.First().ParentModule.ApiNamespace, this.GetFolderPath())
                .AddUsing("Intent.Engine")
                .AddUsing("Intent.Metadata.Models")
                .AddClass($"ApiMetadataDesignerExtensions", @class =>
                {
                    @class.Static();
                    foreach (var designer in Model)
                    {
                        @class.AddField("string", $"{designer.Name.ToCSharpIdentifier()}DesignerId", field => field.Constant($"\"{designer.Id}\""));
                        @class.AddMethod("IDesigner", designer.Name.ToCSharpIdentifier(), method =>
                        {
                            method.Static();
                            method.AddParameter("IMetadataManager", "metadataManager", param => param.WithThisModifier());
                            method.AddParameter("IApplication", "application");
                            method.AddStatement($"return metadataManager.{designer.Name.ToCSharpIdentifier()}(application.Id);");
                        });

                        @class.AddMethod("IDesigner", designer.Name.ToCSharpIdentifier(), method =>
                        {
                            method.Static();
                            method.AddParameter("IMetadataManager", "metadataManager", param => param.WithThisModifier());
                            method.AddParameter("string", "applicationId");
                            method.AddStatement($"return metadataManager.GetDesigner(applicationId, {designer.Name.ToCSharpIdentifier()}DesignerId);");
                        });
                    }
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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