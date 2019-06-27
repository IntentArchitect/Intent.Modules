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
    public partial class H_OptionalAggregateNavMapping : EntityTypeConfiguration<H_OptionalAggregateNav>
    {

        public H_OptionalAggregateNavMapping()
        {
            this.ToTable("H_OptionalAggregateNav");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasMany(x => x.H_MultipleDependents)
                .WithOptional(x => x.H_OptionalAggregateNav)
                .HasForeignKey(x => x.H_OptionalAggregateNavId)
                ;

        }
    }
}
