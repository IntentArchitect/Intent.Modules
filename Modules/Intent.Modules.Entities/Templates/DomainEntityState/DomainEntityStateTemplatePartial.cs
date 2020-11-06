using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    partial class DomainEntityStateTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityStateDecoratorBase>, ITemplatePostCreationHook
    {
        private readonly IMetadataManager _metadataManager;
        public const string Identifier = "Intent.Entities.DomainEntityState";

        private readonly IList<DomainEntityStateDecoratorBase> _decorators = new List<DomainEntityStateDecoratorBase>();

        public DomainEntityStateTemplate(ClassModel model, IProject project, IMetadataManager metadataManager)
            : base(Identifier, project, model)
        {
            _metadataManager = metadataManager;
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, Identifier, "ICollection<{0}>"));
        }

        public string EntityInterfaceName => Project.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(DomainEntityInterfaceTemplate.Identifier, Model))?.ClassName
                                             ?? $"I{Model.Name}";

        
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}",
                fileName: $"{Model.Name}State");
        }

        public void AddDecorator(DomainEntityStateDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityStateDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string GetBaseClass(ClassModel @class)
        {
            try
            {
                return GetDecorators().Select(x => x.GetBaseClass(@class)).SingleOrDefault(x => x != null) ?? @class.ParentClass?.Name ?? GetBaseType();
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {@class.Name}");
            }
        }

        private string GetBaseType()
        {
            var typeId = Model.GetStereotypeProperty<string>("Base Type", "Type");
            if (typeId == null)
            {
                return "Object";
            }

            var type = _metadataManager.Domain(OutputTarget.Application).GetTypeDefinitionModels().FirstOrDefault(x => x.Id == typeId);
            if (type != null)
            {
                return type.Name;
            }
            throw new Exception($"Could not find Base Type for class {Model.Name}");
        }

        public string GetInterfaces(ClassModel @class)
        {
            var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
            return interfaces.Any() ? ", " + interfaces.Aggregate((x, y) => x + ", " + y) : "";
        }

        public string ClassAnnotations(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.ClassAnnotations(@class));
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string BeforeProperties(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        }

        public string PropertyBefore(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        }

        public string PropertyFieldAnnotations(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyFieldAnnotations(attribute));
        }

        public string PropertyAnnotations(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertySetterBefore(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterBefore(attribute));
        }

        public string PropertySetterAfter(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterAfter(attribute));
        }

        public string AssociationBefore(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.AssociationBefore(associationEnd));
        }

        public string PropertyAnnotations(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        }

        public string PropertySetterBefore(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterBefore(associationEnd));
        }

        public string PropertySetterAfter(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterAfter(associationEnd));
        }

        public string AssociationAfter(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.AssociationAfter(associationEnd));
        }

        public bool CanWriteDefaultAttribute(AttributeModel attribute)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        }

        public bool CanWriteDefaultAssociation(AssociationEndModel association)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        }
    }
}
