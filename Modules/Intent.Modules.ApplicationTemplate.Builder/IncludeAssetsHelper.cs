using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.ApplicationTemplate.Builder
{
    internal static class IncludeAssetsHelper
    {
        private static readonly int AssetTypeCount = Enum.GetValues(typeof(AssetType)).Length;
        private const string All = "all";
        private const string None = "none";

        public static string? GetIncludeAssetsString(
            bool applicationSettings,
            bool designerMetadata,
            bool designers,
            bool factoryExtensions,
            bool templateOutputs)
        {
            var assets = new List<AssetType>(AssetTypeCount);

            if (applicationSettings) assets.Add(AssetType.ApplicationSettings);
            if (designerMetadata) assets.Add(AssetType.DesignerMetadata);
            if (designers) assets.Add(AssetType.Designers);
            if (factoryExtensions) assets.Add(AssetType.FactoryExtensions);
            if (templateOutputs) assets.Add(AssetType.TemplateOutputs);

            if (assets.Count == AssetTypeCount)
            {
                return null;
            }

            if (assets.Count == 0)
            {
                return None;
            }

            return string.Join(";", assets.Select(GetSerializedValue).OrderBy(x => x));
        }

        public static bool IncludesNone(string includeAssets)
        {
            if (includeAssets == null)
            {
                return false;
            }

            includeAssets = includeAssets.Trim();

            return includeAssets == string.Empty ||
                   includeAssets.Equals(None, StringComparison.OrdinalIgnoreCase);
        }

        public static (string IncludeAssets, string? MetadataOnlyObsolete) OnAfterDeserialization(
            string includeAssets,
            string metadataOnlyObsolete)
        {
            if (string.IsNullOrWhiteSpace(includeAssets) &&
                bool.TryParse(metadataOnlyObsolete, out var installMetadataOnly) &&
                installMetadataOnly)
            {
                includeAssets = None;
            }

            return (IncludeAssets: includeAssets, MetadataOnlyObsolete: null);
        }

        public static bool IncludesFactoryExtensions(string includeAssets) =>
            HasAssetType(includeAssets, AssetType.FactoryExtensions);

        public static bool IncludesApplicationSettings(string includeAssets) =>
            HasAssetType(includeAssets, AssetType.ApplicationSettings);

        public static bool IncludesDesignerMetadata(string includeAssets) =>
            HasAssetType(includeAssets, AssetType.DesignerMetadata);

        public static bool IncludesDesigners(string includeAssets) =>
            HasAssetType(includeAssets, AssetType.Designers);

        public static bool IncludesTemplateOutputs(string includeAssets) =>
            HasAssetType(includeAssets, AssetType.TemplateOutputs);

        private static bool HasAssetType(string includeAssets, AssetType assetType) =>
            includeAssets == null ||
            includeAssets.Equals(All, StringComparison.OrdinalIgnoreCase) ||
            includeAssets.IndexOf(GetSerializedValue(assetType), StringComparison.OrdinalIgnoreCase) >= 0;

        private static string GetSerializedValue(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.ApplicationSettings => "applicationSettings",
                AssetType.DesignerMetadata => "designerMetadata",
                AssetType.Designers => "designers",
                AssetType.FactoryExtensions => "factoryExtensions",
                AssetType.TemplateOutputs => "templateOutputs",
                _ => throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null)
            };
        }

        private enum AssetType
        {
            ApplicationSettings,
            DesignerMetadata,
            Designers,
            FactoryExtensions,
            TemplateOutputs,
        }
    }
}
