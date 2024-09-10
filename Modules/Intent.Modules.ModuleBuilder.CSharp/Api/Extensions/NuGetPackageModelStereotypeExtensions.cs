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
    public static class NuGetPackageModelStereotypeExtensions
    {
        public static PackageSettings GetPackageSettings(this NuGetPackageModel model)
        {
            var stereotype = model.GetStereotype("265221a5-779c-46c9-a367-8b07b435803b");
            return stereotype != null ? new PackageSettings(stereotype) : null;
        }


        public static bool HasPackageSettings(this NuGetPackageModel model)
        {
            return model.HasStereotype("265221a5-779c-46c9-a367-8b07b435803b");
        }

        public static bool TryGetPackageSettings(this NuGetPackageModel model, out PackageSettings stereotype)
        {
            if (!HasPackageSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PackageSettings(model.GetStereotype("265221a5-779c-46c9-a367-8b07b435803b"));
            return true;
        }

        public class PackageSettings
        {
            private IStereotype _stereotype;

            public PackageSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string FriendlyName()
            {
                return _stereotype.GetProperty<string>("Friendly Name");
            }

            public bool Locked()
            {
                return _stereotype.GetProperty<bool>("Locked");
            }

            public PrivateAssetsOptions[] PrivateAssets()
            {
                return _stereotype.GetProperty<string[]>("Private Assets")?.Select(x => new PrivateAssetsOptions(x)).ToArray() ?? new PrivateAssetsOptions[0];
            }

            public IncludeAssetsOptions[] IncludeAssets()
            {
                return _stereotype.GetProperty<string[]>("Include Assets")?.Select(x => new IncludeAssetsOptions(x)).ToArray() ?? new IncludeAssetsOptions[0];
            }

            public class PrivateAssetsOptions
            {
                public readonly string Value;

                public PrivateAssetsOptions(string value)
                {
                    Value = value;
                }

                public PrivateAssetsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "all":
                            return PrivateAssetsOptionsEnum.All;
                        case "analyzers":
                            return PrivateAssetsOptionsEnum.Analyzers;
                        case "build":
                            return PrivateAssetsOptionsEnum.Build;
                        case "buildTransitive":
                            return PrivateAssetsOptionsEnum.BuildTransitive;
                        case "compile":
                            return PrivateAssetsOptionsEnum.Compile;
                        case "contentFiles":
                            return PrivateAssetsOptionsEnum.ContentFiles;
                        case "native":
                            return PrivateAssetsOptionsEnum.Native;
                        case "runtime":
                            return PrivateAssetsOptionsEnum.Runtime;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAll()
                {
                    return Value == "all";
                }
                public bool IsAnalyzers()
                {
                    return Value == "analyzers";
                }
                public bool IsBuild()
                {
                    return Value == "build";
                }
                public bool IsBuildTransitive()
                {
                    return Value == "buildTransitive";
                }
                public bool IsCompile()
                {
                    return Value == "compile";
                }
                public bool IsContentFiles()
                {
                    return Value == "contentFiles";
                }
                public bool IsNative()
                {
                    return Value == "native";
                }
                public bool IsRuntime()
                {
                    return Value == "runtime";
                }
            }

            public enum PrivateAssetsOptionsEnum
            {
                All,
                Analyzers,
                Build,
                BuildTransitive,
                Compile,
                ContentFiles,
                Native,
                Runtime
            }
            public class IncludeAssetsOptions
            {
                public readonly string Value;

                public IncludeAssetsOptions(string value)
                {
                    Value = value;
                }

                public IncludeAssetsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "all":
                            return IncludeAssetsOptionsEnum.All;
                        case "analyzers":
                            return IncludeAssetsOptionsEnum.Analyzers;
                        case "build":
                            return IncludeAssetsOptionsEnum.Build;
                        case "buildTransitive":
                            return IncludeAssetsOptionsEnum.BuildTransitive;
                        case "compile":
                            return IncludeAssetsOptionsEnum.Compile;
                        case "contentFiles":
                            return IncludeAssetsOptionsEnum.ContentFiles;
                        case "native":
                            return IncludeAssetsOptionsEnum.Native;
                        case "runtime":
                            return IncludeAssetsOptionsEnum.Runtime;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsAll()
                {
                    return Value == "all";
                }
                public bool IsAnalyzers()
                {
                    return Value == "analyzers";
                }
                public bool IsBuild()
                {
                    return Value == "build";
                }
                public bool IsBuildTransitive()
                {
                    return Value == "buildTransitive";
                }
                public bool IsCompile()
                {
                    return Value == "compile";
                }
                public bool IsContentFiles()
                {
                    return Value == "contentFiles";
                }
                public bool IsNative()
                {
                    return Value == "native";
                }
                public bool IsRuntime()
                {
                    return Value == "runtime";
                }
            }

            public enum IncludeAssetsOptionsEnum
            {
                All,
                Analyzers,
                Build,
                BuildTransitive,
                Compile,
                ContentFiles,
                Native,
                Runtime
            }

        }

    }
}