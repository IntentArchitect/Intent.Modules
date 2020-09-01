using System.Collections.Generic;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaFile
    {
        public IList<JavaClass> Classes { get; } = new List<JavaClass>();
    }
}
