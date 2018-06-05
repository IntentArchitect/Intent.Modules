﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.EntityFramework.Repositories.Templates.Repository
{
    using Intent.MetaModel.Domain;
    using Intent.SoftwareFactory.Templates;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<IClass>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\nusing Intent.Framework.Core;\r\nusing Intent.Framework.Domain.Repositories;\r\nusi" +
                    "ng Intent.Framework.EntityFramework;\r\nusing Intent.Framework.EntityFramework.Rep" +
                    "ositories;\r\n");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Ignore)]\r\n\r\nnamespace ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge)]\r\n\tpublic class ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : RepositoryBase<");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(EntityInterfaceName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(EntityName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbContextName));
            
            #line default
            #line hidden
            this.Write(">, ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RepositoryContractName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.I" +
                    "gnore)]\r\n        public ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(IContextBackingStore context) : base (context.Get<DbContextScope>().Get<");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFramework.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbContextName));
            
            #line default
            #line hidden
            this.Write(">())\r\n        {\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
