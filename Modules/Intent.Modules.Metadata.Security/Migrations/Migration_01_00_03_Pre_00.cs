using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.Services.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Metadata.Security.Models.Constants.Stereotypes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.Security.Migrations
{
    public class Migration_01_00_03_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_00_03_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.Security";
        [IntentFully]
        public string ModuleVersion => "1.0.3-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.ServicesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                ConvertPackagePoliciesUp(package);

                var commands = package.GetElementsOfType("ccf14eb6-3a55-4d81-b5b9-d27311c70cb9");
                ConvertElementPoliciesUp(commands);

                var queries = package.GetElementsOfType("e71b0662-e29d-4db2-868b-8a12464b25d0");
                ConvertElementPoliciesUp(queries);

                var services = package.GetElementsOfType("b16578a5-27b1-4047-a8df-f0b783d706bd");
                ConvertElementPoliciesUp(services);

                foreach (var service in services)
                {
                    var operations = service.ChildElements.Where(c => c.SpecializationTypeId == "e030c97a-e066-40a7-8188-808c275df3cb").ToList().AsReadOnly();
                    ConvertElementPoliciesUp(operations);
                }

                package.Save(true);
            }

        }

        private static void ConvertPackagePoliciesUp(PackageModelPersistable package)
        {
            // each Secured stereotype
            foreach (var stereo in package.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
            {
                ConvertPackageLegacyPoliciesUp(package, stereo);
                ConvertPackageSecurityPoliciesUp(package, stereo);
            }
        }

        private static void ConvertElementPoliciesUp(IReadOnlyCollection<ElementPersistable> elements)
        {
            // each command
            foreach (var element in elements.Where(c => c.Stereotypes.Any(s => s.DefinitionId == Secured.Id)))
            {
                // each Secured stereotype
                foreach (var stereo in element.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
                {
                    ConvertElementSecurityPoliciesUp(element, stereo);
                    ConvertElementLegacyPoliciesUp(element, stereo);
                }
            }
        }

        //----

        private static void ConvertElementLegacyPoliciesUp(ElementPersistable element, StereotypePersistable stereo)
        {
            var legacyPolicy = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.TextPolicy);

            var policies = legacyPolicy?.Value?.Split(",");

            legacyPolicy.Value = !string.IsNullOrWhiteSpace(legacyPolicy.Value) ? policies.First() : null;

            if (policies != null && policies.Length > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    element.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = stereo.DefinitionId,
                        Name = stereo.Name,
                        DefinitionPackageId = stereo.DefinitionPackageId,
                        DefinitionPackageName = stereo.DefinitionPackageName,
                        Properties =
                        [
                            new StereotypePropertyPersistable
                                        {
                                            Name = legacyPolicy.Name,
                                            DefinitionId = legacyPolicy.DefinitionId,
                                            IsActive = legacyPolicy.IsActive,
                                            Value = policy
                                        }
                        ]
                    });
                }
            }
        }

        private static void ConvertPackageLegacyPoliciesUp(PackageModelPersistable package, StereotypePersistable stereo)
        {
            var legacyPolicy = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.TextPolicy);

            var policies = legacyPolicy?.Value?.Split(",");

            legacyPolicy.Value = !string.IsNullOrWhiteSpace(legacyPolicy.Value) ? policies.First() : null;

            if (policies != null && policies.Length > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    package.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = stereo.DefinitionId,
                        Name = stereo.Name,
                        DefinitionPackageId = stereo.DefinitionPackageId,
                        DefinitionPackageName = stereo.DefinitionPackageName,
                        Properties =
                        [
                            new StereotypePropertyPersistable
                                        {
                                            Name = legacyPolicy.Name,
                                            DefinitionId = legacyPolicy.DefinitionId,
                                            IsActive = legacyPolicy.IsActive,
                                            Value = policy
                                        }
                        ]
                    });
                }
            }
        }

        private static void ConvertElementSecurityPoliciesUp(ElementPersistable element, StereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            var policies = !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? JsonSerializer.Deserialize<List<string>>(secPoliciesProp.Value)
            : [];

            secPoliciesProp.Value = policies.Any() ? policies.First() : null;

            if (policies.Count > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    element.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = stereo.DefinitionId,
                        Name = stereo.Name,
                        DefinitionPackageId = stereo.DefinitionPackageId,
                        DefinitionPackageName = stereo.DefinitionPackageName,
                        Properties =
                        [
                            new StereotypePropertyPersistable
                                        {
                                            Name = secPoliciesProp.Name,
                                            DefinitionId = secPoliciesProp.DefinitionId,
                                            IsActive = secPoliciesProp.IsActive,
                                            Value = policy
                                        }
                        ]
                    });
                }
            }
        }

        private static void ConvertPackageSecurityPoliciesUp(PackageModelPersistable package, StereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            var policies = !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? JsonSerializer.Deserialize<List<string>>(secPoliciesProp.Value)
            : [];

            secPoliciesProp.Value = policies.Any() ? policies.First() : null;

            if (policies.Count > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    package.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = stereo.DefinitionId,
                        Name = stereo.Name,
                        DefinitionPackageId = stereo.DefinitionPackageId,
                        DefinitionPackageName = stereo.DefinitionPackageName,
                        Properties =
                        [
                            new StereotypePropertyPersistable
                                        {
                                            Name = secPoliciesProp.Name,
                                            DefinitionId = secPoliciesProp.DefinitionId,
                                            IsActive = secPoliciesProp.IsActive,
                                            Value = policy
                                        }
                        ]
                    });
                }
            }
        }

        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.ServicesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                ConvertPackagePoliciesDown(package);

                var commands = package.GetElementsOfType("ccf14eb6-3a55-4d81-b5b9-d27311c70cb9");
                ConvertElementPoliciesDown(commands);

                var queries = package.GetElementsOfType("e71b0662-e29d-4db2-868b-8a12464b25d0");
                ConvertElementPoliciesDown(queries);

                var services = package.GetElementsOfType("b16578a5-27b1-4047-a8df-f0b783d706bd");
                ConvertElementPoliciesDown(services);

                foreach (var service in services)
                {
                    var operations = service.ChildElements.Where(c => c.SpecializationTypeId == "e030c97a-e066-40a7-8188-808c275df3cb").ToList().AsReadOnly();
                    ConvertElementPoliciesDown(operations);
                }

                package.Save(true);
            }

        }

        private static void ConvertPackagePoliciesDown(PackageModelPersistable package)
        {
            // each Secured stereotype
            foreach (var stereo in package.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
            {
                ConvertElementSecurityPoliciesDown(stereo);
            }
        }

        private static void ConvertElementPoliciesDown(IReadOnlyCollection<ElementPersistable> elements)
        {
            // each command
            foreach (var element in elements.Where(c => c.Stereotypes.Any(s => s.DefinitionId == Secured.Id)))
            {
                // each Secured stereotype
                foreach (var stereo in element.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
                {
                    ConvertElementSecurityPoliciesDown(stereo);
                }
            }
        }

        //---

        private static void ConvertElementSecurityPoliciesDown(StereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            if (secPoliciesProp == null)
            {
                return;
            }

            var policies = !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? [secPoliciesProp.Value]
            : Array.Empty<string>();

            secPoliciesProp.Value = policies.Any() ? JsonSerializer.Serialize(policies) : null;
        }

    }
}