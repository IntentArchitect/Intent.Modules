using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateBindingContext : ITemplateBindingContext
    {
        private object _defaultModelContext;
        private Dictionary<string, object> _prefixLookup;

        public TemplateBindingContext(object defaultModelContext)
        {
            _defaultModelContext = defaultModelContext;
        }

        public TemplateBindingContext() : this(null)
        {
        }

        public void SetDefaultModel(object modelContext)
        {
            _defaultModelContext = modelContext;
        }

        public void AddFakeProperty<T>(string fakePropertyName, T obj)
        {
            if (_prefixLookup == null)
            {
                _prefixLookup = new Dictionary<string, object>();
            }
            _prefixLookup[fakePropertyName] = obj;
        }

        public object GetProperty(string propertyName)
        {
            if (_prefixLookup != null && _prefixLookup.ContainsKey(propertyName))
            {
                return _prefixLookup[propertyName];
            }

            return null;
        }

        public object GetRootModel()
        {
            return _defaultModelContext;
        }
    }
}