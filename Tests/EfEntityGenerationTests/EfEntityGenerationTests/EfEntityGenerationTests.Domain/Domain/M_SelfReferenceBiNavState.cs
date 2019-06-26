using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class M_SelfReferenceBiNav : Object, IM_SelfReferenceBiNav
    {
        public M_SelfReferenceBiNav()
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
        private M_SelfReferenceBiNav _self;

        public virtual M_SelfReferenceBiNav Self
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

        public Guid? OriginalId { get; set; }
        private M_SelfReferenceBiNav _original;

        public virtual M_SelfReferenceBiNav Original
        {
            get
            {
                return _original;
            }
            set
            {
                _original = value;
            }
        }

    }
}
