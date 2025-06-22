#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $true)][string]$BuildArtifactStagingDirectory,
    [Parameter(Mandatory = $true)][string]$SourceBranch,
    [Parameter(Mandatory = $true)][string]$IslnRelativePath
)

$repoConfigContent = 
"<?xml version=""1.0"" encoding=""utf-8""?>
<assetRepositories>
  <entries>"

if ($SourceBranch -like 'refs/heads/development-*') {
    $repoConfigContent += '
    <entry>
      <name>Dev Modules</name>
      <address>https://dev-modules.intentarchitect.com/</address>
      <isBuiltIn>false</isBuiltIn>
      <order>9</order>
    </entry>'
}

$repoConfigContent += "
    <entry>
      <name>Pipeline build artifact staging directory</name>
      <address>$BuildArtifactStagingDirectory</address>
      <isBuiltIn>false</isBuiltIn>
      <order>10</order>
    </entry>
  </entries>
</assetRepositories>"

$islnDirectory = [System.IO.Path]::GetDirectoryName("./$IslnRelativePath")
$repositoryConfigPath = [System.IO.Path]::Combine((Get-Location), $islnDirectory, "intent.repositories.config")
$fullPath = [System.IO.Path]::GetFullPath($repositoryConfigPath)

Write-Host "Updating $fullPath to:"
Write-Host $repoConfigContent
Set-Content -Path $fullPath -Value $repoConfigContent -Encoding UTF8
