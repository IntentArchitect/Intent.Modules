using Accelerators.Domain.Entities.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Accelerators.Infrastructure.Persistence.Configurations.DTOFieldSync
{
    public class ShouldNotSeeConfiguration : IEntityTypeConfiguration<ShouldNotSee>
    {
        public void Configure(EntityTypeBuilder<ShouldNotSee> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();
        }
    }
}