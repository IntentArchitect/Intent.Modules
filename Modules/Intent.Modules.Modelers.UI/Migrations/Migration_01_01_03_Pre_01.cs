using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.UI.Migrations
{
    public class Migration_01_01_03_Pre_01 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        private const string UiDesignerId = "f492faed-0665-4513-9853-5a230721786f";
        private const string ComponentTypeId = "b1c481e1-e91e-4c29-9817-00ab9cad4b6b";
        private const string PageStereotypeId = "ea4adc09-8978-4ede-ba5f-265debb2b60c";
        private const string DialogStereotypeId = "1f4165ee-41a0-4520-a193-9ae4d3413d1f";
        private const string ComposableStereotypeId = "5a2ba6fc-8512-4801-8b14-9d532c9c2616";

        public Migration_01_01_03_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.UI";
        [IntentFully]
        public string ModuleVersion => "1.1.3-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(UiDesignerId);
            if (designer == null) return;

            foreach (var package in designer.GetPackages())
            {
                var components = package.GetElementsOfType(ComponentTypeId);
                var needsSave = false;

                foreach (var component in components)
                {
                    if (component.Stereotypes.Any(s => s.DefinitionId == ComposableStereotypeId))
                        continue;
                    if (component.Stereotypes.Any(s => s.DefinitionId == PageStereotypeId || s.DefinitionId == DialogStereotypeId))
                        continue;

                    component.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = ComposableStereotypeId,
                        Name = "Composable",
                        Properties = []
                    });
                    needsSave = true;
                }

                if (needsSave)
                    package.Save(true);
            }
        }

        public void Down()
        {
        }
    }
}
