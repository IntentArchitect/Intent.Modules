using Intent.Modules.Bower.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
