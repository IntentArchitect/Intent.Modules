# Application Modules

This screen allows to manage which modules are installed for your application. These modules are typically your packaged patterns, i.e. templates generating your code , the way you want it, using your meta data. Modules typically should be a set of templates to achieve a pattern or set of related patterns. Modules often encapsulate a technology, and more specifically your usage patterns of the specific technology. Modules are discovered through repositories which can be either service based or folder based.

It is important to note that modules can contain more than just patternized code generation, some other types of modules might include IDE Integration, Code Weavers or Meta Data providers.

Modules can include any of the following plugins:- 

|Plugin Type|Description|
|-|-|
|Template Registrations|Standard code generation plugins namely templates and decorators.|
|Factory Extensions|Allow for the execution of custom code within the software factory. |
|Meta Data Provider|Load your own Meta Data to further enrich the code generation process. |



![Image of the Modules Screen](../../images/user_manual/application_modules.png)


## 1. Repository Configuration
This allows you to configure your repositories, i.e. where Intent Architect is discovering your modules. [![Navigates to Repository Configuration](../../images/navigate.png "Navigates to Repository Configuration screen")](repository_configuration.md)

## 2. Module Filter
This controls what modules you are seeing in the modules list.
- Browse - All modules available from you currently selected repository.
- Installed - All modules installed for current application. 
- Update - All modules installed for current application which have updates available. 

## 3. Search
Finds what you looking for.

## 4. Module List
Displays a list of modules based on your current repository, filters and search input.

## 5. Module List Item
This gives an overview of a specific Module. This would include a name, the author, brief description and version information. Currently installed Modules are denoted by an icon, the colour of the icon provides additional information.

|Colour|Description|
|-|-|
|Green|You have the latest version of the module installed.|
|Blue|There is a newer version of the module available.|
|Red|This module is installed but Intent Architect is unable to locate module in the configured repositories.|
 
## 6. Module Details
This section contains detailed information about the module. It also allows you to install, update, uninstall the module, as well as the ability to configure the module.

## 7. Module Commands
This panel is dynamic and changes based on whether or not the Module is installed. It allows for installing the module, uninstalling the module and changing the version of a currently installed module.

## 8. Module Meta Data
Provides meta data about the module including authoring information, modules dependencies, etc.

## 9. Module Configuration
This section provides technical details about the various plugins present in the modules, typically this would be the list of templates the modules contains but it can also contain other plugin types. These plugins can be enabled or disabled using the check box. These plugins can be configured through the cog icon.