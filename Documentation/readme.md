# Documentation Folder

This folder contains the source [markdown](https://dotnet.github.io/docfx/spec/docfx_flavored_markdown.html) for Intent Architect's documentation. [DocFX](https://dotnet.github.io/docfx/) is then run against the `docfx.json` file and along with the `.md` files, our documentation is generated into the `_site` folder (which is .gitignore'd). The contents of the `_site` folder are then uploaded to the website at [http://intentarchitect.com/docs/](http://intentarchitect.com/docs/) by the Intent Architect team.

## Build and make localhost preview site available

From the command line run:

`_tools\DocFX\docfx.exe docfx.json --server`

Or simply run:

`_build_and_serve.bat`

## Build only

From the command line run:

`_tools\DocFX\docfx.exe docfx.json`

## Troubleshooting

If there is a strange build error, try completely deleting the `obj` folder. This folder only contains build artifacts, the worst that can happen from its deletion is that the next build may take longer.