using System;

namespace Intent.Modules.Common.CSharp.Builder;

public interface ICSharpConstructorParameter : ICSharpParameter
{
    ICSharpConstructorParameter IntroduceField(Action<ICSharpField> configure = null);
    ICSharpConstructorParameter IntroduceField(Action<ICSharpField, ICSharpFieldAssignmentStatement> configure);
    ICSharpConstructorParameter IntroduceReadonlyField(Action<ICSharpField> configure = null);
    ICSharpConstructorParameter IntroduceReadonlyField(Action<ICSharpField, ICSharpFieldAssignmentStatement> configure);
    ICSharpConstructorParameter IntroduceProperty(Action<ICSharpProperty> configure = null);
    ICSharpConstructorParameter IntroduceProperty(Action<ICSharpProperty, ICSharpFieldAssignmentStatement> configure);
}