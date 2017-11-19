# IModSpec Files

## Overview

'IModSpec' files or modules specifications files, are xml configuration whiles used to describe and configure your modules. Every module must have a 'IModSpec' file. 

The structure of the configuration file is as follows.

## Configuration file structure

The Xml file has a root element name 'Package'.

### package Root Element

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
|[factoryExtensions](#factoryExtensions-element)|element|N|Catalog of factory extensions within the module.|
|[projectTypes](#projectTypes-element)|element|N|Catalog of project types within the module.|


### interoperability Element

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

### dependencies Element

A list of modules to be installed with this module as dependencies for this module.

```xml
  <dependencies>
    <dependency id="Intent.Common" version="1.2.1"/>
  </dependencies>
```

dependency element
|Attribute|Required|Description|
|-|-|-|
|id|Y|The identifier of the dependant module you would like installed with your module.|
|version|Y|Version specification of the dependency.|

### files Element

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

file element
|Attribute|Required|Description|
|-|-|-|
|src|Y|File to be packaged into the module.|

### metadata Element

|Child Element|Type|Required|Description|
|-|-|-|-|
||string|Y|.|

### templates Element

|Child Element|Type|Required|Description|
|-|-|-|-|
||string|Y|.|

### decorators Element

|Child Element|Type|Required|Description|
|-|-|-|-|
||string|Y|.|

### factoryExtensions Element

|Child Element|Type|Required|Description|
|-|-|-|-|
||string|Y|.|

### projectTypes Element

|Child Element|Type|Required|Description|
|-|-|-|-|
||string|Y|.|
