namespace Intent.Modules.ModuleBuilder.Api
{
    public interface IModelerModelType
    {
        string Id { get; }
        string ClassName { get; }
        string InterfaceName { get; }
        string Namespace { get; }
        string LoadMethod { get; }
        string PerModelTemplate { get; }
        string SingleListTemplate { get; }
        IModeler Modeler { get; }
    }
}