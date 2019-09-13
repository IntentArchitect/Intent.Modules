namespace Intent.Modules.Common.Templates
{
    public class TemplateVersion
    {
        public int Major { get; }
        public int Minor { get; }

        public TemplateVersion(string version)
        {
            version = version.Trim();
            string[] versions = version.Split('.');
            Major = int.Parse(versions[0]);
            if (versions.Length > 1)
            {
                Minor = int.Parse(versions[1]);
            }
            else
            {
                Minor = 0;
            }
        }

        public TemplateVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public static implicit operator TemplateVersion(string version)
        {
            return new TemplateVersion(version);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}
