using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    partial class DTOTemplate : IntentRoslynProjectItemTemplateBase<DTOModel>, ITemplate, IHasAssemblyDependencies
    {
        public const string Identifier = "Intent.Application.Contracts.DTO";
        private readonly DecoratorDispatcher<IDTOAttributeDecorator> _decoratorDispatcher;

        public DTOTemplate(IProject project, DTOModel model, string identifier = Identifier)
            : base(identifier, project, model)
        {
            _decoratorDispatcher = new DecoratorDispatcher<IDTOAttributeDecorator>(project.ResolveDecorators<IDTOAttributeDecorator>);
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Runtime.Serialization"),
            };
        }

        public string ClassAttributes()
        {
            return _decoratorDispatcher.Dispatch(x => x.ClasssAttributes(Model));
        }

        public string PropertyAttributes(IDTOField field)
        {
            return _decoratorDispatcher.Dispatch(x => x.PropertyAttributes(Model, field));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("\\", GetNamespaceParts().DefaultIfEmpty("DTOs")),
                className: "${Name}",
                @namespace: string.Join(".", new [] { "${Project.ProjectName}" }.Concat(GetNamespaceParts()))
                );
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model.GetFolderPath().Select(x => x.GetPropertyValue<string>(StandardStereotypes.NamespaceProvider, "Namespace")).Where(x => x != null);
        }

        private string GetTypeInfo(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = "List<" + result + ">";
            }
            return result;
        }
    }
}
