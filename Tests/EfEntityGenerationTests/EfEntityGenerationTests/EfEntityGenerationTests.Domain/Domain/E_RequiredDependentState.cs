using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class E_RequiredDependent : Object, IE_RequiredDependent
    {
        public E_RequiredDependent()
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

        private E_RequiredCompositeNav _e_RequiredCompositeNav;

        public virtual E_RequiredCompositeNav E_RequiredCompositeNav
        {
            get
            {
                return _e_RequiredCompositeNav;
            }
            set
            {
                _e_RequiredCompositeNav = value;
            }
        }

    }
}
