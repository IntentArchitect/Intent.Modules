using System;
using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Templates;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.Contracts.Decorators
{
    public class DataContractDTOAttributeDecorator : IDTOAttributeDecorator, IDeclareUsings
    {
        public const string Id = "Intent.Application.Contracts.DataContractDecorator";

        public int Priority { get; } = 0;

        public string ClassAttributes(DTOModel dto)
        {
            
            return $"[DataContract{ GetDataContractPropertiesFormatted(dto) }]";
        }

        public string PropertyAttributes(DTOModel dto, DTOFieldModel field)
        {
            return "[DataMember]";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "System.Runtime.Serialization";
        }

        private string GetDataContractPropertiesFormatted(DTOModel dto)
        {
            var dataContractStereotype = GetDataContractStereotype(dto);
            if (dataContractStereotype != null)
            {
                var sb = new StringBuilder();

                var @namespace = dataContractStereotype.GetProperty<string>("Namespace");
                if (!string.IsNullOrEmpty(@namespace))
                {
                    sb.Append($@"Namespace=""{ @namespace }""");
                }

                var isReference = dataContractStereotype.GetProperty<bool>("IsReference");
                if(isReference)
                {
                    if(sb.Length > 0)
                    {
                        sb.Append(" ");
                    }

                    sb.Append($@"IsReference=""{ isReference }""");
                }

                if (sb.Length > 0)
                {
                    sb.Insert(0, "( ");
                    sb.Append(" )");

                    return sb.ToString();
                }
            }
            return string.Empty;
        }

        private IStereotype GetDataContractStereotype(DTOModel dto)
        {
            IStereotype stereotype;
            stereotype = dto.GetStereotype("DataContract");
            if(stereotype != null)
            {
                return stereotype;
            }

            stereotype = dto.GetStereotypeInFolders("DataContract");
            if (stereotype != null)
            {
                return stereotype;
            }

            return null;
        }
    }
}
