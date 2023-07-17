### Version 3.3.6

- Now calls `dotnet build-server shutdown` before calling `dotnet build` in case of pre-existing build servers which may cause the process to not exit.

### Version 3.3.5

- Added `--disable-build-servers` argument to `dotnet build` to prevent issue where the process would sometimes never exit.

### Version 3.3.4

- Fix issue when run on macOS.

### Version 3.3.3

- Updated dependencies and supported client versions to prevent warnings when used with Intent Architect 4.x.

### Version 3.3.2

- Updated supported client version to [3.3.0-pre.0, 5.0.0).
