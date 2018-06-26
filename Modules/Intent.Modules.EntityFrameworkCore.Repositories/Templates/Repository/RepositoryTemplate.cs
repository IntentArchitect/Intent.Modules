﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository
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
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class RepositoryTemplate : IntentRoslynProjectItemTemplateBase<IClass>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\nusing Intent.Framework.Core.Context;\r\nusing Intent.Framework.Domain.Repositori" +
                    "es;\r\nusing Intent.Framework.EntityFrameworkCore;\r\nusing Intent.Framework.EntityF" +
                    "rameworkCore.Repositories;\r\n");
            
            #line 18 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Ignore)]\r\n\r\nnamespace ");
            
            #line 22 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge)]\r\n\tpublic class ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : RepositoryBase<");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(EntityInterfaceName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(EntityName));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbContextName));
            
            #line default
            #line hidden
            this.Write(">, ");
            
            #line 25 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RepositoryContractName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentManaged(Mode.Fully)]\r\n        public ");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 28 "C:\Dev\Intent.Modules\Modules\Intent.Modules.EntityFrameworkCore.Repositories\Templates\Repository\RepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DbContextName));
            
            #line default
            #line hidden
            this.Write(" dbContext) : base (dbContext)\r\n        {\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
