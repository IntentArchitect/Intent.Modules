using System.Collections;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    partial class DomainEntityInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasDecorators<DomainEntityInterfaceDecoratorBase>, IPostTemplateCreation
    {

        public const string Identifier = "Intent.Entities.DomainEntityInterface";

        private IEnumerable<DomainEntityInterfaceDecoratorBase> _decorators;

        public DomainEntityInterfaceTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public void Created()
        {
            Types.AddClassTypeSource(ClassTypeSource.InProject(Project, DomainEntityStateTemplate.Identifier, nameof(ICollection)));
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "I${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "I${Model.Name}",
                @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<DomainEntityInterfaceDecoratorBase> GetDecorators()
        {
            if (_decorators == null)
            {
                _decorators = Project.ResolveDecorators(this);
                foreach (var decorator in _decorators)
                {
                    decorator.Template = this;
                }
            }
            return _decorators;
        }

        public string GetInterfaces(IClass @class)
        {
            var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
            return interfaces.Any() ? " : " + interfaces.Aggregate((x, y) => x + ", " + y) : "";
        }

        public string InterfaceAnnotations(IClass @class)
        {
            return GetDecorators().Aggregate(x => x.InterfaceAnnotations(@class));
        }

        public string BeforeProperties(IClass @class)
        {
            return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        }

        public string PropertyBefore(IAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        }

        public string PropertyAnnotations(IAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertyBefore(IAssociationEnd associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(associationEnd));
        }

        public string PropertyAnnotations(IAssociationEnd associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        }

        public string AttributeAccessors(IAttribute attribute)
        {
            return GetDecorators().Select(x => x.AttributeAccessors(attribute)).FirstOrDefault(x => x != null) ?? "get; set;";
        }

        public string AssociationAccessors(IAssociationEnd associationEnd)
        {
            return GetDecorators().Select(x => x.AssociationAccessors(associationEnd)).FirstOrDefault(x => x != null) ?? "get; set;";
        }

        public bool CanWriteDefaultAttribute(IAttribute attribute)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        }

        public bool CanWriteDefaultAssociation(IAssociationEnd association)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        }

        public bool CanWriteDefaultOperation(IOperation operation)
        {
            return GetDecorators().All(x => x.CanWriteDefaultOperation(operation));
        }

        public string EmitOperationReturnType(IOperation operation)
        {
            if (operation.ReturnType != null)
            {
                var type = Types.Get(operation.ReturnType.Type);
                if (operation.ReturnType.IsCollection)
                {
                    type = $"ICollection<{type}>";
                }
                return type;
            }
            else
            {
                return "void";
            }
        }
    }
}
