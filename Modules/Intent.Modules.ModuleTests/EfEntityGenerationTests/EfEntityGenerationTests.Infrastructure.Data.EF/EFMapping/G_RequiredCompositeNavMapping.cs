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
    public partial class G_RequiredCompositeNavMapping : EntityTypeConfiguration<G_RequiredCompositeNav>
    {

        public G_RequiredCompositeNavMapping()
        {
            this.ToTable("G_RequiredCompositeNav");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasMany(x => x.G_MultipleDependents)
                .WithRequired(x => x.G_RequiredCompositeNav)
                .HasForeignKey(x => x.G_RequiredCompositeNavId)
                .WillCascadeOnDelete()
                ;

        }
    }
}
