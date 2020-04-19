using Intent.Metadata.Models;
using Intent.Modules.Common;
using System;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using IconType = Intent.IArchitect.Common.Types.IconType;

namespace Intent.Modules.ModuleBuilder.Api
{
    public static class ModelExtensions
    {
        public static IconModelPersistable ToPersistable(this IIconModel icon)
        {
            return icon != null ? new IconModelPersistable { Type = (IconType) icon.Type, Source = icon.Source } : null;
        }
    }
}