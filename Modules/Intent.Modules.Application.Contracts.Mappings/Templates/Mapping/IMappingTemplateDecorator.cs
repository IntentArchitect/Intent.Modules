using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    public interface IMappingTemplateDecorator : ITemplateDecorator
    {
        string[] Usings();
        string[] AdditionalMembers(string contractTypeName, string domainTypeName);
    }
}
