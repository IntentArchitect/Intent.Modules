namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModelerModelType
    {
        string Id { get; }
        string Name { get; }
        string Namespace { get; }
        string LoadMethod { get; }
        string PerModelTemplate { get; }
        string SingleListTemplate { get; }
    }
}