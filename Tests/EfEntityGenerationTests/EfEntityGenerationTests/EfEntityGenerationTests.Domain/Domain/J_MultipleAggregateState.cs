using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using EfEntityGenerationTests.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial class J_MultipleAggregate : Object, IJ_MultipleAggregate
    {
        public J_MultipleAggregate()
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

        public Guid J_RequiredDependentId { get; set; }
        private J_RequiredDependent _j_RequiredDependent;

        public virtual J_RequiredDependent J_RequiredDependent
        {
            get
            {
                return _j_RequiredDependent;
            }
            set
            {
                _j_RequiredDependent = value;
            }
        }

    }
}
