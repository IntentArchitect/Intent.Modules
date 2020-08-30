using System.Linq;
using Intent.Modules.Common.TypeScript.Editor;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypeScriptMethodMerger 
    {
        private readonly TypeScriptClass _existingClass;
        private readonly TypeScriptClass _outputClass;

        public TypeScriptMethodMerger(TypeScriptClass existingClass, TypeScriptClass outputClass)
        {
            _existingClass = existingClass;
            _outputClass = outputClass;
        }

        
    }
}