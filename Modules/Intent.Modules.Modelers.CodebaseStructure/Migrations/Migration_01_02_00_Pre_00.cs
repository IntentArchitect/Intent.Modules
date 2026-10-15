using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Modelers.CodebaseStructure.Migrations
{
    /// <summary>
    /// Converts each Template Output's legacy Output Classification into a File Classification
    /// holding a single <c>**/*</c> entry with the same review importance and classifications, which is the shape
    /// module install stamps from 1.2.0 onwards.
    /// </summary>
    public class Migration_01_02_00_Pre_00 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_02_00_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Modelers.CodebaseStructure";
        [IntentFully]
        public string ModuleVersion => "1.2.0-pre.0";

        public void Up()
        {
            Migrate(TryConvertToEntries);
        }

        public void Down()
        {
            Migrate(TryRevertToOutputClassification);
        }

        private void Migrate(Func<ElementPersistable, IReadOnlyDictionary<string, string>, bool> convert)
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var packages = app.GetDesigners().SelectMany(x => x.GetPackages()).ToList();

            // Entry labels show classification names, and File Classification elements can live in any package.
            var classificationNames = packages
                .SelectMany(x => GetAllElements(x.Classes))
                .Where(x => x.SpecializationTypeId == Ids.FileClassificationSpecializationTypeId)
                .GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First().Name);

            foreach (var package in packages)
            {
                var changed = false;
                foreach (var element in GetAllElements(package.Classes).Distinct().ToList())
                {
                    if (element.SpecializationTypeId == Ids.TemplateOutputSpecializationTypeId)
                    {
                        changed |= convert(element, classificationNames);
                    }
                }

                if (changed)
                {
                    package.Save(true);
                }
            }
        }

        /// <summary>
        /// Replaces Output Classification with a File Classification holding one <c>**/*</c> entry.
        /// Elements that already have a File Classification are left alone, since a user put it there.
        /// </summary>
        internal static bool TryConvertToEntries(ElementPersistable templateOutput, IReadOnlyDictionary<string, string> classificationNames)
        {
            var legacy = templateOutput.Stereotypes.FirstOrDefault(x => x.DefinitionId == Ids.OutputClassification.DefinitionId);
            if (legacy == null ||
                templateOutput.Stereotypes.Any(x => x.DefinitionId == Ids.FileClassification.DefinitionId))
            {
                return false;
            }

            var reviewImportance = GetPropertyValue(legacy, Ids.OutputClassification.ReviewImportanceId);
            var classification = GetPropertyValue(legacy, Ids.OutputClassification.ClassificationId);
            templateOutput.Stereotypes.Remove(legacy);

            if (string.IsNullOrWhiteSpace(reviewImportance) && IsEmptyArray(classification))
            {
                return true;
            }

            var entry = NewStereotype(Ids.FileClassification.EntryDefinitionId, Ids.FileClassification.EntryName);
            SetProperty(entry, Ids.FileClassification.GlobId, Ids.FileClassification.GlobName, Ids.FileClassification.MatchAllGlob);
            SetProperty(entry, Ids.FileClassification.ReviewImportanceId, Ids.FileClassification.ReviewImportanceName, reviewImportance);
            SetProperty(entry, Ids.FileClassification.ClassificationId, Ids.FileClassification.ClassificationName, classification);

            var names = ParseIds(classification)
                .Select(id => classificationNames.TryGetValue(id, out var name) ? name : null);
            var custom = NewStereotype(Ids.FileClassification.DefinitionId, Ids.FileClassification.Name);
            custom.Properties.Add(new StereotypePropertyPersistable
            {
                DefinitionId = Ids.FileClassification.EntriesId,
                Name = Ids.FileClassification.EntriesName,
                // The label cache the designer keeps alongside the items.
                Value = JsonSerializer.Serialize(new[] { FormatEntryLabel(reviewImportance, names, Ids.FileClassification.MatchAllGlob) }),
                ItemList = new List<StereotypePersistable> { entry }
            });

            templateOutput.Stereotypes.Add(custom);
            return true;
        }

        /// <summary>
        /// Reverses <see cref="TryConvertToEntries"/>, but only where the File Classification still
        /// holds exactly the single <c>**/*</c> entry it produced; anything a user has added to is kept.
        /// </summary>
        internal static bool TryRevertToOutputClassification(ElementPersistable templateOutput, IReadOnlyDictionary<string, string> classificationNames)
        {
            var custom = templateOutput.Stereotypes.FirstOrDefault(x => x.DefinitionId == Ids.FileClassification.DefinitionId);
            if (custom == null ||
                templateOutput.Stereotypes.Any(x => x.DefinitionId == Ids.OutputClassification.DefinitionId))
            {
                return false;
            }

            var items = custom.Properties?.FirstOrDefault(x => x.DefinitionId == Ids.FileClassification.EntriesId)?.ItemList;
            if (items is not { Count: 1 } ||
                GetPropertyValue(items[0], Ids.FileClassification.GlobId)?.Trim() != Ids.FileClassification.MatchAllGlob)
            {
                return false;
            }

            var legacy = NewStereotype(Ids.OutputClassification.DefinitionId, Ids.OutputClassification.Name);
            SetProperty(legacy, Ids.OutputClassification.ClassificationId, Ids.OutputClassification.ClassificationName,
                GetPropertyValue(items[0], Ids.FileClassification.ClassificationId));
            SetProperty(legacy, Ids.OutputClassification.ReviewImportanceId, Ids.OutputClassification.ReviewImportanceName,
                GetPropertyValue(items[0], Ids.FileClassification.ReviewImportanceId));

            templateOutput.Stereotypes.Remove(custom);
            templateOutput.Stereotypes.Add(legacy);
            return true;
        }

        /// <summary>
        /// Must match the File Classification Entry stereotype's display function, and the client's
        /// <c>FormatEntryLabel</c> in <c>ClassificationStereotypeConstants</c>.
        /// </summary>
        internal static string FormatEntryLabel(string reviewImportancePropertyValue, IEnumerable<string> classificationNames, string glob)
        {
            var importance = Regex.Replace(reviewImportancePropertyValue ?? string.Empty, @"^\d+\s*-\s*", string.Empty);
            var names = string.Join(", ", classificationNames.Where(x => !string.IsNullOrWhiteSpace(x)));
            var head = string.Join(" - ", new[] { importance, names }.Where(x => !string.IsNullOrWhiteSpace(x)));
            var globs = string.Join(", ", (glob ?? string.Empty)
                .Split('\n')
                .Select(x => x.Trim())
                .Where(x => x.Length > 0));

            return $"[{head}] {globs}".Trim();
        }

        private static StereotypePersistable NewStereotype(string definitionId, string name) => new()
        {
            DefinitionId = definitionId,
            Name = name,
            DefinitionPackageId = Ids.DefinitionPackageId,
            DefinitionPackageName = Ids.DefinitionPackageName,
            // Not initialised by the persistable's constructor.
            Properties = new List<StereotypePropertyPersistable>()
        };

        private static void SetProperty(StereotypePersistable stereotype, string definitionId, string name, string value)
        {
            if (value != null)
            {
                stereotype.Properties.Add(new StereotypePropertyPersistable { DefinitionId = definitionId, Name = name, Value = value });
            }
        }

        private static string GetPropertyValue(StereotypePersistable stereotype, string definitionId) =>
            stereotype.Properties?.FirstOrDefault(x => x.DefinitionId == definitionId)?.Value;

        private static bool IsEmptyArray(string json) => !ParseIds(json).Any();

        private static IReadOnlyList<string> ParseIds(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return [];
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? [];
            }
            catch (JsonException)
            {
                return [];
            }
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
        /// Persisted ids from this module's own metadata, the same values as the client's
        /// <c>ClassificationStereotypeConstants</c>.
        /// </summary>
        internal static class Ids
        {
            public const string DefinitionPackageId = "525868ff-adc0-4ab6-a90e-352288da941f";
            public const string DefinitionPackageName = "Intent.Modelers.CodebaseStructure";
            public const string TemplateOutputSpecializationTypeId = "d421c322-7a51-4094-89fa-e5d8a0a97b27";
            public const string FileClassificationSpecializationTypeId = "86490efd-d823-44cf-bb67-283231ae95f3";

            internal static class OutputClassification
            {
                public const string DefinitionId = "98cff892-b03e-4d9c-b1dc-031cb0c900ee";
                public const string Name = "Output Classification";
                public const string ClassificationId = "0acdff86-3981-4950-921c-f5642a3e2374";
                public const string ClassificationName = "Classification";
                public const string ReviewImportanceId = "277dec79-e6f2-4e4a-ab8e-1849990889a1";
                public const string ReviewImportanceName = "Review Importance";
            }

            internal static class FileClassification
            {
                public const string DefinitionId = "1a5d919b-7ca2-4f2f-8a0d-0995aa6b02ac";
                public const string Name = "File Classification";
                public const string EntriesId = "3e552682-b2c7-450a-acb6-be006206cf97";
                public const string EntriesName = "Entries";
                public const string EntryDefinitionId = "14476d2c-c5fb-4aea-ad8c-55d1b00a8203";
                public const string EntryName = "File Classification Entry";
                public const string GlobId = "14dd36e5-eee3-4b8f-9868-99b965703d23";
                public const string GlobName = "Glob";
                public const string ReviewImportanceId = "1f903649-4ed0-4c2b-b288-34dedacdff05";
                public const string ReviewImportanceName = "Review Importance";
                public const string ClassificationId = "c42e8c3a-962a-4e85-b890-e3234eb6534d";
                public const string ClassificationName = "Classification";
                public const string MatchAllGlob = "**/*";
            }
        }
    }
}
