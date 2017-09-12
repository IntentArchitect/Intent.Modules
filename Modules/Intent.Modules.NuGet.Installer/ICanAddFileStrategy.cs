namespace Intent.SoftwareFactory.NuGet
{
    public interface ICanAddFileStrategy
    {
        bool CanAddFile(string file);
    }
}