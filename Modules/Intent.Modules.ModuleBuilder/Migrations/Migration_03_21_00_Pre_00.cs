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
    /// Moves each Element Visual Settings' <c>Anchor Points</c> value, which no longer exists, into <c>Anchor Segments</c>.
    /// A point is a segment <c>[p, p]</c>, so lines still end exactly where they did before.
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
                foreach (var element in GetAllElements(package.Classes).Distinct().ToList())
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
        /// Copies <c>Anchor Points</c> into <c>Anchor Segments</c> and clears the old value. Anchor Segments
        /// that already has a value is kept, since a user put it there, and a body that never returned an array
        /// is skipped and logged.
        /// </summary>
        internal static bool TryConvertAnchorPoints(ElementPersistable visualSettings)
        {
            var settings = visualSettings.Stereotypes.FirstOrDefault(x => x.DefinitionId == Ids.SettingsDefinitionId);
            var anchorPoints = settings?.Properties?.FirstOrDefault(x => x.Id == Ids.AnchorPointsId);
            if (anchorPoints == null || string.IsNullOrWhiteSpace(anchorPoints.Value))
            {
                return false;
            }

            var segments = settings.Properties.FirstOrDefault(x => x.Id == Ids.AnchorSegmentsId);
            if (!string.IsNullOrWhiteSpace(segments?.Value))
            {
                anchorPoints.Value = null;
                return true;
            }

            if (!ValidBodyStart.IsMatch(anchorPoints.Value))
            {
                Logging.Log?.Warning($"Anchor Points on '{visualSettings.Name}' does not start with return, const, let, var, if or function so it was never read as an array. Skipped; set Anchor Segments by hand if needed.");
                return false;
            }

            var body = $"const points = (() => {{ {anchorPoints.Value.Trim()} }})(); return points.map(p => [p, p]);";
            if (segments == null)
            {
                segments = new StereotypePropertyPersistable { Id = Ids.AnchorSegmentsId, Name = Ids.AnchorSegmentsName };
                settings.Properties.Add(segments);
            }

            segments.Value = body;
            anchorPoints.Value = null;
            return true;
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
        /// Persisted ids from this module's own metadata. Anchor Points is no longer defined, so it has no generated accessor.
        /// </summary>
        internal static class Ids
        {
            public const string SettingsDefinitionId = ElementVisualSettingsModelStereotypeExtensions.Settings.DefinitionId;
            public const string AnchorPointsId = "838dcc53-b73e-4381-a3d2-db0afaf230b2";
            public const string AnchorSegmentsId = "c5c75dd7-f419-4a03-8b78-e395ede92ddd";
            public const string AnchorSegmentsName = "Anchor Segments";
        }
    }
}
