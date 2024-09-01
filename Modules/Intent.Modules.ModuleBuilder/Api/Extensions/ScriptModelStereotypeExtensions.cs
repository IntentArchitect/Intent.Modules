using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class ScriptModelStereotypeExtensions
    {
        public static ScriptSettings GetScriptSettings(this ScriptModel model)
        {
            var stereotype = model.GetStereotype(ScriptSettings.DefinitionId);
            return stereotype != null ? new ScriptSettings(stereotype) : null;
        }


        public static bool HasScriptSettings(this ScriptModel model)
        {
            return model.HasStereotype(ScriptSettings.DefinitionId);
        }

        public static bool TryGetScriptSettings(this ScriptModel model, out ScriptSettings stereotype)
        {
            if (!HasScriptSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ScriptSettings(model.GetStereotype(ScriptSettings.DefinitionId));
            return true;
        }

        public class ScriptSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "da7d632f-b07f-4c83-ab01-bc6175006aa5";

            public ScriptSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public TypeOptions Type()
            {
                return new TypeOptions(_stereotype.GetProperty<string>("Type"));
            }

            public string Script()
            {
                return _stereotype.GetProperty<string>("Script");
            }

            public string FilePath()
            {
                return _stereotype.GetProperty<string>("File Path");
            }

            public IElement[] Dependencies()
            {
                return _stereotype.GetProperty<IElement[]>("Dependencies") ?? new IElement[0];
            }

            public class TypeOptions
            {
                public readonly string Value;

                public TypeOptions(string value)
                {
                    Value = value;
                }

                public TypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Inline Script":
                            return TypeOptionsEnum.InlineScript;
                        case "File Resource":
                            return TypeOptionsEnum.FileResource;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsInlineScript()
                {
                    return Value == "Inline Script";
                }
                public bool IsFileResource()
                {
                    return Value == "File Resource";
                }
            }

            public enum TypeOptionsEnum
            {
                InlineScript,
                FileResource
            }
        }

    }
}