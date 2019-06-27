using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class H_MultipleDependent : Object, IH_MultipleDependent
    {
        public H_MultipleDependent()
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

        public Guid? H_OptionalAggregateNavId { get; set; }
        private H_OptionalAggregateNav _h_OptionalAggregateNav;

        public virtual H_OptionalAggregateNav H_OptionalAggregateNav
        {
            get
            {
                return _h_OptionalAggregateNav;
            }
            set
            {
                _h_OptionalAggregateNav = value;
            }
        }

    }
}
