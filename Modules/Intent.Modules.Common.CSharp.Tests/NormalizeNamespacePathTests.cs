using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Templates;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests
{
    public class NormalizeNamespacePathTests
    {
        private static readonly string[] ProjectNames =
        {
            "Solution.Application.ApplicationLayer",
            "Solution.Application.ApplicationLayer.Identity",
            "Solution.Application.Common",
            "Solution.Application.Contracts.Internal",
            "Solution.Application.Distribution.Web",
            "Solution.Application.Domain",
            "Solution.Application.Domain.TestData",
            "Solution.Application.Domain.UnitTests",
            "Solution.Application.Infrastructure.Data",
            "Solution.Application.Infrastructure.Data.EF",
            "Solution.Application.Infrastructure.Data.EF.Migrations",
            "Solution.Application.Presentation.Web",
            "Solution.Common",
            "Solution.Messages",
        };

        [Fact]
        public void Scenario01()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Common.Types.Country",
                knownOtherNamespaceNames: ProjectNames,
                usingPaths: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Intent.RoslynWeaver.Attributes",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Common.Types.Country", result);
        }

        [Fact]
        public void Scenario02()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Common.Types.Country",
                knownOtherNamespaceNames: ProjectNames,
                usingPaths: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Solution.Common.Types",
                    "Intent.RoslynWeaver.Attributes",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Country", result);
        }

        [Fact]
        public void Scenario03()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.ApplicationLayer",
                fullyQualifiedType: "Solution.Application.Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO",
                knownOtherNamespaceNames: ProjectNames,
                usingPaths: new[]
                {
                    "System.Runtime.Serialization",
                    "AutoMapper",
                    "Intent.RoslynWeaver.Attributes",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO", result);
        }

        [Fact]
        public void Scenario04()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Application.Common.Enums.CompanyDetails.SocialMediaType",
                knownOtherNamespaceNames: ProjectNames,
                usingPaths: Array.Empty<string>(),
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Common.Enums.CompanyDetails.SocialMediaType", result);
        }

        [Fact]
        public void Scenario05()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "MyCompany.Movies.Api",
                fullyQualifiedType: "MyCompany.Movies.Application.Movies",
                knownOtherNamespaceNames: new[]
                {
                    "MyCompany.Movies.Infrastructure.Data",
                    "MyCompany.Movies.Application",
                    "MyCompany.Movies.Application.ServiceCallHandlers.Movies",
                    "MyCompany.Movies.Domain"
                },
                usingPaths: new[]
                {
                    "MyCompany.Movies.Infrastructure.Data",
                    "MyCompany.Movies.Application",
                    "MyCompany.Movies.Application.ServiceCallHandlers.Movies",
                    "MyCompany.Movies.Domain"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Application.Movies", result);
        }

        [Fact]
        public void Scenario06()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.MediatR.Templates.QueryModel",
                fullyQualifiedType: "Intent.Modelers.Services.CQRS.Api.QueryModel",
                knownOtherNamespaceNames: new[]
                {
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modelers.Services.CQRS.Api",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates"
                },
                usingPaths: new[]
                {
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modelers.Services.CQRS.Api",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("Modelers.Services.CQRS.Api.QueryModel", result);
        }

        [Fact]
        public void Scenario07()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection",
                fullyQualifiedType: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection.DependencyInjectionDecorator",
                knownOtherNamespaceNames: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modules.Common",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates",
                },
                usingPaths: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modules.Common",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("DependencyInjectionDecorator", result);
        }

        [Fact]
        public void Scenario08()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application",
                fullyQualifiedType: "Intent.Modules.Application.MyModel.MyModel",
                knownOtherNamespaceNames: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modules.Common",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates",
                },
                usingPaths: new[]
                {
                    "System",
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modules.Common",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("MyModel.MyModel", result);
        }

        [Fact]
        public void Scenario09()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Root",
                fullyQualifiedType: "Root.Sub1.TypeName",
                knownOtherNamespaceNames: new[]
                {
                    "Root.Sub1.TypeName",
                    "Root.Sub2.TypeName"
                },
                usingPaths: new[]
                {
                    "Root.Sub1",
                    "Root.Sub2"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "Root.Sub1.TypeName",
                    "Root.Sub2.TypeName"
                }));

            Assert.Equal("Sub1.TypeName", result);
        }

        [Fact]
        public void Scenario10()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Namespace0",
                fullyQualifiedType: "Namespace1.TypeName",
                knownOtherNamespaceNames: new[]
                {
                    "Namespace1.TypeName",
                },
                usingPaths: new[]
                {
                    "Namespace1",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("TypeName", result);
        }

        [Fact]
        public void Scenario11()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "NewApplication32.Domain.Repositories.Sec",
                fullyQualifiedType: "NewApplication32.Domain.Entities.Sec.Action",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "System",
                    "NewApplication32.Domain.Entities.Sec"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "System.Action"
                }));

            Assert.Equal("Entities.Sec.Action", result);
        }

        [Fact]
        public void Scenario12()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Customer1.Domain.Repositories.Mas",
                fullyQualifiedType: "Customer1.Domain.Entities.Mas.Log",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "Customer1.Domain.Entities.Mas"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "Customer1.Domain.Repositories.Log.ILogRepository"
                }));

            Assert.Equal("Entities.Mas.Log", result);
        }

        [Fact]
        public void Scenario13()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Customer1.Api",
                fullyQualifiedType: "Customer1.Domain.Conflict",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "System",
                    "Customer1.Domain",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "Customer1.Api.Conflict"
                }));

            Assert.Equal("Domain.Conflict", result);
        }

        [Fact]
        public void Scenario14()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "ApplicationName.Application.Invoices.CreateInvoice",
                fullyQualifiedType: "ApplicationName.Domain.Entities.Invoice",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "ApplicationName.Domain.Entities"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "ApplicationName.Application.Invoices.Invoice"
                }));

            Assert.Equal("Domain.Entities.Invoice", result);
        }

        [Fact]
        public void Scenario15()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "ApplicationName.Application.Invoices.CreateInvoice",
                fullyQualifiedType: "ApplicationName.Application.Invoices.Invoice",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: Array.Empty<string>(),
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "ApplicationName.Application.Invoices.Invoice"
                }));

            Assert.Equal("Invoice", result);
        }

        [Fact]
        public void Scenario16()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "ApplicationName.Domain.Repositories",
                fullyQualifiedType: "ApplicationName.Domain.Entities.Invoices",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "ApplicationName.Domain.Entities"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(new[]
                {
                    "ApplicationName.Application.Invoices.TypeName"
                }));

            Assert.Equal("Invoices", result);
        }

        [Fact]
        public void Scenario17()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Api.Controllers",
                fullyQualifiedType: "Solution.Application.Orders.OrderConfirmed.OrderConfirmed",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "Solution.Application.Orders",
                    "Solution.Application.Orders.OrderConfirmed"
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            Assert.Equal("OrderConfirmed", result);
        }

        [Fact]
        public void Scenario18()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Validators.Customers.GetPagedWithParameters",
                fullyQualifiedType: "Solution.Application.Customers.GetPagedWithParameters.Something.GetPagedWithParameters",
                knownOtherNamespaceNames: Array.Empty<string>(),
                usingPaths: new[]
                {
                    "Solution.Application.Customers.GetPagedWithParameters.Something",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

            result.ShouldBe("Application.Customers.GetPagedWithParameters.Something.GetPagedWithParameters");
        }

		[Fact]
		public void Scenario19()
		{
			var result = CSharpTemplateBase.NormalizeNamespace(
				localNamespace: "MyProject.Entities",
				fullyQualifiedType: "MyProject.Entities",
				knownOtherNamespaceNames: Array.Empty<string>(),
				usingPaths: Array.Empty<string>(),
				outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
				knownTypes: new TypeRegistry(Enumerable.Empty<string>()));

			result.ShouldBe("MyProject.Entities");
		}

        [Fact]
        public void Scenario20NestedClass()
        {
            var knownTypes = new TypeRegistry(Enumerable.Empty<string>());
            knownTypes.Add("Namespace0.Sub", "Class1");
            knownTypes.Add("Namespace0.Sub", "Class1.ChildClass");
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Namespace0.Hello",
                fullyQualifiedType: "Namespace0.Sub.Class1.ChildClass",
                knownOtherNamespaceNames: new[]
                {
                    "Namespace1.TypeName",
                },
                usingPaths: new[]
                {
                    "Namespace1",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: knownTypes);

            Assert.Equal("Sub.Class1.ChildClass", result);
        }
        [Fact]
        public void Scenario21NestedClass()
        {
            var knownTypes = new TypeRegistry(Enumerable.Empty<string>());
            knownTypes.Add("Namespace0.Sub", "Class1");
            knownTypes.Add("Namespace0.Sub", "Class1.ChildClass");
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Namespace0.Sub",
                fullyQualifiedType: "Namespace0.Sub.Class1.ChildClass",
                knownOtherNamespaceNames: new[]
                {
                    "Namespace1.TypeName",
                },
                usingPaths: new[]
                {
                    "Namespace1",
                },
                outputTargetNames: new TypeRegistry(Enumerable.Empty<string>()),
                knownTypes: knownTypes);

            Assert.Equal("Class1.ChildClass", result);
        }

    }
}
