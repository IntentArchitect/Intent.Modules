using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class G_MultipleDependent : Object, IG_MultipleDependent
    {
        public G_MultipleDependent()
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

        public Guid G_RequiredCompositeNavId { get; set; }
        private G_RequiredCompositeNav _g_RequiredCompositeNav;

        public virtual G_RequiredCompositeNav G_RequiredCompositeNav
        {
            get
            {
                return _g_RequiredCompositeNav;
            }
            set
            {
                _g_RequiredCompositeNav = value;
            }
        }

    }
}
