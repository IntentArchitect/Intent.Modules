$files = Get-ChildItem **/tsconfig.json -Recurse -Depth 2
$count = 0

foreach ($file in $files) {
    $count++
    Write-Host "Building $count of $($files.Count): $file..."
    tsc -p $file

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Exited early as build failed for $file"
        exit
    }
}
