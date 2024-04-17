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
    public static class FileTemplateModelStereotypeExtensions
    {
        public static FileSettings GetFileSettings(this FileTemplateModel model)
        {
            var stereotype = model.GetStereotype(FileSettings.DefinitionId);
            return stereotype != null ? new FileSettings(stereotype) : null;
        }


        public static bool HasFileSettings(this FileTemplateModel model)
        {
            return model.HasStereotype(FileSettings.DefinitionId);
        }

        public static bool TryGetFileSettings(this FileTemplateModel model, out FileSettings stereotype)
        {
            if (!HasFileSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FileSettings(model.GetStereotype(FileSettings.DefinitionId));
            return true;
        }

        public class FileSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "819dee60-184a-4cb7-9529-0d3910f82ee6";

            public FileSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public OutputFileContentOptions OutputFileContent()
            {
                return new OutputFileContentOptions(_stereotype.GetProperty<string>("Output File Content"));
            }

            public TemplatingMethodOptions TemplatingMethod()
            {
                return new TemplatingMethodOptions(_stereotype.GetProperty<string>("Templating Method"));
            }

            public DataFileOutputTypeOptions DataFileOutputType()
            {
                return new DataFileOutputTypeOptions(_stereotype.GetProperty<string>("Data File Output Type"));
            }

            public string FileExtension()
            {
                return _stereotype.GetProperty<string>("File Extension");
            }

            public class OutputFileContentOptions
            {
                public readonly string Value;

                public OutputFileContentOptions(string value)
                {
                    Value = value;
                }

                public OutputFileContentOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Text":
                            return OutputFileContentOptionsEnum.Text;
                        case "Binary":
                            return OutputFileContentOptionsEnum.Binary;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsText()
                {
                    return Value == "Text";
                }
                public bool IsBinary()
                {
                    return Value == "Binary";
                }
            }

            public enum OutputFileContentOptionsEnum
            {
                Text,
                Binary
            }
            public class TemplatingMethodOptions
            {
                public readonly string Value;

                public TemplatingMethodOptions(string value)
                {
                    Value = value;
                }

                public TemplatingMethodOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Indented File Builder":
                            return TemplatingMethodOptionsEnum.IndentedFileBuilder;
                        case "Data File Builder":
                            return TemplatingMethodOptionsEnum.DataFileBuilder;
                        case "String Interpolation":
                            return TemplatingMethodOptionsEnum.StringInterpolation;
                        case "T4 Template":
                            return TemplatingMethodOptionsEnum.T4Template;
                        case "Custom":
                            return TemplatingMethodOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsIndentedFileBuilder()
                {
                    return Value == "Indented File Builder";
                }
                public bool IsDataFileBuilder()
                {
                    return Value == "Data File Builder";
                }
                public bool IsStringInterpolation()
                {
                    return Value == "String Interpolation";
                }
                public bool IsT4Template()
                {
                    return Value == "T4 Template";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum TemplatingMethodOptionsEnum
            {
                IndentedFileBuilder,
                DataFileBuilder,
                StringInterpolation,
                T4Template,
                Custom
            }
            public class DataFileOutputTypeOptions
            {
                public readonly string Value;

                public DataFileOutputTypeOptions(string value)
                {
                    Value = value;
                }

                public DataFileOutputTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "JSON":
                            return DataFileOutputTypeOptionsEnum.JSON;
                        case "YAML":
                            return DataFileOutputTypeOptionsEnum.YAML;
                        case "OCL":
                            return DataFileOutputTypeOptionsEnum.OCL;
                        case "Custom":
                            return DataFileOutputTypeOptionsEnum.Custom;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsJSON()
                {
                    return Value == "JSON";
                }
                public bool IsYAML()
                {
                    return Value == "YAML";
                }
                public bool IsOCL()
                {
                    return Value == "OCL";
                }
                public bool IsCustom()
                {
                    return Value == "Custom";
                }
            }

            public enum DataFileOutputTypeOptionsEnum
            {
                JSON,
                YAML,
                OCL,
                Custom
            }
        }

    }
}