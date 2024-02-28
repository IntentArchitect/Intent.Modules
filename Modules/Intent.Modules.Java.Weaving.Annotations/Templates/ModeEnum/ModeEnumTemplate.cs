// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Java.Weaving.Annotations.Templates.ModeEnum
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.Java.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.Java\Modules\Intent.Modules.Java.Weaving.Annotations\Templates\ModeEnum\ModeEnumTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ModeEnumTemplate : JavaTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("package ");
            
            #line 10 "C:\Dev\Intent.Modules.Java\Modules\Intent.Modules.Java.Weaving.Annotations\Templates\ModeEnum\ModeEnumTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Package));
            
            #line default
            #line hidden
            this.Write(@";

/**
 * Specifies the Intent Architect instruction mode for the specific targets
 * Instructions can be combined by using the | bitwise operation. For example: @IntentManageClass(members = Mode.CanAdd | Mode.CanRemove)
 * In this case, Intent Architect will only add or remove members, but never update existing ones.
 */
public final class ");
            
            #line 17 "C:\Dev\Intent.Modules.Java\Modules\Intent.Modules.Java.Weaving.Annotations\Templates\ModeEnum\ModeEnumTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@" {
    /**
     *  Inherit instructions (do nothing)
     */
    public static final int Default = 0;
    /**
     *  Instructs Intent Architect to ignore these elements
     */
    public static final int Ignore = 1;
    /**
     *  Allows Intent Architect to add or update, but not remove, elements
     */
    public static final int Merge = 2;
    /**
     *  Allows Intent Architect to add, update, and remove elements
     */
    public static final int Manage = 4;
    /**
     *  Allows Intent Architect to add elements
     */
    public static final int CanAdd = 8;
    /**
     *  Allows Intent Architect to update elements
     */
    public static final int CanUpdate = 16;
    /**
     *  Allows Intent Architect to remove elements
     */
    public static final int CanRemove = 32;
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
