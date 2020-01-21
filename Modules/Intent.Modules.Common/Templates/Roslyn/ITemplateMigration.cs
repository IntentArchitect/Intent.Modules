namespace Intent.Modules.Common.Templates
{
    public interface ITemplateMigration
    {
        TemplateMigrationCriteria Criteria { get; }
        string Execute(string currentText);
    }
}
