using System;
using Intent.Metadata.Models;

namespace Intent.Modules.ModuleBuilder.Api
{
    public interface ICreatableType
    {
        string Id { get; }
        string Name { get; }
        string ApiModelName { get; }
    }
}