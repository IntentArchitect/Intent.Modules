using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;

namespace Intent.Modules.ApplicationTemplate.Builder.Migrations
{
    public class Migration_03_04_08_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;
        private const string AppTemplatesDesignerId = "22091d1e-a855-41af-ba7f-3f0b033c0fc3";
        private const string ComponentModuleElementId = "ef75f8f0-520c-4ab8-814f-5e75f4877dd7";

        public Migration_03_04_08_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string ModuleId => "Intent.ApplicationTemplate.Builder";
        public string ModuleVersion => "3.4.8-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(AppTemplatesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(ComponentModuleElementId);
                if (!elements.Any())
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    element.SpecializationType = "Component Module";
                }

                package.Save(true);
            }
        }

        public void Down()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(AppTemplatesDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                var elements = package.GetElementsOfType(ComponentModuleElementId);
                if (!elements.Any())
                {
                    continue;
                }

                foreach (var element in elements)
                {
                    element.SpecializationType = "Module";
                }

                package.Save(true);
            }
        }
    }
}
