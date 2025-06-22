param(
    [Parameter(Mandatory = $true)][string]$Folder
)

Write-Host "Searching for .sln files..."

$slnFiles = Get-ChildItem "./$folder/**/*.sln" -Recurse -Depth 2 | Where-Object { $_ -NotLike "*previous_output*" }
$count = 0

foreach ($slnFile in $slnFiles) {
    $count++
    Write-Host
    Write-Host "Building $count of $($slnFiles.Count): $slnFile..."
    dotnet build $slnFile --interactive /p:WarningLevel=0 /p:NoWarn="NU5104"

    if ($LASTEXITCODE -ne 0) {
        exit -1
    }
}
