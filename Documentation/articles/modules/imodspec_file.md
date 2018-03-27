# IModSpec Files

## Overview

'IModSpec' files or modules specifications files, are XML configuration files used to describe the module, its content and configurability of the module. Every module must have a 'IModSpec' file. 

At a high level an imodspec file contains
- **Module identity**, the identifier and version 
- **Module Metadata**, information about the module e.g. author, description, etc.
- **Module dependencies**, other modules this module depends on
- **Deployed Files**, list of assemblies which must be deployed i.e. the module assembly, plus any external dependant assemblies
- **Deployed Metadata**, details about any Intent Architect Metadata this module wants to install, typically stereotypes 
- **Plugins**, templates, decorators, factory extensions and project types contained in the modules, as well details about their configuration
- **Module interop**, integration information about other modules this module is designed to work with.

## Configuration file structure

The Xml file has a root element name 'Package'.

### `package` Root Element

|Child Element|Type|Required|Description|
|-|-|-|-|
|id|string|Y|A unique human readable identifier for you module. Typically something MyOrganization.MyModule. |
|version|string|Y|Version number for the module.|
|summary|string|Y|Short summary from the module.|
|description|string|N|Longer description of the module.|
|authors|string|N|Authors of the module.|
|iconUrl|string|N|Url to an icon for display purposes.|
|projectUrl|string|N|Link to a project site, related to the module.|
|tags|string|N|Any tags you would like to associate with the module.|
|[interoperability](#interoperability-element)|N|element|Catalog of interoperability modules. Interoperabilty modules are modules "integrate" two modules, these modules will install if the second module is installed. |
|[dependencies](#dependencies-element)|N|element|List on modules dependencies. These modules will be installed with this module as dependencies.|
|[files](#files-element)|element|Y|List of assemblies to package into the module.|
|[metadata](#metadata-element)|element|N|Catalog of any Metadata which needs to be installed with the module.|
|[templates](#templates-element)|element|N|Catalog of templates within the module.|
|[decorators](#decorators-element)|element|N|Catalog of decorators within the module.|
|[factoryExtensions](#factoryextensions-element)|element|N|Catalog of factory extensions within the module.|
|[projectTypes](#projecttypes-element)|element|N|Catalog of project types within the module.|


### `interoperability` Element

```xml
<interoperability>
    <!-- id : The Module identifier for the Module we have an Interop module for  -->
    <detect id="Intent.Unity">
      <install>
        <!-- id : The Module identifier for the Interop module to install  -->
        <package id="MyModule.Interop.Unity" version="1.2.1"/>
      </install>
    </detect>
  </interoperability>
```

### `dependencies` Element

A list of modules to be installed with this module as dependencies for this module.

```xml
  <dependencies>
    <dependency id="Intent.Common" version="1.2.1"/>
  </dependencies>
```

`dependency` element
|Attribute|Required|Description|
|-|-|-|
|id|Y|The identifier of the dependant module you would like installed with your module.|
|version|Y|Version specification of the dependency.|

### `files` Element

A list of files to be packages into the module, this would typically be your module dll, but can include other dependant assemblies and optional the pdb file for your module to facilitate debugging your module. There are two variables which can be used:-
- **configuration** , the current build configuration.
- **id** , the assembly name of the project being built containing the imodspec file.

```xml
  <files>
    <file src="bin\$configuration$\$id$.dll"/>
    <file src="bin\$configuration$\$id$.pdb"/>
    <file src="bin\$configuration$\SomeOtherAssembly.dll"/>
  </files>
```

`file` element
|Attribute|Required|Description|
|-|-|-|
|src|Y|File to be packaged into the module.|

### `metadata` Element
This section allows you to install Metadata into Intent Architect when your module is installed. This section describes where in your module the Metadata is and what type of Metadata it is.

```xml
  <metadata>
    <install target="Domain" src="MetaData\Domain"/>
    <install target="DataContracts" src="MetaData\DataContracts"/>
  </metadata> 
```

`install` element
|Attribute|Required|Description|
|-|-|-|
|target|Y|The type of Metadata you want to install i.e. Domain, Services, DataContracts|
|src|Y|The location within your module where the Metadata file are|

### `templates` Element

This element describes all the templates contained within your module, as well as any configuration they support. 

>[!NOTE]
>Only templates described in the IModSpec file are accessible to consumers of your module. If you have templates in your modules which are not registered in the IModSpec file they will not be loaded or executed.

```xml
  <templates>
    <template id="MyTemplateId" enabled="true">
      <role>Domain</role>
      <config>
        <add key="Depends On" 
          description="Make this output a code behind file for" 
          default="${Model.Name}.cs" />
      </config>
    </template>
  </templates>
```

`template` element

|Attributes|Type|Required|Description|
|-|-|-|-|
|Id|string|Y|Unique identifier for this template.|
|enabled|bool|N|Specifies whether or not this template is enabled. Default is true. Disabled templates are typically situational templates which consumers of your module can choose to use or not.|

|Child Element|Type|Required|Description|
|-|-|-|-|
|role|string|N|This is the project role this template's output will target i.e. any outputs generated by this template will be put in the project which has this role.|
|config|element|N|This element allows for configuring your template, and allowing consumers to configure it. See [config](#config-element) element for more details.|

### `decorators` Element

This element describes all the decorators contained within your module, as well as any configuration they support. If you are not familiar with the term decorator, it is a template extension, ie. a plugin for a template which modifies its behaviour in some way. 

>[!NOTE]
>Only decorators described in the IModSpec file are accessible to consumers of your module. If you have decorators in your modules which are not registered in the IModSpec file they will not be loaded or executed.

```xml
  <decorators>
    <decorator id="Intent.DDD.Entity.Decorator" enabled="false">
      <config>
        <add key="Aggregate Root Base Class" 
          description="Base class for all aggregates" 
          default="" />
        <add key="Entity Base Class"
          description="Base class for all entities" 
          default="" />
        <add key="Value Object Base Class" 
          description="Base class for all value objects" 
          default="" />
      </config>
    </decorator>
  </decorators>
```

`decorator` element

|Attributes|Type|Required|Description|
|-|-|-|-|
|Id|string|Y|Unique identifier for this decorator.|
|enabled|bool|N|Specifies whether or not this decorator is enabled. Default is true. |

|Child Element|Type|Required|Description|
|-|-|-|-|
|config|element|N|This element allows for configuring your template, and allowing consumers to configure it. See [config](#config-element) element for more details.|

### `factoryExtensions` Element

This element describes all the factory extensions contained within your module, as well as any configuration they support. If you are unfamiliar with the term, Factory Extensions are plugins which allow extending the code generation process, this is typically for cross cutting functionality. 

>[!NOTE]
>Only factory extensions described in the IModSpec file are accessible to consumers of your module. If you have factory extensions in your modules which are not registered in the IModSpec file they will not be loaded or executed.

```xml
  <factoryExtensions>
    <factoryExtension id="Intent.OutputTransformer.FileHeader" enabled="true">
      <config>
        <add key="Append To Once Off" 
          description="Should file headers be applied to once off generated code (true | false)" 
          default="true" />
        <add key="Append Always" 
          description="List of CodeGenTypes which should have files headers appended on every execution. (comma delimited list of strings)" default="Basic,IntentControlledTagWeave" />
        <add key="Append On Create" 
          description="List of CodeGenTypes which should have files headers appended on initial output creation. (comma delimited list of strings)" 
          default="UserControlledTagWeave,UserControlledWeave,RoslynWeave" />
      </config>
    </factoryExtension>
  </factoryExtensions>
```

`factoryExtension` element

|Attributes|Type|Required|Description|
|-|-|-|-|
|Id|string|Y|Unique identifier for this factory extension.|
|enabled|bool|N|Specifies whether or not this factory extension is enabled. Default is true. |

|Child Element|Type|Required|Description|
|-|-|-|-|
|config|element|N|This element allows for configuring your factory extension, and allowing consumers to configure it. See [config](#config-element) element for more details.|

### `config` Element
This element allows you to describe the configurable aspects of a plugin, along with the default configuration. Setting up your plugins configuration in such a manner allows for the configuration to be modified by modules consumers through Intent Architect.

```xml
  <config>
    <add key="Depends On" description="Make this output a code behind file for" default="${Model.Name}.cs" />
  </config>
```
`add` element

|Attributes|Type|Required|Description|
|-|-|-|-|
|key|string|Y|Name of the setting.|
|description|string|Y|Description of the settings and its intent. |
|default|string|Y|The default value for this setting. |

### `projectTypes` Element

This element describes any project types your module contains. Project types is an extension mechanism to allow developers to extend Intent Architect to support development environments beyond its initial scope. Check out `Intent.Modules.VisualStudioProjects` for a comprehensive example of implementing your own project types.

```xml
  <projectTypes>
    <projectType id="MyProjectTypeId">
      <name>C# Library</name>
      <icon>
        <type>CharacterBox</type>
        <source>C|#757575</source>
      </icon>
      <properties>
        <property id="TargetFramework">
          <name>TargetFramework</name>
          <type>string</type>
          <defaultValue>4.5.2</defaultValue>
        </property>
      </properties>
    </projectType>
  <projectTypes>
```

`projectType` element

|Attributes|Type|Required|Description|
|-|-|-|-|
|id|string|Y|Unique Identifier for your project type.|

|Child Element|Type|Required|Description|
|-|-|-|-|
|name|string|Y|Display name of the project type.|
|icon|element|Y|Icon for your project type.|
|properties|element|N|Any custom properties required for you project type.|
