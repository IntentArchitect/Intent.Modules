using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Metadata.Security.Models.Constants.Stereotypes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.Security.Migrations
{
    public class Migration_01_00_03_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_01_00_03_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.Security";
        [IntentFully]
        public string ModuleVersion => "1.0.3-pre.0";

        public void Up()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
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

                package.Save();
            }

        }

        private static void ConvertPackagePoliciesUp(IPackageModelPersistable package)
        {
            // each Secured stereotype
            foreach (var stereo in package.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
            {
                ConvertPackageLegacyPoliciesUp(package, stereo);
                ConvertPackageSecurityPoliciesUp(package, stereo);
            }
        }

        private static void ConvertElementPoliciesUp(IEnumerable<IElementPersistable> elements)
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

        private static void ConvertElementLegacyPoliciesUp(IElementPersistable element, IStereotypePersistable stereo)
        {
            var legacyPolicy = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.TextPolicy);

            if (legacyPolicy is null)
            {
                return;
            }

            var policies = legacyPolicy?.Value?.Split(",");

            legacyPolicy.Value = !string.IsNullOrWhiteSpace(legacyPolicy?.Value) ? policies.First() : null;

            if (policies != null && policies.Length > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    element.Stereotypes.Add(
                        stereo.DefinitionId,
                        stereo.Name,
                        stereo.DefinitionPackageId,
                        stereo.DefinitionPackageName,
                        c => c.Properties.Add(legacyPolicy.DefinitionId, legacyPolicy.Name, policy, x => x.IsActive = legacyPolicy.IsActive)
                        );
                }
            }
        }

        private static void ConvertPackageLegacyPoliciesUp(IPackageModelPersistable package, IStereotypePersistable stereo)
        {
            var legacyPolicy = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.TextPolicy);

            if (legacyPolicy is null)
            {
                return;
            }

            var policies = legacyPolicy?.Value?.Split(",");

            legacyPolicy.Value = !string.IsNullOrWhiteSpace(legacyPolicy?.Value) ? policies.First() : null;

            if (policies != null && policies.Length > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    package.Stereotypes.Add(
                        stereo.DefinitionId,
                        stereo.Name,
                        stereo.DefinitionPackageId,
                        stereo.DefinitionPackageName,
                        c => c.Properties.Add(legacyPolicy.DefinitionId, legacyPolicy.Name, policy, x => x.IsActive = legacyPolicy.IsActive)
                        );
                }
            }
        }

        private static void ConvertElementSecurityPoliciesUp(IElementPersistable element, IStereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            if (secPoliciesProp is null)
            {
                return;
            }

            var policies = secPoliciesProp != null && !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? JsonSerializer.Deserialize<List<string>>(secPoliciesProp.Value)
            : [];

            secPoliciesProp.Value = (policies != null && policies.Count != 0) ? policies.First() : null;

            if (policies != null && policies.Count > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    element.Stereotypes.Add(
                        stereo.DefinitionId,
                        stereo.Name,
                        stereo.DefinitionPackageId,
                        stereo.DefinitionPackageName,
                        c => c.Properties.Add(secPoliciesProp.DefinitionId, secPoliciesProp.Name, policy, x => x.IsActive = secPoliciesProp.IsActive)
                        );
                }
            }
        }

        private static void ConvertPackageSecurityPoliciesUp(IPackageModelPersistable package, IStereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            if (secPoliciesProp is null)
            {
                return;
            }

            var policies = secPoliciesProp != null && !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? JsonSerializer.Deserialize<List<string>>(secPoliciesProp.Value)
            : [];

            secPoliciesProp.Value = (policies != null && policies.Count != 0) ? policies.First() : null;


            if (policies != null && policies.Count > 1)
            {
                foreach (var policy in policies.Skip(1))
                {
                    package.Stereotypes.Add(
                        stereo.DefinitionId,
                        stereo.Name,
                        stereo.DefinitionPackageId,
                        stereo.DefinitionPackageName,
                        c => c.Properties.Add(secPoliciesProp.DefinitionId, secPoliciesProp.Name, policy, x => x.IsActive = secPoliciesProp.IsActive)
                        );
                }
            }
        }

        public void Down()
        {
            var app = _persistenceLoader.LoadCurrentApplication();
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

                package.Save();
            }

        }

        private static void ConvertPackagePoliciesDown(IPackageModelPersistable package)
        {
            // each Secured stereotype
            foreach (var stereo in package.Stereotypes.Where(s => s.DefinitionId == Secured.Id).ToList())
            {
                ConvertElementSecurityPoliciesDown(stereo);
            }
        }

        private static void ConvertElementPoliciesDown(IEnumerable<IElementPersistable> elements)
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

        private static void ConvertElementSecurityPoliciesDown(IStereotypePersistable stereo)
        {
            var secPoliciesProp = stereo.Properties.FirstOrDefault(p => p.DefinitionId == Secured.Properties.Policies);

            if (secPoliciesProp == null)
            {
                return;
            }

            var policies = !string.IsNullOrWhiteSpace(secPoliciesProp.Value)
            ? [secPoliciesProp.Value]
            : Array.Empty<string>();

            secPoliciesProp.Value = (policies != null && policies.Length != 0) ? JsonSerializer.Serialize(policies) : null;
        }

    }
}