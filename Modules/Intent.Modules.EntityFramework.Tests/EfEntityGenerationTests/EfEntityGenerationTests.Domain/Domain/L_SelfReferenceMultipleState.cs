using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class L_SelfReferenceMultiple : Object, IL_SelfReferenceMultiple
    {
        public L_SelfReferenceMultiple()
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

        private ICollection<L_SelfReferenceMultiple> _miniSelves;

        public virtual ICollection<L_SelfReferenceMultiple> MiniSelves
        {
            get
            {
                return _miniSelves ?? (_miniSelves = new List<L_SelfReferenceMultiple>());
            }
            set
            {
                _miniSelves = value;
            }
        }

        public Guid? L_SelfReferenceMultipleId { get; set; }
    }
}
