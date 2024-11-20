namespace Intent.Modules.Common.CSharp.Nuget
{
    public class NuGetInstallOptions
    {
        public NuGetInstallOptions()
        {
        }

        //This will install the package even if the package would already be there transitive-ly
        public bool ForceInstall { get; set; } = false;

        public void Consolidate(NuGetInstallOptions otherRequestOptions)
        {
            //If either wants a ForcceInstall it's ForceInstall
            ForceInstall = ForceInstall || otherRequestOptions.ForceInstall;
        }
    }
}
