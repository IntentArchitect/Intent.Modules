using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class G_RequiredCompositeNav : Object, IG_RequiredCompositeNav
    {
        public G_RequiredCompositeNav()
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

        private ICollection<G_MultipleDependent> _g_MultipleDependents;

        public virtual ICollection<G_MultipleDependent> G_MultipleDependents
        {
            get
            {
                return _g_MultipleDependents ?? (_g_MultipleDependents = new List<G_MultipleDependent>());
            }
            set
            {
                _g_MultipleDependents = value;
            }
        }

    }
}
