using Intent.Modules.Bower.Contracts;
using Intent.Engine;
using Intent.Templates
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Bower
{
    public static class IProjectExtensions
    {
        public static IList<IBowerPackageInfo> BowerPackages(this IProject project)
        {
            return project.TemplateInstances
                    .SelectMany(ti => ti.GetAllBowerDependencies())
                    .Distinct()
                    .ToList();
        }

        public static IEnumerable<IBowerPackageInfo> GetAllBowerDependencies(this ITemplate template)
        {
            return template.GetAll<IHasBowerDependencies, IBowerPackageInfo>((i) => i.GetBowerDependencies());
        }
    }
}
