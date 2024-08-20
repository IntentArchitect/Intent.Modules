using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.CSharp.Api
{
    public static class PackageVersionModelStereotypeExtensions
    {
        public static PackageVersionSettings GetPackageVersionSettings(this PackageVersionModel model)
        {
            var stereotype = model.GetStereotype(PackageVersionSettings.DefinitionId);
            return stereotype != null ? new PackageVersionSettings(stereotype) : null;
        }


        public static bool HasPackageVersionSettings(this PackageVersionModel model)
        {
            return model.HasStereotype(PackageVersionSettings.DefinitionId);
        }

        public static bool TryGetPackageVersionSettings(this PackageVersionModel model, out PackageVersionSettings stereotype)
        {
            if (!HasPackageVersionSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PackageVersionSettings(model.GetStereotype(PackageVersionSettings.DefinitionId));
            return true;
        }

        public class PackageVersionSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "7af88c37-ce54-49fc-b577-bde869c23462";

            public PackageVersionSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public MinimumTargetFrameworkOptions MinimumTargetFramework()
            {
                return new MinimumTargetFrameworkOptions(_stereotype.GetProperty<string>("Minimum Target Framework"));
            }

            public bool Locked()
            {
                return _stereotype.GetProperty<bool>("Locked");
            }

            public class MinimumTargetFrameworkOptions
            {
                public readonly string Value;

                public MinimumTargetFrameworkOptions(string value)
                {
                    Value = value;
                }

                public MinimumTargetFrameworkOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case ".NETCoreApp,Version=v9.0":
                            return MinimumTargetFrameworkOptionsEnum.NETCoreAppVersionV90;
                        case ".NETCoreApp,Version=v8.0":
                            return MinimumTargetFrameworkOptionsEnum.NETCoreAppVersionV80;
                        case ".NETCoreApp,Version=v7.0":
                            return MinimumTargetFrameworkOptionsEnum.NETCoreAppVersionV70;
                        case ".NETCoreApp,Version=v6.0":
                            return MinimumTargetFrameworkOptionsEnum.NETCoreAppVersionV60;
                        case ".NETStandard,Version=v2.0":
                            return MinimumTargetFrameworkOptionsEnum.NETStandardVersionV20;
                        case "Any,Version=v0.0":
                            return MinimumTargetFrameworkOptionsEnum.AnyVersionV00;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsNETCoreAppVersionV90()
                {
                    return Value == ".NETCoreApp,Version=v9.0";
                }

                public bool IsNETCoreAppVersionV80()
                {
                    return Value == ".NETCoreApp,Version=v8.0";
                }
                public bool IsNETCoreAppVersionV70()
                {
                    return Value == ".NETCoreApp,Version=v7.0";
                }
                public bool IsNETCoreAppVersionV60()
                {
                    return Value == ".NETCoreApp,Version=v6.0";
                }
                public bool IsNETStandardVersionV20()
                {
                    return Value == ".NETStandard,Version=v2.0";
                }
                public bool IsAnyVersionV00()
                {
                    return Value == "Any,Version=v0.0";
                }
            }

            public enum MinimumTargetFrameworkOptionsEnum
            {
                NETCoreAppVersionV90,
                NETCoreAppVersionV80,
                NETCoreAppVersionV70,
                NETCoreAppVersionV60,
                NETStandardVersionV20,
                AnyVersionV00
            }

        }

    }
}