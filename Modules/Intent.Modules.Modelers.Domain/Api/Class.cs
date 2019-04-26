using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;

namespace Intent.Modelers.Domain.Api
{
    public class Class : IClass
    {
        private readonly Metadata.Models.IClass _class;
        public Class(Metadata.Models.IClass @class)
        {
            _class = @class;
        }

        public string Id => _class.Id;
        public IEnumerable<IStereotype> Stereotypes => _class.Stereotypes;
        public IFolder Folder => _class.Folder;
        public string Name => _class.Name;
        public bool IsAbstract => _class.IsAbstract;
        public IEnumerable<string> GenericTypes => _class.GenericTypes;
        public IClass ParentClass
        {
            get
            {
                var parent = _class.AssociatedClasses.FirstOrDefault(x =>
                    x.Association.AssociationType == AssociationType.Generalization)?.Class;
                return parent != null ? new Class(parent) : null;
            }
        }
        public bool IsMapped => _class.IsMapped;
        public IClassMapping MappedClass => _class.MappedClass;
        public IApplication Application => _class.Application;
        public IEnumerable<IAttribute> Attributes => _class.Attributes;
        public IEnumerable<IOperation> Operations => _class.Operations;
        public IEnumerable<IAssociationEnd> AssociatedClasses => _class.AssociatedClasses;
        public IEnumerable<IAssociation> OwnedAssociations => _class.OwnedAssociations;
        public string Comment => _class.Id;
    }
}