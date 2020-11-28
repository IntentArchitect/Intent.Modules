using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.ModuleBuilder.Templates.Api
{
    public class NotifyModelHasParentFolderEvent
    {
        public NotifyModelHasParentFolderEvent(string modelId)
        {
            ModelId = modelId;
        }

        public string ModelId { get; set; }
    }
}
