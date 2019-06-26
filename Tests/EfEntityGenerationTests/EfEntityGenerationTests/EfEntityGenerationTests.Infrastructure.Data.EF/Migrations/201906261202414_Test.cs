namespace EfEntityGenerationTests.Infrastructure.Data.EF
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.B_OptionalDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.A_OptionalDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.A_RequiredComposite", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.D_OptionalAggregate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.D_MultipleDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        D_OptionalAggregateId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.D_OptionalAggregate", t => t.D_OptionalAggregateId)
                .Index(t => t.D_OptionalAggregateId);
            
            CreateTable(
                "dbo.J_MultipleAggregate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        J_RequiredDependentId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.J_RequiredDependent", t => t.J_RequiredDependentId)
                .Index(t => t.J_RequiredDependentId);
            
            CreateTable(
                "dbo.J_RequiredDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.K_SelfReference",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SelfId = c.Guid(),
                        K_SelfReferenceId = c.Guid(),
                        Self_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.K_SelfReference", t => t.Self_Id)
                .Index(t => t.Self_Id);
            
            CreateTable(
                "dbo.M_SelfReferenceBiNav",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SelfId = c.Guid(),
                        OriginalId = c.Guid(),
                        Self_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.M_SelfReferenceBiNav", t => t.Self_Id)
                .Index(t => t.Self_Id);
            
            CreateTable(
                "dbo.L_SelfReferenceMultiple",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        L_SelfReferenceMultipleId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.L_SelfReferenceMultiple", t => t.L_SelfReferenceMultipleId)
                .Index(t => t.L_SelfReferenceMultipleId);
            
            CreateTable(
                "dbo.E_RequiredDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.E_RequiredCompositeNav", t => t.Id, cascadeDelete: true)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.E_RequiredCompositeNav",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.C_RequiredComposite",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.C_MultipleDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        C_RequiredCompositeId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.C_RequiredComposite", t => t.C_RequiredCompositeId, cascadeDelete: true)
                .Index(t => t.C_RequiredCompositeId);
            
            CreateTable(
                "dbo.G_RequiredCompositeNav",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.G_MultipleDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        G_RequiredCompositeNavId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.G_RequiredCompositeNav", t => t.G_RequiredCompositeNavId, cascadeDelete: true)
                .Index(t => t.G_RequiredCompositeNavId);
            
            CreateTable(
                "dbo.F_OptionalDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.F_OptionalAggregateNav",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        F_OptionalDependent_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.F_OptionalDependent", t => t.F_OptionalDependent_Id)
                .Index(t => t.F_OptionalDependent_Id);
            
            CreateTable(
                "dbo.A_RequiredComposite",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.H_MultipleDependent",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        H_OptionalAggregateNavId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.H_OptionalAggregateNav", t => t.H_OptionalAggregateNavId)
                .Index(t => t.H_OptionalAggregateNavId);
            
            CreateTable(
                "dbo.H_OptionalAggregateNav",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.B_OptionalAggregate",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        B_OptionalDependent_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.B_OptionalDependent", t => t.B_OptionalDependent_Id)
                .Index(t => t.B_OptionalDependent_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.B_OptionalAggregate", "B_OptionalDependent_Id", "dbo.B_OptionalDependent");
            DropForeignKey("dbo.H_MultipleDependent", "H_OptionalAggregateNavId", "dbo.H_OptionalAggregateNav");
            DropForeignKey("dbo.A_OptionalDependent", "Id", "dbo.A_RequiredComposite");
            DropForeignKey("dbo.F_OptionalAggregateNav", "F_OptionalDependent_Id", "dbo.F_OptionalDependent");
            DropForeignKey("dbo.G_MultipleDependent", "G_RequiredCompositeNavId", "dbo.G_RequiredCompositeNav");
            DropForeignKey("dbo.C_MultipleDependent", "C_RequiredCompositeId", "dbo.C_RequiredComposite");
            DropForeignKey("dbo.E_RequiredDependent", "Id", "dbo.E_RequiredCompositeNav");
            DropForeignKey("dbo.L_SelfReferenceMultiple", "L_SelfReferenceMultipleId", "dbo.L_SelfReferenceMultiple");
            DropForeignKey("dbo.M_SelfReferenceBiNav", "Self_Id", "dbo.M_SelfReferenceBiNav");
            DropForeignKey("dbo.K_SelfReference", "Self_Id", "dbo.K_SelfReference");
            DropForeignKey("dbo.J_MultipleAggregate", "J_RequiredDependentId", "dbo.J_RequiredDependent");
            DropForeignKey("dbo.D_MultipleDependent", "D_OptionalAggregateId", "dbo.D_OptionalAggregate");
            DropIndex("dbo.B_OptionalAggregate", new[] { "B_OptionalDependent_Id" });
            DropIndex("dbo.H_MultipleDependent", new[] { "H_OptionalAggregateNavId" });
            DropIndex("dbo.F_OptionalAggregateNav", new[] { "F_OptionalDependent_Id" });
            DropIndex("dbo.G_MultipleDependent", new[] { "G_RequiredCompositeNavId" });
            DropIndex("dbo.C_MultipleDependent", new[] { "C_RequiredCompositeId" });
            DropIndex("dbo.E_RequiredDependent", new[] { "Id" });
            DropIndex("dbo.L_SelfReferenceMultiple", new[] { "L_SelfReferenceMultipleId" });
            DropIndex("dbo.M_SelfReferenceBiNav", new[] { "Self_Id" });
            DropIndex("dbo.K_SelfReference", new[] { "Self_Id" });
            DropIndex("dbo.J_MultipleAggregate", new[] { "J_RequiredDependentId" });
            DropIndex("dbo.D_MultipleDependent", new[] { "D_OptionalAggregateId" });
            DropIndex("dbo.A_OptionalDependent", new[] { "Id" });
            DropTable("dbo.B_OptionalAggregate");
            DropTable("dbo.H_OptionalAggregateNav");
            DropTable("dbo.H_MultipleDependent");
            DropTable("dbo.A_RequiredComposite");
            DropTable("dbo.F_OptionalAggregateNav");
            DropTable("dbo.F_OptionalDependent");
            DropTable("dbo.G_MultipleDependent");
            DropTable("dbo.G_RequiredCompositeNav");
            DropTable("dbo.C_MultipleDependent");
            DropTable("dbo.C_RequiredComposite");
            DropTable("dbo.E_RequiredCompositeNav");
            DropTable("dbo.E_RequiredDependent");
            DropTable("dbo.L_SelfReferenceMultiple");
            DropTable("dbo.M_SelfReferenceBiNav");
            DropTable("dbo.K_SelfReference");
            DropTable("dbo.J_RequiredDependent");
            DropTable("dbo.J_MultipleAggregate");
            DropTable("dbo.D_MultipleDependent");
            DropTable("dbo.D_OptionalAggregate");
            DropTable("dbo.A_OptionalDependent");
            DropTable("dbo.B_OptionalDependent");
        }
    }
}
