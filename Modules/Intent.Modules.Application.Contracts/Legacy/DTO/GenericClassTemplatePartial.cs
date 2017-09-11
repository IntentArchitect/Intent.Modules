using Intent.SoftwareFactory.MetaModels.Class;

namespace Intent.Packages.Application.Contracts.Legacy.DTO
{
    partial class GenericClassTemplate 
    {
        private readonly string _ns;

        public GenericClassTemplate(string @namespace, ClassModel model)
        {
            _ns = @namespace;
            Model = model;
        }

        public string Namespace
        {
            get { return _ns; }
        }

        public string RunTemplate() => TransformText();
        public ClassModel Model { get; private set; }

    }
}
