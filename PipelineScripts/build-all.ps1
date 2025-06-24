param(
    [Parameter(Mandatory = $true)][string]$Folder,
    [switch]$RestoreFirstWithForceEvaluate
)

Write-Host "Searching for .sln files..."

$slnFiles = Get-ChildItem "./$folder/**/*.sln" -Recurse -Depth 2 | Where-Object { $_ -NotLike "*previous_output*" }
$count = 0

foreach ($slnFile in $slnFiles) {
    $count++
    Write-Host
    Write-Host "Building $count of $($slnFiles.Count): $slnFile..."

    if ($RestoreFirstWithForceEvaluate) {
        dotnet restore $slnFile --force-evaluate

        if ($LASTEXITCODE -ne 0) {
            exit -1
        }
        
        dotnet build $slnFile --interactive --no-restore /p:WarningLevel=0 /p:NoWarn="NU5104"
    }
    else {
        dotnet build $slnFile --interactive /p:WarningLevel=0 /p:NoWarn="NU5104"
    }

    if ($LASTEXITCODE -ne 0) {
        exit -1
    }
}
