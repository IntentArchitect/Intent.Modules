using System;
using System.Collections.Generic;
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
        public void Scenario01()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Common.Types.Country",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Intent.RoslynWeaver.Attributes",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Solution.Common.Types.Country", result);
        }

        [Fact]
        public void Scenario02()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Common.Types.Country",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System",
                    "System.Collections.Generic",
                    "System.Runtime.Serialization",
                    "Solution.Common.Types",
                    "Intent.RoslynWeaver.Attributes",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Country", result);
        }

        [Fact]
        public void Scenario03()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.ApplicationLayer",
                fullyQualifiedType: "Solution.Application.Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                    "System.Runtime.Serialization",
                    "AutoMapper",
                    "Intent.RoslynWeaver.Attributes",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Contracts.Internal.CompanyDetailsManagement.StatutoryInfoDTO", result);
        }

        [Fact]
        public void Scenario04()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Solution.Application.Contracts.Internal.CompanyDetailsManagement",
                fullyQualifiedType: "Solution.Application.Common.Enums.CompanyDetails.SocialMediaType",
                knownOtherPaths: ProjectNames,
                usingPaths: new string[]
                {
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Common.Enums.CompanyDetails.SocialMediaType", result);
        }

        [Fact]
        public void Scenario05()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "MyCompany.Movies.Api",
                fullyQualifiedType: "MyCompany.Movies.Application.Movies",
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
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Application.Movies", result);
        }

        [Fact]
        public void Scenario06()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.MediatR.Templates.QueryModel",
                fullyQualifiedType: "Intent.Modelers.Services.CQRS.Api.QueryModel",
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
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("Modelers.Services.CQRS.Api.QueryModel", result);
        }

        [Fact]
        public void Scenario07()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection",
                fullyQualifiedType: "Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection.DependencyInjectionDecorator",
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
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("DependencyInjectionDecorator", result);
        }

        [Fact]
        public void Scenario08()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Intent.Modules.Application",
                fullyQualifiedType: "Intent.Modules.Application.MyModel.MyModel",
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
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>());

            Assert.Equal("MyModel.MyModel", result);
        }

        [Fact]
        public void Scenario09()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Root",
                fullyQualifiedType: "Root.Sub1.TypeName",
                knownOtherPaths: new string[]
                {
                    "Root.Sub1.TypeName",
                    "Root.Sub2.TypeName"
                },
                usingPaths: new string[]
                {
                    "Root.Sub1",
                    "Root.Sub2"
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>
                {
                    ["Root.Sub1"] = new HashSet<string> { "TypeName" },
                    ["Root.Sub2"] = new HashSet<string> { "TypeName" }
                });

            Assert.Equal("Sub1.TypeName", result);
        }

        [Fact]
        public void Scenario10()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Namespace0",
                fullyQualifiedType: "Namespace1.TypeName",
                knownOtherPaths: new string[]
                {
                    "Namespace1.TypeName",
                },
                usingPaths: new string[]
                {
                    "Namespace1",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>()
                {
                    ["Namespace1.TypeName"] = new HashSet<string> { "TypeName" }
                });

            Assert.Equal("TypeName", result);
        }

        [Fact]
        public void Scenario11()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "NewApplication32.Domain.Repositories.Sec",
                fullyQualifiedType: "NewApplication32.Domain.Entities.Sec.Action",
                knownOtherPaths: Array.Empty<string>(),
                usingPaths: new string[]
                {
                    "System",
                    "NewApplication32.Domain.Entities.Sec"
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>() 
                    { 
                        [ "System" ] = new HashSet<string>() { "Action" }
                    });

            Assert.Equal("Entities.Sec.Action", result);
        }

        [Fact]
        public void Scenario12()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Customer1.Domain.Repositories.Mas",
                fullyQualifiedType: "Customer1.Domain.Entities.Mas.Log",
                knownOtherPaths: Array.Empty<string>(),
                usingPaths: new string[]
                {
                    "System",
                    "Customer1.Domain.Entities.Mas",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>()
                {
                    ["System"] = new HashSet<string>() { "Action" },
                    ["Customer1.Domain.Repositories.Log"] = new HashSet<string>() { "ILogRepository" }
                });

            Assert.Equal("Entities.Mas.Log", result);
        }

        [Fact]
        public void Scenario13()
        {
            var result = CSharpTemplateBase.NormalizeNamespace(
                localNamespace: "Customer1.Api",
                fullyQualifiedType: "Customer1.Domain.Conflict",
                knownOtherPaths: Array.Empty<string>(),
                usingPaths: new string[]
                {
                    "System",
                    "Customer1.Domain",
                },
                knownTypesByNamespace: new Dictionary<string, ISet<string>>()
                {
                    ["Customer1.Api"] = new HashSet<string>() { "Conflict" }
                });

            Assert.Equal("Domain.Conflict", result);
        }


    }
}
