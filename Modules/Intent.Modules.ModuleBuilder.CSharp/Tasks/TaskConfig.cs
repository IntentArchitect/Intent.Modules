using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.ModuleBuilder.CSharp.Tasks
{
    public class TaskConfig
    {
        public TaskConfig()
        {
            NugetPackageIds = new List<string>();
        }

        public List<string> NugetPackageIds { get; set; }
    }
}
