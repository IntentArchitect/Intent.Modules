using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class D_OptionalAggregate : Object, ID_OptionalAggregate
    {
        public D_OptionalAggregate()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private ICollection<D_MultipleDependent> _d_MultipleDependents;

        public virtual ICollection<D_MultipleDependent> D_MultipleDependents
        {
            get
            {
                return _d_MultipleDependents ?? (_d_MultipleDependents = new List<D_MultipleDependent>());
            }
            set
            {
                _d_MultipleDependents = value;
            }
        }

    }
}
