using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.RazorStringInterpolation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RazorStringInterpolationTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$""""
                [assembly: DefaultIntentManaged(Mode.Fully)]
                
                namespace {{Namespace}}
                {
                     [IntentManaged(Mode.Fully, Body = Mode.Merge)]
                     public partial class {{ClassName}}
                     {
                         [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
                         public override string TransformText()
                         {
                             return $"""
                                    @page "{{this.GetPageDirectiveText()}}"
                
                                    <PageTitle>{{this.GetPageTitleText()}}</PageTitle>
                                    """;
                         }
                     }
                }
                """";
        }
    }
}