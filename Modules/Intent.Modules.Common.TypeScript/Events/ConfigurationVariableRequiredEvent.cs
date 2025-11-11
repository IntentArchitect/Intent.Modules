using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Events;

public class ConfigurationVariableRequiredEvent
{
    public ConfigurationVariableRequiredEvent(string key, string defaultValue)
    {
        Key = key;
        DefaultValue = defaultValue;
    }

    public string Key { get; }

    public string DefaultValue { get; }
}
