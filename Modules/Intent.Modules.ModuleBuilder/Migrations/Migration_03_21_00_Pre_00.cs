using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.ModuleBuilder.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Migrations
{
    /// <summary>
    /// Copies each Element Visual Settings' <c>Anchor Points</c> value into <c>Anchor Segments</c>, and re-keys it onto
    /// the deprecated <c>Anchor Points</c> property that is still written for older clients. A point is a segment
    /// <c>[p, p]</c>, so lines still end exactly where they did before.
    /// </summary>
    public class Migration_03_21_00_Pre_00 : IModuleMigration
    {
        private static readonly Regex ValidBodyStart = new(@"^\s*(return|const|let|var|if|function)\b", RegexOptions.Compiled);

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_21_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.ModuleBuilder";
        [IntentFully]
        public string ModuleVersion => "3.21.0-pre.0";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            foreach (var package in app.GetDesigners().SelectMany(x => x.GetPackages()))
            {
                var changed = false;
                foreach (var element in GetAllElements(package.Classes))
                {
                    if (element.SpecializationTypeId == ElementVisualSettingsModel.SpecializationTypeId)
                    {
                        changed |= TryConvertAnchorPoints(element);
                    }
                }

                if (changed)
                {
                    package.Save(true);
                }
            }
        }

        // A segment can't be turned back into the points it came from, so there is nothing to undo.
        public void Down()
        {
        }

        /// <summary>
        /// Moves the value of the original <c>Anchor Points</c> property onto the current one, which is now the
        /// deprecated property still written for older clients, and copies it into <c>Anchor Segments</c>. Anchor
        /// Segments that already has a value is kept, since a user put it there, and a body that never returned an
        /// array is not copied, and is logged.
        /// </summary>
        internal static bool TryConvertAnchorPoints(ElementPersistable visualSettings)
        {
            var settings = visualSettings.Stereotypes.FirstOrDefault(x => x.DefinitionId == Ids.SettingsDefinitionId);
            var original = settings?.Properties?.FirstOrDefault(x => x.Id == Ids.OriginalAnchorPointsId);
            if (original == null)
            {
                return false;
            }

            var points = original.Value;
            settings.Properties.Remove(original);
            if (string.IsNullOrWhiteSpace(points))
            {
                return true;
            }

            SetProperty(settings, Ids.AnchorPointsId, Ids.AnchorPointsName, points);

            var segments = settings.Properties.FirstOrDefault(x => x.Id == Ids.AnchorSegmentsId);
            if (!string.IsNullOrWhiteSpace(segments?.Value))
            {
                return true;
            }

            if (!ValidBodyStart.IsMatch(points))
            {
                Logging.Log?.Warning($"Anchor Points on '{visualSettings.Name}' does not start with return, const, let, var, if or function so it was never read as an array. It is kept as is and not copied into Anchor Segments.");
                return true;
            }

            SetProperty(settings, Ids.AnchorSegmentsId, Ids.AnchorSegmentsName,
                // Newlines keep a trailing // comment in the old body from swallowing the closing brackets.
                $"const points = (() => {{\n{points.Trim()}\n}})(); return points?.map(p => [p, p]);");
            return true;
        }

        private static void SetProperty(StereotypePersistable stereotype, string id, string name, string value)
        {
            var property = stereotype.Properties.FirstOrDefault(x => x.Id == id);
            if (property == null)
            {
                property = new StereotypePropertyPersistable { Id = id, Name = name };
                stereotype.Properties.Add(property);
            }

            property.Value = value;
        }

        private static IEnumerable<ElementPersistable> GetAllElements(IEnumerable<ElementPersistable> elements)
        {
            foreach (var element in elements)
            {
                yield return element;

                foreach (var child in GetAllElements(element.ChildElements))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// Persisted ids from this module's own metadata. The Anchor Points property was removed and re-added as a
        /// deprecated one, so the id consumers' values sit under is no longer defined and has no generated accessor.
        /// </summary>
        internal static class Ids
        {
            public const string SettingsDefinitionId = ElementVisualSettingsModelStereotypeExtensions.Settings.DefinitionId;
            public const string OriginalAnchorPointsId = "838dcc53-b73e-4381-a3d2-db0afaf230b2";
            public const string AnchorPointsId = "a2f67b96-fbe6-48ce-ac40-0283412d5798";
            public const string AnchorPointsName = "Anchor Points";
            public const string AnchorSegmentsId = "c5c75dd7-f419-4a03-8b78-e395ede92ddd";
            public const string AnchorSegmentsName = "Anchor Segments";
        }
    }
}
