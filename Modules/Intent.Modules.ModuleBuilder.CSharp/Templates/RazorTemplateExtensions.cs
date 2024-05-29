using Intent.ModuleBuilder.CSharp.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.ModuleBuilder.CSharp.Templates;

internal static class RazorTemplateExtensions
{
    public static string GetPageDirectiveText(this CSharpTemplateBase<RazorTemplateModel> template)
    {
        var result = $"/{template.Model.Name.RemoveSuffix("Template").ToKebabCase()}";

        if (template.Model.IsFilePerModelTemplateRegistration())
        {
            result += "/{Model.Name.ToKebabCase()}";
        }

        return result;
    }

    public static string GetPageTitleText(this CSharpTemplateBase<RazorTemplateModel> template)
    {
        var result = template.Model.Name.RemoveSuffix("Template").ToSentenceCase();

        if (template.Model.IsFilePerModelTemplateRegistration())
        {
            result += " - {Model.Name.ToSentenceCase()}";
        }

        return result;
    }
}