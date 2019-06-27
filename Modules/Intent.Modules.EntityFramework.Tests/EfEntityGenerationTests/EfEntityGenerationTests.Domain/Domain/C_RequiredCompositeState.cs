using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class C_RequiredComposite : Object, IC_RequiredComposite
    {
        public C_RequiredComposite()
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

        private ICollection<C_MultipleDependent> _c_MultipleDependents;

        public virtual ICollection<C_MultipleDependent> C_MultipleDependents
        {
            get
            {
                return _c_MultipleDependents ?? (_c_MultipleDependents = new List<C_MultipleDependent>());
            }
            set
            {
                _c_MultipleDependents = value;
            }
        }

    }
}
