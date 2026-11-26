#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Common.CSharp.Migrations
{
    /// <summary>
    /// This version removed the <c>csharp-guidelines</c> AI instruction template. Updating the module does not remove
    /// its Template Output elements from the Codebase Structure designer, so this migration deletes them.
    /// </summary>
    public class Migration_03_11_01_Pre_01 : IModuleMigration
    {
        private const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";
        private const string TemplateOutputSpecializationId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
        private const string RemovedTemplateId = "Intent.Modules.Common.CSharp.Templates.StaticContentTemplateRegistrations.AIInstructionsStaticContentTemplateRegistration";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_11_01_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common.CSharp";
        [IntentFully]
        public string ModuleVersion => "3.11.1-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            // TryGetDesigner throws when the application has no such designer (one that uses the Folder Structure
            // designer instead), which failed the upgrade, so look it up instead.
            var designer = app.GetDesigners().SingleOrDefault(d => d.Id == CodebaseStructureDesignerId);
            if (designer is null)
            {
                return;
            }

            foreach (var package in designer.GetPackages())
            {
                var outputs = package.GetElementsOfType(TemplateOutputSpecializationId)
                    .Where(x => x.Name == RemovedTemplateId)
                    .ToArray();
                if (outputs.Length == 0)
                {
                    continue;
                }

                foreach (var output in outputs)
                {
                    var parent = output.ParentFolderId is null ? null : package.GetElementById(output.ParentFolderId);
                    if (parent is not null)
                    {
                        parent.ChildElements.Remove(output);
                    }
                    else
                    {
                        package.Classes.Remove(output);
                    }
                }

                package.Save(true);
            }
        }

        public void Down()
        {
        }
    }
}
