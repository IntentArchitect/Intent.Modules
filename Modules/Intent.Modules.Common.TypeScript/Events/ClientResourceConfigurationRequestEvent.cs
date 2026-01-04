using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.TypeScript.Events;

public class ClientResourceConfigurationRequestEvent
{
    public ClientResourceConfigurationRequestEvent(string relationshipType, string resourceValue)
    {
        RelationshipType = relationshipType;
        ResourceValue = resourceValue;
    }

    public string RelationshipType { get; set; }

    public string ResourceValue { get; set; }
}
