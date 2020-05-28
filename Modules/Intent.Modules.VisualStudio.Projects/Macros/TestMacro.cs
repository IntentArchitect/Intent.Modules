using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Plugins;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Macros
{
    public class TestMacro : IMacro
    {
        private readonly IMetadataManager _metadataManager;

        public TestMacro(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string Id { get; } = "TestMacro";
        public int Order { get; } = 0;
        public string Trigger { get; } = "trigger";
        public void Execute(IApplication application)
        {
            var models = _metadataManager.GetAllProjectModels(application);
            foreach (var project in models)
            {
                Logging.Log.Info("FOUND:" + project.ToString());
            }

        }
    }
}
