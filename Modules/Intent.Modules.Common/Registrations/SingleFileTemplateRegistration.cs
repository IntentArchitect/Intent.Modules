using Intent.Engine;
using Intent.Registrations;
using Intent.Templates;

namespace Intent.Modules.Common.Registrations
{
    /// <summary>
    /// Template Registration that produces as single file.
    /// <para>
    /// Learn more about template registrations in
    /// <seealso href="https://intentarchitect.com/#/redirect/?category=xmlDocComment&amp;subCategory=intent.modules.common&amp;additionalData=templateRegistrations">
    /// this article</seealso>.
    /// </para>
    /// </summary>
    public abstract class SingleFileTemplateRegistration : TemplateRegistrationBase
    {
        public abstract ITemplate CreateTemplateInstance(IOutputTarget project);

        protected override void Register(ITemplateInstanceRegistry registry, IApplication application)
        {
            registry.RegisterTemplate(TemplateId, CreateTemplateInstance);
        }
    }
}