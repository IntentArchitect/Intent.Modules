namespace Intent.Modules.Common.Templates
{
    public class TemplateMigrationCriteria
    {
        public bool SolutionFileMissing { get; }
        public int? FromMajorVersion { get; }
        public int ToMajorVersion { get; }

        private TemplateMigrationCriteria(int? fromMajorVersion, int toMajorVersion, bool solutionFileMissing)
        {
            SolutionFileMissing = solutionFileMissing;
            FromMajorVersion = fromMajorVersion;
            ToMajorVersion = toMajorVersion;
        }

        public static TemplateMigrationCriteria UpgradeWithFileNameRename(int fromMajorVersion, int toMajorVersion)
        {
            return new TemplateMigrationCriteria(fromMajorVersion, toMajorVersion, true);
        }

        public static TemplateMigrationCriteria Upgrade(int fromMajorVersion, int toMajorVersion)
        {
            return new TemplateMigrationCriteria(fromMajorVersion, toMajorVersion, false);
        }

        public static TemplateMigrationCriteria UnversionedUpgrade(int toMajorVersion)
        {
            return new TemplateMigrationCriteria(null, toMajorVersion, false);
        }

        public bool Matches(bool fileExists, int? fromMajorVersion, int toMajorVersion)
        {
            return SolutionFileMissing == !fileExists
                && FromMajorVersion == fromMajorVersion
                && ToMajorVersion == ToMajorVersion;
        }
    }
}
