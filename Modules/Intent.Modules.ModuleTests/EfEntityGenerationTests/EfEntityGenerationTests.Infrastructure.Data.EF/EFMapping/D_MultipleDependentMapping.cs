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
    public partial class D_MultipleDependentMapping : EntityTypeConfiguration<D_MultipleDependent>
    {

        public D_MultipleDependentMapping()
        {
            this.ToTable("D_MultipleDependent");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
        }
    }
}