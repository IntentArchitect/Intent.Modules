using Accelerators.Domain.DTOFieldSync;
using Accelerators.Domain.Entities.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace Accelerators.Infrastructure.Persistence.Configurations.DTOFieldSync
{
    public class Block2Level1Configuration : IEntityTypeConfiguration<Block2Level1>
    {
        public void Configure(EntityTypeBuilder<Block2Level1> builder)
        {
            builder.HasKey(x => x.BlockId);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Added)
                .IsRequired();

            builder.Property(x => x.Renamed)
                .IsRequired();

            builder.Property(x => x.TestEnum)
                .IsRequired();

            builder.Property(x => x.ShouldNotSeeId)
                .IsRequired();

            builder.OwnsOne(x => x.Money, ConfigureMoney)
                .Navigation(x => x.Money).IsRequired();

            builder.OwnsMany(x => x.Block2Level2s, ConfigureBlock2Level2s);

            builder.HasOne(x => x.ShouldNotSee)
                .WithMany()
                .HasForeignKey(x => x.ShouldNotSeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsMany(x => x.NewBlockForBlock2s, ConfigureNewBlockForBlock2s);
        }

        public static void ConfigureMoney(OwnedNavigationBuilder<Block2Level1, Money> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }

        public static void ConfigureMoney(OwnedNavigationBuilder<Block2Level2, Money> builder)
        {
            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }

        public static void ConfigureMoney(OwnedNavigationBuilder<Block2Level3, Money> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }

        public static void ConfigureBlock2Level2s(OwnedNavigationBuilder<Block2Level1, Block2Level2> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Block2Level1Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Block2Level1Id)
                .IsRequired();

            builder.Property(x => x.Added)
                .IsRequired();

            builder.Property(x => x.Renamed)
                .IsRequired();

            builder.Property(x => x.TestEnum)
                .IsRequired();

            builder.Property(x => x.ShouldNotSeeId)
                .IsRequired();

            builder.OwnsOne(x => x.Money, ConfigureMoney)
                .Navigation(x => x.Money).IsRequired();

            builder.OwnsOne(x => x.Block2Level3, ConfigureBlock2Level3)
                .Navigation(x => x.Block2Level3).IsRequired();

            builder.HasOne(x => x.ShouldNotSee)
                .WithMany()
                .HasForeignKey(x => x.ShouldNotSeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigureBlock2Level3(OwnedNavigationBuilder<Block2Level2, Block2Level3> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Added)
                .IsRequired();

            builder.Property(x => x.Renamed)
                .IsRequired();

            builder.Property(x => x.TestEnum)
                .IsRequired();

            builder.Property(x => x.ShouldNotSeeId)
                .IsRequired();

            builder.HasOne(x => x.ShouldNotSee)
                .WithMany()
                .HasForeignKey(x => x.ShouldNotSeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.OwnsOne(x => x.Money, ConfigureMoney)
                .Navigation(x => x.Money).IsRequired();

            builder.OwnsMany(x => x.Monies, ConfigureMonies);
        }

        public static void ConfigureMonies(OwnedNavigationBuilder<Block2Level3, Money> builder)
        {
            builder.WithOwner();

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Currency)
                .IsRequired();
        }

        public static void ConfigureNewBlockForBlock2s(OwnedNavigationBuilder<Block2Level1, NewBlockForBlock2> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.Block2Level1Id);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Block2Level1Id)
                .IsRequired();
        }
    }
}