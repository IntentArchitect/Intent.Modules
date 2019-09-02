using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class E_RequiredCompositeNav : Object, IE_RequiredCompositeNav
    {
        public E_RequiredCompositeNav()
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

        private E_RequiredDependent _e_RequiredDependent;

        public virtual E_RequiredDependent E_RequiredDependent
        {
            get
            {
                return _e_RequiredDependent;
            }
            set
            {
                _e_RequiredDependent = value;
            }
        }

    }
}
