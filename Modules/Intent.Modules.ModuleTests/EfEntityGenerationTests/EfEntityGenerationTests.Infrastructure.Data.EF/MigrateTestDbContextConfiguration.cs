using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.IO;
using Intent.RoslynWeaver.Attributes;
using System.Linq;
using Migrate.MigrateTest.Domain;
using System.Data.Entity;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.DbMigrationsConfiguration", Version = "1.0")]

namespace EfEntityGenerationTests.Infrastructure.Data.EF
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public sealed class MigrateTestDbContextConfiguration
        : DbMigrationsConfiguration<MigrateTestDbContext>
    {
        public MigrateTestDbContextConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MigrateTestDbContext context)
        {
            base.Seed(context);

            CustomSeed(context);
        }

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        private void CustomSeed(MigrateTestDbContext dbContext)
        {
            
        }
    }
}