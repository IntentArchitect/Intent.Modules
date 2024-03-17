namespace Intent.ModuleBuilder.Api
{
    public interface ICreatableType
    {
        string Id { get; }
        string Name { get; }
        string ApiModelName { get; }
    }
}