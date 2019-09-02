using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class K_SelfReference : Object, IK_SelfReference
    {
        public K_SelfReference()
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

        public Guid? SelfId { get; set; }
        private K_SelfReference _self;

        public virtual K_SelfReference Self
        {
            get
            {
                return _self;
            }
            set
            {
                _self = value;
            }
        }

        public Guid? K_SelfReferenceId { get; set; }
    }
}
