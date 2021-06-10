using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    public class ExtensionModelType
    {
        private readonly IElement _element;

        public ExtensionModelType(IElement element)
        {
            _element = element;
        }

        public string Name => _element.Name;

        public string ApiNamespace => new IntentModuleModel(_element.Package).ApiNamespace;

        public string ApiClassName => $"{Name.ToCSharpIdentifier()}Model";
    }
}