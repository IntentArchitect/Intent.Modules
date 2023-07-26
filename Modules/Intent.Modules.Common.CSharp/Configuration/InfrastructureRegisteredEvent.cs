using System.Collections.Generic;

namespace Intent.Modules.Common.CSharp.Configuration;

public class InfrastructureRegisteredEvent : IForProjectWithRoleRequest
{
    public InfrastructureRegisteredEvent(
        string infrastructureComponent,
        IDictionary<string, string> connectionDetails = null)
    {
        InfrastructureComponent = infrastructureComponent;
        ConnectionDetails = connectionDetails ?? new Dictionary<string, string>();
    }

    public string InfrastructureComponent { get; }
    public IDictionary<string, string> ConnectionDetails { get; }

    public InfrastructureRegisteredEvent AddConnectionDetial(string key, string value)
    {
        ConnectionDetails.Add(key, value);
        return this;
    }
    
    /// <inheritdoc />
    public string ForProjectWithRole { get; }

    /// <inheritdoc />
    public bool WasHandled { get; private set; }

    /// <inheritdoc />
    public void MarkHandled() => WasHandled = true;
}