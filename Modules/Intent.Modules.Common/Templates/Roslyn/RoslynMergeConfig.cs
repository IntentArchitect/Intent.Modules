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
        public TemplateMetaData TemplateMetaData { get; }
        public ITemplateMigration[] Migrations { get; }

        public RoslynMergeConfig(TemplateMetaData templateMetaData, params ITemplateMigration[] migrations)
        {
            TemplateMetaData = templateMetaData;
            Migrations = migrations;
        }
    }
}
