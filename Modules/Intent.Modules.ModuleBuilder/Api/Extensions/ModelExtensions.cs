using Intent.Metadata.Models;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.ModuleBuilder.Api
{
    public static class ModelExtensions
    {
        public static IconModelPersistable ToPersistable(this IIconModel icon)
        {
            return icon != null ? new IconModelPersistable { Type = (IconType) icon.Type, Source = icon.Source } : null;
        }
    }
}