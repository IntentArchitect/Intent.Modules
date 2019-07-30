using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class B_OptionalAggregate : Object, IB_OptionalAggregate
    {
        public B_OptionalAggregate()
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

        private B_OptionalDependent _b_OptionalDependent;

        public virtual B_OptionalDependent B_OptionalDependent
        {
            get
            {
                return _b_OptionalDependent;
            }
            set
            {
                _b_OptionalDependent = value;
            }
        }

    }
}
