using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Configuration;

public class InfrastructureRegisteredEvent
{
    /// <summary>
    /// Creates a new instance of <see cref="InfrastructureRegisteredEvent" />.
    /// </summary>
    /// <remarks>
    /// Modules such as <see href="(URL to healthcheck module readme)">Intent.AspNetCore.HealthChecks</see>
    /// may listen to these events and generate appropriate code in response.
    /// </remarks>
    public InfrastructureRegisteredEvent(
        string infrastructureComponent,
        Dictionary<string, string> properties = null)
    {
        InfrastructureComponent = infrastructureComponent;
        Properties = properties ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// The type of infrastructural component, e.g. SqlServer, MongoDb, Redis, etc.
    /// </summary>
    public string InfrastructureComponent { get; }
    
    /// <summary>
    /// Properties of the infrastructure, e.g. connection strings.
    /// </summary>
    public Dictionary<string, string> Properties { get; }

    /// <summary>
    /// Convenience method for adding an item to <see cref="Properties" />.
    /// </summary>
    public InfrastructureRegisteredEvent WithProperty(string key, string value)
    {
        Properties.Add(key, value);
        return this;
    }
}