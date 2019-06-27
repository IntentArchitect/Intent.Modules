using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class A_RequiredComposite : Object, IA_RequiredComposite
    {
        public A_RequiredComposite()
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

        private A_OptionalDependent _a_OptionalDependent;

        public virtual A_OptionalDependent A_OptionalDependent
        {
            get
            {
                return _a_OptionalDependent;
            }
            set
            {
                _a_OptionalDependent = value;
            }
        }

    }
}
