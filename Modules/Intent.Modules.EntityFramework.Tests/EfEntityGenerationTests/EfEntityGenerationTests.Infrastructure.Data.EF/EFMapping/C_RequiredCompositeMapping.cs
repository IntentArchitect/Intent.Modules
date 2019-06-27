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
    public partial class C_RequiredCompositeMapping : EntityTypeConfiguration<C_RequiredComposite>
    {

        public C_RequiredCompositeMapping()
        {
            this.ToTable("C_RequiredComposite");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasMany(x => x.C_MultipleDependents)
                .WithRequired(x => x.C_RequiredComposite)
                .HasForeignKey(x => x.C_RequiredCompositeId)
                .WillCascadeOnDelete()
                ;

        }
    }
}
