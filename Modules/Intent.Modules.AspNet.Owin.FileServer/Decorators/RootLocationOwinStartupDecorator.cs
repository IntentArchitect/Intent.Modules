using Intent.Modules.AspNet.Owin.Templates.OwinStartup;
using System.Collections.Generic;
using System.IO;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.Owin.FileServer.Decorators
{
    public class RootLocationOwinStartupDecorator : IOwinStartupDecorator, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Owin.FileServer.OwinConfiguration";

        private string _relativePath = "wwwroot";
        public RootLocationOwinStartupDecorator()
        {

        }


        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "Microsoft.Owin.FileSystems",
                "Microsoft.Owin.StaticFiles"
            };
        }

        public IEnumerable<string> Configuration()
        {
            return new[]
            {
                "ConfigureRootLocation(app, \"wwwroot\");"
            };
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public IEnumerable<string> Methods()
        {
            return new[]
            {
                @"
        private static void ConfigureRootLocation(IAppBuilder app, string rootLocation)
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string wwwroot = Path.Combine(root, rootLocation);

            var fileServerOptions = new FileServerOptions()
            {
                EnableDefaultFiles = true,
                EnableDirectoryBrowsing = false,
                RequestPath = new PathString(string.Empty),
                FileSystem = new PhysicalFileSystem(wwwroot)
            };

            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            app.UseFileServer(fileServerOptions);
        }"
            };
        }

        public int Priority { get; set; } = -200;

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftOwinFileSystems,
                NugetPackages.MicrosoftOwinStaticFiles,
            };
        }
    }
}