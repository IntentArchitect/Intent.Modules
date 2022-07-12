namespace Intent.Modules.Common.CSharp.DependencyInjection;

public class ServiceConfigurationRequest
{ 
    private ServiceConfigurationRequest(
        string extensionMethodName,
        string @namespace,
        bool supplyConfiguration)
    {
        ExtensionMethodName = extensionMethodName;
        Namespace = @namespace;
        SupplyConfiguration = supplyConfiguration;
    }

    public string ExtensionMethodName { get; }
    public string Namespace { get; }
    public bool SupplyConfiguration { get; }
    
    public int Priority { get; private set; }
    public bool IsHandled { get; private set; }
    
    public static ServiceConfigurationRequest ForExtensionMethod(
        string extensionMethodName,
        string @namespace,
        bool supplyConfiguration = false)
    {
        return new ServiceConfigurationRequest(extensionMethodName, @namespace, supplyConfiguration);
    }

    public ServiceConfigurationRequest WithPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    
    public void MarkAsHandled()
    {
        IsHandled = true;
    }
}