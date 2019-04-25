using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Templates
{
    public class TemplateMetaData
    {
        public string TemplateId { get; }
        public TemplateVersion Version { get; }

        public TemplateMetaData(string templateId, TemplateVersion version)
        {
            TemplateId = templateId;
            Version = version;
        }
    }
}
