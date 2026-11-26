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
    /// Removes the Template Output elements left behind by the removed <c>csharp-guidelines</c> AI instruction
    /// template. <see cref="Migration_03_11_01_Pre_01"/> only covered the Codebase Structure designer, but an
    /// application can place the output in a Folder Structure designer instead, so this migration covers both.
    /// A Codebase Structure designer that 3.11.1-pre.1 already cleaned has nothing left to remove.
    /// </summary>
    public class Migration_03_11_01_Pre_02 : IModuleMigration
    {
        private const string RemovedTemplateId = "Intent.Modules.Common.CSharp.Templates.StaticContentTemplateRegistrations.AIInstructionsStaticContentTemplateRegistration";

        // Designer id, and the id of the Template Output specialization in that designer.
        private static readonly (string DesignerId, string TemplateOutputSpecializationId)[] Targets =
        {
            ("0701433c-36c0-4569-b1f4-9204986b587d", "d421c322-7a51-4094-89fa-e5d8a0a97b27"), // Codebase Structure
            ("27916ede-58c6-4356-87b1-0b0628d1e2a7", "09de2192-2507-41a2-8044-286c7ecadec2"), // Folder Structure
        };

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_11_01_Pre_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Common.CSharp";
        [IntentFully]
        public string ModuleVersion => "3.11.1-pre.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            foreach (var (designerId, templateOutputSpecializationId) in Targets)
            {
                // TryGetDesigner throws when the application has no such designer, so look it up instead.
                var designer = app.GetDesigners().SingleOrDefault(d => d.Id == designerId);
                if (designer is null)
                {
                    continue;
                }

                foreach (var package in designer.GetPackages())
                {
                    var outputs = package.GetElementsOfType(templateOutputSpecializationId)
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
        }

        public void Down()
        {
        }
    }
}
