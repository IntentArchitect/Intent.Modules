// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ModuleBuilder.Templates.ReadMe
{
    using Intent.Modules.Common.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\ReadMe\ReadMeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ReadMeTemplate : IntentTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"INTENT ARCHITECT - MODULE BUILDER

To run this module in Intent Architect, the following steps are required:

NOTE: In Visual Studio, it is recommended to use a T4 editor such as this one (https://t4-editor.tangible-engineering.com) to edit T4 templates.

# 1. Build Module
    - Compile this project as per usual for a .NET project.
    - This will automatically package and create an Intent Module (.imod) file in the ${SolutionFolder}/Intent.Modules folder.

# 2. Add a Module Repository to your Intent Architect Workspace
    - In Intent Architect, navigate to the application you wish to install this module into.
    - Navigate to the Modules section.
    - Click the Repository Setting cog in the top right corner of the Modules section.
    - Click Add New, and specify the Name (e.g. Local Modules) and Address which will be the full (or relative) path to the ${SolutionFolder}/Intent.Modules folder.
      TIP: for the current workspace, this will be at the following relative location (copy directly into your repository location):
      ./");
            
            #line 20 "C:\Dev\Intent.Modules\Modules\Intent.Modules.ModuleBuilder\Templates\ReadMe\ReadMeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(OutputContext.Application.Name));
            
            #line default
            #line hidden
            this.Write(@"/Intent.Modules
    - Click Save.

#3. Install Module
    - Select your newly added module repository from the Repository dropdown.
    - A list including your module will automatically be loaded into the modules view.
    - Click on your module and then click Install in the details section on the right.

#4. Run the Software Factory
    - Click the ""play"" button in the right of the navigation bar to run the Software Factory.
    - Outputs (if any) for your module will be listed. 
    - Errors can be debugged by clicking on the ""bug"" button to left of the ""play"" button, and selecting this solution.

Issues can be sent to support@intentarchitect.com or posted on our GitHub Repository at https://github.com/IntentSoftware/IntentArchitect.");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
