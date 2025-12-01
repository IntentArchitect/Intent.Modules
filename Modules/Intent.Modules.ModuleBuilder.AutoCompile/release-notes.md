### Version 3.4.2

- Improvement: Auto compilation can now be skipped by setting the `INTENT_MODULE_BUILDER_AUTO_COMPILE_DISABLED` environment variable to any non-empty value.

### Version 3.4.1

- Improvement: Removed dependency on the `Intent.ModuleBuilder` module.

### Version 3.4.0

- Improvement: Updated Module from .NET Core 3.5 to .NET 8.

### Version 3.3.7

- Now sets `DOTNET_CLI_UI_LANGUAGE` environment variable to `en` for `dotnet build` to ensure it is using the English UI language so that fallback strategy of looking for `Time Elapsed` in the output still works.

### Version 3.3.6

- Now calls `dotnet build-server shutdown` before calling `dotnet build` in case of pre-existing build servers which may cause the process to not exit. Additionally, will force exit the process when "Time Elapsed " is detected.

### Version 3.3.5

- Added `--disable-build-servers` argument to `dotnet build` to prevent issue where the process would sometimes never exit.

### Version 3.3.4

- Fix issue when run on macOS.

### Version 3.3.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.2

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
