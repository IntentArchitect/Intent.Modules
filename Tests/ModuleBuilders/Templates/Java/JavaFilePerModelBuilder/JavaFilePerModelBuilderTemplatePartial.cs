using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Java;
using Intent.Modules.Common.Java.Builder;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Java.Templates.JavaFileTemplatePartial", Version = "1.0")]

namespace ModuleBuilders.Templates.Java.JavaFilePerModelBuilder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class JavaFilePerModelBuilderTemplate : JavaTemplateBase<Intent.Modelers.Domain.Api.ClassModel>, IJavaFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilders.Java.JavaFilePerModelBuilder";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public JavaFilePerModelBuilderTemplate(IOutputTarget outputTarget, Intent.Modelers.Domain.Api.ClassModel model) : base(TemplateId, outputTarget, model)
        {
            JavaFile = new JavaFile(this.GetPackage(), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("string", "exampleParam", param =>
                        {
                            param.IntroduceField();
                        });
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public JavaFile JavaFile { get; }

        [IntentManaged(Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return JavaFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return JavaFile.ToString();
        }
    }
}