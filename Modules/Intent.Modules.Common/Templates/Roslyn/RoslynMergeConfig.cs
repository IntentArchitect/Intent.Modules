using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.SoftwareFactory.RoslynMerge.Migrations;

namespace Intent.SoftwareFactory.Templates
{
    public class RoslynMergeConfig
    {
        public TemplateMetadata TemplateMetadata { get; }
        public ITemplateMigration[] Migrations { get; }

        public RoslynMergeConfig(TemplateMetadata templateMetadata, params ITemplateMigration[] migrations)
        {
            TemplateMetadata = templateMetadata;
            Migrations = migrations;
        }
    }
}
