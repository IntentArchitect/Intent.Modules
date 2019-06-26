using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class F_OptionalAggregateNav : Object, IF_OptionalAggregateNav
    {
        public F_OptionalAggregateNav()
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

        private F_OptionalDependent _f_OptionalDependent;

        public virtual F_OptionalDependent F_OptionalDependent
        {
            get
            {
                return _f_OptionalDependent;
            }
            set
            {
                _f_OptionalDependent = value;
            }
        }

    }
}
