using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFramework.DbContext", Version = "1.0")]

namespace EfEntityGenerationTests.Infrastructure.Data.EF
{
    public class EfEntityGenerationTestsDbContext : DbContext
    {
        public EfEntityGenerationTestsDbContext() : base("EfEntityGenerationTestsDB")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureConventions(modelBuilder);

            modelBuilder.Configurations.Add(new B_OptionalDependentMapping());
            modelBuilder.Configurations.Add(new A_OptionalDependentMapping());
            modelBuilder.Configurations.Add(new D_OptionalAggregateMapping());
            modelBuilder.Configurations.Add(new D_MultipleDependentMapping());
            modelBuilder.Configurations.Add(new J_MultipleAggregateMapping());
            modelBuilder.Configurations.Add(new K_SelfReferenceMapping());
            modelBuilder.Configurations.Add(new M_SelfReferenceBiNavMapping());
            modelBuilder.Configurations.Add(new L_SelfReferenceMultipleMapping());
            modelBuilder.Configurations.Add(new E_RequiredDependentMapping());
            modelBuilder.Configurations.Add(new C_RequiredCompositeMapping());
            modelBuilder.Configurations.Add(new G_RequiredCompositeNavMapping());
            modelBuilder.Configurations.Add(new F_OptionalDependentMapping());
            modelBuilder.Configurations.Add(new A_RequiredCompositeMapping());
            modelBuilder.Configurations.Add(new F_OptionalAggregateNavMapping());
            modelBuilder.Configurations.Add(new C_MultipleDependentMapping());
            modelBuilder.Configurations.Add(new H_MultipleDependentMapping());
            modelBuilder.Configurations.Add(new H_OptionalAggregateNavMapping());
            modelBuilder.Configurations.Add(new G_MultipleDependentMapping());
            modelBuilder.Configurations.Add(new E_RequiredCompositeNavMapping());
            modelBuilder.Configurations.Add(new J_RequiredDependentMapping());
            modelBuilder.Configurations.Add(new B_OptionalAggregateMapping());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureConventions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Customize Default Schema
            // modelBuilder.HasDefaultSchema("EfEntityGenerationTests");
        }
    }
}