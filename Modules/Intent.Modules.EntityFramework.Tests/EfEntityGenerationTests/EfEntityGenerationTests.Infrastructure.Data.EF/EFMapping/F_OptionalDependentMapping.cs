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
    public partial class F_OptionalDependentMapping : EntityTypeConfiguration<F_OptionalDependent>
    {

        public F_OptionalDependentMapping()
        {
            this.ToTable("F_OptionalDependent");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
        }
    }
}
