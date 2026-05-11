using Intent.Engine;
using Intent.Modules.Common.FileBuilders.MarkdownFileBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.Templates
{
    public abstract class MarkdownBaseTemplate<T> : IntentTemplateBase<T>
    {
        private string? _currentContent;
        private bool _dontOverrideContent = false;
        private bool _withContentHashing = true;

        protected MarkdownBaseTemplate(string templateId, IOutputTarget outputTarget, T model) : base(templateId, outputTarget, model)
        {            
        }

        public bool WithContentHashing
        {
            get => _withContentHashing;
            set => _withContentHashing = value;
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            if (!_withContentHashing)
            {
                return;
            }
            string? existingFile = this.GetExistingFilePath();
            if (existingFile != null && File.Exists(existingFile))
            {
                var existingContent = File.ReadAllText(existingFile);
                if (MarkdownContentHash.HasChanged(existingContent))
                {
                    _currentContent = existingContent;
                    _dontOverrideContent = true;
                }
            }
        }

        abstract public IMarkdownFile MarkdownFile { get; }

        public override string TransformText()
        {
            if (!_withContentHashing)
            {
                return MarkdownFile.ToString();
            }

            if (_dontOverrideContent && _currentContent != null)
            {
                return _currentContent;
            }

            var result = MarkdownFile.ToString();
            result = MarkdownContentHash.AddOrUpdateContentHash(result);

            return result;
        }

    }
}
