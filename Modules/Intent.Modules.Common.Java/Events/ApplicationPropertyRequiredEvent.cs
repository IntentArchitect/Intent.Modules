namespace Intent.Modules.Common.Java.Events;

public class ApplicationPropertyRequiredEvent
{
    public ApplicationPropertyRequiredEvent(
        string name,
        string value,
        string profile = null)
    {
        Name = name;
        Value = value;
        Profile = profile;
    }

    public string Name { get; }
    public string Value { get; }
    public string Profile { get; }
}