#!/usr/bin/env pwsh

param(
    [string]$buildArtifactStagingDirectory,
    [string]$sourceBranch,
    [string]$modulesIntentSolutionRelativePath,
    [string]$testsIntentSolutionRelativePath
)

$repoConfigContent = 
"<?xml version=""1.0"" encoding=""utf-8""?>
<assetRepositories>
  <entries>"

if ($sourceBranch -like 'refs/heads/development-*')
{
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
      <address>$buildArtifactStagingDirectory</address>
      <isBuiltIn>false</isBuiltIn>
      <order>10</order>
    </entry>
  </entries>
</assetRepositories>"

$moduleLookup = @{}
$moduleFileNames = Get-ChildItem "$buildArtifactStagingDirectory/*.imod" | % {
    $file = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
    $dotNumber = 0
    $dotIndex = -1
    $dotIndexNo3 = -1
    for ($i = $file.Length - 1; $i -gt 0; $i--) {
        $cur = $file[$i]
        if ($cur -eq '.') {
            $dotNumber++
            $dotIndex = $i
        }
        if (($dotNumber -eq 3) -and ([System.Char]::IsDigit($file[$i + 1]))) {
            $dotIndexNo3 = $dotIndex
        }
        if ($dotNumber -eq 4) {
            if ([System.Char]::IsDigit($file[$i + 1])) {
                $dotIndex = $i
            } else {
                $dotIndex = $dotIndexNo3
            }
            break
        }
    }

    if ($dotIndex -gt -1) {
        $moduleLookup.Add($file.Substring(0, $dotIndex), $file.Substring($dotIndex + 1))
    }
}

$curLocation = Get-Location;
Write-Host "`$curLocation = $curLocation"

$moduleSlnDir = [System.IO.Path]::GetDirectoryName($modulesIntentSolutionRelativePath)
Write-Host "`$moduleSlnDir = $moduleSlnDir"

$moduleRepoPath = [System.IO.Path]::Combine($curLocation, $moduleSlnDir, "intent.repositories.config")
Write-Host "`$moduleRepoPath = $moduleRepoPath"
$repoConfigContent | Set-Content $moduleRepoPath -Encoding UTF8

$testSlnDir = [System.IO.Path]::GetDirectoryName($testsIntentSolutionRelativePath)
Write-Host "`$testSlnDir = $testSlnDir"

$testsRepoPath = [System.IO.Path]::Combine($curLocation, $testSlnDir, "intent.repositories.config")
Write-Host "`$testsRepoPath = $testsRepoPath"
$repoConfigContent | Set-Content $testsRepoPath -Encoding UTF8

$discrepanciesFound = $false

$testSln = [xml] (Get-Content "./$testsIntentSolutionRelativePath" -Encoding UTF8)
$testSln.solution.applications.application | % {
    $appRelPath = [System.IO.Path]::Combine($curLocation, $testSlnDir, $_.relativePath)
    $basePath = [System.IO.Path]::GetDirectoryName($appRelPath)
    $modulesConfig = [System.IO.Path]::Combine($basePath, "modules.config")
    $name = $_.name

    $modulesConfigContent = [xml] (Get-Content $modulesConfig -Encoding UTF8)
    $modulesConfigContent.modules.module | % { 
        $module = $_
        $moduleVersionFound = $moduleLookup[$module.moduleId]
        if ($moduleVersionFound -ne $null -and $module.version -ne $moduleVersionFound) { 
            Write-Host "##vso[task.logissue type=error;]$($name): Version discrepancy found for module '$($module.moduleId)': expected '$moduleVersionFound', found '$($module.version)'"
            $discrepanciesFound = $true
        }
    }
}

if ($discrepanciesFound) {
    Write-Host "##vso[task.logissue type=error;]Review the $($testsIntentSolutionRelativePath) Intent Solution and make sure the module dependencies are installed to the appropriate versions needed for the test suite to execute successfully."
    exit 1
}
