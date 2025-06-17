# Intent.ModuleBuilder

This module contains a Designer for designing Intent Architect modules.

## `File Template`

This template can be used to produce any kind of generic file.

### File Settings - File Extension setting

This setting controls the output files extension. e.g., txt, cs, java, png, etc.

### File Settings - Output File Content setting

This setting specifies whether the output file is Text-based or a binary file. This distinction is important as Text files and Binary files have different features, for example, Text-based file support weaving and live edits.

### File Settings - Templating Method setting

This setting is only relevant for Text-based files and allows you to choose which templating method to use when creating the Template.

## `Static Content Template`

This template can be used to output a relatively static set of files. This template does support some basic search and replace functionality, allowing you to do some basic customization to the outputted files.

### Template Settings - Content Subfolder

The name of the subfolder within the (`content` or `resources`) folder. This folder serves as the source location of the files the template should transform and output.
If this setting is not specified the template will work directly with the contents of the (`content` or `resources`) folder.

### Template Settings - Binary File Globbing Patterns

These settings contain a list of patterns to identify which files in the collection are binary files. These patterns are standard `File globbing` patterns, for more information [see](https://learn.microsoft.com/en-us/dotnet/core/extensions/file-globbing).
The standard exclusions are images (jpg, png, ico), Excel files and PDFs. This list can be adapted to your specific requirements.

### Content custom keyword substitution

Though the content being processed will be static, there is a **basic keyword substitution** feature which can be set inside the Registration class by populating the `Replacements` property.

For example:

```cs
public override IReadOnlyDictionary<string, string> Replacements => new Dictionary<string, string> { {"Today", DateTime.Today.ToString("yyyy-MM-dd")} };
```

So any file content that features the following phrase `<#= Today #>` will be replaced by the Date for the current day when the content was generated.

> [!IMPORTANT]
> Please ensure that a single space is preserved between the `<#=`, `keyword` and `#>` symbols.
