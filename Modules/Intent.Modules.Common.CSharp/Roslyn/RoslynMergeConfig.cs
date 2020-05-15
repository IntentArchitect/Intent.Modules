namespace Intent.Modules.Common.Templates
{
    public class RoslynMergeConfig
    {
        public TemplateMetadata TemplateMetadata { get; }
        public ITemplateMigration[] Migrations { get; }

        public RoslynMergeConfig(TemplateMetadata templateMetadata, params ITemplateMigration[] migrations)
        {
            TemplateMetadata = templateMetadata;
            Migrations = migrations;
        }
    }
}
