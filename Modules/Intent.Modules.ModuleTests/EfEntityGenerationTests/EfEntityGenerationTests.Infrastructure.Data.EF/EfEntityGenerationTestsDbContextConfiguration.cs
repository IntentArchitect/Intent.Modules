using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.IO;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.DbMigrationsConfiguration", Version = "1.0")]

namespace EfEntityGenerationTests.Infrastructure.Data.EF
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public sealed class EfEntityGenerationTestsDbContextConfiguration
        : DbMigrationsConfiguration<EfEntityGenerationTestsDbContext>
    {
        public EfEntityGenerationTestsDbContextConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EfEntityGenerationTestsDbContext context)
        {
            base.Seed(context);

            CustomSeed(context);
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        private void CustomSeed(EfEntityGenerationTestsDbContext dbContext)
        {
            // Put your seed data here
        }
    }
}