using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    public class ExtensionModelType : IMetadataModel
    {
        private readonly ICanBeReferencedType _element;

        public ExtensionModelType(ICanBeReferencedType element)
        {
            _element = element;
        }

        public string Name => _element.Name;

        public string ApiNamespace => new IntentModuleModel(_element.Package).ApiNamespace;

        public string ApiClassName => $"{Name.ToCSharpIdentifier()}Model";

        public string Id => _element.Id;

        public override string ToString()
        {
            return $"{nameof(ExtensionModelType)}: {_element}";
        }
    }
}