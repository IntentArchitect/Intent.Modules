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
    public partial class M_SelfReferenceBiNavMapping : EntityTypeConfiguration<M_SelfReferenceBiNav>
    {

        public M_SelfReferenceBiNavMapping()
        {
            this.ToTable("M_SelfReferenceBiNav");
            this.HasKey(x => x.Id);
            this.Property(x => x.Id).HasColumnName("Id");
            this.HasOptional(x => x.Self)
            .WithOptionalDependent(x => x.Original)
            ;
        }
    }
}
