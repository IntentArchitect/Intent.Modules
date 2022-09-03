### Version 3.3.18

- Fixed: Non-first generic type parameters were not being normalized. For example `Dictionary<System.String, System.String>` should have been normalized to `Dictionary<String, String>` but was incorrectly normalized to `Dictionary<String, System.String>`.

### Version 3.3.17

 - Builder pattern for templating C# files (see type `Intent.Modules.Common.CSharp.Builder.CSharpFile`). 
	Note that this is an experimental templating pattern and, if used, one should expect changes to the API in future releases.
	An example of this templating approach can be found in the `Intent.Modules.ValueObjects` module in the `Intent.Modules.NET` open-source repository.