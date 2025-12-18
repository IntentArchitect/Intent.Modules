using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.AI.Configuration
{
    public static class JsonNet
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(
                    processDictionaryKeys: true,
                    overrideSpecifiedNames: false)
            }
        };

        static JsonNet()
        {
            Settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
        }
    }
}
