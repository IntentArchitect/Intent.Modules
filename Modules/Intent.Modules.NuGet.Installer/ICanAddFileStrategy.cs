namespace Intent.Modules.NuGet.Installer
{
    public interface ICanAddFileStrategy
    {
        bool CanAddFile(string file);
    }
}