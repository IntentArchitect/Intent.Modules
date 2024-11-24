using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate", Version= "1.0")]

namespace Intent.Modules.ModuleBuilder.TypeScript.Templates.TypescriptTemplateStringInterpolation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TypescriptTemplateStringInterpolationTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            if (IsForInterface())
            {
                return $$$$"""
                       using System;
                       using System.Collections.Generic;
                       
                       [assembly: DefaultIntentManaged(Mode.Fully)]
                       
                       namespace {{{{Namespace}}}}
                       {
                           [IntentManaged(Mode.Fully, Body = Mode.Merge)]
                           public partial class {{{{ClassName}}}}
                           {
                               [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
                               public override string TransformText()
                               {
                                   return $@"export interface {ClassName} {{{string.Join(Environment.NewLine, GetMembers())}
                       }}
                       ";
                               }
                       
                               [IntentInitialGen] private IEnumerable<string> GetMembers()
                               {
                                   var members = new List<string>();

                                   return members;
                               }
                           }
                       }
                       """;
            }

            return $$$$"""
                   using System;
                   using System.Collections.Generic;
                   
                   [assembly: DefaultIntentManaged(Mode.Fully)]
                   
                   namespace {{{{Namespace}}}}
                   {
                       [IntentManaged(Mode.Fully, Body = Mode.Merge)]
                        public partial class {{{{ClassName}}}}
                       {
                           [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
                           public override string TransformText()
                           {
                               return $@"export class {ClassName} {{{string.Join(Environment.NewLine, GetMembers())}
                   }}
                   ";
                           }
                   
                           [IntentInitialGen] private IEnumerable<string> GetMembers()
                           {
                               var members = new List<string>();
                   
                               // example: adding a constructor
                               members.Add($@"
                       constructor() {{
                       }}");
                   
                               return members;
                           }
                       }
                   }
                   """;
        }
    }
}