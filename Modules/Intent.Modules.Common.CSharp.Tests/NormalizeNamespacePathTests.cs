using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Xunit;

namespace Intent.Modules.Common.Tests
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
        public void Scenario1()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                foreignType: "Solution.Common.Types.Country",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Intent.RoslynWeaver.Attributes",
                });

            Assert.Equal("Solution.Common.Types.Country", result);
        }

        [Fact]
        public void Scenario2()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                foreignType: "Solution.Common.Types.Country",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Solution.Common.Types",
                    "Intent.RoslynWeaver.Attributes",
                });

            Assert.Equal("Country", result);
        }

        [Fact]
        public void Scenario3()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.ApplicationLayer",
                foreignType: "Solution.Application.Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System.Runtime.Serialization",
                    "AutoMapper",
                    "Intent.RoslynWeaver.Attributes",
                });

            Assert.Equal("Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO", result);
        }

        [Fact]
        public void Scenario4()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                foreignType: "Solution.Application.Common.Enums.CompanyDetails.SocialMediaType",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                });

            Assert.Equal("Common.Enums.CompanyDetails.SocialMediaType", result);
        }

        [Fact]
        public void Scenario5()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "MyCompany.Movies.Api",
                foreignType: "MyCompany.Movies.Application.Movies",
                knownOtherPaths: new string[]
                {
                    "MyCompany.Movies.Infrastructure.Data",
                    "MyCompany.Movies.Application",
                    "MyCompany.Movies.Application.ServiceCallHandlers.Movies",
                    "MyCompany.Movies.Domain"
                },
                usingPaths: new string[]
                {
                    "MyCompany.Movies.Infrastructure.Data",
                    "MyCompany.Movies.Application",
                    "MyCompany.Movies.Application.ServiceCallHandlers.Movies",
                    "MyCompany.Movies.Domain"
                });

            Assert.Equal("Application.Movies", result);
        }

        [Fact]
        public void Scenario6()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.MediatR.Templates.QueryModel",
                foreignType: "Intent.Modelers.Services.CQRS.Api.QueryModel",
                knownOtherPaths: new string[]
                {
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modelers.Services.CQRS.Api",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates"
                },
                usingPaths: new string[]
                {
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modelers.Services.CQRS.Api",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates"
                });

            Assert.Equal("Modelers.Services.CQRS.Api.QueryModel", result);
        }

        [Fact]
        public void Scenario7()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection",
                foreignType: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection.DependencyInjectionDecorator",
                knownOtherPaths: new string[]
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
                usingPaths: new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "Intent.Engine",
                    "Intent.Modules.Common",
                    "Intent.Modules.Common.CSharp.Templates",
                    "Intent.Modules.Common.Templates",
                    "Intent.RoslynWeaver.Attributes",
                    "Intent.Templates",
                });

            Assert.Equal("DependencyInjectionDecorator", result);
        }
    }
}
