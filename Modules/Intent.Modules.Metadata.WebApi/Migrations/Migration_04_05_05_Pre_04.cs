using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Metadata.WebApi.Migrations
{
    public class Migration_04_05_05_Pre_04 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string ServicesDesignerId = "81104ae6-2bc5-4bae-b05a-f987b0372d81";
        private const string OpenApiStereotypeId = "b6197544-7e0e-4900-a6e2-9747fb7e4ac4";

        public Migration_04_05_05_Pre_04(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Metadata.WebApi";
        [IntentFully]
        public string ModuleVersion => "4.5.5-pre.4";

        public void Up()
        {
            // On Elements that have the Open API stereotype, ensure that the OperationId field
            // is populated with the "{MethodName}" tag

            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ServicesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var needsSave = false;

                foreach (var element in GetAllElements(package))
                {
                    var openApiStereotype = element.Stereotypes.FirstOrDefault(x => x.DefinitionId == OpenApiStereotypeId);
                    if (openApiStereotype == null)
                    {
                        continue;
                    }

                    var property = openApiStereotype.Properties.Single(x => x.Name == "OperationId");
                    if (!string.IsNullOrWhiteSpace(property.Value))
                    {
                        continue;
                    }

                    property.Value = "{MethodName}";
                    needsSave = true;
                }

                if (needsSave)
                {
                    package.Save(true);
                }
            }
        }

        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ServicesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var needsSave = false;

                foreach (var element in GetAllElements(package))
                {
                    var openApiStereotype = element.Stereotypes.FirstOrDefault(x => x.DefinitionId == OpenApiStereotypeId);
                    if (openApiStereotype == null)
                    {
                        continue;
                    }

                    var property = openApiStereotype.Properties.Single(x => x.Name == "OperationId");
                    if (property.Value != "{MethodName}")
                    {
                        continue;
                    }

                    property.Value = null;
                    needsSave = true;
                }

                if (needsSave)
                {
                    package.Save(true);
                }
            }
        }

        private static IEnumerable<ElementPersistable> GetAllElements(PackageModelPersistable package)
        {
            return package.ChildElements.SelectMany(GetAllElements);

            static IEnumerable<ElementPersistable> GetAllElements(ElementPersistable element)
            {
                yield return element;

                foreach (var descendentElement in element.ChildElements.SelectMany(GetAllElements))
                {
                    yield return descendentElement;
                }
            }
        }
    }
}
