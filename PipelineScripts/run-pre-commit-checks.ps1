param(
    [Parameter(Mandatory = $true)][string]$ModulesIsln,
    [Parameter(Mandatory = $true)][string]$TestsIsln,
    [switch]$Reset
)

if ($Reset) {
    if ($Env:INTENT_PRE_COMMIT_CHECK_PHASE) {
        Remove-Item Env:INTENT_PRE_COMMIT_CHECK_PHASE
    }

    Write-Host "Reset performed, all phases will be run on the next execution"
    exit
}

$currentPhase = 0;
$modulesFolder = [System.IO.Path]::GetDirectoryName($ModulesIsln)
$testsFolder = [System.IO.Path]::GetDirectoryName($TestsIsln)

if ($Env:INTENT_PRE_COMMIT_CHECK_PHASE -gt 0) {
    Write-Host "Resuming from last successfully completed phase, use the -Reset parameter to remove memory of successfully completed phases."
}

if ([int]$Env:INTENT_PRE_COMMIT_CHECK_PHASE -ge ++$currentPhase) {
    Write-Host "Skipping `"pre-build validations`" phase as was successfully completed previously."
}
else {
    ./PipelineScripts/pre-build-validations.ps1 -ModulesFolder "$modulesFolder" -TestsFolder "$testsFolder"
    if ($LASTEXITCODE -ne 0) {
        exit
    }

    $Env:INTENT_PRE_COMMIT_CHECK_PHASE = $currentPhase
}

if ([int]$Env:INTENT_PRE_COMMIT_CHECK_PHASE -ge ++$currentPhase) {
    Write-Host "Skipping `"ensure no outstanding changes to modules`" phase as was successfully completed previously."
}
else {
    ./PipelineScripts/ensure-no-outstanding-sf-changes.ps1 -IslnPath "$ModulesIsln"
    if ($LASTEXITCODE -ne 0) {
        exit
    }

    $Env:INTENT_PRE_COMMIT_CHECK_PHASE = $currentPhase
}

if ([int]$Env:INTENT_PRE_COMMIT_CHECK_PHASE -ge ++$currentPhase) {
    Write-Host "Skipping `"build all modules`" phase as was successfully completed previously."
}
else {
    ./PipelineScripts/build-all.ps1 -Folder "$modulesFolder"
    if ($LASTEXITCODE -ne 0) {
        exit
    }

    $Env:INTENT_PRE_COMMIT_CHECK_PHASE = $currentPhase
}

if ([int]$Env:INTENT_PRE_COMMIT_CHECK_PHASE -ge ++$currentPhase) {
    Write-Host "Skipping `"ensure no outstanding changes to tests`" phase as was successfully completed previously."
}
else {
    ./PipelineScripts/ensure-no-outstanding-sf-changes.ps1 -IslnPath "$TestsIsln" -CheckDeviations
    if ($LASTEXITCODE -ne 0) {
        exit
    }

    $Env:INTENT_PRE_COMMIT_CHECK_PHASE = $currentPhase
}

if ([int]$Env:INTENT_PRE_COMMIT_CHECK_PHASE -ge ++$currentPhase) {
    Write-Host "Skipping `"build all tests`" phase as was successfully completed previously."
}
else {
    ./PipelineScripts/build-all.ps1 -Folder "$testsFolder"
    if ($LASTEXITCODE -ne 0) {
        exit
    }

    $Env:INTENT_PRE_COMMIT_CHECK_PHASE = $currentPhase
}

Write-Host "✅ All checks completed successfully, commit, push and the CI build should hopefully succeed 🤞"
Remove-Item Env:INTENT_PRE_COMMIT_CHECK_PHASE
exit 0
