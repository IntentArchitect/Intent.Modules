using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;
using Intent.RoslynWeaver.Attributes;

using EfEntityGenerationTests.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.EFMapping", Version = "1.0")]

namespace EfEntityGenerationTests.Infrastructure.Data.EF
{
    public partial class E_RequiredCompositeNavMapping : EntityTypeConfiguration<E_RequiredCompositeNav>
    {

        public E_RequiredCompositeNavMapping()
        {
            this.ToTable("E_RequiredCompositeNav");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasRequired(x => x.E_RequiredDependent)
            .WithRequiredPrincipal(x => x.E_RequiredCompositeNav)
            .WillCascadeOnDelete()
            ;
        }
    }
}
