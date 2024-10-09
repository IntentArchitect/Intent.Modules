using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace ModuleBuilders.Templates.StaticContent.StaticContentTemplateRegistrations
{
    public class StaticContentTextStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "ModuleBuilders.Templates.StaticContent.StaticContentTemplateRegistrations.StaticContentTextStaticContentTemplateRegistration";

        public StaticContentTextStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        [IntentIgnore]
        protected override OverwriteBehaviour GetDefaultOverrideBehaviour(IOutputTarget outputTarget)
        {
            return base.GetDefaultOverrideBehaviour(outputTarget);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}