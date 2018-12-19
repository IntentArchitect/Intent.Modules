using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCompany.MyMovies.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EFMapping", Version = "1.0")]

namespace MyCompany.MyMovies.Infrastructure.Data
{
    public class MovieMapping : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movie", "dbo");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.Title)
                .IsRequired();

            builder.Property(x => x.ReleaseDate)
                .IsRequired();

            builder.Property(x => x.Genre)
                .IsRequired();

            builder.Property(x => x.Price)
                .IsRequired();

        }
    }
}