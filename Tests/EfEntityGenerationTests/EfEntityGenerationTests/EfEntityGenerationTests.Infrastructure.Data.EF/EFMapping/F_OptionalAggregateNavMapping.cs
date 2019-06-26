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
    public partial class F_OptionalAggregateNavMapping : EntityTypeConfiguration<F_OptionalAggregateNav>
    {

        public F_OptionalAggregateNavMapping()
        {
            this.ToTable("F_OptionalAggregateNav");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasOptional(x => x.F_OptionalDependent)
            .WithOptionalDependent(x => x.F_OptionalAggregateNav)
            ;
        }
    }
}
