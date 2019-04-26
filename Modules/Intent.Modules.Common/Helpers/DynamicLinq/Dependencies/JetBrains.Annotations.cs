using System;
using System.Collections.Generic;
using System.Text;

namespace JetBrains.Annotations
{
    public class NotNullAttribute : Attribute { }
    public class CanBeNullAttribute : Attribute { }
    public class PublicAPIAttribute : Attribute { }
    public class NoEnumerationAttribute : Attribute { }
    public class InvokerParameterNameAttribute : Attribute { }
    public class ContractAnnotationAttribute : Attribute { public ContractAnnotationAttribute(string param) { } }

}
